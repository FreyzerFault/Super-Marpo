using System;
using Object = UnityEngine.Object;
using UniDebug = UnityEngine.Debug;

namespace Debugging
{
    public static class Debug
    {
        private static bool active = true;

        public static bool Active
        {
            get => active;
            set
            {
                active = value;
                onDebugModeChanged?.Invoke(value);
            }
        }
        
        public static event Action<bool> onDebugModeChanged;

        public static void Log(string message, Object ctx = null) => UniDebug.Log(message, ctx);

        public static void LogWarning(string message, Object ctx = null) => UniDebug.LogWarning(message, ctx);

        public static void LogError(string message, Object ctx = null) => UniDebug.LogError(message, ctx);

        public static void LogException(Exception exception, Object ctx = null) => UniDebug.LogException(exception, ctx);
    }
}
