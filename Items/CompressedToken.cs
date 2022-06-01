using Terraria.ID;
using Terraria.ModLoader;


namespace UpgraderPlus.Items
{
	public class CompressedToken : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Upgrade Token Sleeve");
			Tooltip.SetDefault("A roll of 32,766 tokens\nJackpot.");
		}

		public override void SetDefaults() 
		{
			item.rare = ItemRarityID.Orange;
			item.value = 500 * (short.MaxValue-1);
			item.maxStack = short.MaxValue-1; // Terraria syncs items using shorts, so any higher value will cause oddities in MP
			item.width = 32;
			item.height = 32;
		}
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<UpgradeToken>(), short.MaxValue - 1);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}