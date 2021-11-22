namespace SkillzSDK
{
	/// <summary>
	/// A collection of namespaces that are used by Skillz to maintain
	/// different sets of player progression.
	/// </summary>
	public class ProgressionNamespace
	{
		/// <summary>
		/// The default progression data namspace. This is READ ONLY and contains
		/// information tracked and maintained by Skillz.
		/// </summary>
		public static readonly string DEFAULT_PLAYER_DATA = "DefaultPlayerData";
		/// <summary>
		/// The namespace for all Player Data keys configured and tracked by the 
		/// game developer. 
		/// </summary>
		public static readonly string PLAYER_DATA = "PlayerData";
		/// <summary>
		/// The namespace for all In Game Items keys configured and tracked by the 
		/// game developer. 
		/// </summary>
		public static readonly string IN_GAME_ITEMS = "InGameItems";
	}
}
