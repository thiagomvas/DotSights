using DotSights.Dashboard.Models;
using System.Text.Json.Serialization;

namespace DotSights.Core.Common.Types
{
	[JsonSerializable(typeof(List<ActivityData>))]
	[JsonSourceGenerationOptions(WriteIndented = true)]
	internal partial class ActivityDataListGenerationContext : JsonSerializerContext
	{
	}
	[JsonSerializable(typeof(DotSightsSettings))]
	[JsonSourceGenerationOptions(WriteIndented = true)]
	internal partial class DotSightSettingsGenerationContext : JsonSerializerContext
	{
	}
}
