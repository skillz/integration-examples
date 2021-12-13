using System;
using System.Collections.Generic;

namespace SkillzSDK
{
	/// <summary>
	/// A collection of metadata that is stored within a ProgressionValue. Not all pieces
	/// of metadata are used in every namespace.
	/// </summary>
	public class ProgressionMetadata
	{
		/// <summary>
		/// The list of game ids relevant for the associated ProgressionValue.
		/// This is used for InGameItems that may be associated with multiple games.
		/// </summary>
		public readonly List<string> GameIds;

		public ProgressionMetadata(List<string> gameIds)
		{
			GameIds = gameIds;
		}

        public string ToString()
        {
            return String.Format("ProgressionMetadata: GameIds [{0}]", String.Join(",", GameIds.ToArray()));
        }
	}
}
