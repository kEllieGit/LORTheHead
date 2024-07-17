using UI;
using HarmonyLib;

namespace TheHead
{
    public static class Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIAlarmPopup), nameof(UIAlarmPopup.OnYesButton))]
        public static bool LoadCustomBattle(UIAlarmPopup __instance)
        {
            var alarmType = (UIAlarmType)AccessTools.Field(typeof(UIAlarmPopup), "currentAlarmType").GetValue(__instance);
            if (alarmType == UIAlarmType.StartANOTHERETC)
            {
                StartCustomBattle();
                __instance.Close();
                return false;
            }

            return true;
        }

        private static void StartCustomBattle(bool showStory = true)
        {
            // Replace Binah with non-degraded version
            Singleton<BookXmlList>.Instance.GetData(8).EquipEffect = Singleton<BookXmlList>.Instance.GetData(new LorId(Parameters.PackageId, 10000550), true).EquipEffect;
            Singleton<DeckXmlList>.Instance.GetData(8)._cardIdList = Singleton<DeckXmlList>.Instance.GetData(new LorId(Parameters.PackageId, 379330000))._cardIdList;
            Singleton<DeckXmlList>.Instance.GetData(8).cardIdList = Singleton<DeckXmlList>.Instance.GetData(new LorId(Parameters.PackageId, 379330000)).cardIdList;

            UI.UIController.Instance.CallUIPhase(UIPhase.Sephirah);
            UI.UIController.Instance.PrepareBattleEndContents(Singleton<StageClassInfoList>.Instance.GetData(new LorId(Parameters.PackageId, 379390001)), showStory, false, true);
        }
    }
}
