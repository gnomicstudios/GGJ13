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
        const float THROW_RELOAD_TIME   = 1.0f;
        const float FIRE_RELOAD_TIME    = 1.0f;
        const float FEAR_STRENGTH       = 2.0f;
        const float FEAR_RATE_OF_CHANGE = 0.1f;

        internal Texture2D AimTexture;

        Vector2 dragStart, dragEnd, dragVector, fireOffset;
        float dragDistance, angle, timeSinceLastFired;
        bool isDragging;
        ProjectileType currentProjectileType = ProjectileType.DirectHit;
        int lastScrollPos, updatesSinceWeaponChange, throwBombTime, shootArrowTime;
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
        }

        private void Fire()
        {
            dragVector = dragStart - dragEnd;
            dragVector *= POWER;
            
            this.LevelScreen.FireProjectile(this.Position + fireOffset, dragVector, angle, currentProjectileType);
            timeSinceLastFired = 0;

            if (currentProjectileType == ProjectileType.DirectHit)
            {
                state = PrincessState.FiringBow;
            }
            else
            {
                state = PrincessState.ThrowingBomb;
            }
        }

        public override void Update(float dt)
        {
            timeSinceLastFired += dt;

            if (Input.MouseDown(MouseButton.Left))
            {
                if (!isDragging && timeSinceLastFired > MAX_RATE_OF_FIRE)
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
                if (u.UnitType != UnitType.Knight)
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
                currentProjectileType = currentProjectileType == ProjectileType.DirectHit ? ProjectileType.Splash : ProjectileType.DirectHit;
                lastScrollPos = Input.MouseDW;
                updatesSinceWeaponChange = 0;
            }
            else
            {
                updatesSinceWeaponChange++;
                lastScrollPos = Input.MouseDW;
            }

            // update state
            if (state == PrincessState.ThrowingBomb)
            {
                if (timeSinceLastFired > THROW_RELOAD_TIME)
                {
                    state = PrincessState.IdleBomb;
                }
            }
            else if (state == PrincessState.FiringBow)
            {
                if (timeSinceLastFired > THROW_RELOAD_TIME)
                {
                    state = PrincessState.IdleBow;
                }
            }

            base.Update(dt);
        }

        public override void Draw2D(SpriteBatch spriteBatch)
        {
            // set current clip
            switch (currentProjectileType)
            {
                case ProjectileType.DirectHit:
                    {
                        switch(state)
                        {
                            case PrincessState.FiringBow:
                            {
                                if (this.ClipInstance.CurrentAnimationName != "firing-bow") // TODO name?
                                {
                                    this.ClipInstance.Play("firing-bow"); 
                                }
                            }
                            break;
                        }
                    }
                    break;
                case ProjectileType.Splash:
                    {
                        if(isDragging)
                            this.ClipInstance.Play("readyThrow");
                        else
                            this.ClipInstance.Play("idleBall");

                        case PrincessState.ThrowingBomb:
                            {
                                if (this.ClipInstance.CurrentAnimationName != "throwing-bomb") // TODO name?
                                {
                                    this.ClipInstance.Play("throwing-bomb");
                                }
                            }
                            break;
                    }
                    break;
            }

            base.Draw2D(spriteBatch);

            if(isDragging)
                spriteBatch.Draw(this.AimTexture, dragStart + LevelScreen.Camera2D.Position, null, Color.Red, angle, Vector2.Zero, new Vector2(dragDistance, 5), SpriteEffects.None, 0);
        }

    }
}
