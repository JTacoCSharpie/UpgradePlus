using UpgradePlus.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;
using Terraria.ModLoader;

namespace UpgradePlus
{

    public class UpgradePlus : Mod
    {

        public override void PostSetupContent()
        {
            if (ModLoader.HasMod("Census"))
            {
                Mod censusMod = ModLoader.GetMod("Census");
                if (censusMod != null)
                {
                    censusMod.Call("TownNPCCondition", Find<ModNPC>("Blacksmith").Type, "When EoC, EoW, BoC, or Skeletron are defeated");
                }
            }
        }

    }
}