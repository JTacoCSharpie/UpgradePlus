using Terraria;
using Terraria.ModLoader;


namespace UpgraderPlus
{
    public class UPPlayer : ModPlayer
    {
        public float size = 1f;

		public override void GetWeaponCrit(Item item, ref int crit)
		{
			int lv = item.GetGlobalItem<Globals.ItemLevelHooks>().level;
			crit += (lv > 0) ? (int)Levelhandler.GetStat(lv, "CritChance") : 0;
	    }
		public override void ModifyManaCost(Item item,ref float reduce, ref float mult)
		{
			int lv = item.GetGlobalItem<Globals.ItemLevelHooks>().level;
			mult *= (lv > 0) ? (1 - (Levelhandler.GetStat(lv, "ManaCost") / 100)) : 1;
		}

        public override bool PreItemCheck()
        {
            Item held = player.HeldItem;
            if (!player.HeldItem.IsAir)
            {
                int lv = held.GetGlobalItem<Globals.ItemLevelHooks>().level;
                if (held.pick == 0 && held.hammer == 0 && held.axe == 0)
                {
                    held.scale = (Levelhandler.doWeaponSize && lv > 0) ? size + Levelhandler.GetStat(lv, "Size") : (float)held.GetGlobalItem<Globals.ItemLevelHooks>().defaultScale;
                }
                if (!held.autoReuse && Levelhandler.setReuse && lv >= Levelhandler.reuseLevel)
                {
                    held.autoReuse = true;
                }
                else 
                {
                    held.autoReuse = held.GetGlobalItem<Globals.ItemLevelHooks>().defaultReuse;
                }
            }
            return base.PreItemCheck();
        }

        public override void ResetEffects()
        {
            size = 1f;
        }

    }
}
