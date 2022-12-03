using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;


namespace UpgradePlus.Items
{
	public class CompressedToken : ModItem
	{
		public override string Texture
		{
			get { return "UpgradePlus/Items/CompressedToken" + ModContent.GetInstance<Client>().artStyle; }
		}

		public override void SetStaticDefaults() 
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
		}

		public override void SetDefaults() 
		{
			Item.rare = ItemRarityID.Orange;
			Item.value = 500 * (short.MaxValue-1);
			Item.maxStack = short.MaxValue-1; // Terraria syncs items using shorts, so any higher value will cause oddities in MP
			Item.width = 32;
			Item.height = 32;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<UpgradeToken>(), short.MaxValue - 1);
			recipe.Register();
		}
	}
}