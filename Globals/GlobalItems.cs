using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI;

namespace UpgraderPlus.Globals
{
	public class ItemLevelHooks : GlobalItem
	{
		public int level;
		public double defaultScale = 1f;
		public bool defaultReuse;
		public int defaultDefence;
		public float defaultVelocity;

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

        public override void SetDefaults(Item item)
        {
			defaultScale = item.scale;
			defaultReuse = item.autoReuse;
			defaultDefence = item.defense;
			defaultVelocity = item.shootSpeed;
		}
		public override void PostReforge(Item item)
        {
			defaultScale = item.scale;
			defaultDefence = item.defense;
			defaultVelocity = item.shootSpeed;
		}
		public override bool NewPreReforge(Item item) // Refund items when calamity is enabled
		{
			if (ModLoader.GetMod("CalamityMod") != null && level > 0) // Calamity 1.5 breaks (UU+40) reforges, somehow
			{
				Levelhandler.Refund(item);
			}
			return true;
		}
		public override GlobalItem Clone(Item item, Item itemClone)
		{
			ItemLevelHooks myClone = (ItemLevelHooks)base.Clone(item, itemClone);
			myClone.level = level;
			myClone.defaultScale = defaultScale;
			myClone.defaultReuse = defaultReuse;
			myClone.defaultDefence = defaultDefence;
			myClone.defaultVelocity = defaultVelocity;
			return myClone;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (!item.social && level > 0)
			{
				Color cappedTitle = ItemRarity.GetColor(ItemRarityID.LightRed);
				Color cappedStats = ItemRarity.GetColor(ItemRarityID.Green);
				if (level >= Levelhandler.tierCap)
				{
					cappedTitle = Main.DiscoColor;
					cappedStats = ItemRarity.GetColor(ItemRarityID.Quest);
				}

				int type = Levelhandler.GetItemType(item);
				string stats = "";
				TooltipLine line1 = new TooltipLine(mod, "UpgradePlusLevel", "Level " + level + " : " + ((ItemType)type).ToString()) { overrideColor = cappedTitle };
				tooltips.Add(line1);
				if (type == (int)ItemType.Weapon)
				{
					stats +=
						((Levelhandler.GetStat(level, "Damage") * 100)+100) + "% damage";
						if (Levelhandler.doCritDamage)
						{
							stats += " / " + (2 + Levelhandler.GetStat(level, "CritDamage")) + "X Crits";
						}
					stats += "\n" + (Levelhandler.GetStat(level, "Size") + 1) + "X Size / " + (1 + Levelhandler.GetStat(level, "Speed")) + "X Firerate" +
						"\n+" + Levelhandler.GetStat(level, "CritChance") + "% critical chance" +
						"\n" + (int)((Levelhandler.GetStat(level, "Knockback") * Levelhandler.KBMulti)+100) + "% knockback";
					if (item.shoot > ProjectileID.None)
					{
						stats += "\n" + (Levelhandler.GetStat(level, "Velocity")*Levelhandler.velMulti + 100) + "% velocity";
					}
					if (item.mana > 0)
					{
						stats += "\n-" + Levelhandler.GetStat(level, "ManaCost") + "% mana cost";
					}
				}
				if (type == (int)ItemType.Armor)
				{
					stats += Levelhandler.GetStat(level, "Defence") + " defence" +
					"\n+" + (int)Levelhandler.GetStat(level, "Summons") + " sentry & minion slots";
				}
				if (type == (int)ItemType.Wings)
				{
					stats += "+" + Levelhandler.GetStat(level, "Defence") + " defence" +
					"\n+" + (Levelhandler.GetStat(level, "WingPower") * Levelhandler.wingMulti) + "% Vertical & Horizontal wing speed";
				}
				if (type == (int)ItemType.Accessory)
				{
					stats += "+" + Levelhandler.GetStat(level, "Defence") + " defence";
				}
				TooltipLine line2 = new TooltipLine(mod, "UpgradePlusStats", stats) { overrideColor = cappedStats };
				tooltips.Add(line2);
			}
		}
        public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			item.shootSpeed = (level > 0) ? defaultVelocity * (1 + (Levelhandler.GetStat(level, "Velocity")*Levelhandler.velMulti*0.01f)) : defaultVelocity;
			return base.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }
		public override void ModifyWeaponDamage(Item item, Player player, ref float add, ref float mult, ref float flat)
		{
			if (level > 0)
			{
				mult *= 1 + Levelhandler.GetStat(level, "Damage");
			}
		}
        public override void GetWeaponKnockback(Item item, Player player, ref float knockback)
		{
			if (Levelhandler.doKnockback && level > 0)
			{
				knockback *= 1 + ((Levelhandler.GetStat(level, "Knockback") * Levelhandler.KBMulti) * 0.01f);
			}
		}
		public override float UseTimeMultiplier(Item item, Player player)
		{
			return (level > 0) ? 1f + Levelhandler.GetStat(level, "Speed") : 1f;
		}

