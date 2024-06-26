﻿
namespace DotSights.ConsoleApp;

using DotSights.Core;
using DotSights.Core.Common.Types;

class Program
{
	public static void Main(string[] args)
	{
		var result = DotSights.GetDataFromDataPath();

		DotSightsSettings settings = new()
		{
			GroupItemsWithSameProcessName = true,
			GroupItemsUsingGroupingRules = true,
			GroupingRules = [new() { Name = "Browsing!", RegexQuery = ".*Brave.*" },
				new() { Name = "Coding!", RegexQuery = ".*devenv.*" },
				new() { Name = "Coding and Browsing!!", RegexQuery = "(.*devenv.*|.*Brave.*)" }],
			ShowOnlyRegexMatchedItems = true,
			RegexMatchProcessName = false,
			TrackerSaveInterval = TimeSpan.FromMinutes(15),
			OptimizeForStorageSpace = false
		};
		var filtered = DotSights.FilterDataFromSettings(result, settings);
		foreach (var item in filtered)
		{
			Console.WriteLine(item);
		}



	}
}

