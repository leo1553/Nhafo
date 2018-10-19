using System.Runtime.CompilerServices;

namespace Nhafo.Code.Utils {
    public static class Debug {
        private static string FormatFilePath(string callingFilePath, string caller) {
            callingFilePath = callingFilePath.Remove(callingFilePath.LastIndexOf('.'));
            callingFilePath = callingFilePath.Substring(callingFilePath.LastIndexOf('\\') + 1);
            return string.Format("{1} at {0}", callingFilePath, caller);
        }

        public static void Log(object obj,
            _SeparatorClass separatorClass = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string callingFilePath = null, [CallerMemberName] string caller = null) {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("{0}:{1} - {2}", FormatFilePath(callingFilePath, caller), lineNumber, obj));
#endif
        }

        public static void Log(string format, object arg,
            _SeparatorClass separatorClass = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string callingFilePath = null, [CallerMemberName] string caller = null) {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("{0}:{1} - {2}", FormatFilePath(callingFilePath, caller), lineNumber,
                string.Format(format, arg)));
#endif
        }

        public static void Log(string format, object arg1, object arg2,
            _SeparatorClass separatorClass = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string callingFilePath = null, [CallerMemberName] string caller = null) {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("{0}:{1} - {2}", FormatFilePath(callingFilePath, caller), lineNumber,
                string.Format(format, arg1, arg2)));
#endif
        }

        public static void Log(string format, object arg1, object arg2, object arg3,
            _SeparatorClass separatorClass = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string callingFilePath = null, [CallerMemberName] string caller = null) {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("{0}:{1} - {2}", FormatFilePath(callingFilePath, caller), lineNumber,
                string.Format(format, arg1, arg2, arg3)));
#endif
        }

        public static void Log(string format, object arg1, object arg2, object arg3, object arg4,
            _SeparatorClass separatorClass = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string callingFilePath = null, [CallerMemberName] string caller = null) {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("{0}:{1} - {2}", FormatFilePath(callingFilePath, caller), lineNumber,
                string.Format(format, arg1, arg2, arg3, arg4)));
#endif
        }

        public static void Log(string format, object arg1, object arg2, object arg3, object arg4, object arg5,
            _SeparatorClass separatorClass = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string callingFilePath = null, [CallerMemberName] string caller = null) {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("{0}:{1} - {2}", FormatFilePath(callingFilePath, caller), lineNumber,
                string.Format(format, arg1, arg2, arg3, arg4, arg5)));
#endif
        }

        public static void Log(string format, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6,
            _SeparatorClass separatorClass = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string callingFilePath = null, [CallerMemberName] string caller = null) {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("{0}:{1} - {2}", FormatFilePath(callingFilePath, caller), lineNumber,
                string.Format(format, arg1, arg2, arg3, arg4, arg5, arg6)));
#endif
        }

        public static void Log(string format, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7,
            _SeparatorClass separatorClass = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string callingFilePath = null, [CallerMemberName] string caller = null) {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("{0}:{1} - {2}", FormatFilePath(callingFilePath, caller), lineNumber,
                string.Format(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7)));
#endif
        }

        public static void Log(string format, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8,
            _SeparatorClass separatorClass = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string callingFilePath = null, [CallerMemberName] string caller = null) {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("{0}:{1} - {2}", FormatFilePath(callingFilePath, caller), lineNumber,
                string.Format(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8)));
#endif
        }

        public static void Log(string format, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9,
            _SeparatorClass separatorClass = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string callingFilePath = null, [CallerMemberName] string caller = null) {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("{0}:{1} - {2}", FormatFilePath(callingFilePath, caller), lineNumber,
                string.Format(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9)));
#endif
        }

        public class _SeparatorClass {
            private _SeparatorClass() { }
        }
    }
}