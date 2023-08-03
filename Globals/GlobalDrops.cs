using System;
using Terraria;
using Terraria.ModLoader;

namespace UpgradePlus.Globals
{
    class GlobalDrops : GlobalNPC
    {
        const short conv = short.MaxValue - 1;

        public override void OnKill(NPC npc)
        {
            if (npc.boss || npc.lifeMax >= 1000 && npc.damage > 0)
            {
                Random random = new();
                double dropCalc;
                int dropCount;

                float stageMulti = 1; // World progression multiplier
                if (Main.hardMode && npc.boss)
                {
                    stageMulti = 2;
                    if (NPC.downedMoonlord)
                    {
                        stageMulti = 4;
                    }
                }

                // One token for every 1k hp + extra for every 100 defence and damage, multiply 1-2x for extra variety
                dropCalc = Math.Max((npc.lifeMax / 1000) + (npc.defDefense / 100) + (npc.defDamage / 100), 1);
                if (Levelhandler.toughTokens)
                {
                    dropCalc *= 1.1f - Math.Log10(dropCalc)/10;
                }
                dropCalc *= (1 + random.NextDouble()) * stageMulti;
                dropCount = (int)Math.Ceiling(dropCalc);
                if (dropCount > 100000)
                {
                    dropCount = 100000 + (int)(dropCount * 0.1f);
                }
                if (dropCount >= conv)
                {
                    float tier2 = dropCount / (short.MaxValue - 1);
                    npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<Items.CompressedToken>(), (int)tier2, true);
                    npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<Items.UpgradeToken>(), dropCount - (int)(tier2 * conv), true);
                }
                else
                {
                    npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<Items.UpgradeToken>(), dropCount, true);
                }
            }

        }

    }
}