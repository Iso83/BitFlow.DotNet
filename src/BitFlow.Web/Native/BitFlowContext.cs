using System.Runtime.InteropServices;

namespace BitFlow.Web.Native
{
    public sealed class BitFlowContext : IDisposable
    {
        private IntPtr _handle;

        public BitFlowContext()
        {
            _handle = NativeMethods.BF_CreateContext();

            if (_handle == IntPtr.Zero)
                throw new InvalidOperationException(
                    "Failed to create BitFlow context."
                );
        }

        public uint Parse(string expression)
        {
            var result = NativeMethods.BF_Parse(
                _handle,
                expression,
                out var exprId
            );

            if (result != 0)
                throw new InvalidOperationException(
                    GetLastError()
                );

            return exprId;
        }

        public uint Rewrite(uint exprId)
        {
            var result = NativeMethods.BF_Rewrite(
                _handle,
                exprId,
                out var rewrittenId
            );

            if (result != 0)
                throw new InvalidOperationException(
                    GetLastError()
                );

            return rewrittenId;
        }

        public string ToText(uint exprId)
        {
            var ptr = NativeMethods.BF_ToString(
                _handle,
                exprId
            );

            if (ptr == IntPtr.Zero)
                throw new InvalidOperationException(
                    GetLastError()
                );

            try
            {
                return Marshal.PtrToStringUTF8(ptr) ?? "";
            }
            finally
            {
                NativeMethods.BF_FreeString(ptr);
            }
        }

        static string EscapeJson(string text)
        {
            return text
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }

        public string ToLatex(uint exprId)
        {
            var ptr = NativeMethods.BF_ToLatex(
                _handle,
                exprId
            );

            if (ptr == IntPtr.Zero)
                throw new InvalidOperationException(
                    GetLastError()
                );

            try
            {
                return EscapeJson(Marshal.PtrToStringUTF8(ptr) ?? "");
            }
            finally
            {
                NativeMethods.BF_FreeString(ptr);
            }
        }

        public string GetTraceJson()
        {
            var ptr = NativeMethods.BF_GetTraceJson(
                _handle
            );

            if (ptr == IntPtr.Zero)
                throw new InvalidOperationException(
                    GetLastError()
                );

            try
            {
                return Marshal.PtrToStringUTF8(ptr) ?? "[]";
            }
            finally
            {
                NativeMethods.BF_FreeString(ptr);
            }
        }

        private string GetLastError()
        {
            var ptr = NativeMethods.BF_GetLastError(
                _handle
            );

            return ptr == IntPtr.Zero
                ? "Unknown native BitFlow error."
                : Marshal.PtrToStringUTF8(ptr)
                    ?? "Unknown native BitFlow error.";
        }

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {

                NativeMethods.BF_DestroyContext(
                    _handle
                );

                _handle = IntPtr.Zero;
            }
        }
    }
}
