using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace CustomStomachStorage
{
    public static class UDebug
    {
        public static bool DebugMode = false;
        public static bool LogReset = true;

        private static readonly object _lock = new object();
        private static readonly string _logFilePath = "CustomStomachStorageLog.txt";
        private static bool _isInitialized = false;

        private static readonly StringBuilder _buffer = new StringBuilder();
        private static DateTime _lastFlush = DateTime.Now;

        // 缓存条件结果，避免每次重复判断
        private static bool ShouldLog =>
            DebugMode || (MyOptions.Instance?.DebugMode?.Value ?? false);

        public static void Log(string message) => LogInternal(message, UnityEngine.Debug.Log);
        public static void LogWarning(string message) => LogInternal(message, UnityEngine.Debug.LogWarning);
        public static void LogError(string message) => LogInternal(message, UnityEngine.Debug.LogError);

        private static void LogInternal(string message, Action<string> unityLogger)
        {
            if (!ShouldLog || string.IsNullOrEmpty(message)) return;

            unityLogger(message);
            OutputLog(message);
        }

        public static void OutputLog(string content)
        {
            if (string.IsNullOrEmpty(content)) return;

            lock (_lock)
            {
                _buffer.AppendLine($"{DateTime.Now:HH:mm:ss} {content}");

                // 每秒或每50条刷新一次
                if ((DateTime.Now - _lastFlush).TotalSeconds > 1 || _buffer.Length > 5000)
                {
                    FlushBuffer();
                }

                /*try
                {
                    InitializeLogFile();
                    File.AppendAllText(_logFilePath, $"{DateTime.Now:HH:mm:ss} {content}{Environment.NewLine}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"日志写入失败: {ex.Message}");
                }*/
            }
        }

        private static void FlushBuffer()
        {
            try
            {
                InitializeLogFile();
                File.AppendAllText(_logFilePath, _buffer.ToString());
                _buffer.Clear();
                _lastFlush = DateTime.Now;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"日志写入失败: {ex.Message}");
            }
        }

        private static void InitializeLogFile()
        {
            if (_isInitialized && !LogReset) return;

            var header = LogReset
                ? $"=== 新日志 {DateTime.Now:yyyy-MM-dd HH:mm:ss} ==={Environment.NewLine}{Environment.NewLine}"
                : string.Empty;

            if (LogReset || !File.Exists(_logFilePath))
            {
                File.WriteAllText(_logFilePath, header);
                LogReset = false;
            }

            _isInitialized = true;
        }
    }
}