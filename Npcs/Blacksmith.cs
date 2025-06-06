using static UpgradePlus.Localization;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using System;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;


namespace UpgradePlus.Npcs
{

	[AutoloadHead]
	public class Blacksmith : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 26;
			NPCID.Sets.ExtraFramesCount[NPC.type] = 6;
			NPCID.Sets.AttackFrameCount[NPC.type] = 5;
			// How far a Town NPC can detect an enemy.
			NPCID.Sets.DangerDetectRange[NPC.type] = 700;
			// The attack style an NPC copies. Set to 0 if the NPC has a custom attack.
			NPCID.Sets.AttackType[NPC.type] = 0;
			NPCID.Sets.AttackTime[NPC.type] = 120;
			NPCID.Sets.AttackAverageChance[NPC.type] = 45;
			NPCID.Sets.HatOffsetY[NPC.type] = 6;
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0)
			{
				// Draws the NPC in the bestiary as if its walking +X tiles in the x direction
				Velocity = 0.0001f,
				Rotation = MathHelper.ToRadians(0)
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);
			NPCID.Sets.NoTownNPCHappiness[NPC.type] = true;
		}
		public override string HeadTexture => ("UpgradePlus/Npcs/Blacksmith_Head" + ModContent.GetInstance<Client>().artStyle);
		public override string Texture { get { return "UpgradePlus/Npcs/Blacksmith" + ModContent.GetInstance<Client>().artStyle; } }


		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
				new FlavorTextBestiaryInfoElement( GetTrans("BlacksmithNPC.BestiaryBlurb") ),
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
		public override bool CanTownNPCSpawn(int numTownNPCs)
		{
			// Spawn if EoC, EoW/BoC, or Skele have been defeated
			return (NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3);
		}
		public override List<string> SetNPCNameList()
		{
			List<string> names = new() { "Ketsuban", "Hayato", "Jeff", "Doug", "Hugh", "Derf", "Sam", "Peter", "Bartholomew", "Adam", "Sprout", "Jeb!", "ZZAZZ", "Bobby", "Luso" };
			return names;
		}
		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 15;
			knockback = 4f;
			if (Main.hardMode)
			{
				damage *= 4;
				knockback *= 2;
			}
		}
		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			// The amount of ticks the Town NPC takes to cool down. Every 60 in-game ticks is a second.
			cooldown = 30;
			// How long it takes until the NPC attacks again, but with a chance.
			randExtraCooldown = 30;
		}
		public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ProjectileID.VampireKnife;
			// Delays attacks by X number of frames
			attackDelay = 1;
		}
		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 10f;
			gravityCorrection = -1f;
			randomOffset = -1f;
		}

		/// <summary> Loops over "Mods.UpgradePlus."+partialPath+"Line"+i so the destination is expected to have Line0 and onward contained </summary>
		private List<string> GetMultiLinesAtPath(string partialPath)
		{
			List<string> lines = new();
			for (int i = 0; i < 100; i++)
			{
				// If our output is the same as our input then we've hit an invalid localization key
				if (GetTrans(partialPath + ".Line" + i) != "Mods.UpgradePlus." + partialPath + ".Line" + i)
				{
					lines.Add(GetTrans(partialPath + ".Line" + i));
				}
				// using invalid localization keys as a stopping point so I can be lazy and not hardcode the loop stops
				else
				{
					//If the keys was invalid from the get-go
					if (i == 0 && Main.netMode != NetmodeID.Server)
					{ Main.NewText("Please report an error with localization: [" + partialPath + "] to ths UpgradePlus dev"); }
					break;
				}
			}
			return lines;
		}

		public List<string> recentDialogue = new();
		public override string GetChat()
		{
			double spent = Main.LocalPlayer.GetModPlayer<UPPlayer>().SpentTokens;
			List<string> nonconditionals = new();
			List<string> condLines = new();

			// Standard Dialogue
			nonconditionals.AddRange(GetMultiLinesAtPath("BlacksmithNPC.Dialogue.StandardLines"));


			// Modded Messages
			if (ModLoader.HasMod("CalamityMod"))
			{
				// Calamity lines
				condLines.AddRange(GetMultiLinesAtPath("BlacksmithNPC.Dialogue.Calamity"));
				Mod Calamity = ModLoader.GetMod("CalamityMod");
				if (Calamity.TryFind<ModNPC>("FAP", out ModNPC fab) && NPC.FindFirstNPC(fab.Type)  > -1)
				{
					// Fabsol lines
					condLines.AddRange(GetMultiLinesAtPath("BlacksmithNPC.Dialogue.Calamity.DrunkPrincessExists"));
				}
                if (Calamity.TryFind<ModNPC>("WITCH", out ModNPC cal) && NPC.FindFirstNPC(cal.Type) > -1)
				{
					// Calamitas lines
					List<string> potentialJobs = GetMultiLinesAtPath("BlacksmithNPC.Dialogue.Calamity.CalamitasJobs");
					condLines.Add(GetTrans("BlacksmithNPC.Dialogue.Calamity.CalamitasExists", Main.rand.Next(potentialJobs)));
				}
			}
			if (ModLoader.HasMod("UpgradeEquipment") || ModLoader.HasMod("UpgradeEquipment_hrr"))
			{
				// Upgrade Equip lines
				condLines.AddRange(GetMultiLinesAtPath("BlacksmithNPC.Dialogue.UpgradeMods"));
			}

			// Conditional Vanilla Messages
			if (NPC.FindFirstNPC(NPCID.TaxCollector) == -1)
			{
				// No Tax Collector
				condLines.AddRange(GetMultiLinesAtPath("BlacksmithNPC.Dialogue.NoTaxes"));
			}
			else
			{
				// Tax Collector
				condLines.AddRange(GetMultiLinesAtPath("BlacksmithNPC.Dialogue.Taxes"));
			}
			if (NPC.FindFirstNPC(NPCID.BestiaryGirl) > -1)
			{
				// Zoologist
				condLines.Add(GetTrans("BlacksmithNPC.Dialogue.Flirt", Main.npc[NPC.FindFirstNPC(NPCID.BestiaryGirl)].GivenName));
			}
			if (NPC.FindFirstNPC(NPCID.Steampunker) > -1)
			{
				// Steampunker
				condLines.Add(GetTrans("BlacksmithNPC.Dialogue.Flirt", Main.npc[NPC.FindFirstNPC(NPCID.Steampunker)].GivenName));
			}
			if (Main.dayTime)
			{
				// Daytime
				condLines.AddRange(GetMultiLinesAtPath("BlacksmithNPC.Dialogue.DayLines"));
			}
			else
			{
				// Night
				condLines.AddRange(GetMultiLinesAtPath("BlacksmithNPC.Dialogue.NightLines"));
			}
			if (Main.LocalPlayer.ZoneBeach)
			{
				// Ocean
				condLines.AddRange(GetMultiLinesAtPath("BlacksmithNPC.Dialogue.OceanLines"));
			}
			if (spent > 1)
			{
				// Spent tokens info
				condLines.Add(GetTrans("BlacksmithNPC.Dialogue.SpentTokens", String.Format("{0:n0}", spent), String.Format("{0:n0}", Math.Floor((spent + 1) / (short.MaxValue - 1)))));
			}

			//Default line
			string MSG = "Wow, you found a secret message! The error message. Either the mod itself or an interaction with another mod has lead to this error :thumbsup:";
			int category = Main.rand.Next(10);

			for (int i = 0; i < 10; i++)
			{
				if (category > 1)
				{
					MSG = Main.rand.Next(nonconditionals);
				}
				else
				{
					MSG = Main.rand.Next(condLines);
				}

				if (recentDialogue.Contains(MSG))
				{
					continue;
				}
				else
				{
					recentDialogue.Add(MSG);
					if (recentDialogue.Count > 12)
					{
						recentDialogue.RemoveAt(0);
					}
					break;
				}
			}
			return MSG;

		}

		public override void OnChatButtonClicked(bool firstButton, ref string shopName)
		{
			if (firstButton)
			{
				Main.playerInventory = true;
				Main.npcChatText = "";
				ModContent.GetInstance<UI.UpgradePlusUI>()._userInterface.SetState(new UI.UpgradeCanvas());
			}
		}
		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = GetTrans("BlacksmithNPC.UpgradeButtonBlurb");
		}
	}
}