		public override void UpdateEquip(Item item, Player player)
		{
			if (level > 0)
			{
				if (Levelhandler.GetItemType(item) == (int)ItemType.Armor)
				{
					player.maxTurrets += (int)Levelhandler.GetStat(level, "Summons");
					player.maxMinions += (int)Levelhandler.GetStat(level, "Summons");
				}
				item.defense = defaultDefence + (int)Levelhandler.GetStat(level, "Defence");
			}
			else
            {
				item.defense = defaultDefence;
			}
		}

		public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
		{
			if (level > 0 && Levelhandler.doWingUpgrade)
			{
				float xWingSpeed = 1 + (Levelhandler.GetStat(level, "WingPower") * Levelhandler.wingMulti * 0.01f);
				speed *= xWingSpeed;
				acceleration *= xWingSpeed;
			}
		}
        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
			if (level > 0 && Levelhandler.doWingUpgrade)
			{
				float yWingSpeed = 1 + (Levelhandler.GetStat(level, "WingPower") * Levelhandler.wingMulti * 0.01f);
				ascentWhenRising *= yWingSpeed;
				maxCanAscendMultiplier *= yWingSpeed;
				maxAscentMultiplier *= yWingSpeed;
				constantAscend *= yWingSpeed;
			}
		}


        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if (Levelhandler.doWeaponSize && level > 0)
            {
				scale += Levelhandler.GetStat(level, "Size");
				item.scale += Levelhandler.GetStat(level, "Size");
			}
			return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
		}

		public override void ModifyHitNPC(Item item, Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
		{
			int lv = level;
			if (crit && Levelhandler.critRollover)
			{
				int gItemCritMods = 0, gPlayerCritMods = 0;
				ItemLoader.GetWeaponCrit(item, player, ref gItemCritMods);
				PlayerHooks.GetWeaponCrit(player, item, ref gPlayerCritMods);
				int chance = gPlayerCritMods + gItemCritMods;
				if (item.ranged) { chance += player.rangedCrit; }
				else if (item.melee) { chance += player.meleeCrit; }
				else if (item.magic) { chance += player.magicCrit; }
				else if (item.thrown) { chance += player.thrownCrit; }
				chance -= 100; // Remove the chance of the crit starting this chain
				if (chance > 100) // Begin extra crit rolls
				{
					bool keepLooping = true;
					float haveLooped = 0.5f;
					while (chance > 0 && keepLooping) // While we have a chance to crit more
					{
						if (Main.rand.NextBool((int)Math.Max((100f / chance), 1))) // If we roll a crit
						{
							damage = (lv > 0 && Levelhandler.doCritDamage) ? (int)((damage * haveLooped) * (2 + Levelhandler.GetStat(lv, "CritDamage"))) : damage * 2; // Either use crit damage, or x2
						}
						else // If we fail a crit
						{
							damage = (lv > 0 && haveLooped == 0.5f && Levelhandler.doCritDamage) ? (int)((damage * 0.5) * (2 + Levelhandler.GetStat(lv, "CritDamage"))) : damage; // Apply damage multi to base crit if we've fail our first rollover, do nothing if we're failing a subsequent
							keepLooping = false;
						}
						chance -= 100;
						haveLooped = 1;
					}
				}
				else if (lv > 0 && Levelhandler.doCritDamage) // Our chance was lower than 100 but we still need to apply crit damage
				{
					damage = (int)((damage * 0.5) * (2 + Levelhandler.GetStat(lv, "CritDamage")));
				}
			}
			else if (crit) // critRollover is disabled
			{
				damage = (lv > 0 && Levelhandler.doCritDamage) ? (int)((damage * 0.5) * (2 + Levelhandler.GetStat(lv, "CritDamage"))) : damage; // Either use crit damage, or don't apply
			}
		}

        public override bool NeedsSaving(Item item)
		{
			return level > 0;
		}
		public override TagCompound Save(Item item)
		{
			return new TagCompound {
				{"UpgradePlusLevel", level}
			};
		}
		public override void Load(Item item, TagCompound tag)
		{
			level = tag.GetInt("UpgradePlusLevel");
		}
		public override void NetSend(Item item, BinaryWriter writer)
		{
			writer.Write(level);
			writer.Write(defaultScale);
			writer.Write(defaultReuse);
			writer.Write(defaultDefence);
			writer.Write(defaultVelocity);
		}
		public override void NetReceive(Item item, BinaryReader reader)
		{
			level = reader.ReadInt32();
			defaultScale = reader.ReadDouble();
			defaultReuse = reader.ReadBoolean();
			defaultDefence = reader.ReadInt32();
			defaultVelocity = reader.ReadSingle();
		}
    }


	public class ProjectileHooks : GlobalProjectile
	{
        public override bool InstancePerEntity => true;

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
			if (!projectile.npcProj && Main.player[projectile.owner].HeldItem.active)
			{
				Projectile proj = projectile;
				Item held = Main.player[proj.owner].HeldItem;
				int lv = held.GetGlobalItem<Globals.ItemLevelHooks>().level;
				if (crit && Levelhandler.critRollover)
				{

					int gItemCritMods = 0, gPlayerCritMods = 0;
					ItemLoader.GetWeaponCrit(held, Main.player[projectile.owner], ref gItemCritMods);
					PlayerHooks.GetWeaponCrit(Main.player[projectile.owner], held, ref gPlayerCritMods);
					int critChance = gItemCritMods + gPlayerCritMods;
					if (projectile.ranged) { critChance += Main.player[projectile.owner].rangedCrit; }
					else if (projectile.melee) { critChance += Main.player[projectile.owner].meleeCrit; }
					else if (projectile.magic) { critChance += Main.player[projectile.owner].magicCrit; }
					else if (projectile.thrown) { critChance += Main.player[projectile.owner].thrownCrit; }
					if (Levelhandler.doDebug)
					{
						Main.NewText("Crit chance was counted as " + critChance + "%");
					}
					critChance -= 100; // Remove beginning crit's chance
					if (critChance > 100) // Begin extra crit rolls
					{
						bool keepLooping = true;
						float haveLooped = 0.5f;
						while (critChance > 0 && keepLooping) // While we have a chance to crit more
						{
							if (Main.rand.NextBool((int)Math.Max((100f / critChance), 1))) // If we roll a crit
							{
								damage = (lv > 0 && Levelhandler.doCritDamage) ? (int)((damage * haveLooped) * (2 + Levelhandler.GetStat(lv, "CritDamage"))) : damage * 2; // Either use crit damage, or x2
							}
							else // If we fail a crit
							{
								damage = (lv > 0 && haveLooped == 0.5f && Levelhandler.doCritDamage) ? (int)((damage * 0.5) * (2 + Levelhandler.GetStat(lv, "CritDamage"))) : damage; // Apply damage multi to base crit if we've fail our first rollover, do nothing if we're failing a subsequent
								keepLooping = false;
							}
							critChance -= 100;
							haveLooped = 1;
						}
					}
					else if (lv > 0 && Levelhandler.doCritDamage) // Our chance was lower than 100 but we still need to apply crit damage
					{
						damage = (int)((damage * 0.5) * (2 + Levelhandler.GetStat(lv, "CritDamage")));
					}

				}
				else if (crit) // critRollover is disabled
				{
					damage = (lv > 0 && Levelhandler.doCritDamage) ? (int)((damage * 0.5) * (2 + Levelhandler.GetStat(lv, "CritDamage"))) : damage; // Either use crit damage, or don't apply
				}
			}
		}


	}
}