using Harmony;
using Assets.Scripts.Unity.UI_New.Main;

namespace BtdAPI_Injector.Patches
{
    [HarmonyPatch(typeof(MainMenu),nameof(MainMenu.OnEnable))]
    internal class MainMenu_OnEnable
    {
        [HarmonyPostfix]
        internal static void Postfix()
        {

        }
    }
}