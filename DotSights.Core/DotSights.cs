using DotSights.Core.Common.Types;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace DotSights.Core;

public static class DotSights
{
	public static string DataFilePath { get; set; } = "data.json";

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

	public static List<ActivityData> GetDataFromDataPath()
	{
		if (File.Exists(DataFilePath))
		{
			string data = File.ReadAllText(DataFilePath);
			if (DeserializeData(data, out List<ActivityData>? result))
			{
				return result;
			}
		}
		return new List<ActivityData>();
	}

	public static void SaveDataToDataPath(List<ActivityData> data)
	{
		File.WriteAllText(DataFilePath, SerializeData(data));
	}
}

