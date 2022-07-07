using Jotunn.Entities;
using OdinRing.Data;
using OdinRing.Extensions;

namespace OdinRing.Commands
{
    public class ResetCommand : ConsoleCommand
    {
        public override string Name => "orreset";
        public override string Help => "Sets base level 1, eitr to 0, and all stats to 1.";

        public override void Run(string[] args)
        {
            Player.m_localPlayer.ResetOdinRingData();
        }
    }
}
