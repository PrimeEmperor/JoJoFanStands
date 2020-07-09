using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;
using System.Collections.Generic;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
	public class CoolOutT2 : ModItem
	{
        public override string Texture
        {
            get { return mod.Name + "/Items/Stands/CoolOutT1"; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cool Out (Tier 2)");
            Tooltip.SetDefault("Left-click to shoot an Ice Bolt and hold right-click to charge up a spear!\nSpecial: Send out an ice wave!\nUser Name: NekroSektor \nReference: Cool Out and Natural by Imagine Dragons");
        }

        public override void SetDefaults()
        {
            item.damage = 24;
            item.width = 32;
            item.height = 32;
            item.useTime = 12;
            item.useAnimation = 12;
            item.maxStack = 1;
            item.knockBack = 2f;
            item.value = 0;
            item.noUseGraphic = true;
            item.rare = ItemRarityID.LightPurple;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            MyPlayer mPlayer = Main.player[Main.myPlayer].GetModPlayer<MyPlayer>();
            TooltipLine tooltipAddition = new TooltipLine(mod, "Speed", "Shoot Speed: " + (35 - mPlayer.standSpeedBoosts));
            tooltips.Add(tooltipAddition);
        }

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            mult *= (float)player.GetModPlayer<MyPlayer>().standDamageBoosts;
        }

        public override void AddRecipes()
        {
            Mod JoJoStands = ModLoader.GetMod("JoJoStands");
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<CoolOutT1>());
            recipe.AddIngredient(ItemID.DemoniteOre, 20);
            recipe.AddIngredient(ItemID.Shiverthorn, 5);
            recipe.AddIngredient(JoJoStands.ItemType("WillToProtect"));
            recipe.AddIngredient(JoJoStands.ItemType("WillToControl"));
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<CoolOutT1>());
            recipe.AddIngredient(ItemID.CrimtaneOre, 20);
            recipe.AddIngredient(ItemID.Shiverthorn, 5);
            recipe.AddIngredient(JoJoStands.ItemType("WillToProtect"));
            recipe.AddIngredient(JoJoStands.ItemType("WillToControl"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}