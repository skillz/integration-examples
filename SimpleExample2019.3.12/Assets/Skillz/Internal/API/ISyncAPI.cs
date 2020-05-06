namespace SkillzSDK.Internal.API
{
	/// <summary>
	/// Represents platform-agnognistic Skillz APIs for client-authoritative
	/// synchronous matches. Also known as Sync v1.
	/// </summary>
	internal interface ISyncAPI
	{
		IRandom Random
		{
			get;
		}

		bool IsMatchCompleted
		{
			get;
		}

		void SendData(byte[] data);

		int GetConnectedPlayerCount();

		ulong GetCurrentPlayerId();

		ulong GetCurrentOpponentPlayerId();

		double GetServerTime();

		long GetTimeLeftForReconnection(ulong playerId);
	}
}