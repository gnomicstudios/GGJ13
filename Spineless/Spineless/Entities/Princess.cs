using System;
using Gnomic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spineless.Entities
{
    public class Princess : SpinelessEntity
    {
        const float MAX_DRAG_DISTANCE   = 100;
        const float MIN_DRAG_DISTANCE   = 20;       // distance at which to register was indeed a "drag"
        const int DRAG_RADIUS           = 100;
        const float POWER               = 0.05f;
        const float MAX_RATE_OF_FIRE    = 1.0f;
        const float FEAR_STRENGTH       = 2.0f;
        const float FEAR_RATE_OF_CHANGE = 0.1f;

        internal Texture2D AimTexture;

        Vector2 dragStart, dragEnd, dragVector, fireOffset;
        float dragDistance, angle, timeSinceLastFired;
        bool isDragging;
        
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
            this.LevelScreen.FireProjectile(this.Position + fireOffset, dragVector);
            timeSinceLastFired = 0;
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

                angle = (float)Math.Atan2(dragEnd.Y - dragStart.Y, dragEnd.X - dragStart.X);
                
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

            base.Update(dt);
        }

        public override void Draw2D(SpriteBatch spriteBatch)
        {
            base.Draw2D(spriteBatch);

            if(isDragging)
                spriteBatch.Draw(this.AimTexture, dragStart + LevelScreen.Camera2D.Position, null, Color.Red, angle, Vector2.Zero, new Vector2(dragDistance, 2), SpriteEffects.None, 0);
        }

    }
}
