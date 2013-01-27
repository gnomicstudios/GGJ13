using System;
using Gnomic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spineless.Entities
{
    enum PrincessState
    {
        IdleBow,
        IdleBomb,
        PullingBackBow,
        AimingBomb,
        FiringBow,
        ThrowingBomb
    }

    public class Princess : SpinelessEntity
    {
        const float MAX_DRAG_DISTANCE   = 100;
        const float MIN_DRAG_DISTANCE   = 20;       // distance at which to register was indeed a "drag"
        const int DRAG_RADIUS           = 100;
        const float POWER               = 0.05f;
        const float FEAR_STRENGTH       = 2.0f;
        const float FEAR_RATE_OF_CHANGE = 0.1f;

        const string BOMB_IDLE_CLIP_NAME    = "idleBall";
        const string BOW_IDLE_CLIP_NAME     = "idleShoot";
        const string BOMB_AIM_CLIP_NAME     = "readyThrow";
        const string BOW_AIM_CLIP_NAME      = "readyShoot";
        const string BOMB_FIRE_CLIP_NAME    = "throw";
        const string BOW_FIRE_CLIP_NAME     = "shoot";

        internal Texture2D AimTexture;

        Vector2 dragStart, dragEnd, dragVector, fireOffset;
        float dragDistance, angle, timeSinceLastFired, bombReloadTime, bowReloadTime;
        bool isDragging;
        ProjectileType currentProjectileType = ProjectileType.Arrow;
        int lastScrollPos, updatesSinceWeaponChange;
        PrincessState state;
        
        public float FearFactor { get; private set; }
        float fearFactorTarget;

        public Princess()
        { 
        }

        public static Princess CreatePrincess(
            Vector2 position,
            Vector2 sizePhysicsCoords,
            Vector2 offsetPhysicsCoords)
        {
            SpinelessEntitySettings princessClipSettings = new SpinelessEntitySettings();
            princessClipSettings.EntityClass = "Spineless.Entities.Princess, Spineless";
            princessClipSettings.ClipFile = "princess.clipxml";
            princessClipSettings.DefaultAnimName = "idle";
            princessClipSettings.Position = position;
            princessClipSettings.Physics = new SpinelessPhysicsSettings();
            princessClipSettings.Physics.Width = sizePhysicsCoords.X;
            princessClipSettings.Physics.Height = sizePhysicsCoords.Y;
            princessClipSettings.Physics.Offset = offsetPhysicsCoords;
            return (Princess)princessClipSettings.CreateEntity();
        }

        public override void Initialize(GameScreen parentScreen)
        {
            base.Initialize(parentScreen);

            AimTexture = new Texture2D(ParentScreen.ParentGame.GraphicsDevice, 1, 1);
            AimTexture.SetData<Color>(new Color[] { Color.White });

            // find fire durations
            ClipInstance.Play(BOMB_FIRE_CLIP_NAME);
            bombReloadTime = ClipInstance.CurrentAnim.DurationInSeconds;
            ClipInstance.Play(BOW_FIRE_CLIP_NAME);
            bowReloadTime = ClipInstance.CurrentAnim.DurationInSeconds;

            // set state and current clip
            state = PrincessState.IdleBow;
            ClipInstance.Play(BOW_IDLE_CLIP_NAME);
        }

        private void Fire()
        {
            dragVector = dragStart - dragEnd;
            dragVector *= POWER;
            
            this.LevelScreen.FireProjectile(this.Position + fireOffset, dragVector, angle, currentProjectileType);
            timeSinceLastFired = 0;

            if (currentProjectileType == ProjectileType.Arrow)
            {
                state = PrincessState.FiringBow;
                ClipInstance.Play(BOW_FIRE_CLIP_NAME);
            }
            else
            {
                state = PrincessState.ThrowingBomb;
                ClipInstance.Play(BOMB_FIRE_CLIP_NAME);
            }
        }

        public override void Update(float dt)
        {
            timeSinceLastFired += dt;

            if (Input.MouseDown(MouseButton.Left))
            {
                if (!isDragging && timeSinceLastFired > (currentProjectileType == ProjectileType.Arrow ? bowReloadTime : bombReloadTime))
                {
                    if (Vector2.Distance(this.Position - LevelScreen.Camera2D.Position, new Vector2(Input.MouseX, Input.MouseY)) < DRAG_RADIUS)
                    {
                        isDragging = true;
                        dragStart = new Vector2(Input.MouseX, Input.MouseY); 
                    }
                }

                dragEnd = new Vector2(Input.MouseX, Input.MouseY);
                dragDistance = MathHelper.Clamp(Vector2.Distance(dragStart, dragEnd), 0, MAX_DRAG_DISTANCE);

                // TODO update drag distance sound
                // TODO update drag distance animation

                angle = (float)Math.Atan2(dragStart.Y - dragEnd.Y, dragStart.X - dragEnd.X);
                
            }
            else if (Input.MouseJustUp(MouseButton.Left) && isDragging)
            {
                isDragging = false;

                if (dragDistance < MIN_DRAG_DISTANCE)
                {
                    // do nothing...
                }
                else
                {
                    Fire();
                }
            }

            float fear = 0.0f;
            foreach (Unit u in LevelScreen.Units.ActiveUnits)
            {
                if (u.UnitType != UnitType.Knight && u.Health > 0.0f)
                {
                    float distToPrincess = Math.Abs(u.Physics.Bodies[0].Position.X - Physics.Bodies[0].Position.X);
                    fear += FEAR_STRENGTH / (distToPrincess * distToPrincess);
                }
            }
            fearFactorTarget = fear;
            FearFactor += (fearFactorTarget - FearFactor) * dt * FEAR_RATE_OF_CHANGE;
            FearFactor = Math.Min(1.0f, FearFactor);

            if (Input.MouseDW != lastScrollPos && updatesSinceWeaponChange > 30)
            {
                // change current project type, state and clip
                if (currentProjectileType == ProjectileType.Bomb)
                {
                    currentProjectileType = ProjectileType.Arrow;
                    state = PrincessState.IdleBow;
                    ClipInstance.Play(BOW_IDLE_CLIP_NAME);
                }
                else
                {
                    currentProjectileType = ProjectileType.Bomb;
                    state = PrincessState.IdleBomb;
                    ClipInstance.Play(BOMB_IDLE_CLIP_NAME);
                }

                lastScrollPos = Input.MouseDW;
                updatesSinceWeaponChange = 0;
            }
            else
            {
                updatesSinceWeaponChange++;
                lastScrollPos = Input.MouseDW;
            }

            // Update state and the current clip, if need be. NOTE: The only other state/clip transitions are when changing gun and in Fire().
            switch (state)
            {
                case PrincessState.IdleBomb:
                    {
                        if (isDragging)
                        {
                            state = PrincessState.AimingBomb;
                            ClipInstance.Play(BOMB_AIM_CLIP_NAME);
                        }
                    }
                    break;
                case PrincessState.IdleBow:
                    {
                        if (isDragging)
                        {
                            state = PrincessState.PullingBackBow;
                            ClipInstance.Play(BOW_AIM_CLIP_NAME);
                        }
                    }
                    break;
                case PrincessState.AimingBomb:
                    {
                        if (!isDragging)
                        {
                            state = PrincessState.IdleBomb;
                            ClipInstance.Play(BOMB_IDLE_CLIP_NAME);
                        } 
                    }
                    break;
                case PrincessState.PullingBackBow:
                    {
                        if (!isDragging)
                        {
                            state = PrincessState.IdleBow;
                            ClipInstance.Play(BOW_IDLE_CLIP_NAME);
                        }
                    }
                    break;
                case PrincessState.ThrowingBomb:
                    {
                        if (timeSinceLastFired > bombReloadTime)
                        {
                            state = PrincessState.IdleBomb;
                            ClipInstance.Play(BOMB_IDLE_CLIP_NAME);
                        }
                    }
                    break;
                case PrincessState.FiringBow:
                    {
                        if (timeSinceLastFired > bowReloadTime)
                        {
                            state = PrincessState.IdleBow;
                            ClipInstance.Play(BOW_IDLE_CLIP_NAME);
                        }
                    }
                    break;
            }

            base.Update(dt);
        }

        public override void Draw2D(SpriteBatch spriteBatch)
        {
            base.Draw2D(spriteBatch);

            if(isDragging)
                spriteBatch.Draw(this.AimTexture, dragStart + LevelScreen.Camera2D.Position, null, Color.Red, angle, Vector2.Zero, new Vector2(dragDistance, 5), SpriteEffects.None, 0);
        }

    }
}
