using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace UpgraderPlus.Npcs
{

	[AutoloadHead]
	public class Blacksmith : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blacksmith");
			Main.npcFrameCount[npc.type] = 26;
			NPCID.Sets.ExtraFramesCount[npc.type] = 6;
			NPCID.Sets.AttackFrameCount[npc.type] = 5;
			NPCID.Sets.DangerDetectRange[npc.type] = 700; // How far a Town NPC can detect an enemy.
			NPCID.Sets.AttackType[npc.type] = 0; // The attack style an NPC copies. Set to 0 if the NPC has a custom attack.
			NPCID.Sets.AttackTime[npc.type] = 120; // How fast your NPC can attack.
			NPCID.Sets.AttackAverageChance[npc.type] = 45;
			NPCID.Sets.HatOffsetY[npc.type] = 6;
		}

		public override void SetDefaults()
		{
			npc.friendly = true;
			npc.townNPC = true;
			npc.width = 27;
			npc.height = 45;
			npc.aiStyle = 7;
			npc.damage = 15;
			npc.defense = 20;
			npc.lifeMax = 300;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.knockBackResist = 0.5f;
			aiType = 22; // What type of AI an NPC copies.
			animationType = NPCID.Guide;
		}

		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.WhoopieCushion, 1);
		}

		public override bool CanGoToStatue(bool toKingStatue)
		{
			return true;
		}
		public override bool CanTownNPCSpawn(int numTownNPCs, int money)
		{
			return (NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3); // Spawn if EoC, EoW/BoC, or Skele have been defeated
		}
		public override string TownNPCName()
		{
			string[] names = {"Ketsuban", "Hayato", "Jeff", "Doug,", "Derf", "Sam", "Peter", "Bartholomew", "Adam", "Sprout", "Jeb!", "ZZAZZ", "Bobby"};
			return Main.rand.Next(names);
		}
		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 15; // The amount of damage the Town NPC inflicts.
			knockback = 4f; // The amount of knockback the Town NPC inflicts.
			if (Main.hardMode)
            {
				damage *= 4;
            }
		}
		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 30; // The amount of ticks the Town NPC takes to cool down. Every 60 in-game ticks is a second.
			randExtraCooldown = 30; // How long it takes until the NPC attacks again, but with a chance.
		}
		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ProjectileID.VampireKnife; // The Projectile this NPC shoots. Search up Terraria Projectile IDs, I cannot link the websites in this code
			attackDelay = 1; // Delays the attacks, obviously.
		}
		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 10f; // The Speed of the Projectile
			gravityCorrection = -1f;
			randomOffset = -1f; // Random Offset
		}

		public override string GetChat()
		{
			int rng = Main.rand.Next(18);
			string[] jobs = {"at the PD.", "at the deli", "in the fire department", "as a life guard", "as a plumber", "as a streamer", "as a vtuber"};
			if (rng == 0)
			{
				Mod Calamity = ModLoader.GetMod("CalamityMod");
				if (Calamity != null)
				{
					if (NPC.FindFirstNPC(Calamity.NPCType("WITCH")) > 0 && Main.rand.NextBool() )
					{
						return "I hear calamitas got a job working " + Main.rand.Next(jobs) + " to pay rent the other day.";
					}
					else
                    {
						if (Main.rand.NextBool())
						{
							return "You run yharim for his foams yet?";
						}
						else
                        {
							return "Fabsol scares me. I once dared her to drink a bucket of lava and she looked me dead in the eye then started chugging.";
						}
					}
				}
				else if (NPC.FindFirstNPC(NPCID.TaxCollector) < 0)
				{
					return "Living in your head, rent free.";
				}
				else
                {
					return "10% off your next upgrade if you lower my rent.";
                }
			}
			else 
			{
				if (rng == 1) return "Turn your KO cannons into Killer Cannons.";
				else if (rng == 2) return "Don't you hate it when people don't finish their";
				else if (rng == 3) return "How far could a fargo go if a fargo could go far?";
				else if (rng == 4) return "I keep hearing awful stories about \"shoebox\" houses.";
				else if (rng == 5) return "Don't be ridiculous, these tokens hoarded by powerful creatures have no value outside of giving them to me.";
				else if (rng == 6)
				{
					if (Main.dayTime)
					{
						return "Hey go kill some more blue slimes, I need to craft some sticky pistons.";
					}
					else
					{
						return "Where do all these zombies keep coming from?";
					}
				}
				else if (rng == 7) return "So what's got you all dolled up today?";
				else if (rng == 8) return "You know, as our landlord you're obligated to have more fire extinquishers around here.";
				else if (rng == 9) return "Violence is never the answer, it's always the question, and the answer is yes.";
				else if (rng == 10) return "I got my technique from a guy in a blue cap, real nice guy but whenever I asked his name he'd always reply \"Upyours\".";
				else if (rng == 11) return "Quick, the secret formula for crafting upgrade tokens is-";
				else if (rng == 12) return "Why in the world are there so many gold critters? Was there an explosion in a flask factory?";
				else if (rng == 13) return "Can you check if the Travelling Merchant sells girlscout cookies next time you see him?";
				else if (rng == 14) return "I'd say my favorite hobby is spreading misinformation, but that'd be a lie.";
				else if (rng == 15) return "I've heard of a cultist that holds the legendary artifact of Dee.";
				else if (rng == 16) return "no";
				else return "Bonjour, afrikan.";
			}
		}
		public override void OnChatButtonClicked(bool firstButton, ref bool shop)
		{
			if (firstButton)
			{
				Main.playerInventory = true;
				Main.npcChatText = "";
				ModContent.GetInstance<UpgradePlus>()._userInterface.SetState(new UI.UpgradeCanvas());
			}
		}
		public override void SetChatButtons(ref string button, ref string button2)
		{
			button =  "Upgrade Gear";
		}
	}
}