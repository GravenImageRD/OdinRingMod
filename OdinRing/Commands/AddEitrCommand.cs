using Jotunn.Entities;
using OdinRing.Data;
using OdinRing.Extensions;

namespace OdinRing.Commands
{
    public class AddEitrCommand : ConsoleCommand
    {
        public override string Name => "oraddeitr";
        public override string Help => "Adds Eitr";

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

            if (int.TryParse(args[0], out int eitr))
            {
                odinRingData.Eitr += eitr;
            }
        }
    }
}
