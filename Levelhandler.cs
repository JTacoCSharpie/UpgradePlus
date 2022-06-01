using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace UpgraderPlus
{

    public enum ItemType
    {
        Weapon,
        Accessory,
        Wings,
        Armor,
    }

    class Levelhandler
    {

        public static int formula;
        public static int tierCap;
        public static bool doHardmodeCap;
        public static bool doClientRefunds;
        public static bool doServerRefunds;
        public static bool critRollover;
        public static bool anythingLevels;

        public static bool setReuse;
        public static int reuseLevel;

        public static int velMulti;
        public static int KBMulti;
        public static int wingMulti;

        public static bool doWeaponSize;
        public static bool doKnockback;
        public static bool doCritDamage;
        public static bool doWingUpgrade;

        public static bool doDebug;

        public static int GetItemType(Item item)
        {
            int ret = 0; // Defaults to Weapon
            ret = (item.accessory) ? (int)ItemType.Accessory : ret;
            ret = (item.wingSlot > 0) ? (int)ItemType.Wings : ret;
            ret = (item.headSlot > -1 || item.bodySlot > -1 || item.legSlot > -1) ? (int)ItemType.Armor : ret;
            return ret;
        }

        /// <summary> Gets the cost of an item at specified level </summary>
        public static int GetCost(Item item, int level)
        {
            float costMulti = 1f; // Default weapon price
            int type = GetItemType(item);
            costMulti = (type == (int)ItemType.Wings) ? 0.7f : costMulti;
            costMulti = (type == (int)ItemType.Armor) ? 0.5f : costMulti;
            costMulti = (type == (int)ItemType.Accessory) ? 0.35f : costMulti;
            int cost = (level > 20) ? (int)(level * 6 * costMulti)  :  (int)(((level * level * 0.1) + level+1) * costMulti)+1;
            return cost;
        }
        public static int GetCost(int level, int type) // This one doesn't repeat calls to GetItemType
        {
            float costMulti = 1f; // Default weapon price
            costMulti = (type == (int)ItemType.Wings) ? 0.7f : costMulti;
            costMulti = (type == (int)ItemType.Armor) ? 0.5f : costMulti;
            costMulti = (type == (int)ItemType.Accessory) ? 0.35f : costMulti;
            int cost = (level > 20) ? (int)(level * 6 * costMulti)  :  (int)(((level * level * 0.1) + level+1) * costMulti)+1;
            return cost;
        }

        /// <summary> Get total cost to reach given level </summary>
        public static int GetCostForGivenLevel(Item item, int level)
        {
            int itemType = GetItemType(item);
            int total = 0;
            for (int i = 1; i <= level; i++)
            {
                total += GetCost(i, itemType);
            }
            return total;
        }
        /// <summary> Attempt to buy upgrades for item, from player, until cap </summary>
        public static void MultiBuy(Player player, Item item, int cap)
        {
            const short conv = short.MaxValue - 1;

            int remainder = 0; // The remaining tokens carried over between buys
            int level = item.GetGlobalItem<Globals.ItemLevelHooks>().level;
            int type = GetItemType(item);
            for (int i = level; i < cap; i++)
            {
                int taken;
                int tokTaken = 0, remTaken = 0;
                float compTaken = 0f;
                int price = GetCost(item.GetGlobalItem<Globals.ItemLevelHooks>().level + 1, type);
                if (remainder > 0)
                {
                    taken = Math.Min(price, remainder);
                    price -= taken;
                    remTaken += taken;
                }
                if (price > 0)
                {
                    for (int slot = 0; slot < player.inventory.Length; slot++)
                    {
                        if (player.inventory[slot].type == ModContent.ItemType<Items.UpgradeToken>())
                        {
                            taken = Math.Min(price, player.inventory[slot].stack);
                            tokTaken += taken;
                            price -= taken;
                            if (price == 0)
                            {
                                break;
                            }
                        }
                    }
                }
                if (price > 0)
                {
                    for (int slot = 0; slot < player.inventory.Length; slot++)
                    {
                        if (player.inventory[slot].type == ModContent.ItemType<Items.CompressedToken>())
                        {
                            double comp = player.inventory[slot].stack * conv; // Convert compressed tokens to real tokens
                            taken = (int)Math.Min(price, comp);
                            compTaken += taken; // Add the amount of real tokens taken
                            price -= taken;
                            if (price == 0)
                            {
                                break;
                            }
                        }
                    }
                }

                if (price > 0) // Can't afford
                {
                    break;
                }
                else // Buy
                {
                    item.GetGlobalItem<Globals.ItemLevelHooks>().level++;
                    if (remTaken > 0)
                    {
                        remainder -= remTaken;
                    }
                    if (tokTaken > 0)
                    {
                        for (int slot = 0; slot < player.inventory.Length; slot++)
                        {
                            if (player.inventory[slot].type == ModContent.ItemType<Items.UpgradeToken>())
                            {
                                taken = Math.Min(tokTaken, player.inventory[slot].stack);
                                tokTaken -= taken;
                                player.inventory[slot].stack -= taken;
                                if (player.inventory[slot].stack == 0)
                                {
                                    player.inventory[slot].TurnToAir();
                                }
                                if (tokTaken == 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (compTaken > 0)
                    {
                        compTaken /= conv; // Convert raw tokens used back to compressed values
                        for (int slot = 0; slot < player.inventory.Length; slot++)
                        {
                            if (player.inventory[slot].type == ModContent.ItemType<Items.CompressedToken>())
                            {
                                taken = (int)Math.Min(player.inventory[slot].stack, Math.Ceiling(compTaken));
                                compTaken -= taken;
                                player.inventory[slot].stack -= taken;
                                if (player.inventory[slot].stack == 0)
                                {
                                    player.inventory[slot].TurnToAir();
                                }
                                if (compTaken < 0)
                                {
                                    remainder += (int)(conv * Math.Abs(compTaken));
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (remainder > 0) // Give remaining value in tokens
            {
                player.QuickSpawnItem(ModContent.ItemType<Items.UpgradeToken>(), remainder);
            }

        }

        /// <summary> Refund the tokens from given item </summary>
        public static void Refund(Item item)
        {
            if (doClientRefunds && doServerRefunds)
            {
                int lv = item.GetGlobalItem<Globals.ItemLevelHooks>().level;
                item.GetGlobalItem<Globals.ItemLevelHooks>().level -= lv;
                int tokens = (int)Math.Ceiling((GetCostForGivenLevel(item, lv) * 0.9));
                int compressed = (int)(tokens / (short.MaxValue - 1));
                tokens -= ((short.MaxValue - 1) * compressed);
                Main.LocalPlayer.QuickSpawnItem(ModContent.ItemType<Items.UpgradeToken>(), tokens);
                if (compressed > 0)
                {
                    Main.LocalPlayer.QuickSpawnItem(ModContent.ItemType<Items.CompressedToken>(), compressed);
                }
            }
        }

        /// <summary> Returns the calculation for the given stat string at the given level
        /// <para>Acceptable values are as follows: </para>
        /// <br>Damage, Speed, CritChance, CritDamage, Size, Knockback </br>
        /// <br>Velocity, ManaCost, WingPower, Defence, Summon</br> </summary>
        public static float GetStat(int level, string statType)
        {
            float ret = new float();
            if (formula == 0) // OP
            {
                ret = (statType == "Damage") ? (0.4f * level) : ret;
                ret = (statType == "Speed") ? Math.Min((0.1f * level), 4) : ret;
                ret = (statType == "CritChance") ? (2f * level) : ret;
                ret = (statType == "CritDamage") ? (level * 0.05f * level) : ret;
                ret = (statType == "Size") ? Math.Min((0.1f * level), 2f) : ret;
                ret = (statType == "Knockback") ? (0.1f * level) : ret;
                ret = (statType == "Velocity") ? Math.Min((0.1f * level), 4) : ret;
                ret = (statType == "ManaCost") ? Math.Min(75, (2 * level)) : ret;
                ret = (statType == "WingPower") ? Math.Min((0.075f * level), 4) : ret;
                ret = (statType == "Defence") ? (0.25f * level) : ret;
                ret = (statType == "Summons") ? (0.025f * level) : ret;
            }
            if (formula == 1) // Balanced
            {
                // total *= value
                ret = (statType == "Damage") ? (0.06f * level) : ret;
                // useTime *= 1(base)+value
                ret = (statType == "Speed") ? Math.Min((0.025f * level), 3) : ret;
                // total += CritChance
                ret = (statType == "CritChance") ? (1f * level) : ret;
                // crits do 2(base) + crit damage
                ret = (statType == "CritDamage") ? (0.025f * level) : ret;
                // scale = base + size
                ret = (statType == "Size") ? Math.Min((0.05f * level), 1) : ret;
                // base *= 1 + knockback
                ret = (statType == "Knockback") ? (0.05f * level) : ret;
                // velocity = base + velocity
                ret = (statType == "Velocity") ? Math.Min((0.075f * level), 3) : ret;
                // final mana cost minus ManaCost%
                ret = (statType == "ManaCost") ? Math.Min(50, (1.5f * level)) : ret;
                // base * wingPower
                ret = (statType == "WingPower") ? Math.Min((0.025f * level), 2) : ret;
                // +flat defence
                ret = (statType == "Defence") ? (0.1f * level) : ret;
                // +summons (rounded down to nearest number by int casting)
                ret = (statType == "Summons") ? Math.Min((0.05f * level), 3) : ret;
            }
            if (formula == 2) // Underpowered
            {
                ret = (statType == "Damage") ? (0.01f * level) : ret;
                ret = (statType == "Speed") ? Math.Min((0.01f * level), 1) : ret;
                ret = (statType == "CritChance") ? (0.5f * level) : ret;
                ret = (statType == "CritDamage") ? 0f : ret;
                ret = (statType == "Size") ? Math.Min((0.05f * level), 0.5f) : ret;
                ret = (statType == "Knockback") ? (0.05f * level) : ret;
                ret = (statType == "Velocity") ? Math.Min((0.1f * level), 2) : ret;
                ret = (statType == "ManaCost") ? Math.Min(20, (0.5f * level)) : ret;
                ret = (statType == "WingPower") ? 0f : ret;
                ret = (statType == "Defence") ? (0.05f * level) : ret;
                ret = (statType == "Summons") ? 0f : ret;
            }
            return ret;
        }


    }
}
