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
        private static bool _initialized = false;
        private static bool _configDebugMode = false;

        private static void UpdateConfigValue()
        {
            if (!_initialized)
            {
                try
                {
                    // 只在初始化时读取一次配置，避免递归
                    if (MyOptions.Instance?.DebugMode != null)
                    {
                        _configDebugMode = MyOptions.Instance.DebugMode.Value;
                    }
                    _initialized = true;
                }
                catch
                {
                    // 配置还没准备好，忽略
                }
            }
        }

        public static void Log(string message)
        {
            UpdateConfigValue();
            if (_configDebugMode || DebugMode)
            {
                UnityEngine.Debug.Log(message);
            }
        }

        public static void LogWarning(string message)
        {
            UpdateConfigValue();
            if (_configDebugMode || DebugMode)
            {
                UnityEngine.Debug.LogWarning(message);
            }
        }

        public static void LogError(string message)
        {
            UpdateConfigValue();
            if (_configDebugMode || DebugMode)
            {
                UnityEngine.Debug.LogError(message);
            }
        }
    }
}
