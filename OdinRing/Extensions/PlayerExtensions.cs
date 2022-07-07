using OdinRing.Components;
using OdinRing.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinRing.Extensions
{
    public static class PlayerExtensions
    {
        private static OdinRingComponent GetOdinRingComponent(Player player)
        {
            var odinRingComponent = player.GetComponent<OdinRingComponent>();
            if (odinRingComponent == null)
            {
                Jotunn.Logger.LogInfo("OdinRing: Initializing OdinRingComponent");
                odinRingComponent = player.gameObject.AddComponent<OdinRingComponent>();
            }

            return odinRingComponent;
        }

        public static void ResetOdinRingData(this Player player)
        {
            GetOdinRingComponent(player).Data = new OdinRingData();
        }

        public static OdinRingData GetOdinRingData(this Player player)
        {
            return GetOdinRingComponent(player).Data;
        }
    }
}
