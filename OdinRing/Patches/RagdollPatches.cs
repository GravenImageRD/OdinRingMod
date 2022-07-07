using HarmonyLib;
using Jotunn.Managers;
using OdinRing.Components;
using OdinRing.Extensions;
using System.Linq;
using UnityEngine;

namespace OdinRing.Patches
{
    [HarmonyPatch(typeof(Ragdoll), "Setup")]
    public static class Ragdoll_Setup_Patch
    {
        public static void Postfix(Ragdoll __instance, CharacterDrop characterDrop)
        {
            // Only care about monsters.
            if (characterDrop == null || characterDrop.m_character == null || characterDrop.m_character.IsPlayer())
            {
                return;
            }

            __instance.m_nview.m_zdo.Set("monsterZDOID", characterDrop.m_character.GetZDOID());
            __instance.m_nview.m_zdo.Set("monsterName", characterDrop.m_character.GetHoverName());
        }
    }

    [HarmonyPatch(typeof(Ragdoll), "SpawnLoot")]
    public static class Ragdoll_SpawnLoot_Patch
    {
        public static void Postfix(Ragdoll __instance, Vector3 center)
        {
            var monsterId = __instance.m_nview.m_zdo.GetZDOID("monsterZDOID");
            var monsterName = __instance.m_nview.m_zdo.GetString("monsterName");
            if (monsterId != null)
            {
                // Spawn an Eitr object for each player that tagged the monster.
                var taggedPlayers = Player.GetAllPlayers().Where(x => x.GetOdinRingData().UntagMonster(monsterId));
                foreach (var player in taggedPlayers)
                {
                    var eitrPrefab = PrefabManager.Instance.GetPrefab("vfx_eitr");
                    var eitr = GameObject.Instantiate(eitrPrefab, center + (Vector3.up * 0.25f), Quaternion.identity);
                    var eitrComponent = eitr.AddComponent<EitrComponent>();
                    eitrComponent.Player = player;
                    eitrComponent.Monster = monsterName;
                }
            }
        }
    }
}
