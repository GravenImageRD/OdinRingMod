using Jotunn.Entities;
using OdinRing.Data;
using OdinRing.Extensions;

namespace OdinRing.Commands
{
    public class StatsCommand : ConsoleCommand
    {
        public override string Name => "orstats";
        public override string Help => "Prints OdinRing stats (level, eitr, attributes)";

        public override void Run(string[] args)
        {
            var odinRingData = Player.m_localPlayer.GetOdinRingData();
            if (odinRingData == null)
            {
                Jotunn.Logger.LogInfo("OdinRing: Failed to find OdinRingComponent on local player!");
                return;
            }

            Jotunn.Logger.LogInfo($"Level: {odinRingData.Level}");
            Jotunn.Logger.LogInfo($"Eitr: {odinRingData.Eitr}");
            foreach (var attribute in odinRingData.Attributes)
            {
                Jotunn.Logger.LogInfo($"{attribute.Key}: {attribute.Value}");
            }
        }
    }
}
