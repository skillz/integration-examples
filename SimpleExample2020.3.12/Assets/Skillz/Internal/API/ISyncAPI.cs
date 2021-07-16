namespace SkillzSDK.Internal.API
{
	/// <summary>
	/// Represents platform-agnognistic Skillz APIs for client-authoritative
	/// synchronous matches. Also known as Sync v1.
	/// </summary>
	[System.Obsolete("The RTTB APIs are obsolete and will be removed in a future version of the Skillz SDK.")]
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