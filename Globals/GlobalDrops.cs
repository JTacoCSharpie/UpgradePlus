using System;
using Terraria;
using Terraria.ModLoader;

namespace UpgradePlus.Globals
{
    class GlobalDrops : GlobalNPC
    {

        public override void OnKill(NPC npc)
        {

            if (npc.boss || npc.lifeMax >= 1000 && npc.damage > 0)
            {
                double dropCalc;
                int dropCount;
                float stageMulti = 1; // World progression multiplier
                Random random = new();
                if (Main.hardMode)
                {
                    stageMulti = 2;
                    if (NPC.downedMoonlord)
                    {
                        stageMulti = 4;
                    }
                }

                if (npc.boss)
                {
                    dropCalc = Math.Max((npc.lifeMax / 1000) + (npc.defDefense / 100) + (npc.defDamage / 100), 1); // One token for every 1k hp + extra for every 100 defence and damage
                    dropCalc *= (1 + random.NextDouble()) * stageMulti; // Multiply the result by 100% to 150% for extra variety
                    dropCount = (int)Math.Ceiling(dropCalc); // Round down to the nearest whole number
                    if (dropCount >= short.MaxValue) // Compression
                    {
                        if (dropCount > 100000)
                        {
                            dropCount = 100000 + (int)(dropCount * 0.1f);
                        }
                        float tier2 = dropCount / (short.MaxValue-1);
                        npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<Items.CompressedToken>(), (int)tier2, true);
                        npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<Items.UpgradeToken>(), dropCount - (int)(tier2 * (short.MaxValue-1)), true);
                    }
                    else
                    {
                        npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<Items.UpgradeToken>(), dropCount, true);
                    }

                }
                else // Minibosses, modded enemies, vanilla enemies on modded difficulties
                {
                    dropCalc = Math.Max((npc.lifeMax / 1000) + (npc.defDefense / 100) + (npc.defDamage / 100), 1);
                    dropCalc *= (1 + random.NextDouble());
                    dropCount = (int)Math.Ceiling(dropCalc);
                    if (dropCount >= short.MaxValue)
                    {
                        float tier2 = dropCount / (short.MaxValue - 1);
                        npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<Items.CompressedToken>(), (int)tier2, true);
                        npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<Items.UpgradeToken>(), dropCount - (int)(tier2 * (short.MaxValue - 1)), true);
                    }
                    else
                    {
                        npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<Items.UpgradeToken>(), dropCount, true);
                    }
                }
            }

        }

    }
}