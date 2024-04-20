using DotSights.Core.Common.Types;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace DotSights.Core;

public static class DotSights
{
	[DllImport("user32.dll")]
	private static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern int GetWindowText(IntPtr hWnd, string lpString, int nMaxCount);

	public static string GetFocusedWindow()
	{
		IntPtr foregroundWindowHandle = GetForegroundWindow();
		if (foregroundWindowHandle != IntPtr.Zero)
		{
			const int nChars = 256;
			string windowTitle = new string(' ', nChars);
			GetWindowText(foregroundWindowHandle, windowTitle, nChars);
			return windowTitle.Substring(0, windowTitle.IndexOf('\0'));
		}
		return "";
	}

	public static string SerializeActivityData(List<ActivityData> data)
	{
		return JsonConvert.SerializeObject(data, Formatting.Indented);
	}

	public static string SerializeActivityData(ActivityData activityData)
	{
		return JsonConvert.SerializeObject(activityData, Formatting.Indented);
	}

	public static ActivityData? DeserializeActivityData(string serializedData)
	{
		return JsonConvert.DeserializeObject<ActivityData>(serializedData);
	}

	public static List<ActivityData>? DeserializeActivityDataList(string serializedData)
	{
		return JsonConvert.DeserializeObject<List<ActivityData>>(serializedData);
	}

}

