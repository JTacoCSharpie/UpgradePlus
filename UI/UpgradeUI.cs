using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.GameInput;
using Terraria.ModLoader;
using System.Collections.Generic;


namespace UpgraderPlus.UI
{
    internal class UpgradeCanvas : UIState
    {
        private ItemWrapper itemWrapper;

        public override void OnInitialize()
        {
            itemWrapper = new ItemWrapper(ItemSlot.Context.InventoryItem, 0.9f)
            {
                Left = { Pixels = 50 },
                Top = { Pixels = 270 },
                ValidItemFunc = item => item.IsAir || item.headSlot > -1 || item.bodySlot > -1 || item.legSlot > -1 || item.Prefix(-3) || item.accessory || !item.IsAir && Levelhandler.anythingLevels && item.maxStack == 1
            };
            Append(itemWrapper);
        }
        public override void OnDeactivate()
        {
            if (!itemWrapper.item.IsAir)
            {
                Main.LocalPlayer.QuickSpawnClonedItem(itemWrapper.item, itemWrapper.item.stack);
                itemWrapper.item.TurnToAir();
            }
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Main.LocalPlayer.talkNPC == -1 || Main.npc[Main.LocalPlayer.talkNPC].type != ModContent.NPCType<Npcs.Blacksmith>())
            {
                ModContent.GetInstance<UpgradePlus>()._userInterface.SetState(null);
            }
        }

        private int whoAmItem;
        private int lastLevel;
        private string maxCostCache;
        private bool tick1Played;
        private bool tick2Played;
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Main.HidePlayerCraftingMenu = true;

