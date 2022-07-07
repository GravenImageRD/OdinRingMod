using HarmonyLib;
using OdinRing.Components;
using OdinRing.Extensions;

namespace OdinRing.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.EatFood))]
    public static class Player_EatFood_Patch
    {
        public static void Postfix(Player __instance, ItemDrop.ItemData item)
        {
            var food = __instance.m_foods.Find(x => x.m_name == item.m_shared.m_name);
            food.m_time *= __instance.GetOdinRingData().GetFoodDurationBonus();
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.GetJogSpeedFactor))]
    public static class Player_GetJogSpeedFactor_Patch
    {
        public static void Postfix(Player __instance, ref float __result)
        {
            var moveSpeedBonus = __instance.GetOdinRingData().GetMovementSpeedBonus();
            __result *= moveSpeedBonus;
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
    public static class Player_GetTotalFoodValue_Patch
    {
        public static void Postfix(Player __instance, ref float hp, ref float stamina)
        {
            hp += __instance.GetOdinRingData().Attributes[Data.OdinRingData.Attribute.Vigor];
            stamina += __instance.GetOdinRingData().Attributes[Data.OdinRingData.Attribute.Endurance];
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.GetBaseFoodHP))]
    public static class Vigor_Character_GetBaseFood_Patch
    {
        public static void Postfix(Player __instance, ref float __result)
        {
            __result += __instance.GetOdinRingData().Attributes[Data.OdinRingData.Attribute.Vigor];
        }
    }

    [HarmonyPatch(typeof(Player), "Load")]
    public static class Player_Load_Patch
    {
        public static void Postfix(Player __instance)
        {
            __instance.gameObject.AddComponent<OdinRingComponent>().Load();
        }
    }

    [HarmonyPatch(typeof(Player), "Save")]
    public static class Player_Save_Patch
    {
        public static bool Prefix(Player __instance)
        {
            __instance.gameObject.GetComponent<OdinRingComponent>().Save();
            return true;
        }
    }
}
