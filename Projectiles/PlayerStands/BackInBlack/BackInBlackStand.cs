using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.BackInBlack
{
    public class BackInBlackStand : StandClass
    {
        public override float ProjectileSpeed => 16f;
        public override int ShootTime => 40;
        public override int ProjectileDamage => 52;
        public override StandAttackType StandType => StandAttackType.Ranged;
        public override int HalfStandHeight => 33;
        public override Vector2 StandOffset => Vector2.Zero;
        public override bool CanUseRangeIndicators => false;

        private int blackHoleWhoAmI = -1;
        private int wormholeWhoAmI = -1;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            secondaryAbilityFrames = player.ownedProjectileCounts[ModContent.ProjectileType<BlackHole>()] != 0;

            if (shootCount > 0)
                shootCount--;
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BlackHole>()] == 0)
                blackHoleWhoAmI = -1;

            if (!secondaryAbilityFrames)
            {
                StayBehind();
            }
            else
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.position = Main.projectile[blackHoleWhoAmI].Center - new Vector2(0f, -300f);
            }
            /*if (blackHoleWhoAmI == 0 && player.ownedProjectileCounts[ModContent.ProjectileType<BlackHole")] != 0)
            {
                blackHoleWhoAmI = BlackHole.whoAmI;
            }*/

            if (Main.mouseLeft && !secondaryAbilityFrames)
            {
                int xOffset = 0;
                if (Projectile.spriteDirection == -1)
                    xOffset = 12;

                Vector2 armPosition = Projectile.Center + new Vector2((-10f + xOffset) * Projectile.spriteDirection, 0f);
                attackFrames = true;
                if (shootCount <= 0)
                {
                    SoundEngine.PlaySound(SoundID.Item78, Projectile.position);
                    shootCount += newShootTime;
                    Vector2 shootVel = Main.MouseWorld - Projectile.Center;
                    if (shootVel == Vector2.Zero)
                        shootVel = new Vector2(0f, 1f);

                    shootVel.Normalize();
                    shootVel *= 1.5f;
                    Vector2 perturbedSpeed = new Vector2(shootVel.X, shootVel.Y);
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), armPosition + new Vector2(4f), perturbedSpeed, ModContent.ProjectileType<BackInBlackOrb>(), newProjectileDamage, 2f, player.whoAmI);
                    Main.projectile[proj].netUpdate = true;
                    Projectile.netUpdate = true;
                }
                if (Main.rand.Next(0, 2 + 1) == 1)
                {

                    int dustIndex = Dust.NewDust(armPosition, 16, 16, DustID.Smoke, Scale: 0.5f);
                    Vector2 velocity = armPosition - Main.dust[dustIndex].position;
                    velocity.Normalize();
                    Main.dust[dustIndex].velocity = velocity * 0.5f;
                    Main.dust[dustIndex].color = Color.Black;
                }
            }
            if (!Main.mouseLeft && !secondaryAbilityFrames)
            {
                idleFrames = true;
                attackFrames = false;
            }
            if (Main.mouseRight && shootCount <= 0 && player.whoAmI == Main.myPlayer && !secondaryAbilityFrames)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<TeleportationWormhole>()] == 0)
                {
                    wormholeWhoAmI = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<TeleportationWormhole>(), 0, 0f, player.whoAmI);
                    shootCount += 60;
                }
                else
                {
                    Projectile wormhole = Main.projectile[wormholeWhoAmI];
                    player.position = wormhole.position;
                    wormholeWhoAmI = -1;
                    wormhole.Kill();
                    shootCount += 60;
                }
            }
            if (SpecialKeyCurrent())
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<BlackHole>()] == 0)
                {
                    blackHoleWhoAmI = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y - 300f, 0f, 0f, ModContent.ProjectileType<BlackHole>(), 0, 0f, player.whoAmI);
                    Main.projectile[blackHoleWhoAmI].scale = 0.05f;
                    Main.projectile[blackHoleWhoAmI].netUpdate = true;
                }
                else
                {
                    Main.projectile[blackHoleWhoAmI].scale += 0.003f;
                    Main.projectile[blackHoleWhoAmI].timeLeft += 2;
                }
            }
            if (!SpecialKeyCurrent() && secondaryAbilityFrames)
            {
                Main.projectile[blackHoleWhoAmI].scale -= 0.005f;
            }
        }

        public override bool PreDraw(ref Color drawColor)      //from ExampleMod ExampleDeathShader
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, GameShaders.Misc["BackInBlackDistortion"].Shader, Main.GameViewMatrix.ZoomMatrix);        //starting a draw with dyes that work

            if (UseProjectileAlpha)
                drawColor *= Projectile.alpha / 255f;

            effects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                effects = SpriteEffects.FlipHorizontally;

            if (standTexture != null && Main.netMode != NetmodeID.Server)
            {
                int frameHeight = standTexture.Height / Main.projFrames[Projectile.type];
                Vector2 drawOffset = StandOffset;
                drawOffset.X *= Projectile.spriteDirection;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition + drawOffset;
                Rectangle animRect = new Rectangle(0, frameHeight * Projectile.frame, standTexture.Width, frameHeight);
                Vector2 standOrigin = new Vector2(standTexture.Width / 2f, frameHeight / 2f);
                Main.EntitySpriteDraw(standTexture, drawPosition, animRect, drawColor, Projectile.rotation, standOrigin, 1f, effects, 0);
            }
            return true;
        }

        public override void SelectAnimation()
        {
            if (attackFrames)
            {
                idleFrames = false;
                PlayAnimation("Attack");
            }
            if (idleFrames)
            {
                attackFrames = false;
                PlayAnimation("Idle");
            }
            if (secondaryAbilityFrames)
            {
                idleFrames = false;
                attackFrames = false;
                PlayAnimation("BlackHole");
            }
            if (Main.player[Projectile.owner].GetModPlayer<MyPlayer>().posing)
            {
                idleFrames = false;
                attackFrames = false;
                PlayAnimation("Pose");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/BackInBlack/BackInBlack_" + animationName).Value;
            if (animationName == "Idle")
            {
                AnimateStand(animationName, 4, 10, true);
            }
            if (animationName == "Attack")
            {
                AnimateStand(animationName, 1, 8, true);
            }
            if (animationName == "BlackHole")
            {
                AnimateStand(animationName, 1, 8, true);
            }
            if (animationName == "Pose")
            {
                AnimateStand(animationName, 4, 15, true);
            }
        }
    }
}