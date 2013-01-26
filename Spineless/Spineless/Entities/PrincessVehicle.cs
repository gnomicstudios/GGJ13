using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gnomic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Spineless.Entities
{
    public class PrincessVehicle : SpinelessEntity
    {
        bool isMoving;
        Vector2 cameraOffset;
        public override void Initialize(GameScreen parentScreen)
        {
            base.Initialize(parentScreen);

            cameraOffset = parentScreen.Camera2D.Position - this.Position;
        }

        public override void Update(float dt)
        {       
            base.Update(dt);
            var iconMove = ((LevelScreen)ParentScreen).Hud.IconMove;
            if ((Input.MouseJustDown(MouseButton.Left) && iconMove.DestRect.Contains(Input.MouseX, Input.MouseY)) ||
                 Input.KeyJustDown(Keys.Right))
            {
                isMoving = true;
                ClipInstance.Play("moving");
            }
            else if (isMoving && !(Input.MouseDown(MouseButton.Left) || Input.KeyDown(Keys.Right)))
            {
                isMoving = false;
                ClipInstance.Stop();
            }
            
            if (isMoving)
            {
                physics.Bodies[0].ApplyForce(new Vector2(50f, 0.0f));
            }

            // Follow!
            ParentScreen.Camera2D.Position = Position + cameraOffset;

        }


        internal static PrincessVehicle CreateDefaultEntity(
            Vector2 position,
            Vector2 sizePhysicsCoords,
            Vector2 offsetPhysicsCoords)
        {
            SpinelessEntitySettings settings = new SpinelessEntitySettings();
            settings.EntityClass = "Spineless.Entities.PrincessVehicle,Spineless";
            settings.ClipFile = "siegeTower";
            settings.DefaultAnimName = "rig";
            settings.Position = position;
            settings.Physics = new SpinelessPhysicsSettings();
            settings.Physics.Width = sizePhysicsCoords.X;
            settings.Physics.Height = sizePhysicsCoords.Y;
            settings.Physics.Offset = offsetPhysicsCoords;
            return (PrincessVehicle)settings.CreateEntity();
        }
    }
}
