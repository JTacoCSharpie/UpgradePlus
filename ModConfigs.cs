using System;
using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace UpgradePlus
{

    public class Client : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("$Mods.UpgradePlus.Configs.SharedHeaders.CoreFeatures")]
        [DefaultValue(true)]
        public bool doCritDamage;
        [DefaultValue(false)]
        public bool setReuse;
        [DefaultValue(true)]
        public bool doClientRefunds;

        [Header("$Mods.UpgradePlus.Configs.SharedHeaders.TuneFeatures")]
        [Increment(5)]
        [Range(0, 100)]
        [DefaultValue(100)]
        [Slider]
        public int sizeMulti;
        [Increment(5)]
        [Range(0, 200)]
        [DefaultValue(100)]
        [Slider]
        public int velocityMulti;
        [Increment(5)]
        [Range(0, 200)]
        [DefaultValue(100)]
        [Slider]
        public int knockbackMulti;
        [Increment(5)]
        [Range(0, 100)]
        [DefaultValue(100)]
        [Slider]
        public int speedMulti;
        [Increment(5)]
        [Range(0, 200)]
        [DefaultValue(100)]
        [Slider]
        public int wingMulti;

        [Header("$Mods.UpgradePlus.Configs.SharedHeaders.Misc")]
        [ReloadRequired]
        [Increment(1)]
        [Range(0, 1)]
        [DefaultValue(0)]
        [Slider]
        [DrawTicks]
        public int artStyle;
        [DefaultValue(false)]
        public bool doDebug;

        [Header("$Mods.UpgradePlus.Configs.NextPage")]
        public bool ThisOptionDoesNothingButItdBeFunnyIfItSecretlyRecordedWhoActivatesItInSomeSortOfMassSocialExperimentTooBadItsJustAHeader;

        public override void OnChanged()
        {
            Levelhandler.doCritDamage = doCritDamage;
            Levelhandler.setReuse = setReuse;
            Levelhandler.doClientRefunds = doClientRefunds;

            Levelhandler.velMulti = velocityMulti;
            Levelhandler.KBMulti = knockbackMulti;
            Levelhandler.sizeMulti = sizeMulti;
            Levelhandler.speedMulti = speedMulti;
            Levelhandler.wingMulti = wingMulti;

            Levelhandler.doDebug = doDebug;
        }
    }

    [BackgroundColor(30, 15, 15, 150)]
    public class Server : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("$Mods.UpgradePlus.Configs.SharedHeaders.CoreFeatures")]
        [BackgroundColor(100, 50, 50, 255)]
        [Increment(5)]
        [Range(20, 255)]
        [DefaultValue(40)]
        public int tierCap;

        [BackgroundColor(100, 50, 50, 255)]
        [Range(0, 3)]
        [DefaultValue(1)]
        [Slider]
        [DrawTicks]
        public int statFormulaModel;

        [BackgroundColor(100, 50, 50, 255)]
        [Range(20, 256)]
        [DefaultValue(20)]
        public int hardmodeCap;
        [BackgroundColor(100, 50, 50, 255)]
        [Range(20, 256)]
        [DefaultValue(256)]
        public int moonlordCap;


        [BackgroundColor(100, 50, 50, 255)]
        [DefaultValue(true)]
        public bool doServerRefunds;

        [Header("$Mods.UpgradePlus.Configs.SharedHeaders.TuneFeatures")]
        [BackgroundColor(100, 50, 50, 255)]
        [Increment(5)]
        [Range(0, 256)]
        [DefaultValue(20)]
        public int reuseLevel;
        [BackgroundColor(100, 50, 50, 255)]
        [DefaultValue(false)]
        public bool toughTokens;

        [Header("$Mods.UpgradePlus.Configs.SharedHeaders.Fun")]
        [BackgroundColor(100, 50, 50, 255)]
        [DefaultValue(false)]
        public bool critRollover;
        [BackgroundColor(100, 50, 50, 255)]
        [DefaultValue(false)]
        public bool anythingLevels;

        public override void OnChanged()
        {
            Levelhandler.formula = statFormulaModel;
            Levelhandler.tierCap = tierCap;
            Levelhandler.hardmodeCap = hardmodeCap;
            Levelhandler.moonlordCap = moonlordCap;
            Levelhandler.doServerRefunds = doServerRefunds;

            Levelhandler.critRollover = critRollover;
            Levelhandler.reuseLevel = reuseLevel;
            Levelhandler.toughTokens = toughTokens;
            Levelhandler.anythingLevels = anythingLevels;
        }
    }

}
