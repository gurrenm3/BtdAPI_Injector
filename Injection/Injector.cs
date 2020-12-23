using MelonLoader;
using System;
using System.Diagnostics;
using System.IO;

namespace BtdAPI_Injector.Injection
{
    internal unsafe class Injector
    {
        private string modsDir = $"{Environment.CurrentDirectory}\\Mods";
        public void InjectBtdApiMods()
        {
            MelonLogger.Log("Searching for BTD API mods...");            

            var files = new DirectoryInfo(modsDir).GetFiles("*.btd6mod");
            if (files.Length == 0)
            {
                MelonLogger.Log("Did not find any BTD API mods");
                return;
            }

            MelonLogger.Log("Found BTD API mods! Injecting mods...");

            var btd6Proc = GetProcess();
            foreach (var file in files)
            {
                InjectDll(file.FullName, btd6Proc);
            }
        }

        public Process GetProcess(string name = "BloonsTD6")
        {
            var processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if (process.ProcessName == name)
                    return process;
            }

            return null;
        }

        //Code from https://github.com/erfg12/memory.dll/blob/master/Memory/memory.cs
        public void InjectDll(String strDllName, Process procToInject)
        {
            IntPtr bytesout;

            // Use try catch to find potential errors
            try
            {
                var test = procToInject.Modules;
                if (test == null)
                    return;
            }
            catch (Exception e)
            {
                if (e.Message.ToLower().Contains("access"))
                {
                    MelonLogger.Log("An exception prevented the mods from being injected... \nException: " + e.Message);
                    MelonLogger.Log("Try re-opening the prgram as Admin");
                    return;
                }
                if (e.Message.ToLower().Contains("readprocess") || e.Message.ToLower().Contains("writeprocess"))
                {
                    MelonLogger.Log("An exception prevented the mods from being injected... \nException: " + e.Message);
                    MelonLogger.Log("Your virus protection might be preventing the injection from working. Try adding an exeption to" +
                        " the program, or disable your virus protection while using the program.");
                    return;
                }
                MelonLogger.Log("An exception prevented the mods from being injected... \nException: " + e.Message);
                return;
            }


            foreach (ProcessModule pm in procToInject.Modules)
            {
                if (pm.ModuleName.StartsWith("inject", StringComparison.InvariantCultureIgnoreCase))
                {
                    MelonLogger.Log($"module name starts with inject: {pm.ModuleName}");
                    return;
                }
            }

            // Removing this for the time being. procToInject seems to never be responding, however it works anyways
            /*if (!procToInject.Responding)
            {
                MelonLogger.Log($"procToInject not responding");
                return;
            }*/

            IntPtr pHandle = Win32.OpenProcess(0x1F0FFF, true, procToInject.Id);

            int lenWrite = strDllName.Length + 1;
            UIntPtr allocMem = Win32.VirtualAllocEx(pHandle, (UIntPtr)null, (uint)lenWrite, 0x00001000 | 0x00002000, 0x04);

            Win32.WriteProcessMemory(pHandle, allocMem, strDllName, (UIntPtr)lenWrite, out bytesout);
            UIntPtr injector = Win32.GetProcAddress(Win32.GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            if (injector == null)
            {
                MelonLogger.Log($"injector is null");
                return;
            }

            IntPtr hThread = Win32.CreateRemoteThread(pHandle, (IntPtr)null, 0, injector, allocMem, 0, out bytesout);
            if (hThread == null)
            {
                MelonLogger.Log($"hThread is null");
                return;
            }

            int Result = Win32.WaitForSingleObject(hThread, 10 * 1000);
            if (Result == 0x00000080L || Result == 0x00000102L)
            {
                if (hThread != null)
                    Win32.CloseHandle(hThread);
                return;
            }
            Win32.VirtualFreeEx(pHandle, allocMem, (UIntPtr)0, 0x8000);

            if (hThread != null)
                Win32.CloseHandle(hThread);

            return;
        }
    }
}
