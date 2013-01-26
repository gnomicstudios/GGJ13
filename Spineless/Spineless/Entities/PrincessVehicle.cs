using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gnomic;
using Gnomic.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Spineless.Entities
{
    public class PrincessVehicle : SpinelessEntity
    {
        Vector2 moveForce;
        Vector2 cameraOffset;
        Cue cueMove;
        float cueVolume = 0.1f;

        const float MOVE_FORCE = 15.0f;
        const float STOP_FORCE = 40.0f;
        const float CUE_MIN_VOL = 0.1f;

        public override void Initialize(GameScreen parentScreen)
        {
            base.Initialize(parentScreen);

            cameraOffset = parentScreen.Camera2D.Position - this.Position;
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            var iconMoveRight = ((LevelScreen)ParentScreen).Hud.IconMoveRight;
            var iconMoveLeft = ((LevelScreen)ParentScreen).Hud.IconMoveLeft;
            if ((Input.MouseJustDown(MouseButton.Left) && iconMoveRight.DestRect.Contains(Input.MouseX, Input.MouseY)) ||
                 Input.KeyJustDown(Keys.Right))
            {
                moveForce = new Vector2(MOVE_FORCE, 0.0f);
                ClipInstance.Play("moving", true, true, 1.0f);
                if (Cue.IsValidForPlay(cueMove))
                {
                    cueMove = ParentScreen.ParentGame.Audio.Play("seige");
                    cueMove.Sound.Volume = CUE_MIN_VOL;
                }
            }
            else if ((Input.MouseJustDown(MouseButton.Left) && iconMoveLeft.DestRect.Contains(Input.MouseX, Input.MouseY)) ||
                 Input.KeyJustDown(Keys.Left))
            {
                moveForce = new Vector2(-MOVE_FORCE, 0.0f);
                ClipInstance.Play("moving", true, false, 1.0f);
                if (Cue.IsValidForPlay(cueMove))
                {
                    cueMove = ParentScreen.ParentGame.Audio.Play("seige");
                    cueMove.Sound.Volume = CUE_MIN_VOL;
                }
            }
            
            if (moveForce != Vector2.Zero)
            {
                if (!Input.MouseDown(MouseButton.Left) && !Input.KeyDown(Keys.Right) && !Input.KeyDown(Keys.Left))
                {
                    moveForce = Vector2.Zero;
                    ClipInstance.Stop();
                }
                
                if (cueMove != null && !cueMove.IsDisposed && cueMove.IsPlaying)
                {
                    if (cueVolume < cueMove.Settings.Volume)
                    {
                        cueVolume += Math.Max(dt, (cueMove.Settings.Volume - cueVolume) * 4f * dt);
                        cueVolume = Math.Min(1.0f, cueVolume);
                        cueMove.Sound.Volume = cueVolume;
                    }          
                }
            }
            else
            {
                if (cueMove != null && !cueMove.IsDisposed && cueMove.IsPlaying)
                {
                    if (cueVolume > CUE_MIN_VOL)
                    {
                        cueVolume -= Math.Max(dt, (cueVolume - CUE_MIN_VOL) * 4f * dt);
                        cueVolume = Math.Max(0.0f, cueVolume);
                        cueMove.Sound.Volume = cueVolume;
                    }
                    else
                    {
                        cueMove.Stop(AudioStopOptions.Immediate);
                    }
                }
            }

            if (moveForce != Vector2.Zero && moveForce.X * physics.Bodies[0].LinearVelocity.X >= 0)
            {
                physics.Bodies[0].ApplyForce(ref moveForce);
            }
            else 
            {
                if (physics.Bodies[0].LinearVelocity.Length() > 0.001f)
                {
                    Vector2 stopForce = -STOP_FORCE * Vector2.Normalize(physics.Bodies[0].LinearVelocity);
                    physics.Bodies[0].ApplyForce(ref stopForce);
                }
                else
                {
                    physics.Bodies[0].LinearVelocity = Vector2.Zero;
                }
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
