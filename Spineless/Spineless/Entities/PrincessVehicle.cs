using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gnomic;
using Microsoft.Xna.Framework;

namespace Spineless.Entities
{
    public class PrincessVehicle : SpinelessEntity
    {
        bool isMoving;

        public override void Initialize(GameScreen parentScreen)
        {
            base.Initialize(parentScreen);
        }

        public override void Update(float dt)
        {       
            base.Update(dt);
            if (Input.MouseJustDown(MouseButton.Left))
            {
                var iconMove = ((LevelScreen)ParentScreen).Hud.IconMove;
                if (iconMove.DestRect.Contains(Input.MouseX, Input.MouseY))
                {
                    isMoving = true;
                    ClipInstance.Play("moving");
                }
            }
            else if (isMoving && !Input.MouseDown(MouseButton.Left))
            {
                isMoving = false;
                ClipInstance.Stop();
            }
            
            if (isMoving)
            {
                physics.Bodies[0].ApplyForce(new Vector2(50f, 0.0f));
            }
        }

    }
}
