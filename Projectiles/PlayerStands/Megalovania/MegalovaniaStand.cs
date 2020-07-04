using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;

namespace JoJoFanStands.Projectiles.PlayerStands.Megalovania
{
    public class MegalovaniaStand : StandClass
    {
        public override int projectileDamage => 18;
        public override int altDamage => 96;
        public override int halfStandHeight => 28;

        public static int abilityNumber = 0;

        private string abilityName;
        private string direction;
        private float mouseDistance = 0f;
        private int maxFrames = 0;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (shootCount > 0)
            {
                shootCount--;
            }
            if (mPlayer.StandOut)
            {
                projectile.timeLeft = 2;
            }
            drawOriginOffsetY = -halfStandHeight;
            StayBehind();


            if (Main.mouseLeft && abilityNumber == 0 && player.whoAmI == Main.myPlayer)
            {
                if (shootCount <= 0)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        float NPCdist = Vector2.Distance(Main.MouseWorld, Main.npc[i].position);
                        if (NPCdist < 25f)
                        {
                            shootCount += newShootTime;
                            Main.npc[i].StrikeNPC(97, 0f, projectile.direction * -1);
                        }
                    }
                }
            }
            if (JoJoStands.JoJoStands.SpecialHotKey.JustPressed && !UI.AbilityChooserUI.Visible)
            {
                UI.AbilityChooserUI.Visible = true;
            }
            if (player.whoAmI == Main.myPlayer)
            {
                if (Main.mouseX >= projectile.position.X)
                {
                    projectile.direction = 1;
                }
                if (Main.mouseX < projectile.position.X)
                {
                    projectile.direction = -1;
                }
                if (abilityNumber == 0)
                {
                    if (Main.mouseY <= Main.screenHeight / 5)
                    {
                        direction = "Up";
                        //AnimationStates(2, 1.5f, true, false);
                    }
                    if (Main.mouseY > Main.screenHeight / 5 && Main.mouseY < (Main.screenHeight / 5) * 2)
                    {
                        direction = "SlightUp";
                        //AnimationStates(2, 1.5f, true, false);
                    }
                    if (Main.mouseY > (Main.screenHeight / 5) * 2 && Main.mouseY < (Main.screenHeight / 5) * 4)
                    {
                        direction = "Straight";
                        //AnimationStates(2, 1.5f, true, false);
                    }
                    if (Main.mouseY > (Main.screenHeight / 5) * 4)
                    {
                        direction = "Down";
                        //AnimationStates(2, 1.5f, true, false);
                    }
                }
            }
            //Main.NewText(Main.mouseY);


            if (abilityNumber == 1)     //push everything away
            {
                abilityName = "PushBack";
                //AnimationStates(1, 0.5f, false, true);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.position.X >= projectile.position.X)
                    {
                        npc.velocity.X = 25f;
                    }
                    if (npc.position.X < projectile.position.X)
                    {
                        npc.velocity.X = -25f;
                    }
                    if (npc.position.Y >= projectile.position.Y)
                    {
                        npc.velocity.Y = 25f;
                    }
                    if (npc.position.Y < projectile.position.Y)
                    {
                        npc.velocity.Y = -25f;
                    }
                }
            }

            if (abilityNumber == 2)     //forcefield
            {
                abilityName = "ForceField";
                //AnimationStates(1, 0.05f, false, true);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    float npcDist = Vector2.Distance(npc.Center, player.Center);
                    if (npcDist <= 60f)
                    {
                        npc.velocity.X = 10f * -npc.direction;
                        if (npc.position.Y >= projectile.position.Y)
                        {
                            npc.velocity.Y = 10f;
                        }
                        if (npc.position.Y < projectile.position.Y)
                        {
                            npc.velocity.Y = -10f;
                        }
                    }
                }
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile otherProjectile = Main.projectile[i];
                    float projectileDist = Vector2.Distance(otherProjectile.Center, player.Center);
                    if (projectileDist <= 60f && otherProjectile.hostile)
                    {
                        projectile.velocity = -projectile.velocity * 10f;
                    }
                }
            }

            if (abilityNumber == 3)     //crystal shower
            {
                abilityName = "Crystal";
                //AnimationStates(1, 0.16f, false, true);
                if (shootCount <= 0)
                {
                    shootCount += 85;
                    Projectile.NewProjectile(projectile.position.X + 5f, projectile.position.Y, 0f, 0f, mod.ProjectileType("Crystal"), 82, 2f, Main.myPlayer);
                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y - 5f, 0f, 0f, mod.ProjectileType("Crystal"), 82, 2f, Main.myPlayer);
                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y + 5f, 0f, 0f, mod.ProjectileType("Crystal"), 82, 2f, Main.myPlayer);
                    Projectile.NewProjectile(projectile.position.X - 5f, projectile.position.Y, 0f, 0f, mod.ProjectileType("Crystal"), 82, 2f, Main.myPlayer);
                }
            }

            if (abilityNumber == 4)     //make NPCs reverse gravity
            {
                abilityName = "Gravity";
                //AnimationStates(1, 0.5f, false, true);
                if (shootCount <= 0)
                {
                    shootCount += 120;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (!npc.noGravity)
                        {
                            npc.noGravity = true;
                            npc.velocity.Y -= 0.1f;
                        }
                        if (npc.noGravity)
                        {
                            npc.noGravity = false;
                            npc.velocity.Y += 1f;
                        }
                    }
                }
            }

            if (abilityNumber == 5)     //non-existant: Whatever this projectile touches will no longer ever spawn in the game for the time you are in the world
            {
                abilityName = "Genocide";
                //AnimationStates(1, 0.16f, false, true);
                if (shootCount <= 0)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        float NPCdist = Vector2.Distance(Main.MouseWorld, Main.npc[i].position);
                        if (NPCdist < 25f)
                        {
                            shootCount += 60;
                            //Main.npc[i].StrikeNPC(10, 0f, projectile.direction * -1);
                            Main.npc[i].GetGlobalNPC<NPCs.FanGlobalNPC>().nonExistant = true;
                            if (NPCs.FanGlobalNPC.nonExistantTypes.Count != NPCs.FanGlobalNPC.nonExistantTypes.Capacity)
                            {
                                NPCs.FanGlobalNPC.nonExistantTypes.Add(Main.npc[i].type);
                                Main.NewText(Main.npc[i].TypeName + "s don't exist anymore.");
                            }
                            else
                            {
                                Main.NewText("The amount of things that can't exist has already been exceeded...");
                            }
                        }
                    }
                }
            }
        }

        /*public virtual void AnimationStates(int frameAmount, float fps, bool loop, bool ability)        //remember that 'fps' refers to how many frames is supposed to play every second, not how fast it plays
        {
            Main.projFrames[projectile.whoAmI] = frameAmount;
            projectile.frameCounter++;
            standTexture = mod.GetTexture("Projectiles/PlayerStands/Megalovania/M" + direction);
            float framesPerSecond = 60f / fps;
            if (projectile.frameCounter >= framesPerSecond)
            {
                projectile.frameCounter = 0;
                projectile.frame += 1;
            }
            if (projectile.frame >= frameAmount && loop)
            {
                projectile.frame = 0;
            }
            if (projectile.frame >= frameAmount && !loop)
            {
                direction = "Idle";
            }
            if (projectile.frame >= frameAmount && ability)
            {
                direction = "Idle";
                abilityNumber = 0;
            }
        }*/

        public override void SelectAnimation()
        {
            if (abilityNumber == 0)
            {
                PlayAnimation(direction);
                maxFrames = 2;
            }
            else
            {
                PlayAnimation(abilityName);
                maxFrames = 1;
                if (projectile.frame >= maxFrames)
                {
                    abilityNumber = 0;
                }
            }
            /*if (attackFrames)
            {
                normalFrames = false;
                PlayAnimation("Attack");
            }
            if (normalFrames)
            {
                attackFrames = false;
                PlayAnimation("Idle");
            }
            if (secondaryAbilityFrames)
            {
                normalFrames = false;
                attackFrames = false;
                PlayAnimation("Pose");
            }
            if (Main.player[projectile.owner].GetModPlayer<MyPlayer>().poseMode)
            {
                normalFrames = false;
                attackFrames = false;
                PlayAnimation("Pose");
            }*/
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = mod.GetTexture("Projectiles/PlayerStands/Megalovania/M" + animationName);
            if (animationName == "Up")
            {
                AnimationStates(animationName, 2, 30, true);
            }
            if (animationName == "SlightUp")
            {
                AnimationStates(animationName, 2, 30, true);
            }
            if (animationName == "Straight")
            {
                AnimationStates(animationName, 2, 30, true);
            }
            if (animationName == "Down")
            {
                AnimationStates(animationName, 2, 30, false);
            }
            if (animationName == "PushBack")
            {
                AnimationStates(animationName, 1, 120, false);
            }
            if (animationName == "ForceField")
            {
                AnimationStates(animationName, 1, 1200, false);
            }
            if (animationName == "Crystal")
            {
                AnimationStates(animationName, 1, 375, false);
            }
            if (animationName == "Gravity")
            {
                AnimationStates(animationName, 1, 120, false);
            }
            if (animationName == "Genocide")
            {
                AnimationStates(animationName, 1, 375, false);
            }
        }
    }
}