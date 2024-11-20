# Cheat Engine Lib
Execute per DLL Lua Scripts.

# Cheat Engine Version
7.5

# Examples (Soon more Code)
C# Code
```c#
        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        extern static IntPtr LoadLibraryW(String strLib);

        public delegate void ILua([MarshalAs(UnmanagedType.BStr)] string script);

        public IntPtr dllInit;

        public ILua iLua;
```
