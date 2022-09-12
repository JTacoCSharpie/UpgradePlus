using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.DataStructures;
using ReLogic.Content;
using Terraria.ModLoader.IO;


namespace UpgradePlus.Npcs
{

	[AutoloadHead]
	public class Blacksmith : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blacksmith");
			Main.npcFrameCount[NPC.type] = 26;
			NPCID.Sets.ExtraFramesCount[NPC.type] = 6;
			NPCID.Sets.AttackFrameCount[NPC.type] = 5;
			NPCID.Sets.DangerDetectRange[NPC.type] = 700; // How far a Town NPC can detect an enemy.
			NPCID.Sets.AttackType[NPC.type] = 0; // The attack style an NPC copies. Set to 0 if the NPC has a custom attack.
			NPCID.Sets.AttackTime[NPC.type] = 120;
			NPCID.Sets.AttackAverageChance[NPC.type] = 45;
			NPCID.Sets.HatOffsetY[NPC.type] = 6;
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0)
			{
				Velocity = 0.0001f, // Draws the NPC in the bestiary as if its walking +X tiles in the x direction
				Rotation = MathHelper.ToRadians(0)
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);

			NPC.Happiness
				.SetBiomeAffection<ForestBiome>(AffectionLevel.Like)
				.SetBiomeAffection<SnowBiome>(AffectionLevel.Like)

				.SetBiomeAffection<OceanBiome>(AffectionLevel.Love)
				.SetBiomeAffection<DesertBiome>(AffectionLevel.Hate)

				.SetNPCAffection(NPCID.Dryad, AffectionLevel.Love)
				.SetNPCAffection(633, AffectionLevel.Love) // Zoologist
				.SetNPCAffection(NPCID.Steampunker, AffectionLevel.Love)

				.SetNPCAffection(NPCID.Wizard, AffectionLevel.Like)
				.SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Like)
				.SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Dislike)
			;
		}


		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
				new FlavorTextBestiaryInfoElement("A no-longer-wandering Blacksmith looking to amass serious financial gains by offering gear upgrades for boss tokens."),
			});
		}

		public override void SetDefaults()
		{
			NPC.friendly = true;
			NPC.townNPC = true;
			NPC.width = 27;
			NPC.height = 45;
			NPC.aiStyle = 7;
			NPC.damage = 15;
			NPC.defense = 20;
			NPC.lifeMax = 300;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;
			AIType = 22; // What type of AI an NPC copies.
			AnimationType = NPCID.Guide;
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.WhoopieCushion));
		}

		public override bool CanGoToStatue(bool toKingStatue)
		{
			return true;
		}
		public override bool CanTownNPCSpawn(int numTownNPCs, int money)
		{
			return (NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3); // Spawn if EoC, EoW/BoC, or Skele have been defeated
		}
		public override List<string> SetNPCNameList()
		{
			List<string> names = new(){ "Ketsuban", "Hayato", "Jeff", "Doug", "Hugh", "Derf", "Sam", "Peter", "Bartholomew", "Adam", "Sprout", "Jeb!", "ZZAZZ", "Bobby" };
			return names;
		}
		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 15; // The amount of damage the Town NPC inflicts.
			knockback = 4f; // The amount of knockback the Town NPC inflicts.
			if (Main.hardMode)
            {
				damage *= 4;
				knockback *= 2;
            }
		}
		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 30; // The amount of ticks the Town NPC takes to cool down. Every 60 in-game ticks is a second.
			randExtraCooldown = 30; // How long it takes until the NPC attacks again, but with a chance.
		}
		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ProjectileID.VampireKnife;
			attackDelay = 1; // Delays attacks by X number of frames
		}
		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 10f; // The Speed of the Projectile
			gravityCorrection = -1f;
			randomOffset = -1f; // Random Offset
		}

		public override string GetChat()
		{
			double spent = Main.LocalPlayer.GetModPlayer<UPPlayer>().SpentTokens;
			List<string> nonconditionals = new()
			{
				"Turn your KO cannons into Killer Cannons.",
				"Upgrade your bee gun to see all your enemies begone.",
				"Some of my finest works could dig hundreds of meters in seconds with the right ammo. No I didn't bring any of them with me.",
				"Don't you hate it when people don't finish their",
				"How far could a fargo go if a fargo could go far?",
				"I keep hearing awful stories about \"shoebox\" houses.",
				"Don't be ridiculous, these tokens hoarded by powerful creatures have no value outside of giving them to me.",
				"So what's got you all dolled up today?",
				"You know, as our landlord you're obligated to have more fire extinquishers around here.",
				"Violence is never the answer, it's always the question, and the answer is yes.",
				"I got my technique from a guy in a blue cap, real nice guy but whenever I asked his name he'd always reply \"Upyours\".",
				"Quick, the secret formula for crafting upgrade tokens is-",
				"Why in the world are there so many gold critters? Was there an explosion in a flask factory?",
				"Can you check if the Travelling Merchant sells girlscout cookies next time you see him?",
				"I'd say my favorite hobby is spreading misinformation, but that'd be a lie.",
				"I've heard of a cultist that holds the legendary artifact of Dee.",
				"Bonjour, afrikan.",
				"no",
		};

			// Mod messages
			List<string> condLines = new();
			string[] potentialJobs = { "at the PD.", "at the deli", "in the fire department", "as a life guard", "as a plumber", "as a streamer", "as a vtuber" };
			if (ModLoader.HasMod("CalamityMod"))
            {
				condLines.Add("You run Yharim for his foams yet?");
				condLines.Add("The only thing calamitous around here is your fashion sense.");
				Mod Calamity = ModLoader.GetMod("CalamityMod");
				if (NPC.FindFirstNPC(Calamity.Find<ModNPC>("FAP").Type) > 0)
                {
					condLines.Add("Cirrus scares me. I dared her to drink a bucket of lava and she looked me dead in the eye 'n started chugging.");
				}
				if (NPC.FindFirstNPC(Calamity.Find<ModNPC>("WITCH").Type) > 0)
                {
					condLines.Add("I hear calamitas got a job working " + Main.rand.Next(potentialJobs) + " to pay rent the other day");
				}
			}
			if (ModLoader.HasMod("UpgradeEquipment") || ModLoader.HasMod("UpgradeEquipment_hrr"))
            {
				condLines.Add("So what if my prices are high? What are you going to do, find another equipment upgrader?");
			}

			// Conditional vanilla messages
			if (NPC.FindFirstNPC(NPCID.TaxCollector) < 0)
			{
				condLines.Add("Living in your head, rent free.");
				condLines.Add("Taxes evaded, my truth goes unstated, living in your head rent freee~");
			}
			else
			{
				condLines.Add("10% off your next upgrade if you lower my rent.");
			}
			if (Main.dayTime)
            {
				condLines.Add("Hey go kill some more blue slimes, I need to craft some sticky pistons.");
            }
			else
            {
				condLines.Add("Where do all these zombies keep coming from?");
				condLines.Add("My prices may be high but the sun is not. Upgrade or perish.");
            }
			if (spent > 1)
            {
				condLines.Add("I checked the numbers and you've spent " + String.Format("{0:n0}", spent) + " tokens on upgrades, that's " + String.Format("{0:n0}", Math.Floor((spent + 1) / (short.MaxValue - 1))) + " full sleeves of tokens.");
            }

			int category = Main.rand.Next(10);
			if (category > 1) // 8 in 10 chance
            {
				return Main.rand.Next(nonconditionals);
            }
			else
			{
				return Main.rand.Next(condLines);
			}
		}

		public override void OnChatButtonClicked(bool firstButton, ref bool shop)
		{
			if (firstButton)
			{
				Main.playerInventory = true;
				Main.npcChatText = "";
				ModContent.GetInstance<UI.UpgradePlusUI> ()._userInterface.SetState(new UI.UpgradeCanvas());
			}
		}
		public override void SetChatButtons(ref string button, ref string button2)
		{
			button =  "Upgrade Gear";
		}
	}
}