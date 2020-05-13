using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using UnityEngine;

namespace Servers
{
    /// <summary>
    /// Used to connect to and interact with a Sync v2 Game Server.
    /// This is a scriptable object so that it can live across
    /// scene changes and avoids having to implement this as a
    /// singleton.
    /// </summary>
    [CreateAssetMenu(fileName = "SyncServerController", menuName = "Sync Server Controller")]
    public sealed class SyncServerController : ScriptableObject
    {
        private const int MaxMessageBytes = 1024;
        private const long InvalidUserId = 0;
        private const int ReadTimeoutMs = 15000;

        /// <summary>
        /// Connecting to the Game Server.
        /// </summary>
        public event EventHandler Connecting;

        /// <summary>
        /// Connected to the Game Server.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Raised if there was a failure to open a TCP socket
        /// when connecting to the game server.
        /// </summary>
        public event EventHandler CouldNotOpenSocket;

        /// <summary>
        /// Raised if there was a failure to open an SSL stream
        /// when connecting to the game server.
        /// </summary>
        public event EventHandler CouldNotOpenSslStream;

        /// <summary>
        /// Raised because the server was not a legitimate Game Server.
        /// </summary>
        public event EventHandler InvalidServer;

        /// <summary>
        /// The TLS handshake with the Game Server failed.
        /// This means the server was not a legitimate Game Server.
        /// </summary>
        public event EventHandler ServerHandshakeFailed;

        /// <summary>
        /// Raised if receiving a packet from the game server timed out.
        /// </summary>
        public event EventHandler ReceiveTimedOut;

        /// <summary>
        /// Raised for unknown errors;
        /// </summary>
        public event EventHandler UnknownError;

        public bool CanPlay
        {
            get
            {
                return broadcastCount > 0;
            }
        }

        public long UserId
        {
            get
            {
                return userId;
            }
        }

        public bool IsConnected
        {
            get
            {
                return
                    client != null &&
                    client.Client != null &&
                    client.Client.Connected;
            }
        }

        [Tooltip("Expected TLS certificate of the server being connected to.")]
        [SerializeField]
        private TLSConfiguration tlsConfiguration;

        private TcpClient client;
        private SslStream sslStream;

        private Thread receivePacketsThread;
        private Thread sendPacketsThread;

        private SafeQueue<byte[]> receivePacketsQueue = new SafeQueue<byte[]>();
        private SafeQueue<byte[]> sendPacketsQueue = new SafeQueue<byte[]>();

        // Signals the send thread that there are messages
        // queued up to send to the game server.
        private ManualResetEvent sendPacketsPending = new ManualResetEvent(false);

        private volatile bool isConnecting;

        private uint broadcastCount;
        private bool abortingMatch;

        private long userId;
        private string matchId;

        private SynchronizationContext mainThreadContext;

        /// <summary>
        /// Connects to the game server with the given <see cref="ConnectArgs"/>.
        /// This should be called from the main Unity thread.
        /// </summary>
        /// <param name="connectArgs"></param>
        public void Connect(ConnectArgs connectArgs)
        {
            if (isConnecting || IsConnected)
            {
                return;
            }

            InitializeFields(connectArgs);

            userId = UnityEngine.Random.Range(0, int.MaxValue);

            isConnecting = true;
            RaiseConnecting();

            // Instantiating with new TcpClient(url, port) will immediately
            // try to connect and block. Simply instantiate and connect
            // in the receive background thread when it starts so the
            // main Unity thread doesn't hang.
            client = new TcpClient();
            client.Client = null;

            // Clear the queues when connecting instead of disconnecting
            // in case you want to still process packets that were received.
            receivePacketsQueue.Clear();
            sendPacketsQueue.Clear();

            receivePacketsThread = new Thread(() => { ReceivePacketsThread(connectArgs); });
            receivePacketsThread.IsBackground = true;
            receivePacketsThread.Start();
        }

        /// <summary>
        /// Disconnects from the game server.
        /// </summary>
        public void Disconnect()
        {
            if (!isConnecting && !IsConnected)
            {
                Debug.Log("Already disconnected...");
                return;
            }

            Debug.Log("Disconnecting...");

            sslStream?.Close();
            client?.Close();

            // Ensure the receive packets thread exits, and in turn
            // that the send packet thread exits as well
            receivePacketsThread?.Interrupt();

            // We interrupted the receive Thread, so we can't guarantee that
            // connecting was reset. let's do it manually.
            isConnecting = false;

            // No need to continue sending because we're disconnecting.
            // The receive queue though, may still have packets that we
            // want to process.
            sendPacketsQueue.Clear();

            client = null;
        }

        /// <summary>
        /// Sends the player's world position to the game server. This is non-blocking.
        /// </summary>
        /// <param name="touchWorldPos"></param>
        public void SendPlayerPosition(Vector2 touchWorldPos)
        {
            Debug.Log($"Sending player position=({touchWorldPos.x}, {touchWorldPos.y})");
            SendCore(PacketFactory.MakePlayerUpdatedBuffer(touchWorldPos));
        }

