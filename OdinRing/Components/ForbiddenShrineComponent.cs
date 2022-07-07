using Jotunn.Managers;
using OdinRing.Data;
using OdinRing.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OdinRing.Components
{
    public class ForbiddenShrineComponent : MonoBehaviour, Interactable, Hoverable
    {
		GameObject menu;
		OdinRingData currentData;
		OdinRingData effectiveData;
		int totalEitrForLevels;
		Button confirmButton;
		Button closeButton;
		Button strUp;
		Button strDown;
		Button dexUp;
		Button dexDown;
		Button endUp;
		Button endDown;
		Button vigUp;
		Button vigDown;
		readonly Dictionary<string, Text> menuTexts = new Dictionary<string, Text>();

		private void Awake()
        {
        }

		public bool Interact(Humanoid user, bool repeat, bool alt)
		{
			if (repeat)
			{
				return false;
			}

			if (user == Player.m_localPlayer)
			{
				if (!InUseDistance(user))
				{
					return false;
				}
				Player.m_localPlayer.GetOdinRingData().InForbiddenShrine = this;
				ShowLevelMenu();
				return false;
			}

			return false;
		}

		public void Update()
        {
			if (menu == null)
            {
				return;
            }

			bool canLevelUp = effectiveData.Eitr > currentData.GetEitrToLevel(effectiveData.Level + 1);
			confirmButton.interactable = effectiveData.Level > currentData.Level;
			strUp.interactable = canLevelUp;
			dexUp.interactable = canLevelUp;
			endUp.interactable = canLevelUp;
			vigUp.interactable = canLevelUp;
			if (!canLevelUp)
            {
				if (menuTexts.ContainsKey("EitrNeededText"))
				{
					menuTexts["EitrNeededText"].color = Color.red;
				}
            }

			strDown.interactable = effectiveData.Attributes[OdinRingData.Attribute.Strength] != currentData.Attributes[OdinRingData.Attribute.Strength];
			dexDown.interactable = effectiveData.Attributes[OdinRingData.Attribute.Dexterity] != currentData.Attributes[OdinRingData.Attribute.Dexterity];
			endDown.interactable = effectiveData.Attributes[OdinRingData.Attribute.Endurance] != currentData.Attributes[OdinRingData.Attribute.Endurance];
			vigDown.interactable = effectiveData.Attributes[OdinRingData.Attribute.Vigor] != currentData.Attributes[OdinRingData.Attribute.Vigor];
		}

		void ConfirmChoices()
        {
			currentData.Level = effectiveData.Level;
			currentData.Eitr = effectiveData.Eitr;
			foreach (var attribute in effectiveData.Attributes)
            {
				currentData.Attributes[attribute.Key] = attribute.Value;
            }
			CloseMenu();
        }

		void StatUp(OdinRingData.Attribute attribute)
		{
			effectiveData.Level += 1;
			totalEitrForLevels += effectiveData.GetEitrToLevel(effectiveData.Level + 1);
			effectiveData.Eitr -= effectiveData.GetEitrToLevel(effectiveData.Level);
			effectiveData.Attributes[attribute] += 1;
			UpdateEffectiveTexts();
		}

		void StatDown(OdinRingData.Attribute attribute)
		{
			totalEitrForLevels -= effectiveData.GetEitrToLevel(effectiveData.Level + 1);
			effectiveData.Eitr += effectiveData.GetEitrToLevel(effectiveData.Level);
			effectiveData.Level -= 1;
			effectiveData.Attributes[attribute] -= 1;
			UpdateEffectiveTexts();
		}

		void StrUp()
        {
			StatUp(OdinRingData.Attribute.Strength);
        }

		void DexUp()
		{
			StatUp(OdinRingData.Attribute.Dexterity);
		}

		void EndUp()
		{
			StatUp(OdinRingData.Attribute.Endurance);
		}

		void VigUp()
		{
			StatUp(OdinRingData.Attribute.Vigor);
		}

		void StrDown()
		{
			StatDown(OdinRingData.Attribute.Strength);
		}

		void DexDown()
		{
			StatDown(OdinRingData.Attribute.Dexterity);
		}

		void EndDown()
		{
			StatDown(OdinRingData.Attribute.Endurance);
		}

		void VigDown()
		{
			StatDown(OdinRingData.Attribute.Vigor);
		}

		void ShowLevelMenu()
        {
			Jotunn.Logger.LogInfo("Opening menu");
			menu = PrefabManager.Instance.CreateClonedPrefab("menu", "CompactLevelUpMenu");
			menu.transform.SetParent(GUIManager.CustomGUIFront.transform);
			menu.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			var buttons = menu.GetComponentsInChildren<Button>();
			foreach (var button in buttons)
			{
				if (button.name == "Confirm")
				{
					confirmButton = button;
					confirmButton.onClick.AddListener(ConfirmChoices);
				}
				else if (button.name == "Close")
				{
					closeButton = button;
					closeButton.onClick.AddListener(CloseMenu);
				}
				else if (button.name.Contains("Up"))
				{
					if (button.name == "StrUp")
					{
						strUp = button;
						strUp.onClick.AddListener(StrUp);
					}
					else if (button.name == "DexUp")
					{
						dexUp = button;
						dexUp.onClick.AddListener(DexUp);
					}
					else if (button.name == "EndUp")
					{
						endUp = button;
						endUp.onClick.AddListener(EndUp);
					}
					else if (button.name == "VigUp")
					{
						vigUp = button;
						vigUp.onClick.AddListener(VigUp);
					}
				}
				else if (button.name.Contains("Down"))
                {
					if (button.name == "StrDown")
					{
						strDown = button;
						strDown.onClick.AddListener(StrDown);
					}
					else if (button.name == "DexDown")
					{
						dexDown = button;
						dexDown.onClick.AddListener(DexDown);
					}
					else if (button.name == "EndDown")
					{
						endDown = button;
						endDown.onClick.AddListener(EndDown);
					}
					else if (button.name == "VigDown")
					{
						vigDown = button;
						vigDown.onClick.AddListener(VigDown);
					}
				}
            }

			currentData = Player.m_localPlayer.GetOdinRingData();
			effectiveData = currentData.CloneLevelData();
			totalEitrForLevels = currentData.GetEitrToLevel(currentData.Level + 1);

			var texts = menu.GetComponentsInChildren<Text>();
			foreach (Text text in texts)
            {
				if (text.name == "CurrentDataText")
				{
					text.text = $"{currentData.Level}\n\n{currentData.Eitr}";
				}
				else if (text.name == "CurrentAttributesText")
				{
					text.text = String.Join("\n", currentData.Attributes.Values);
				}
				else if (text.name.EndsWith("Label"))
                {
					text.text = LocalizationManager.Instance.TryTranslate(text.name);
                }
				else
                {
					if (!menuTexts.ContainsKey(text.name))
					{
						menuTexts.Add(text.name, text);
					}
                }
            }

			UpdateEffectiveTexts();

			GUIManager.BlockInput(true);
        }

		void UpdateEffectiveTexts()
        {
			menuTexts["EffectiveDataText"].text = $"{effectiveData.Level}\n\n{effectiveData.Eitr}";
			menuTexts["EffectiveAttributesText"].text = String.Join("\n", effectiveData.Attributes.Values);
			menuTexts["EitrNeededText"].text = totalEitrForLevels.ToString();

			menuTexts["MeleeDamageText"].text = $"+{(effectiveData.GetMeleeDamageBonus() - 1.0f) * 100:F0}%";
			menuTexts["MeleeDamageText"].color = effectiveData.GetMeleeDamageBonus() > currentData.GetMeleeDamageBonus() ? Color.green : Color.white;
			menuTexts["RangedDamageText"].text = $"+{(effectiveData.GetRangedDamageBonus() - 1.0f) * 100:F0}%";
			menuTexts["RangedDamageText"].color = effectiveData.GetRangedDamageBonus() > currentData.GetRangedDamageBonus() ? Color.green : Color.white;
			menuTexts["MaxHealthText"].text = $"+{(int)effectiveData.GetMaxHealthBonus()}";
			menuTexts["MaxHealthText"].color = effectiveData.GetMaxHealthBonus() > currentData.GetMaxHealthBonus() ? Color.green : Color.white;
			menuTexts["MaxStaminaText"].text = $"+{(int)effectiveData.GetMaxStaminaBonus()}";
			menuTexts["MaxStaminaText"].color = effectiveData.GetMaxStaminaBonus() > currentData.GetMaxStaminaBonus() ? Color.green : Color.white;
			menuTexts["MovementSpeedText"].text = $"+{(effectiveData.GetMovementSpeedBonus() - 1.0f) * 100:F1}%";
			menuTexts["MovementSpeedText"].color = effectiveData.GetMovementSpeedBonus() > currentData.GetMovementSpeedBonus() ? Color.green : Color.white;
			menuTexts["JumpStaminaText"].text = $"-{(1.0f - effectiveData.GetJumpStaminaBonus()) * 100:F1}%";
			menuTexts["JumpStaminaText"].color = effectiveData.GetJumpStaminaBonus() < currentData.GetJumpStaminaBonus() ? Color.green : Color.white;
			menuTexts["DamageReductionText"].text = $"-{(1.0f - effectiveData.GetPhysicalResistBonus()) * 100:F1}%";
			menuTexts["DamageReductionText"].color = effectiveData.GetPhysicalResistBonus() < currentData.GetPhysicalResistBonus() ? Color.green : Color.white;
			menuTexts["CarryWeightText"].text = $"+{(int)effectiveData.GetCarryWeightBonus()}";
			menuTexts["CarryWeightText"].color = effectiveData.GetCarryWeightBonus() > currentData.GetCarryWeightBonus() ? Color.green : Color.white;
			menuTexts["FoodDurationText"].text = $"+{(effectiveData.GetFoodDurationBonus() - 1.0f) * 100:F1}%";
			menuTexts["FoodDurationText"].color = effectiveData.GetFoodDurationBonus() > currentData.GetFoodDurationBonus() ? Color.green : Color.white;
		}

		public void CloseMenu()
        {
			Player.m_localPlayer.GetOdinRingData().InForbiddenShrine = null;
			Jotunn.Logger.LogInfo("Closing menu");
			menuTexts.Clear();
			UnityEngine.Object.Destroy(menu);
			GUIManager.BlockInput(false);
		}

		bool InUseDistance(Humanoid human)
		{
			return Vector3.Distance(human.transform.position, base.transform.position) < 3f;
		}

        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
			return false;
        }

        public string GetHoverText()
        {
			if (!InUseDistance(Player.m_localPlayer))
			{
				return Localization.instance.Localize("<color=grey>$piece_toofar</color>");
			}
			return Localization.instance.Localize("$piece_odinring_levelstatue\n[<color=yellow><b>$KEY_Use</b></color>] $piece_use ");
		}

        public string GetHoverName()
        {
			return "$piece_odinring_levelstatue";
		}
    }
}
