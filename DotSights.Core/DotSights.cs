using DotSights.Core.Common.Types;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace DotSights.Core;

public static class DotSights
{
	[DllImport("user32.dll")]
	private static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern int GetWindowText(IntPtr hWnd, string lpString, int nMaxCount); 

	[DllImport("user32.dll")]
	private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


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

	public static string GetFocusedProcessName()
	{
		IntPtr foregroundWindowHandle = GetForegroundWindow();
		if (foregroundWindowHandle != IntPtr.Zero)
		{
			GetWindowThreadProcessId(foregroundWindowHandle, out uint processId);
			Process process = Process.GetProcessById((int)processId);
			return process.ProcessName;
		}
		return "";
	}

	public static string SerializeData(List<ActivityData> data)
	{
		return JsonSerializer.Serialize(data, typeof(List<ActivityData>), ActivityDataListGenerationContext.Default);
	}

	public static bool DeserializeData(string data, out List<ActivityData>? result)
	{
		try
		{
			result = JsonSerializer.Deserialize(data, typeof(List<ActivityData>), ActivityDataListGenerationContext.Default) as List<ActivityData>;
			return true;
		}
		catch (Exception)
		{
			result = null;
			return false;
		}
	}
}

