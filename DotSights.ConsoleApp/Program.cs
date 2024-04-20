
namespace DotSights.ConsoleApp;
using DotSights.Core.Common;

class Program
{
	public static void Main(string[] args)
	{
		TrackerSingleton.Instance.Track();
	}
}

