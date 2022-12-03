using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace UpgradePlus
{

    [Label("$Mods.UpgradePlus.Configs.ClientClassName")]
    public class Client : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("$Mods.UpgradePlus.Configs.Shared.CoreFeatures")]
        [Label("$Mods.UpgradePlus.Configs.CritDamage")]
        [Tooltip("$Mods.UpgradePlus.Configs.CritDamageDesc")]
        [DefaultValue(true)]
            public bool doCritDamage;
        [Label("$Mods.UpgradePlus.Configs.DoAutofire")]
        [Tooltip("$Mods.UpgradePlus.Configs.DoAutofireDesc")]
        [DefaultValue(false)]
            public bool setReuse;
        [Label("$Mods.UpgradePlus.Configs.RefundLevels")]
        [Tooltip("$Mods.UpgradePlus.Configs.RefundLevelsDesc")]
        [DefaultValue(true)]
            public bool doClientRefunds;

        [Header("$Mods.UpgradePlus.Configs.Shared.TuneFeatures")]
        [Label("$Mods.UpgradePlus.Configs.SizeEff")]
        [Tooltip("$Mods.UpgradePlus.Configs.SharedTuneDesc")]
        [Increment(5)]
        [Range(0, 100)]
        [DefaultValue(100)]
        [Slider]
            public int sizeMulti;
        [Label("$Mods.UpgradePlus.Configs.VelEff")]
        [Tooltip("$Mods.UpgradePlus.Configs.SharedTuneDesc")]
        [Increment(5)]
        [Range(0, 200)]
        [DefaultValue(100)]
        [Slider]
            public int velocityMulti;
        [Label("$Mods.UpgradePlus.Configs.KBEff")]
        [Tooltip("$Mods.UpgradePlus.Configs.SharedTuneDesc")]
        [Increment(5)]
        [Range(0, 200)]
        [DefaultValue(100)]
        [Slider]
            public int knockbackMulti;
        [Label("$Mods.UpgradePlus.Configs.SpeedEff")]
        [Tooltip("$Mods.UpgradePlus.Configs.SharedTuneDesc")]
        [Increment(5)]
        [Range(0, 100)]
        [DefaultValue(100)]
        [Slider]
            public int speedMulti;
        [Label("$Mods.UpgradePlus.Configs.WingEff")]
        [Tooltip("$Mods.UpgradePlus.Configs.SharedTuneDesc")]
        [Increment(5)]
        [Range(0, 200)]
        [DefaultValue(100)]
        [Slider]
            public int wingMulti;

        [Header("$Mods.UpgradePlus.Configs.Shared.Misc")]
        [ReloadRequired]
        [Label("$Mods.UpgradePlus.Configs.AltSprites")]
        [Tooltip("$Mods.UpgradePlus.Configs.AltSpritesDesc")]
        [Increment(1)]
        [Range(0,1)]
        [DefaultValue(0)]
        [Slider]
            public int artStyle;
        [Label("$Mods.UpgradePlus.Configs.CritRolloverDebug")]
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

    [Label("$Mods.UpgradePlus.Configs.ServerClassName")]
    [BackgroundColor(30, 15, 15, 150)]
    public class Server : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("$Mods.UpgradePlus.Configs.Shared.CoreFeatures")]
        [BackgroundColor(100, 50, 50, 255)]
        [Label("$Mods.UpgradePlus.Configs.LevelCap")]
        [Tooltip("$Mods.UpgradePlus.Configs.LevelCapDesc")]
        [Increment(5)]
        [Range(20, 255)]
        [DefaultValue(40)]
            public int tierCap;

        [BackgroundColor(100, 50, 50, 255)]
        [Label("$Mods.UpgradePlus.Configs.StatBalance")]
        [Tooltip("$Mods.UpgradePlus.Configs.StatBalanceDesc")]
        [Range(0, 2)]
        [DefaultValue(1)]
        [Slider]
            public int statFormulaModel;

        [BackgroundColor(100, 50, 50, 255)]
        [Label("$Mods.UpgradePlus.Configs.HardmodeCap")]
        [Tooltip("$Mods.UpgradePlus.Configs.HardmodeCapDesc")]
        [DefaultValue(true)]
            public bool doHardmodeCaps;
        [BackgroundColor(100, 50, 50, 255)]
        [Label("$Mods.UpgradePlus.Configs.RefundOverride")]
        [Tooltip("$Mods.UpgradePlus.Configs.RefundOverrideDesc")]
        [DefaultValue(true)]
            public bool doServerRefunds;

        [Header("$Mods.UpgradePlus.Configs.Shared.TuneFeatures")]
        [Label("$Mods.UpgradePlus.Configs.Autofire")]
        [BackgroundColor(100, 50, 50, 255)]
        [Tooltip("$Mods.UpgradePlus.Configs.AutofireDesc")]
        [Increment(5)]
        [Range(0, 256)]
        [DefaultValue(20)]
            public int reuseLevel;

        [Header("$Mods.UpgradePlus.Configs.Shared.Fun")]
        [BackgroundColor(100, 50, 50, 255)]
        [Label("$Mods.UpgradePlus.Configs.CritRollover")]
        [Tooltip("$Mods.UpgradePlus.Configs.CritRolloverDesc")]
        [DefaultValue(false)]
            public bool critRollover;
        [BackgroundColor(100, 50, 50, 255)]
        [Label("$Mods.UpgradePlus.Configs.AnythingLevels")]
        [Tooltip("$Mods.UpgradePlus.Configs.AnythingLevelsDesc")]
        [DefaultValue(false)]
            public bool anythingLevels;

        public override void OnChanged()
        {
            Levelhandler.formula = statFormulaModel;
            Levelhandler.tierCap = tierCap;
            Levelhandler.doHardmodeCap = doHardmodeCaps;
            Levelhandler.doServerRefunds = doServerRefunds;

            Levelhandler.critRollover = critRollover;
            Levelhandler.reuseLevel = reuseLevel;
            Levelhandler.anythingLevels = anythingLevels;
        }
    }

}
