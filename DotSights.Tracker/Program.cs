using DotSights.Core.Common.Types;
using System.Runtime.InteropServices;
using DotSights.Core;
using DotSights.Core.Common;
namespace DotSights.Tracker
{
	class Program
	{

		static void Main(string[] args)
		{
			TrackerSingleton.Instance.Track();
		}

	}


}
