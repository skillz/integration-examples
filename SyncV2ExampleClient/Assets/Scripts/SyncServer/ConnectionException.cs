using System;

namespace Servers
{
    internal sealed class ConnectionException : Exception
    {
        public ConnectionErrorType ConnectionErrorType
        {
            get
            {
                return connectionErrorType;
            }
        }

        private readonly ConnectionErrorType connectionErrorType;

        public ConnectionException(ConnectionErrorType connectionErrorType, Exception innerException)
            : base("Failed to open a connection to the server", innerException)
        {
            this.connectionErrorType = connectionErrorType;
        }
    }
}