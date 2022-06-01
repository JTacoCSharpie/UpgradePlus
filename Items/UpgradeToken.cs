using Terraria.ID;
using Terraria.ModLoader;


namespace UpgraderPlus.Items
{
	public class UpgradeToken : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Upgrade Token");
			Tooltip.SetDefault("It's a cheap plastic disc\nIt says \"Made in China\" on the back");
		}

		public override void SetDefaults() 
		{
			item.rare = ItemRarityID.Blue;
			item.value = 500;
			item.maxStack = short.MaxValue-1; // Terraria syncs items using shorts, so any higher value will cause oddities in MP
			item.width = 25;
			item.height = 25;
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<CompressedToken>(), 1);
			recipe.SetResult(this, short.MaxValue-1);
			recipe.AddRecipe();
		}
	}
}