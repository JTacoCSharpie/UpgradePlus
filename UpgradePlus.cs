using static UpgradePlus.Localization;
using Terraria.ModLoader;
using Terraria.Localization;

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
                    // See Localization.cs for translating
                    censusMod.Call("TownNPCCondition", Find<ModNPC>("Blacksmith").Type, GetModCallTranslation("Census", Language.ActiveCulture.Name));
                }
            }
        }

    }
}