        /// <summary>
        /// Sends a keep-alive message to the game server. This is non-blocking.
        /// </summary>
        public void SendKeepAlive()
        {
            Debug.Log($"Sending keep alive...");
            SendCore(PacketFactory.MakeKeepAliveBuffer());
        }

        /// <summary>
        /// Tells the game server that the player is ending the match. This is non-blocking.
        /// </summary>
        public void QuitMatch()
        {
            Debug.Log($"Sending Abort match packet");
            SendCore(PacketFactory.MakeAbortBuffer());
        }

        /// <summary>
        /// Gets the next Flat Buffer packet received from the game server.
        /// </summary>
        /// <param name="data">The received Flat Buffer formatted packet.</param>
        /// <returns><c>true</c> if there was a packet present</returns>
        public bool GetNextPacket(out byte[] data)
        {
            return receivePacketsQueue.TryDequeue(out data);
        }

        private void InitializeFields(ConnectArgs connectArgs)
        {
            broadcastCount = 0;

            abortingMatch = false;

            userId = UnityEngine.Random.Range(0, int.MaxValue);
            connectArgs.UserId = userId;

            Debug.Log($"Your userId={userId}");

            matchId = connectArgs.MatchId;
            Debug.Log($"Connect with matchId={matchId}");

            // Can only get the main thread's context from the main thread
            mainThreadContext = SynchronizationContext.Current;
        }

        private void ReceivePacketsThread(ConnectArgs connectArgs)
        {
            // Thread exceptions are silent, so
            // catching is absolutely required.
            try
            {
                TryOpenTcpSocket(connectArgs);
                TryOpenSslStream();
                TryValidateServer();
                TryHandshake(connectArgs);

                RaiseConnected();

                // Start the send packets thread now that there's a connection
                sendPacketsThread = new Thread(() => { SendPacketsThread(); });
                sendPacketsThread.IsBackground = true;
                sendPacketsThread.Start();

                while (true)
                {
                    byte[] content;
                    if (!ReadPacketBlocking(out content))
                    {
                        break;
                    }

                    receivePacketsQueue.Enqueue(content);
                }

            }
            catch (SocketException exception)
            {
                Debug.Log($"Failed to connect to url={connectArgs.Url} port={connectArgs.Port}\nReason:\n{exception}");
            }
            catch (ThreadInterruptedException)
            {
                // Expected if Disconnect() is called
            }
            catch (ThreadAbortException)
            {
                // Expected if Disconnect() is called
            }
            catch (TimeoutException)
            {
                RaiseReceiveTimedOut();
            }
            catch (Exception exception)
            {
                // something went wrong. probably important.
                Debug.LogError($"Receive packets thread exception:\n{exception}");
                RaiseUnknownError();
            }
            finally
            {
                // The send packets thread might be waiting on ManualResetEvent,
                // interrupt it to ensure that it is stopped.
                sendPacketsThread?.Interrupt();

                // Connect might have failed.
                isConnecting = false;

                sslStream?.Close();
                client?.Close();
            }
        }

        private void SendPacketsThread()
        {
            try
            {
                while (client.Connected)
                {
                    sendPacketsPending.Reset();

                    byte[][] packets;
                    if (sendPacketsQueue.TryDequeueAll(out packets))
                    {
                        if (!SendPacketsBlocking(packets))
                        {
                            break;
                        }
                    }

                    sendPacketsPending.WaitOne();
                }
            }
            catch (ThreadAbortException)
            {
                // Occurs on Disconnect()
            }
            catch (ThreadInterruptedException)
            {
                // Thrown if the receive packets thread interrupted this thread,
                // while it is exiting, if this thread is waiting on the ManualResetEvent
            }
            catch (IOException)
            {
                // TODO: Handle timeout error
            }
            catch (Exception exception)
            {
                Debug.LogError($"Send packets thread exception:\n{exception}");
                RaiseUnknownError();
            }
            finally
            {
                // Clean up no matter what
                // We might get SocketExceptions when sending if the 'host has
                // failed to respond' - in which case we should close the connection
                // which causes the ReceiveLoop to end and fire the Disconnected
                // message. otherwise the connection would stay alive forever even
                // though we can't send anymore.
                sslStream?.Close();
                client?.Close();
            }
        }

        private void TryOpenTcpSocket(ConnectArgs connectArgs)
        {
            try
            {
                client.Connect(connectArgs.Url, (int)connectArgs.Port);
                isConnecting = false;

                client.NoDelay = true;
            }
            catch (Exception)
            {
                RaiseCouldNotOpenSocket();
                throw;
            }
        }

        private void TryOpenSslStream()
        {
            try
            {
                sslStream = new SslStream(
                    client.GetStream(),
                    false,
                    new RemoteCertificateValidationCallback(ValidateServerCertificate),
                    null
                );

                sslStream.ReadTimeout = ReadTimeoutMs;
            }
            catch (Exception)
            {
                RaiseCouldNotOpenSslStream();
                throw;
            }
        }

