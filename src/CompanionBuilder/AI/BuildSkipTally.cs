using System.Collections.Generic;

namespace CompanionBuilder;

/// <summary>
/// 18/11/2026 (the easy build mode): how many skipped lines a .blueprint has for <b>one</b> reason.
///
/// <para><b>Why it exists.</b> <see cref="BuildBlueprintReport.Skipped"/> is a single total that adds the two reasons
/// together (<see cref="BuildBlueprintSkip.NotAPiece"/> and <see cref="BuildBlueprintSkip.RecipeLocked"/>,
/// <c>BuildBlueprintReport.cs:44-46</c>). With the easy mode on, the recipe lines stop being skipped at all, so that
/// total counts only the prefabs this game does not have — and the panel's seal ("N pieces without a recipe") read
/// from it would say <b>zero</b> exactly when the mode freed hundreds of lines. The warning of the normal mode needs
/// the two numbers apart too: "24 pieces without a recipe" may in fact be 24 rugs, which no recipe unlocks.</para>
///
/// <para><b>Pure on purpose:</b> no <c>UnityEngine</c>, no game types — the report is Unity-bound (it asks
/// <c>Player</c>/<c>Piece</c>), so the counting cannot live there and be tested outside the game. The report maps its
/// own rows into <see cref="Entry"/> and calls this; the panel calls the report.</para>
/// </summary>
internal static class BuildSkipTally
{
	/// <summary>One group of skipped lines, as the report already groups them.</summary>
	internal sealed class Entry
	{
		internal string Reason;
		internal int Count;

		/// <summary>A row. The constructor exists so the fields are assigned where the C# compiler can see it (a
		/// write-only-from-outside struct-less class warns CS0649 in this project, which must stay at 0 warnings).</summary>
		internal Entry(string reason, int count)
		{
			Reason = reason;
			Count = count;
		}
	}

	/// <summary>
	/// The number of skipped lines whose reason is exactly <paramref name="reason"/>. A null list or a null reason
	/// answers 0 rather than throwing: the panel asks this on every refresh, including with no file selected.
	/// </summary>
	internal static int Count(List<Entry> entries, string reason)
	{
		if (entries == null || reason == null)
		{
			return 0;
		}
		int total = 0;
		for (int i = 0; i < entries.Count; i++)
		{
			Entry entry = entries[i];
			if (entry != null && string.Equals(entry.Reason, reason))
			{
				total += entry.Count;
			}
		}
		return total;
	}
}
