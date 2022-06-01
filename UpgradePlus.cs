using UpgraderPlus.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;
using Terraria.ModLoader;

namespace UpgraderPlus
{

    public class UpgradePlus : Mod
    {

        internal UpgradeCanvas upgradeCanvas;
        public UserInterface _userInterface;


        public override void Load()
        {
            if (!Main.dedServ)
            {
                upgradeCanvas = new UpgradeCanvas();
                upgradeCanvas.Activate();
                _userInterface = new UserInterface();
                _userInterface.SetState(null);
            }
        }
        public override void Unload()
        {
            if (!Main.dedServ)
            {
                upgradeCanvas.Deactivate();
                upgradeCanvas = null;
                _userInterface.SetState(null);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _userInterface?.Update(gameTime);
        }

        public override void PostSetupContent()
        {
            Mod censusMod = ModLoader.GetMod("Census");
            if (censusMod != null)
            {
                censusMod.Call("TownNPCCondition", NPCType("Blacksmith"), "When EoC, EoW, BoC, or Skeletron are defeated");
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "UpgradePlus interface",
                    delegate
                    {
                        _userInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

        }
    }
}