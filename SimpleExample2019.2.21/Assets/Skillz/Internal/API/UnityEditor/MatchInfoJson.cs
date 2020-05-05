namespace SkillzSDK.Internal.API.UnityEditor
{
	internal static class MatchInfoJson
	{
		public static string Build(int gameId)
		{
			return "{ \"matchDescription\": \"A mocked Skillz match in Unity\", "
				+ "\"entryCash\": 0, "
				+ "\"entryPoints\": 2, "
				+ $"\"id\": {gameId}, "
				+ "\"templateId\": 1061, "
				+ "\"name\": \"Warmup!\""
				+ "\"isCash\": false, "
				+ "\"isSynchronous\": false, "
				+ "\"players\": [{}], "
				+ "}";
		}
	}
}