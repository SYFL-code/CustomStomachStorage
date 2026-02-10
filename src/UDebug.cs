using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStomachStorage
{
	class UDebug
	{
		public static bool DebugMode = false;

		public static void Log(string message)
		{
            if ((Options.Instance?.DebugMode != null && Options.Instance.DebugMode.Value) || DebugMode)
			{
				UnityEngine.Debug.Log(message);
			}
        }

		public static void LogWarning(string message)
		{
            if ((Options.Instance?.DebugMode != null && Options.Instance.DebugMode.Value) || DebugMode)
            {
				UnityEngine.Debug.LogWarning(message);
			}
        }

		public static void LogError(string message)
		{
            if ((Options.Instance?.DebugMode != null && Options.Instance.DebugMode.Value) || DebugMode)
            {
				UnityEngine.Debug.LogError(message);
			}
        }

	}
}
