using System;
using System.Runtime.InteropServices;

internal class DLLHelper
{
    [DllImport("kernel32.dll", EntryPoint = "LoadLibraryW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr LoadLibraryW(string lpLibFileName);

    [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", SetLastError = true)]
    private static extern bool FreeLibrary(IntPtr hModule);

    public delegate int ILua([MarshalAs(UnmanagedType.BStr)] string luaCommand);

    private IntPtr dllHandle = IntPtr.Zero;
    public ILua LuaFunction { get; private set; } = null;

    public bool LoadFunctions(string dllName, string functionName)
    {
        dllHandle = LoadLibraryW(dllName);
        if (dllHandle == IntPtr.Zero)
        {
            Console.WriteLine($"Failed to load DLL: {dllName} (Error Code: {Marshal.GetLastWin32Error()})");
            return false;
        }

        IntPtr procAddress = GetProcAddress(dllHandle, functionName);
        if (procAddress == IntPtr.Zero)
        {
            Console.WriteLine($"Failed to get function: {functionName} (Error Code: {Marshal.GetLastWin32Error()})");
            UnloadLibrary();
            return false;
        }

        LuaFunction = (ILua)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(ILua));
        return true;
    }

    public void UnloadLibrary()
    {
        if (dllHandle != IntPtr.Zero)
        {
            FreeLibrary(dllHandle);
            dllHandle = IntPtr.Zero;
        }
    }
}

internal class Program
{
    private static DLLHelper dllHelper = new();

    internal static void Main()
    {
        if (dllHelper.LoadFunctions("CE-Import.dll", "ILua"))
        {
            dllHelper.LuaFunction?.Invoke("print('Hello from CE DLL')");
        }
        else
        {
            Console.WriteLine("Failed to load functions.");
        }

        dllHelper.UnloadLibrary();
    }
}
