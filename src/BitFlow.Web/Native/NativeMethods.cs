using System.Runtime.InteropServices;

namespace BitFlow.Web.Native
{
    internal static class NativeMethods
    {
        [DllImport("BitFlow.Native", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BF_CreateContext();

        [DllImport("BitFlow.Native", CallingConvention = CallingConvention.Cdecl)]
        public static extern void BF_DestroyContext(IntPtr context);

        [DllImport("BitFlow.Native", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BF_Parse(
            IntPtr context,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string expression,
            out uint exprId);

        [DllImport("BitFlow.Native", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BF_Rewrite(
            IntPtr context,
            uint exprId,
            out uint outExprId);

        [DllImport("BitFlow.Native", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BF_ToString(
            IntPtr context,
            uint exprId);

        [DllImport("BitFlow.Native", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BF_GetTraceJson(
            IntPtr context);

        [DllImport("BitFlow.Native", CallingConvention = CallingConvention.Cdecl)]
        public static extern void BF_FreeString(
            IntPtr value);

        [DllImport("BitFlow.Native", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr BF_GetLastError(
            IntPtr context);
    }
}
