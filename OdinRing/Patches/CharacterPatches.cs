using HarmonyLib;
using OdinRing.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinRing.Patches
{
    [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
    public static class ModifyResistance_Character_RPC_Damage_Patch
    {
        public static void Prefix(Character __instance, HitData hit)
        {
            if (__instance is Player player)
            {
                float physicalResistBonus = player.GetOdinRingData().GetPhysicalResistBonus();
                hit.m_damage.m_blunt *= physicalResistBonus;
                hit.m_damage.m_slash *= physicalResistBonus;
                hit.m_damage.m_pierce *= physicalResistBonus;
                hit.m_damage.m_chop *= physicalResistBonus;
            }
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.Jump))]
    public static class Character_Jump_Patch
    {
        public static bool Prefix(Character __instance, ref float ___m_jumpStaminaUsage, out float __state)
        {
            __state = ___m_jumpStaminaUsage;
            if (__instance is Player player)
            {
                ___m_jumpStaminaUsage *= player.GetOdinRingData().GetJumpStaminaBonus();
            }
            return true;
        }

        public static void Postfix(Character __instance, ref float ___m_jumpStaminaUsage, float __state)
        {
            if (__instance is Player player)
            {
                ___m_jumpStaminaUsage = __state;
            }
        }
    }

    [HarmonyPatch(typeof(Character), "ApplyDamage")]
    public static class Character_ApplyDamage_Patch
    {
        public static bool Prefix(Character __instance, HitData hit)
        {
            Player player = Player.GetAllPlayers().FirstOrDefault(x => x.GetZDOID() == hit.m_attacker);
            if (player != null)
            {
                var odinRingData = player.GetOdinRingData();
                var totalDamage = hit.GetTotalDamage();
                float mod = hit.m_ranged ? odinRingData.GetRangedDamageBonus() : odinRingData.GetMeleeDamageBonus();
                hit.ApplyModifier(mod);

                // If the hit will do damage, "tag" the enemy for the player for Eitr drops.
                if (hit.GetTotalDamage() >= 1.0f)
                {
                    odinRingData.TagMonster(__instance.GetZDOID());
                }
            }

            return true;
        }
    }
}
