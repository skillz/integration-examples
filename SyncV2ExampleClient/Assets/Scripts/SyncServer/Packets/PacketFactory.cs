using FlatBuffers;
using UnityEngine;
using SyncServer;

namespace Servers
{
    /// <summary>
    /// Provides convenience methods to create <see cref="Packet"/>s and/or
    /// their byte array representations.
    /// </summary>
    public static class PacketFactory
    {
        private const int DefaultBufferBytes = 1024;

        public static ConnectPacket MakeConnectPacket(long userId, string matchId, string matchToken)
        {
            var buffer = new ByteBuffer(MakeConnectBuffer(userId, matchId, matchToken));

            return ConnectPacket.GetRootAsConnectPacket(buffer);
        }

        public static byte[] MakeConnectBuffer(long userId, string matchId, string matchToken)
        {
            var builder = new FlatBufferBuilder(DefaultBufferBytes);

            var matchIdOffset = builder.CreateString(matchId);
            var matchTokenOffset = builder.CreateString(matchToken);

            ConnectPacket.StartConnectPacket(builder);

            ConnectPacket.AddOpcode(builder, (short)Opcode.Connect);
            ConnectPacket.AddUserId(builder, userId);
            ConnectPacket.AddMatchId(builder, matchIdOffset);
            ConnectPacket.AddExternalToken(builder, matchTokenOffset);

            var connectPacketOffset = ConnectPacket.EndConnectPacket(builder);
            builder.Finish(connectPacketOffset.Value);

            return builder.SizedByteArray();
        }

        public static Packet BytesToPacket(byte[] bytes)
        {
            return Packet.GetRootAsPacket(new ByteBuffer(bytes));
        }

        public static Packet MakeInvalidPacket()
        {
            var buffer = new ByteBuffer(MakePacketBuffer(Opcode.Invalid));

            return Packet.GetRootAsPacket(buffer);
        }

        public static byte[] MakePacketBuffer(Opcode opcode)
        {
            var builder = new FlatBufferBuilder(DefaultBufferBytes);

            Packet.StartPacket(builder);

            Packet.AddOpcode(builder, (short)opcode);

            var packetOffset = Packet.EndPacket(builder);
            builder.Finish(packetOffset.Value);

            return builder.SizedByteArray();
        }

        public static byte[] MakePlayerUpdatedBuffer(Vector2 touchWorldPos)
        {
            var builder = new FlatBufferBuilder(DefaultBufferBytes);

            PlayerUpdatePacket.StartPlayerUpdatePacket(builder);

            PlayerUpdatePacket.AddOpcode(builder, (short)Opcode.PlayerUpdate);

            var positionOffset = Vec2.CreateVec2(builder, touchWorldPos.x, touchWorldPos.y);
            PlayerUpdatePacket.AddPosX(builder, touchWorldPos.x);
            PlayerUpdatePacket.AddPosY(builder, touchWorldPos.y);

            var playerUpdatedOffset = PlayerUpdatePacket.EndPlayerUpdatePacket(builder);

            builder.Finish(playerUpdatedOffset.Value);

            return builder.SizedByteArray();
        }

        public static byte[] MakeAbortBuffer()
        {
            var builder = new FlatBufferBuilder(DefaultBufferBytes);

            AbortPacket.StartAbortPacket(builder);

            AbortPacket.AddOpcode(builder, (short)Opcode.Abort);

            var abortOffset = AbortPacket.EndAbortPacket(builder);

            builder.Finish(abortOffset.Value);

            return builder.SizedByteArray();
        }
      
        public static byte[] MakeKeepAliveBuffer()
        {
            var builder = new FlatBufferBuilder(DefaultBufferBytes);

            KeepAlivePacket.StartKeepAlivePacket(builder);

            KeepAlivePacket.AddOpcode(builder, (short)Opcode.KeepAlive);

            var abortOffset = KeepAlivePacket.EndKeepAlivePacket(builder);

            builder.Finish(abortOffset.Value);

            return builder.SizedByteArray();
        }
    }
}