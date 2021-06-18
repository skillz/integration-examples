namespace SkillzSDK
{
	/// <summary>
	/// Sandbox allows for testing of both cash and Z games.
	/// Production should only be used for the actual final release into the app store.
	/// </summary>
	public enum Environment
	{
		Sandbox,
		Production
	};

	/// <summary>
	/// Apps come in either landscape or portrait
	/// </summary>
	public enum Orientation
	{
		Landscape,
		Portrait
	};
}