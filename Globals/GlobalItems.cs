using static UpgradePlus.Localization;
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

namespace UpgradePlus.Globals
{
	public class ItemLevelHooks : GlobalItem
	{
		public int level;
		public int defaultDefence;

		public override bool InstancePerEntity => true;
		protected override bool CloneNewInstances => true;

        public override void SetDefaults(Item item) => defaultDefence = item.defense;
		public override void PostReforge(Item item) => defaultDefence = item.defense;

        /*public override bool PreReforge(Item item) // Refund items if/when Calamity breaks again
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
			lines.Add(new(Mod, "UpgradePlusLevel", GetTrans("UI.Tooltips.ItemType." + ((ItemType)type).ToString(), level)) { OverrideColor = cappedTitle });
			if (type == (int)ItemType.Weapon)
			{
				string topLayerStats = GetTrans("UI.Tooltips.Damage", (int)((Levelhandler.GetStat(level, Stat.Damage) * 100) + 100));
				if (Levelhandler.doCritDamage)
				{
					topLayerStats += " / " + GetTrans("UI.Tooltips.CritDamage", TrimTrailingDigits( 2 + Levelhandler.GetStat(level, Stat.CritDamage) ));
				}
				lines.Add(new(Mod, "UpgradePlusToolTipLayerOne", topLayerStats) { OverrideColor = cappedStats });

				if (Levelhandler.sizeMulti > 0 || Levelhandler.speedMulti > 0)
                {
					string layerTwoStats = "";
					if (Levelhandler.sizeMulti > 0)
					{
						layerTwoStats += GetTrans("UI.Tooltips.Size", TrimTrailingDigits( (Levelhandler.GetStat(level, Stat.Size) * Levelhandler.sizeMulti * 0.01f) + 1) );
						if (Levelhandler.speedMulti > 0)
						{
							layerTwoStats += " / ";
						}
					}
					if (Levelhandler.speedMulti > 0)
					{
						layerTwoStats += GetTrans("UI.Tooltips.Speed", TrimTrailingDigits( (Levelhandler.GetStat(level, Stat.Speed) * Levelhandler.speedMulti * 0.01f) + 1) );
					}
					lines.Add(new(Mod, "UpgradePlusToolTipLayerTwo", layerTwoStats) { OverrideColor = cappedStats });
				}

				lines.Add(new(Mod, "UpgradePlusToolTipCritChance", GetTrans("UI.Tooltips.CritChance", Levelhandler.GetStat(level, Stat.CritChance)) ) { OverrideColor = cappedStats });
				if (Levelhandler.KBMulti > 0)
				{
					lines.Add(new(Mod, "UpgradePlusToolTipKnockback", GetTrans("UI.Tooltips.Knockback", (int)((Levelhandler.GetStat(level, Stat.Knockback) * Levelhandler.KBMulti) + 100)) ) { OverrideColor = cappedStats });
				}
				if (Levelhandler.velMulti > 0 && item.shoot > ProjectileID.None)
				{
					lines.Add(new(Mod, "UpgradePlusToolTipVelocity", GetTrans("UI.Tooltips.Velocity", (int)Levelhandler.GetStat(level, Stat.Velocity) * Levelhandler.velMulti + 100) ) { OverrideColor = cappedStats });
				}
				if (item.mana > 0)
				{
					lines.Add(new(Mod, "UpgradePlusToolTipManaCost", GetTrans("UI.Tooltips.ManaCost", Levelhandler.GetStat(level, Stat.ManaCost)) ) { OverrideColor = cappedStats });
				}
			}
			else if (type == (int)ItemType.Armor)
			{
				string armorLines = GetTrans("UI.Tooltips.Defence", Levelhandler.GetStat(level, Stat.Defence)) + "\n" + GetTrans("UI.Tooltips.Summons", (int)Levelhandler.GetStat(level, Stat.Summons));
				lines.Add(new(Mod, "UpgradePlusToolTipArmorStats", armorLines) { OverrideColor = cappedStats });
			}
			else if (type == (int)ItemType.Wings)
			{
				string wingStats = GetTrans("UI.Tooltips.Defence", Levelhandler.GetStat(level, Stat.Defence));
				if (Levelhandler.wingMulti > 0)
				{
					wingStats += "\n" + GetTrans("UI.Tooltips.WingSpeed", Levelhandler.GetStat(level, Stat.WingPower) * Levelhandler.wingMulti);
				}
				lines.Add(new(Mod, "UpgradePlusToolTipWingStats", wingStats) { OverrideColor = cappedStats });
			}
			else if (type == (int)ItemType.Accessory)
			{
				lines.Add(new(Mod, "UpgradePlusToolTipAccessoryStats", GetTrans("UI.Tooltips.Defence", Levelhandler.GetStat(level, Stat.Defence))) { OverrideColor = cappedStats });
			}
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
				velocity *= (1 + (Levelhandler.GetStat(level, Stat.Velocity) * Levelhandler.velMulti * 0.01f));
			}
        }
		public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
		{
			if (level > 0)
			{
				damage *= 1 + Levelhandler.GetStat(level, Stat.Damage);
			}
		}
        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
		{
			if (level > 0)
			{
				knockback *= 1 + ((Levelhandler.GetStat(level, Stat.Knockback) * Levelhandler.KBMulti) * 0.01f);
			}
		}
        public override float UseSpeedMultiplier(Item item, Player player)
        {
			return (level > 0) ? (Levelhandler.GetStat(level, Stat.Speed) * Levelhandler.speedMulti * 0.01f) + 1f : 1f;
		}

        public override void UpdateEquip(Item item, Player player)
		{
			if (level > 0)
			{
				if (Levelhandler.GetItemType(item) == (int)ItemType.Armor)
				{
					player.maxTurrets += (int)Levelhandler.GetStat(level, Stat.Summons);
					player.maxMinions += (int)Levelhandler.GetStat(level, Stat.Summons);
				}
				item.defense = defaultDefence + (int)Levelhandler.GetStat(level, Stat.Defence);
			}
			else
            {
				item.defense = defaultDefence;
			}
		}
		public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
		{
			if (level > 0 && Levelhandler.wingMulti > 0)
			{
				float xWingSpeed = 1 + (Levelhandler.GetStat(level, Stat.WingPower) * Levelhandler.wingMulti * 0.01f);
				speed *= xWingSpeed;
				acceleration *= xWingSpeed;
			}
		}
        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
			if (level > 0 && Levelhandler.wingMulti > 0)
			{
				float yWingSpeed = 1 + (Levelhandler.GetStat(level, Stat.WingPower) * Levelhandler.wingMulti * 0.01f);
				ascentWhenRising *= yWingSpeed;
				maxCanAscendMultiplier *= yWingSpeed;
				maxAscentMultiplier *= yWingSpeed;
				constantAscend *= yWingSpeed;
			}
		}

        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
			if (item.pick == 0 && item.hammer == 0 && item.axe == 0 && level > 0 && Levelhandler.sizeMulti > 0)
			{
				scale += (Levelhandler.GetStat(level, Stat.Size) * Levelhandler.sizeMulti * 0.01f);
			}
		}
        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if (level > 0 && Levelhandler.sizeMulti > 0)
            {
				scale += (Levelhandler.GetStat(level, Stat.Size) * Levelhandler.sizeMulti * 0.01f);
			}
			return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
		}

		public override void ModifyHitNPC(Item item, Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
		{

			if (crit)
			{
				// Potentially apply boosted Crit Damage
				damage = (level > 0 && Levelhandler.doCritDamage) ? (int)((damage * 0.5) * (2 + Levelhandler.GetStat(level, Stat.CritDamage))) : damage;

				if (Levelhandler.critRollover)
                {
					float gItemCritMods = 0, gPlayerCritMods = 0;
					ItemLoader.ModifyWeaponCrit(item, player, ref gItemCritMods);
					PlayerLoader.ModifyWeaponCrit(player, item, ref gPlayerCritMods);
					int chance = (int)(gPlayerCritMods + gItemCritMods + player.GetTotalCritChance(item.DamageType));
					chance -= 100; // Remove the chance of the crit starting this chain
					if (chance > 0) // Begin extra crit rolls
					{
						bool keepLooping = true;
						while (chance > 0 && keepLooping) // While we have a chance to crit more
						{
							if ((Main.rand.NextFloat() * 100) > 100 - chance) // If we roll a crit
							{
								damage *= (level > 0 && Levelhandler.doCritDamage) ? (int)(2 + Levelhandler.GetStat(level, Stat.CritDamage)) : 2; // Either use crit damage, or x2
							}
							else // If we fail a crit
							{
								keepLooping = false;
							}
							chance -= 100;
						}
					}
				}
			}
		}

		public override void SaveData(Item item, TagCompound tag)
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
					critMulti = Levelhandler.GetStat(level, Stat.CritDamage);
					lvled = (level > 0);
					if (projectile.minion || projectile.sentry)
					{
						minionMulti += Levelhandler.GetStat(level, Stat.Damage);
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
			if (source is EntitySource_Misc castedMisc) // Desert Tiger & Abigail being wacky little fellas
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
				if (crit)
				{
					// Potentially apply boosted Crit Damage
					damage = (lvled && Levelhandler.doCritDamage) ? (int)((damage * 0.5) * (2 + critMulti)) : damage;

					if (Levelhandler.critRollover)
					{
						int critChance = projectile.CritChance;
						if (Levelhandler.doDebug && Main.netMode != NetmodeID.Server)
						{
							Main.NewText(GetTrans("UI.CritRolloverDebugLine1", critChance, critMulti, projectile.DamageType.DisplayName));
							Main.NewText(GetTrans("UI.CritRolloverDebugLine2", projectile.minion, projectile.sentry));
						}
						critChance -= 100; // Remove the chance of the crit starting this chain
						if (critChance > 0) // Begin extra crit rolls
						{
							int i = 0;
							bool keepLooping = true;
							while (critChance > 0 && keepLooping) // While we have a chance to crit more
							{
								if ((Main.rand.NextFloat() * 100) > 100 - critChance) // If we roll a crit
								{
									damage *= (lvled && Levelhandler.doCritDamage) ? (int)(2 + critMulti) : 2; // Either use crit damage, or x2
									if (Levelhandler.doDebug && Main.netMode != NetmodeID.Server)
									{
										i++;
										Main.NewText(GetTrans("UI.CritRolloverDebugLoop", critChance, damage, i));
									}
								}
								else // If we fail a crit
								{
									keepLooping = false;
								}
								critChance -= 100;
							}
						}
					}
				}
            }
		}

	}
}