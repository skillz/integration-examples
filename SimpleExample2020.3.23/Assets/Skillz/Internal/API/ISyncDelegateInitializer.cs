namespace SkillzSDK.Internal.API
{
	/// <summary>
	/// Represents a means to initialize a <see cref="SkillzSyncDelegate"/> instance.
	/// </summary>
	internal interface ISyncDelegateInitializer
	{
		void Initialize(SkillzSyncDelegate syncDelegate);
	}
}