            const int slotX = 50;
            const int slotY = 270;
            if (!itemWrapper.item.IsAir)
            {
                int level = itemWrapper.item.GetGlobalItem<Globals.ItemLevelHooks>().level;
                if (level >= 5) // REFUND --
                {
                    int refundX;
                    int refundY;
                    if (level >= Levelhandler.tierCap)
                    {
                        refundX = slotX + 100;
                        refundY = slotY;
                    }
                    else
                    {
                        refundX = slotX + 150;
                        refundY = slotY + 30;
                    }
                    bool hoveringOverRefundButton = Main.mouseX > refundX - 55 && Main.mouseX < refundX + 125 && Main.mouseY > refundY - 5 && Main.mouseY < refundY + 20 && !PlayerInput.IgnoreMouseInterface;
                    int refundCost = (int)Math.Ceiling(Levelhandler.GetCostForGivenLevel(itemWrapper.item, level) * 0.9);
                    Color col = Color.Gray;
                    if (hoveringOverRefundButton)
                    {
                        col = Color.Green;
                        if (!tick2Played)
                        {
                            Main.PlaySound(SoundID.MenuTick, -1, -1, 1, 1f, 0f);
                        }
                        tick2Played = true;
                        Main.LocalPlayer.mouseInterface = true;

                        if (Main.mouseLeftRelease && Main.mouseLeft) // Refund Item
                        {
                            Levelhandler.Refund(itemWrapper.item);
                            Main.PlaySound(SoundID.Item37, -1, -1);
                        }
                    }
                    else
                    {
                        tick2Played = false;
                    }
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, "Refund  " + string.Format("{0:n0}", refundCost) + " tokens?", new Vector2(refundX - 50, refundY), col, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
                }

                int upgradeCap = Math.Min(Levelhandler.tierCap, 20);
                if (Main.hardMode || !Levelhandler.doHardmodeCap)
                {
                    upgradeCap = Levelhandler.tierCap;
                }

                if (level < upgradeCap)
                {
                    int reforgeX = slotX + 70;
                    int reforgeY = slotY + 40;
                    bool hoveringOverReforgeButton = Main.mouseX > reforgeX - 15 && Main.mouseX < reforgeX + 15 && Main.mouseY > reforgeY - 15 && Main.mouseY < reforgeY + 15 && !PlayerInput.IgnoreMouseInterface;
                    Texture2D reforgeTexture = Main.reforgeTexture[hoveringOverReforgeButton ? 1 : 0];
                    Main.spriteBatch.Draw(reforgeTexture, new Vector2(reforgeX, reforgeY), null, Color.White, 0f, reforgeTexture.Size() / 2f, 0.8f, SpriteEffects.None, 0f);
                    int cost = Levelhandler.GetCost(itemWrapper.item, level + 1);
                    if (whoAmItem != itemWrapper.item.type || maxCostCache == string.Empty || level != lastLevel) // Only recalculate the max cost when something's changed
                    {
                        maxCostCache = string.Format("{0:n0}", Levelhandler.GetCostForGivenLevel(itemWrapper.item, upgradeCap) - Levelhandler.GetCostForGivenLevel(itemWrapper.item, level));
                        lastLevel = level;
                        whoAmItem = itemWrapper.item.type;
                    }

                    string costStr = "[c/" + Colors.AlphaDarken(Colors.CoinPlatinum).Hex3() + ":" + "Cost: " + String.Format("{0:n0}", cost) + " Upgrade Tokens/" + maxCostCache + " Max]";
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, costStr, new Vector2(slotX + 50, (float)slotY), Color.White, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
                    if (hoveringOverReforgeButton) // UPGRADE --
                    {
                        Main.hoverItemName = "Upgrade\nRight click to max";
                        if (!tick1Played)
                        {
                            Main.PlaySound(SoundID.MenuTick, -1, -1, 1, 1f, 0f);
                        }
                        tick1Played = true;
                        Main.LocalPlayer.mouseInterface = true;
                        if (Main.mouseLeftRelease && Main.mouseLeft || Main.mouseRight && Main.mouseRightRelease)
                        {
                            if (Main.mouseRight && Main.mouseRightRelease && !Main.mouseLeft) // Loop upgrades
                            {
                                Levelhandler.MultiBuy(Main.LocalPlayer, itemWrapper.item, upgradeCap);
                            }
                            else if (Main.mouseLeftRelease && Main.mouseLeft && !Main.mouseRight) // Buy once
                            {
                                Levelhandler.MultiBuy(Main.LocalPlayer, itemWrapper.item, level + 1);
                            }
                            itemWrapper.item.position.X = Main.LocalPlayer.position.X + (float)(Main.LocalPlayer.width / 2) - (float)(itemWrapper.item.width / 2);
                            itemWrapper.item.position.Y = Main.LocalPlayer.position.Y + (float)(Main.LocalPlayer.height / 2) - (float)(itemWrapper.item.height / 2);
                            CombatText.NewText(new Rectangle((int)Main.npc[Main.LocalPlayer.talkNPC].position.X, (int)Main.npc[Main.LocalPlayer.talkNPC].position.Y, 5, 0), Main.DiscoColor, "+" + itemWrapper.item.GetGlobalItem<Globals.ItemLevelHooks>().level, false, false);
                            Main.PlaySound(SoundID.Item37, -1, -1);
                        }
                    }
                    else
                    {
                        tick1Played = false;
                    }
                }
                else if (level < Levelhandler.tierCap && Levelhandler.doHardmodeCap && upgradeCap >= 20)
                {
                    string hmCheck = "[c/" + Colors.AlphaDarken(Colors.RarityRed).Hex3() + ":" + "Reach Hardmode to further upgrade]";
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, hmCheck, new Vector2(slotX + 50, (float)slotY), Color.White, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
                }

            }
        }
    }

    internal class ItemWrapper : UIElement      // Item Wrapper taken from examplemod
    {
        internal Item item;
        private readonly int _context;
        private readonly float _scale;
        internal Func<Item, bool> ValidItemFunc;
        public ItemWrapper(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;
            item = new Item();
            item.SetDefaults(0);

            Width.Set(Main.inventoryBack9Texture.Width * scale, 0f);
            Height.Set(Main.inventoryBack9Texture.Height * scale, 0f);
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();

            if (ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (ValidItemFunc == null || ValidItemFunc(Main.mouseItem))
                {
                    // Handle handles all the click and hover actions based on the context.
                    ItemSlot.Handle(ref item, _context);
                }
            }
            // Draw draws the slot itself and Item. Depending on context, the color will change, as will drawing other things like stack counts.
            ItemSlot.Draw(spriteBatch, ref item, _context, rectangle.TopLeft());
            Main.inventoryScale = oldScale;
        }
    }
}