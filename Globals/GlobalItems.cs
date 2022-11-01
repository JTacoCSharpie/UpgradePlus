using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI;
using System.Collections.ObjectModel;

namespace UpgradePlus.Globals
{
	public class ItemLevelHooks : GlobalItem
	{
		public int level;
		public int defaultDefence;

		public override bool InstancePerEntity => true;
		protected override bool CloneNewInstances => true;

        public override void SetDefaults(Item item)
        {
			defaultDefence = item.defense;
		}
		public override void PostReforge(Item item)
        {
			defaultDefence = item.defense;
		}
        /*public override bool PreReforge(Item item) // Refund items when Calamity breaks again
		{
			if (ModLoader.HasMod("CalamityMod") && level > 0)
			{
				Levelhandler.Refund(item);
			}
			return true;
		}*/
        public override GlobalItem Clone(Item item, Item itemClone)
		{
			ItemLevelHooks myClone = (ItemLevelHooks)base.Clone(item, itemClone);
			myClone.level = level;
			myClone.defaultDefence = defaultDefence;
			return myClone;
		}

		public List<TooltipLine> GetNewTooltip(Item item) // Moved to it's own method so I'll have it if I ever find an ideal performant way to cache tooltips
        {
			List<TooltipLine> lines = new();
			Color cappedTitle = ItemRarity.GetColor(ItemRarityID.LightRed);
			Color cappedStats = ItemRarity.GetColor(ItemRarityID.Green);
			if (level >= Levelhandler.tierCap)
			{
				cappedTitle = Main.DiscoColor;
				cappedStats = ItemRarity.GetColor(ItemRarityID.Quest);
			}

			int type = Levelhandler.GetItemType(item);
			string stats = "";
			if (type == (int)ItemType.Weapon)
			{
				stats +=
					(int)((Levelhandler.GetStat(level, "Damage") * 100) + 100) + "% damage";
				if (Levelhandler.doCritDamage)
				{
					stats += " / " + (2 + Levelhandler.GetStat(level, "CritDamage")) + "X Crits";
				}
				stats += "\n";
				if (Levelhandler.doWeaponSize)
				{
					stats += (Levelhandler.GetStat(level, "Size") + 1) + "X Size / ";
				}
				stats += ((Levelhandler.GetStat(level, "Speed") * Levelhandler.speedMulti * 0.01) + 1) + "X Firerate" +
				"\n+" + Levelhandler.GetStat(level, "CritChance") + "% critical chance";
				if (Levelhandler.doKnockback)
				{
					stats += "\n" + (int)((Levelhandler.GetStat(level, "Knockback") * Levelhandler.KBMulti) + 100) + "% knockback";
				}
				if (item.shoot > ProjectileID.None)
				{
					stats += "\n" + (Levelhandler.GetStat(level, "Velocity") * Levelhandler.velMulti + 100) + "% velocity";
				}
				if (item.mana > 0)
				{
					stats += "\n-" + Levelhandler.GetStat(level, "ManaCost") + "% mana cost";
				}
			}
			else if (type == (int)ItemType.Armor)
			{
				stats += Levelhandler.GetStat(level, "Defence") + " defence" +
				"\n+" + (int)Levelhandler.GetStat(level, "Summons") + " sentry & minion slots";
			}
			else if (type == (int)ItemType.Wings)
			{
				stats += "+" + Levelhandler.GetStat(level, "Defence") + " defence";
				if (Levelhandler.doWingUpgrade)
				{
					stats += "\n+" + (Levelhandler.GetStat(level, "WingPower") * Levelhandler.wingMulti) + "% Vertical & Horizontal wing speed";
				}
			}
			else if (type == (int)ItemType.Accessory)
			{
				stats += "+" + Levelhandler.GetStat(level, "Defence") + " defence";
			}
			lines.Add( new(Mod, "UpgradePlusLevel", "Level " + level + " : " + ((ItemType)type).ToString()) { OverrideColor = cappedTitle } );
			lines.Add( new(Mod, "UpgradePlusStats", stats) { OverrideColor = cappedStats } );
			return lines;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (!item.social && level > 0)
			{
				tooltips.AddRange(GetNewTooltip(item));
			}
		}
		
