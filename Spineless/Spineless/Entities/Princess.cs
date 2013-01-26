using System;
using Gnomic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spineless.Entities
{
    class Princess : SpinelessEntity
    {
        const float MAX_DRAG_DISTANCE = 100;
        const float MIN_DRAG_DISTANCE = 20; // distance at which to register was indeed a "drag"
        const int DRAG_RADIUS = 100;
        const float POWER = 0.05f;
        
        internal Texture2D AimTexture;

        Vector2 dragStart, dragEnd, dragVector, fireOffset;
        float dragDistance, angle;
        bool isDragging;
        
        public Princess()
        { 
        }

        private void Fire()
        {
            dragVector = dragStart - dragEnd;
            dragVector *= POWER;
            this.LevelScreen.FireProjectile(this.Position + fireOffset, dragVector);
        }

        public override void Update(float dt)
        {
            if (Input.MouseDown(MouseButton.Left))
            {
                if (!isDragging)
                {
                    if (Vector2.Distance(this.Origin, new Vector2(Input.MouseX, Input.MouseY)) < DRAG_RADIUS)
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

            base.Update(dt);
        }

        public override void Draw2D(SpriteBatch spriteBatch)
        {
            base.Draw2D(spriteBatch);

            if(isDragging)
                spriteBatch.Draw(this.AimTexture, dragStart, null, Color.Red, angle, Vector2.Zero, new Vector2(dragDistance, 2), SpriteEffects.None, 0);
        }

    }
}
