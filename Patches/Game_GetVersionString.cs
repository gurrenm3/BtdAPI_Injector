using Harmony;
using Assets.Scripts.Unity;
using BtdAPI_Injector.Injection;
using System;
using System.IO;

namespace BtdAPI_Injector.Patches
{
    [HarmonyPatch(typeof(Game), nameof(Game.GetVersionString))]
    internal class Game_GetVersionString
    {
        [HarmonyPostfix]
        internal static void Postfix()
        {
            Injector injector = new Injector();
            injector.InjectBtdApiMods();
        }
    }
}