        public override bool? CanAutoReuseItem(Item item, Player player)
		{
			return (Levelhandler.setReuse && level >= Levelhandler.reuseLevel) ? true : base.CanAutoReuseItem(item, player);
		}

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			if (level > 0)
			{
				velocity *= (1 + (Levelhandler.GetStat(level, "Velocity") * Levelhandler.velMulti * 0.01f));
			}
        }
		public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
		{
			if (level > 0)
			{
				damage *= 1 + Levelhandler.GetStat(level, "Damage");
			}
		}
        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
		{
			if (Levelhandler.doKnockback && level > 0)
			{
				knockback *= 1 + ((Levelhandler.GetStat(level, "Knockback") * Levelhandler.KBMulti) * 0.01f);
			}
		}
        public override float UseSpeedMultiplier(Item item, Player player)
        {
			return (level > 0) ? (Levelhandler.GetStat(level, "Speed") * Levelhandler.speedMulti * 0.01f) + 1f : 1f;
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

        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
			if (item.pick == 0 && item.hammer == 0 && item.axe == 0 && Levelhandler.doWeaponSize && level > 0)
			{
				scale += Levelhandler.GetStat(level, "Size");
			}
		}
        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if (Levelhandler.doWeaponSize && level > 0)
            {
				scale += Levelhandler.GetStat(level, "Size");
			}
			return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
		}

		public override void ModifyHitNPC(Item item, Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
		{
			int lv = level;
			if (crit && Levelhandler.critRollover)
			{
				float gItemCritMods = 0, gPlayerCritMods = 0;
				ItemLoader.ModifyWeaponCrit(item, player, ref gItemCritMods);
				PlayerLoader.ModifyWeaponCrit(player, item, ref gPlayerCritMods);
				int chance = (int)(gPlayerCritMods + gItemCritMods + player.GetTotalCritChance(item.DamageType));
				chance -= 100; // Remove the chance of the crit starting this chain
				if (chance > 0) // Begin extra crit rolls
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
				else if (lv > 0 && Levelhandler.doCritDamage) // Our remaining chance was lower than 1% but we still need to apply crit damage
				{
					damage = (int)((damage * 0.5) * (2 + Levelhandler.GetStat(lv, "CritDamage")));
				}
			}
			else if (crit) // critRollover is disabled
			{
				damage = (lv > 0 && Levelhandler.doCritDamage) ? (int)((damage * 0.5) * (2 + Levelhandler.GetStat(lv, "CritDamage"))) : damage; // Either use crit damage, or don't apply
			}
		}

		public override void SaveData(Item item, TagCompound tag)/* Suggestion: Edit tag parameter rather than returning new TagCompound */
		{
			if (level > 0)
            {
				tag["UpgradePlusLevel"] = level;
			}
		}
		public override void LoadData(Item item, TagCompound tag)
		{
			level = tag.GetInt("UpgradePlusLevel");
		}
		public override void NetSend(Item item, BinaryWriter writer)
		{
			writer.Write(level);
			writer.Write(defaultDefence);
		}
		public override void NetReceive(Item item, BinaryReader reader)
		{
			level = reader.ReadInt32();
			defaultDefence = reader.ReadInt32();
		}
    }


	public class ProjectileHooks : GlobalProjectile
	{
        public override bool InstancePerEntity => true;
		public float minionMulti = 1;
		public float critMulti;
		public bool lvled;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
			if (source is EntitySource_Parent castedParent)
            {
				if (castedParent.Entity.GetType() == typeof(Projectile))
                {
					Projectile proj = (Projectile)castedParent.Entity;
					critMulti = proj.GetGlobalProjectile<ProjectileHooks>().critMulti;
					lvled = proj.GetGlobalProjectile<ProjectileHooks>().lvled;
					minionMulti = proj.GetGlobalProjectile<ProjectileHooks>().minionMulti;
				}
            }
			if (source is EntitySource_ItemUse castedSource)
			{
				Item spawnedFrom = castedSource.Item;
				if (spawnedFrom.TryGetGlobalItem(out ItemLevelHooks Upgrade))
                {
					int level = Upgrade.level;
					critMulti = Levelhandler.GetStat(level, "CritDamage");
					lvled = (level > 0);
					if (projectile.minion || projectile.sentry)
					{
						minionMulti += Levelhandler.GetStat(level, "Damage");
						if (projectile.type == ProjectileID.StormTigerGem && !projectile.npcProj)
						{
							Main.player[castedSource.Entity.whoAmI].GetModPlayer<UPPlayer>().tigerBoost = minionMulti;
							Main.player[castedSource.Entity.whoAmI].GetModPlayer<UPPlayer>().tigerCrit = critMulti;
						}
						if (projectile.type == ProjectileID.AbigailCounter && !projectile.npcProj)
						{
							Main.player[castedSource.Entity.whoAmI].GetModPlayer<UPPlayer>().abigailBoost = minionMulti;
							Main.player[castedSource.Entity.whoAmI].GetModPlayer<UPPlayer>().abigailCrit = critMulti;
						}
					}
				}
			}
			if (source is EntitySource_Misc castedMisc) // Desert Tiger & Abigail being a wacky little fellas
            {
				if (castedMisc.Context == "StormTigerTierSwap" && !projectile.npcProj)
                {
					minionMulti = Main.player[projectile.owner].GetModPlayer<UPPlayer>().tigerBoost;
					critMulti = Main.player[projectile.owner].GetModPlayer<UPPlayer>().tigerCrit;
					lvled = (minionMulti > 1);
                }
				if (castedMisc.Context == "AbigailTierSwap" && !projectile.npcProj)
				{
					minionMulti = Main.player[projectile.owner].GetModPlayer<UPPlayer>().abigailBoost;
					critMulti = Main.player[projectile.owner].GetModPlayer<UPPlayer>().abigailCrit;
					lvled = (minionMulti > 1);
				}

			}
		}

		public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
			if (!projectile.npcProj)
			{
				damage = (int)(damage * minionMulti);
				if (crit && Levelhandler.critRollover)
				{
					int critChance = projectile.CritChance;
					if (Levelhandler.doDebug && Main.netMode != NetmodeID.Server)
					{
						Main.NewText("Crit chance is supposedly: " + critChance + "% with " + critMulti + "+2 times damage on crits and is " + projectile.DamageType.DisplayName);
						Main.NewText("Minion: " + projectile.minion + " / Sentry: " + projectile.sentry);
					}
					critChance -= 100; // Remove beginning crit's chance
					if (critChance > 0) // Begin extra crit rolls
					{
						int i = 0;
						bool keepLooping = true;
						float haveLooped = 0.5f;
						while (critChance > 0 && keepLooping) // While we have a chance to crit more
						{
							if (Main.rand.NextBool((int)Math.Max((100f / critChance), 1))) // If we roll a crit
							{
								damage = (lvled && Levelhandler.doCritDamage) ? (int)((damage * haveLooped) * (2 + critMulti)) : damage * 2; // Either use crit damage, or x2
								if (Levelhandler.doDebug && Main.netMode != NetmodeID.Server) 
								{
									i++;
									Main.NewText("> " + critChance + "% crit chance & " + damage + " damage after rollover #" + i);
								}
							}
							else // If we fail a crit
							{
								damage = (lvled && haveLooped == 0.5f && Levelhandler.doCritDamage) ? (int)((damage * 0.5) * (2 + critMulti)) : damage; // Apply damage multi to base crit if we've fail our first rollover, do nothing if we're failing a subsequent
								keepLooping = false;
							}
							critChance -= 100;
							haveLooped = 1;
						}
					}
					else if (lvled && Levelhandler.doCritDamage) // Our remaining chance was lower than 1% but we still need to apply crit damage
					{
						damage = (int)((damage * 0.5) * (2 + critMulti));
					}

				}
				else if (crit) // critRollover is disabled
				{
					damage = (lvled && Levelhandler.doCritDamage) ? (int)((damage * 0.5) * (2 + critMulti)) : damage; // Either use crit damage, or don't apply
				}
			}
		}


	}
}