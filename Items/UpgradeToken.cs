using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;


namespace UpgradePlus.Items
{
	public class UpgradeToken : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Upgrade Token");
			Tooltip.SetDefault("It's a cheap plastic disc\nIt says \"Made in China\" on the back");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
		}

		public override void SetDefaults() 
		{
			Item.rare = ItemRarityID.Blue;
			Item.value = 500;
			Item.maxStack = short.MaxValue-1; // Terraria syncs items using shorts, so any higher value will cause oddities in MP
			Item.width = 25;
			Item.height = 25;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(short.MaxValue-1);
			recipe.AddIngredient(ModContent.ItemType<CompressedToken>(), 1);
			recipe.Register();
		}
	}
}