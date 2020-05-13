using System;

namespace Servers
{
    public sealed class ReadCompletedArgs : EventArgs
    {
        public byte[] FlatBufferBytes
        {
            get;
        }

        private readonly string jsonServerMessage;

        public ReadCompletedArgs(byte[] flatBufferBytes)
        {
            FlatBufferBytes = flatBufferBytes;
        }
    }
}
