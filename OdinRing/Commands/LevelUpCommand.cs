using Jotunn.Entities;
using OdinRing.Data;
using OdinRing.Extensions;

namespace OdinRing.Commands
{
    public class LevelUpCommand : ConsoleCommand
    {
        public override string Name => "orlevelup";
        public override string Help => "increases an OdinRing stat and character base level, ex: levelup (str|dex|end|vig)";

        public override void Run(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            var odinRingData = Player.m_localPlayer.GetOdinRingData();
            if (odinRingData == null)
            {
                Jotunn.Logger.LogInfo("OdinRing: Failed to find OdinRingComponent on local player!");
                return;
            }

            switch (args[0])
            {
                case "str":
                    {
                        odinRingData.LevelUp(OdinRingData.Attribute.Strength);
                        break;
                    }

                case "dex":
                    {
                        odinRingData.LevelUp(OdinRingData.Attribute.Dexterity);
                        break;
                    }

                case "end":
                    {
                        odinRingData.LevelUp(OdinRingData.Attribute.Endurance);
                        break;
                    }

                case "vig":
                    {
                        odinRingData.LevelUp(OdinRingData.Attribute.Vigor);
                        break;
                    }
            }
        }
    }
}
