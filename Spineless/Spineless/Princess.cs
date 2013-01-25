using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Gnomic.Anim;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Gnomic;
using Gnomic.Entities;

namespace Spineless
{
    class Princess : ClipEntity
    {
        const float MAX_DRAG_DISTANCE = 150;
        const float MIN_DRAG_DISTANCE = 20; // distance at which to register was indeed a "drag"
        const int DRAG_RADIUS = 100;

        public Vector2 Pos;

        internal Texture2D AimTexture;

        Vector2 dragStart, dragEnd, lastDragEnd;
        float dragDistance, angle;
        bool isDragging;
        //List<Weapon> weapons;
        //Weapon currentWeapon;
        
        public Princess()
        { 
            //this.weapons = new List<Weapon>();
            //this.currentWeapon = new Bow() { 
            //    Angle = MathHelper.Pi,
            //    Pos = this.Origin,

            //};
            //this.weapons.Add(this.currentWeapon);
        }

        private void Fire()
        {
 
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
                dragDistance = MathHelper.Clamp(Vector2.Distance(dragStart, dragEnd), MIN_DRAG_DISTANCE, MAX_DRAG_DISTANCE);

                // TODO update drag distance sound
                // TODO update drag distance animation

                angle = (float)Math.Atan2(dragEnd.Y - dragStart.Y, dragEnd.X - dragStart.X);
                
                //currentWeapon.Angle = angle;
            }
            else if (Input.MouseJustUp(MouseButton.Left) && isDragging)
            {
                isDragging = false;

                if (dragDistance < MIN_DRAG_DISTANCE)
                {
                }
                else
                {
                }
            }
        }

        public override void Draw2D(SpriteBatch spriteBatch)
        {
            base.Draw2D(spriteBatch);

            if(isDragging)
                spriteBatch.Draw(this.AimTexture, dragStart, null, Color.Red, angle, Vector2.Zero, new Vector2(dragDistance, 2), SpriteEffects.None, 0);

            //currentWeapon.Draw(spriteBatch);
        }

    }
}
