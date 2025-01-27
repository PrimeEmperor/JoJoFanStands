using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.Banks
{
    public class BanksStandT2 : StandClass
    {
        public override float ProjectileSpeed => 16f;
        public override int ShootTime => 10;
        public override int ProjectileDamage => 7;
        public override int TierNumber => 2;
        public override StandAttackType StandType => StandAttackType.Ranged;
        public override int HalfStandHeight => 32;
        public override Vector2 StandOffset => Vector2.Zero;


        private const float TargetDetectionRange = 32f * 16f;
        private NPC target = null;
        private int shotgunChargeTimer = 0;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            //Lighting.AddLight((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f), 0.6f, 0.9f, 0.3f);
            //Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 35, Projectile.velocity.X * -0.5f, Projectile.velocity.Y * -0.5f);

            if (shootCount > 0)
                shootCount--;
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (Main.mouseLeft && player.whoAmI == Projectile.owner)
            {
                if (target == null)
                {
                    target = FindNearestTarget(TargetDetectionRange);
                }
                else
                {
                    attackFrames = true;
                    Projectile.position = target.Center + new Vector2(((target.width / 2f) + Projectile.width) * -target.direction, 0f);
                    Projectile.direction = target.direction;
                    if (Projectile.Distance(target.Center) >= TargetDetectionRange)
                    {
                        target = null;
                    }

                    if (shootCount <= 0 && Projectile.frame == 2)
                    {
                        shootCount += ShootTime;
                        NPC.HitInfo hitInfo = new NPC.HitInfo()
                        {
                            Damage = newProjectileDamage,
                            Knockback = 0.2f,
                            HitDirection = Projectile.direction
                        };
                        target.StrikeNPC(hitInfo);
                        SoundStyle item41 = SoundID.Item41;
                        item41.Pitch = 3f;
                        SoundEngine.PlaySound(item41, Projectile.Center);
                    }
                }
            }
            if (Main.mouseRight && player.whoAmI == Projectile.owner && !attackFrames)
            {
                if (target == null)
                {
                    shotgunChargeTimer = 0;
                    target = FindNearestTarget(TargetDetectionRange);
                }
                else
                {
                    secondaryAbilityFrames = true;
                    Projectile.Center = target.Center + new Vector2(((target.width / 2f) + Projectile.width) * -target.direction, 0f);
                    Projectile.direction = target.direction;
                    if (Projectile.Distance(target.Center) >= TargetDetectionRange)
                    {
                        target = null;
                    }

                    shotgunChargeTimer++;
                    if (shotgunChargeTimer >= 90)
                    {
                        shootCount += newShootTime;
                        Vector2 shootVel = target.Center - Projectile.Center;
                        shootVel.Normalize();
                        shootVel *= ProjectileSpeed;

                        float numberProjectiles = 6;
                        float rotation = MathHelper.ToRadians(30f);
                        float random = Main.rand.NextFloat(-6f, 6f + 1f);
                        for (int i = 0; i < numberProjectiles; i++)
                        {
                            Vector2 perturbedSpeed = new Vector2(shootVel.X + random, shootVel.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
                            int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, ProjectileID.Bullet, newProjectileDamage * 2, 1f, player.whoAmI);
                            Main.projectile[proj].netUpdate = true;
                        }
                        shotgunChargeTimer = 0;
                        SoundEngine.PlaySound(SoundID.Item36, Projectile.position);
                    }
                }
            }
            if (!Main.mouseLeft && !Main.mouseRight)
            {
                idleFrames = true;
                attackFrames = false;
                secondaryAbilityFrames = false;
                target = null;
            }

            if (!attackFrames && !secondaryAbilityFrames)
            {
                StayBehind();
            }
            Projectile.spriteDirection = Projectile.direction;
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
                PlayAnimation("Secondary");
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
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Banks/BanksStand_" + animationName).Value;
            if (animationName == "Idle")
            {
                AnimateStand(animationName, 4, 15, true);
            }
            if (animationName == "Attack")
            {
                AnimateStand(animationName, 5, ShootTime / 5, true);
            }
            if (animationName == "Secondary")
            {
                AnimateStand(animationName, 1, 60, true);
            }
            if (animationName == "Pose")
            {
                AnimateStand(animationName, 1, 300, true);
            }
        }
    }
}