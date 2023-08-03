using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace UpgradePlus
{
    public class UPPlayer : ModPlayer
    {
        public double SpentTokens = 0;
        public float tigerBoost, tigerCrit;
        public float abigailBoost, abigailCrit;

        public override void LoadData(TagCompound tag) => SpentTokens = tag.GetAsDouble("UpgradePlusSpentTokens");
        public override void SaveData(TagCompound tag)
        {
            if (SpentTokens > 0)
            {
                tag.Add("UpgradePlusSpentTokens", SpentTokens);
            }
        }

        public override void ModifyWeaponCrit(Item item, ref float crit)
		{
            item.TryGetGlobalItem(out Globals.ItemLevelHooks hook);
            if (hook != null)
            {
				crit += (hook.level > 0) ? (int)Levelhandler.GetStat(hook.level, Stat.CritChance) : 0;
			}			
	    }
		public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
		{
            item.TryGetGlobalItem(out Globals.ItemLevelHooks hook);
            if (hook != null)
            {
                mult *= (hook.level > 0) ? (1 - (Levelhandler.GetStat(hook.level, Stat.ManaCost) / 100)) : 1;
            }
		}

    }
}