        private void TryValidateServer()
        {
            try
            {
                sslStream.AuthenticateAsClient(tlsConfiguration.TargetHost);
            }
            catch (Exception)
            {
                RaiseInvalidServer();
                throw;
            }
        }

        private void TryHandshake(ConnectArgs connectArgs)
        {
            try
            {
                // Write directly to the SSL stream instead of enqueueing
                // to the send packets queue in order to know immediately
                // if the server closes the socket because of an invalid
                // match token.
                sslStream.Write(PacketFactory.MakeConnectBuffer(userId, connectArgs.MatchId, connectArgs.MatchToken));
            }
            catch (Exception)
            {
                // The server will close the socket if the client's match
                // token was either invalid or its encrypted timestamp
                // is expired.
                RaiseServerHandshakeFailed();
                throw;
            }
        }

        private void SendCore(byte[] data)
        {
            if (!IsConnected)
            {
                Debug.LogWarning("Not sending data - no connection to the game server.");
                return;
            }

            sendPacketsQueue.Enqueue(data);
            sendPacketsPending.Set();
        }

        private bool ReadPacketBlocking(out byte[] content)
        {
            try
            {
                var data = new byte[MaxMessageBytes];
                var bytesRead = sslStream.Read(data, 0, MaxMessageBytes);
                if (bytesRead == 0)
                {
                    content = new byte[0];
                    return false;
                }

                content = new byte[bytesRead];
                Array.Copy(data, content, bytesRead);
            }
            catch (AggregateException aggregateException)
            {
                if (IsServerTimeout(aggregateException, out var socketException))
                {
                    throw new TimeoutException("Timed out waiting to receive data from the server", socketException);
                }

                throw;
            }
            catch (Exception)
            {
                throw;
            }

            return true;
        }

        private bool SendPacketsBlocking(byte[][] packets)
        {
            // stream.Write throws exceptions if client sends with high
            // frequency and the server stops
            try
            {
                for (var i = 0; i < packets.Length; i++)
                {
                    sslStream.Write(packets[i]);
                }

                return true;
            }
            catch (Exception exception)
            {
                // Log as regular message because servers do shut down sometimes
                Debug.Log($"SendPacketsBlocking: stream.Write exception: {exception}");
                return false;
            }
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            if (string.CompareOrdinal(certificate.GetPublicKeyString(), tlsConfiguration.PublicKey) == 0)
            {
                return true;
            }

            Debug.LogError("The server does not have the expected public key.");

            return false;
        }

        private bool IsServerTimeout(AggregateException aggregateException, out SocketException socketException)
        {
            var ioException = aggregateException.InnerExceptions != null && aggregateException.InnerExceptions.Count == 1
                ? aggregateException.InnerExceptions[0] as IOException
                : null;

            socketException = ioException != null && ioException.InnerException != null
                ? ioException.InnerException as SocketException
                : null;

            return socketException != null && socketException.SocketErrorCode == SocketError.WouldBlock;
        }

        private void IncrementBroadcastCount()
        {
            broadcastCount++;
        }

        private void RaiseConnecting()
        {
            // Not posted to Unity's main thread as this
            // should be already called from it
            Connecting?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseConnected()
        {
            mainThreadContext.Post(new SendOrPostCallback(parameter =>
                {
                    Connected?.Invoke(this, EventArgs.Empty);
                }),
                null
            );
        }

        private void RaiseInvalidServer()
        {
            mainThreadContext.Post(new SendOrPostCallback(parameter =>
                {
                    InvalidServer?.Invoke(this, EventArgs.Empty);
                }),
                null
            );
        }

        private void RaiseServerHandshakeFailed()
        {
            mainThreadContext.Post(new SendOrPostCallback(parameter =>
                {
                    ServerHandshakeFailed?.Invoke(this, EventArgs.Empty);
                }),
                null
            );
        }

        private void RaiseCouldNotOpenSocket()
        {
            mainThreadContext.Post(new SendOrPostCallback(parameter =>
                {
                    CouldNotOpenSocket?.Invoke(this, EventArgs.Empty);
                }),
                null
            );
        }

        private void RaiseCouldNotOpenSslStream()
        {
            mainThreadContext.Post(new SendOrPostCallback(parameter =>
                {
                    CouldNotOpenSslStream?.Invoke(this, EventArgs.Empty);
                }),
                null
            );
        }

        private void RaiseUnknownError()
        {
            mainThreadContext.Post(new SendOrPostCallback(parameter =>
                {
                    UnknownError?.Invoke(this, EventArgs.Empty);
                }),
                null
            );
        }

        private void RaiseReceiveTimedOut()
        {
            mainThreadContext.Post(new SendOrPostCallback(parameter =>
                {
                    ReceiveTimedOut?.Invoke(this, EventArgs.Empty);
                }),
                null
            );
        }
    }
}