using Jotunn.Managers;
using OdinRing.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OdinRing.Data
{
    public class OdinRingData
    {
        public delegate void EitrChangedHandler(int amount);

        public event EitrChangedHandler EitrChanged;

        public enum Attribute
        {
            Strength,
            Dexterity,
            Endurance,
            Vigor
        }

        HashSet<ZDOID> taggedMonsters = new HashSet<ZDOID>();

        public ForbiddenShrineComponent InForbiddenShrine { get; set; } = null;
        public int Level { get; set; } = 0;
        public int Eitr { get; set; } = 0;
        public Dictionary<Attribute, int> Attributes { get; set; } = new Dictionary<Attribute, int>()
        {
            { Attribute.Strength, 1 },
            { Attribute.Dexterity, 1 },
            { Attribute.Endurance, 1 },
            { Attribute.Vigor, 1 },
        };

        public bool KilledEikthyr { get; set; } = false;
        public bool KilledElder { get; set; } = false;
        public bool KilledBonemass { get; set; } = false;
        public bool KilledModer { get; set; } = false;
        public bool KilledYagluth { get; set; } = false;

        public OdinRingData CloneLevelData()
        {
            var clone = new OdinRingData();
            clone.Level = Level;
            clone.Eitr = Eitr;
            foreach (var attribute in Attributes)
            {
                clone.Attributes[attribute.Key] = attribute.Value;
            }
            return clone;
        }

        public int GetEitrToLevel(int level)
        {
            const double a = 6.432748538;
            const double b = -42.98245614;
            const double c = 86.5497076;
            
            if (level < 10)
            {
                double q = level / 10.0;
                return (int)Math.Floor(50 + (250 * q * q * q));
            }

            return (int)Math.Floor((a * level * level) + (b * level) + c);
        }

        public bool LevelUp(Attribute attribute)
        {
            int eitrToLevel = GetEitrToLevel(Level);
            if (Eitr >= eitrToLevel)
            {
                Level += 1;
                Eitr -= eitrToLevel;
                Attributes[attribute] += 1;
                EitrChanged?.Invoke(-eitrToLevel);
                return true;
            }

            return false;
        }

        Dictionary<string, int> EitrTable = new Dictionary<string, int>()
        {
            { "Deer", 2 },
            { "Neck", 3 },
            { "Boar", 3 },
            { "Greyling", 3 },
            { "Greydwarf Shaman", 15 },
            { "Greydwarf Brute", 20 },
            { "Greydwarf", 10 },
            { "Rancid Remains", 20 },
            { "Skeleton", 10 },
            { "Troll", 45 },
            { "Ghost", 30 },
            { "Draugr", 30 },
            { "Draugr Elite", 45 },
            { "Blob", 35 },
            { "Leech", 25 },
            { "Wraith", 45 },
            { "Wolf", 55 },
            { "Cultist", 95 },
            { "Ulv", 95 },
            { "Fenring", 75 },
            { "Drake", 60 },
            { "Abomination", 150 },
            { "Bat", 25 },
            { "Growth", 120 },
            { "Deathsquito", 90 },
            { "Fuling", 100 },
            { "Fuling Berserker", 150 },
            { "Fuling Shaman", 120 },
            { "Lox", 120 },
            { "Serpent", 150 },
            { "Stone Golem", 100 },
            { "Surtling", 65 },
        };

        public void GiveEitr(string monsterName)
        {
            int eitrGained = 0;

            // Check bosses.
            if (monsterName.Contains("Eikthyr"))
            {
                eitrGained += KilledEikthyr ? 30 : 150;
                KilledEikthyr = true;
            }
            else if (monsterName.Contains("Elder"))
            {
                eitrGained += KilledElder ? 60 : 300;
                KilledElder = true;
            }
            else if (monsterName.Contains("Bonemass"))
            {
                eitrGained += KilledBonemass ? 100 : 500;
                KilledBonemass = true;
            }
            else if (monsterName.Contains("Moder"))
            {
                eitrGained += KilledModer ? 200 : 1000;
                KilledModer = true;
            }
            else if (monsterName.Contains("Yagluth"))
            {
                eitrGained += KilledYagluth ? 400 : 2000;
                KilledYagluth = true;
            }
            else
            {
                foreach (var eitrEntry in EitrTable)
                {
                    if (monsterName.Contains(eitrEntry.Key))
                    {
                        Jotunn.Logger.LogInfo($"Giving {eitrEntry.Value} Eitr for killing {eitrEntry.Key}");
                        eitrGained += eitrEntry.Value;
                    }
                }
            }

            if (eitrGained == 0)
            {
                Jotunn.Logger.LogInfo($"Character {monsterName} has no Eitr entry.");
            }
            else
            {
                Eitr += eitrGained;
                EitrChanged?.Invoke(eitrGained);
            }
        }

        public void TagMonster(ZDOID monsterId)
        {
            taggedMonsters.Add(monsterId);
        }

        public bool UntagMonster(ZDOID monsterId)
        {
            return taggedMonsters.Remove(monsterId);
        }

        public float GetMeleeDamageBonus()
        {
            return 1.0f + (Attributes[Attribute.Strength] / 100.0f);
        }

        public float GetRangedDamageBonus()
        {
            return 1.0f + (Attributes[Attribute.Dexterity] / 100.0f);
        }

        public float GetMaxHealthBonus()
        {
            return Attributes[Attribute.Vigor];
        }

        public float GetMaxStaminaBonus()
        {
            return Attributes[Attribute.Endurance];
        }

        public float GetCarryWeightBonus()
        {
            return (0.4f * Attributes[Attribute.Strength]) + (0.3f * Attributes[Attribute.Endurance]) + (0.2f * Attributes[Attribute.Vigor]);
        }

        public float GetMovementSpeedBonus()
        {
            var strMod = 0.3f * Attributes[Attribute.Strength];
            var dexMod = 0.5f * Attributes[Attribute.Dexterity];
            var vigMod = 0.2f * Attributes[Attribute.Vigor];
            return 1.0f + ((strMod + dexMod + vigMod) / 100f);
        }

        public float GetJumpStaminaBonus()
        {
            var strMod = 0.3f * Attributes[Attribute.Strength];
            var dexMod = 0.5f * Attributes[Attribute.Dexterity];
            var vigMod = 0.2f * Attributes[Attribute.Vigor];
            return 1.0f - ((strMod + dexMod + vigMod) / 100f);
        }

        public float GetPhysicalResistBonus()
        {
            var strMod = 0.2f * Attributes[Attribute.Strength];
            var dexMod = 0.1f * Attributes[Attribute.Dexterity];
            var endMod = 0.2f * Attributes[Attribute.Endurance];
            var vigMod = 0.4f * Attributes[Attribute.Vigor];
            return 1.0f - ((strMod + dexMod + endMod + vigMod) / 100f);
        }

        public float GetFoodDurationBonus()
        {
            var endMod = 0.3f * Attributes[Attribute.Endurance];
            var vigMod = 0.3f * Attributes[Attribute.Vigor];
            return 1.0f + ((endMod + vigMod) / 100f);
        }
    }
}
