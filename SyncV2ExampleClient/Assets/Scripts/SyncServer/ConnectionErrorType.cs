namespace Servers
{
    internal enum ConnectionErrorType
    {
        CouldNotOpenSocket,
        CouldNotOpenSslStream,
        InvalidServer,
        HandshakeFailed,
        Unknown
    }
}