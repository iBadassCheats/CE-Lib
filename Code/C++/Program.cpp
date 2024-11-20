#include <windows.h>
#include <iostream>
#include <string>

class DLLHelper {
private:
    HMODULE dllHandle = nullptr;

public:
    using ILua = int(__stdcall*)(const wchar_t* luaCommand);

    ILua LuaFunction = nullptr;

    bool LoadFunctions(const std::wstring& dllName, const std::string& functionName) {
        dllHandle = LoadLibraryW(dllName.c_str());
        if (!dllHandle) {
            std::wcerr << L"Failed to load DLL: " << dllName
                << L" (Error Code: " << GetLastError() << L")" << std::endl;
            return false;
        }

        FARPROC procAddress = GetProcAddress(dllHandle, functionName.c_str());
        if (!procAddress) {
            std::cerr << "Failed to get function: " << functionName
                << " (Error Code: " << GetLastError() << ")" << std::endl;
            UnloadLibrary();
            return false;
        }

        LuaFunction = reinterpret_cast<ILua>(procAddress);
        return true;
    }

    void UnloadLibrary() {
        if (dllHandle) {
            FreeLibrary(dllHandle);
            dllHandle = nullptr;
        }
    }

    ~DLLHelper() {
        UnloadLibrary();
    }
};

int main() {
    DLLHelper dllHelper;

    if (dllHelper.LoadFunctions(L"CE-Import.dll", "ILua")) {
        if (dllHelper.LuaFunction) {
            dllHelper.LuaFunction(L"print('Hello from CE DLL')");
        }
    }
    else {
        std::cerr << "Failed to load functions." << std::endl;
    }

    return 0;
}
