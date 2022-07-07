using fastJSON;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using OdinRing.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OdinRing.Components
{
    [RequireComponent(typeof(Player))]
    public class OdinRingComponent : MonoBehaviour
    {
        const float ratio4by3 = 4.0f / 3.0f;
        const float ratio5by4 = 5.0f / 4.0f;
        const float ratio16by9 = 16.0f / 9.0f;
        const float ratio21by9 = 21.0f / 9.0f;
        const float ratio43by18 = 43.0f / 18.0f;
        const string SaveDataKey = OdinRing.PluginGUID + "+" + "OdinRingData";
        Player player;
        GameObject eitrText;
        int lastWidth;
        int lastHeight;
        int lastEitrTotal;
        float nextUpdate;

        public OdinRingData Data = new OdinRingData();

        public void Awake()
        {
            player = GetComponent<Player>();
            Load();
            lastWidth = 0;
            lastHeight = 0;
            lastEitrTotal = Data.Eitr;
            nextUpdate = Time.time;
        }

        public Vector3 GetAnchorByRatio(float ratio)
        {
            if (ratio > (ratio21by9 - 0.03f) && ratio < (ratio21by9 + 0.03f))
            {
                Jotunn.Logger.LogInfo($"Adjusting Eitr text for 21:9 resolution");
                return new Vector2(0.06f, 0.98f);
            }
            else if (ratio > (ratio43by18 - 0.03f) && ratio < (ratio43by18 + 0.03f))
            {
                Jotunn.Logger.LogInfo($"Adjusting Eitr text for 43:18 resolution");
                return new Vector2(0.06f, 0.97f);
            }
            else if (ratio > (ratio16by9 - 0.03f) && ratio < (ratio16by9 + 0.03f))
            {
                Jotunn.Logger.LogInfo($"Adjusting Eitr text for 16:9 resolution");
                return new Vector2(0.08f, 0.97f);
            }
            else if (ratio > (ratio4by3 - 0.03f) && ratio < (ratio4by3 + 0.03f) ||
                     ratio > (ratio5by4 - 0.03f) && ratio < (ratio5by4 + 0.03f))
            {
                Jotunn.Logger.LogInfo($"Adjusting Eitr text for 4:3 or 5:4 resolution");
                return new Vector2(0.08f, 0.98f);
            }

            return Vector3.zero;
        }

        public void Update()
        {
            if (ZNetScene.instance == null || ZNetScene.instance.name != "_GameMain")
            {
                return;
            }

            if (eitrText == null)
            {
                Vector3 anchorVector = GetAnchorByRatio((1.0f * Screen.width) / Screen.height);
                var eitrString = LocalizationManager.Instance.TryTranslate("odinring_eitr");
                eitrText = GUIManager.Instance.CreateText($"{eitrString}: {Data.Eitr}", GUIManager.CustomGUIBack.transform, anchorVector, anchorVector, new Vector2(0f, 0f), GUIManager.Instance.AveriaSerif, 16, Color.white, true, Color.black, 100f, 20f, true);
            }

            if ((player.InCutscene() || player.IsDead()) && eitrText.activeSelf)
            {
                eitrText.SetActive(false);
            }
            else if (!eitrText.activeSelf && !player.InCutscene() && !player.IsDead())
            {
                eitrText.SetActive(true);
            }

            if (Screen.width != lastWidth || Screen.height != lastHeight)
            {
                float ratio = (1.0f * Screen.width) / Screen.height;
                Jotunn.Logger.LogInfo($"Changing resolution from {lastWidth}x{lastHeight} to {Screen.width}x{Screen.height} with a ratio of {ratio}");
                lastWidth = Screen.width;
                lastHeight = Screen.height;
                var rectTransform = eitrText.GetComponent<RectTransform>();
                rectTransform.anchorMin = rectTransform.anchorMax = GetAnchorByRatio(ratio);
            }

            if (lastEitrTotal != Data.Eitr && Time.time > nextUpdate)
            {
                int diff = Data.Eitr - lastEitrTotal;
                int change = 1;
                if (diff > 600 || diff < -600)
                {
                    nextUpdate = Time.time + 0.002f;
                    change = 100;
                }
                else if (diff > 300 || diff < -300)
                {
                    nextUpdate = Time.time + 0.005f;
                    change = 60;
                }
                else if (diff > 100 || diff < -100)
                {
                    nextUpdate = Time.time + 0.01f;
                    change = 30;
                }
                else if (diff > 50 || diff < -50)
                {
                    nextUpdate = Time.time + 0.02f;
                    change = 15;
                }
                else if (diff > 25 || diff < -25)
                {
                    nextUpdate = Time.time + 0.03f;
                    change = 5;
                }
                else if (diff > 10 || diff < -10)
                {
                    nextUpdate = Time.time + 0.04f;
                }
                else
                {
                    nextUpdate = Time.time + 0.05f;
                }

                if (diff > 0)
                {
                    lastEitrTotal += change;
                }
                else
                {
                    lastEitrTotal -= change;
                }

                var eitrString = LocalizationManager.Instance.TryTranslate("odinring_eitr");
                eitrText.GetComponent<Text>().text = $"{eitrString}: {lastEitrTotal}";
            }
        }

        public void ShowUI()
        {
            if (eitrText != null)
            {
                eitrText.SetActive(true);
            }
        }

        public void HideUI()
        {
            if (eitrText != null)
            {
                eitrText.SetActive(false);
            }
        }

        public void Save()
        {
            if (player == null)
            {
                return;
            }

            Jotunn.Logger.LogInfo("OdinRing: Saving player data");
            player.m_knownTexts[SaveDataKey] = JSON.ToJSON(Data);
        }

        public void Load()
        {
            if (player.m_knownTexts.TryGetValue(SaveDataKey, out string jsonData))
            {
                try
                {
                    var savedData = JSON.ToObject<OdinRingData>(jsonData);
                    Data.Level = savedData.Level;
                    Data.Eitr = savedData.Eitr;
                    Data.KilledEikthyr = savedData.KilledEikthyr;
                    Data.KilledElder = savedData.KilledElder;
                    Data.KilledBonemass = savedData.KilledBonemass;
                    Data.KilledModer = savedData.KilledModer;
                    Data.KilledYagluth = savedData.KilledYagluth;
                    foreach (var attribute in savedData.Attributes)
                    {
                        Data.Attributes[attribute.Key] = attribute.Value;
                    }
                    Jotunn.Logger.LogInfo("Loaded from JSON data.");
                    return;
                }
                catch
                {
                }
            }
        }

        [HarmonyPatch(typeof(TextsDialog), nameof(TextsDialog.UpdateTextsList))]
        public static class TextsDialog_UpdateTextsList_Patch
        {
            public static void Postfix(TextsDialog __instance)
            {
                __instance.m_texts.RemoveAll(x => x.m_topic.Equals(OdinRingComponent.SaveDataKey, StringComparison.InvariantCulture));
            }
        }
    }
}
