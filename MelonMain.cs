using Assets.Scripts.Unity;
using Assets.Scripts.Unity.UI_New.InGame;
using Il2CppMicrosoft.Win32;
using MelonLoader;
using System;
using System.Diagnostics;
using System.Reflection;

namespace BtdAPI_Injector
{
    public class MelonMain : MelonMod
    {
        internal static string modDir = $"{Environment.CurrentDirectory}\\Mods\\{Assembly.GetExecutingAssembly().GetName().Name}";

        public override void OnApplicationStart()
        {
            CheckRequiredFiles();
            MelonLogger.Log("Mod has finished loading");
        }

        private void CheckRequiredFiles()
        {
            bool hasRuntime = (Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\VisualStudio\14.0\VC\Runtimes") != null);
            bool hasAdditionals = (Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\DevDiv\vc\Servicing\14.0\RuntimeAdditional") != null);

            if (!hasRuntime || !hasAdditionals)// basically checks for x64 vc redist
            {
                MelonLogger.Log("You do not have the x64 Microsoft Visual C++ Redistributable for Visual Studio 2015, 2017 and 2019 installed. " +
                    "Clicking OK will bring you to the direct download link. Mods will not work without it.", "Error!");
                Process.Start("https://aka.ms/vs/16/release/vc_redist.x64.exe");
            }
        }

        public override void OnUpdate()
        {
            if (Game.instance is null)
                return;

            if (InGame.instance is null)
                return;

        }
    }
}