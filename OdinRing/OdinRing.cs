// OdinRing
// a Valheim mod skeleton using Jötunn
// 
// File:    OdinRing.cs
// Project: OdinRing

using BepInEx;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using OdinRing.Commands;
using OdinRing.Components;
using OdinRing.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace OdinRing
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    internal class OdinRing : BaseUnityPlugin
    {
        public const string PluginGUID = "com.jotunn.gravenimagerd.OdinRing";
        public const string PluginName = "OdinRing";
        public const string PluginVersion = "0.0.1";

        private Harmony _harmony;

        // Use this class to add your own localization to the game
        // https://valheim-modding.github.io/Jotunn/tutorials/localization.html
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        private void Awake()
        {
            CommandManager.Instance.AddConsoleCommand(new LevelUpCommand());
            CommandManager.Instance.AddConsoleCommand(new StatsCommand());
            CommandManager.Instance.AddConsoleCommand(new ResetCommand());
            CommandManager.Instance.AddConsoleCommand(new AddEitrCommand());
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginGUID);
            var assetBundle = AssetUtils.LoadAssetBundleFromResources("odinring", typeof(OdinRing).Assembly);
            PrefabManager.Instance.AddPrefab(assetBundle.LoadAsset<GameObject>("vfx_eitr.prefab"));
            PrefabManager.Instance.AddPrefab(assetBundle.LoadAsset<GameObject>("vfx_eitrGained.prefab"));
            PrefabManager.Instance.AddPrefab(assetBundle.LoadAsset<GameObject>("CompactLevelUpMenu.prefab"));

            var levelStatueConfig = new PieceConfig()
            {
                Name = "$piece_odinring_levelstatue",
                PieceTable = "Hammer",
                Requirements = new RequirementConfig[]
                {
                    new RequirementConfig("Stone", 20),
                    new RequirementConfig("TrophyEikthyr", 1),
                    new RequirementConfig("Resin", 15)
                }
            };
            var levelStatue = new CustomPiece("$piece_odinring_levelstatue", true, levelStatueConfig);
            var piece = levelStatue.Piece;
            piece.m_icon = assetBundle.LoadAsset<Sprite>("StatueIcon.png");
            levelStatue.FixReference = true;
            PieceManager.Instance.AddPiece(levelStatue);
            var kitbashStatue = KitbashManager.Instance.AddKitbash(levelStatue.PiecePrefab, new KitbashConfig
            {
                Layer = "piece",
                KitbashSources = new List<KitbashSourceConfig>
                {
                    new KitbashSourceConfig
                    {
                        Name = "statue_base",
                        SourcePrefab = "StatueEvil",
                        SourcePath = "default",
                        Position = new Vector3(0f, 0f, 0f),
                        Rotation = Quaternion.Euler(0f, 0f, 0f),
                        Scale = new Vector3(0.7f, 0.7f, 0.7f),
                        Materials = new string[] { "heathrock" }
                    },
                    new KitbashSourceConfig
                    {
                        Name = "statue_eikthyr",
                        SourcePrefab = "TrophyEikthyr",
                        SourcePath = "attach/model",
                        Position = new Vector3(0f, 3.964f, 0.35f),
                        Rotation = Quaternion.Euler(0f, 180f, 0f),
                        Scale = new Vector3(2f, 1.4f, 1.4f),
                        Materials = new string[] { "heathrock" }
                    }
                }
            });
            kitbashStatue.OnKitbashApplied += () =>
            {
                UnityEngine.Object.Destroy(kitbashStatue.Prefab.transform.Find("statue_eikthyr").GetComponent<MeshCollider>());
            };

            levelStatue.PiecePrefab.AddComponent<ForbiddenShrineComponent>();
        }
    }

    [HarmonyPatch(typeof(SEMan), nameof(SEMan.ModifyMaxCarryWeight))]
    public static class SEMan_ModifyMaxCarryWeight_Patch
    {
        public static void Postfix(SEMan __instance, ref float limit)
        {
            if (__instance.m_character is Player player)
            {
                var carryWeightBonus = player.GetOdinRingData().GetCarryWeightBonus();
                limit += carryWeightBonus;
            }
        }
    }
}

