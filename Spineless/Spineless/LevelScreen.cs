using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics.Dynamics.Joints;
using Gnomic;
using Gnomic.Graphics;
using Gnomic.Entities;
using Gnomic.Physics;

using Spineless.Entities;

namespace Spineless
{
    public class LevelScreen : Gnomic.GameScreen
    {
        Camera2D camera;
        UnitManager units;
        ProjectileManager projectiles;
        public HudScreen Hud;
        public PrincessVehicle Vehicle;
        public Princess lilMissBadAss;
        RevoluteJoint standingJoint;


        public LevelScreen()
        {
        }
        
        public override void Initialize(GnomicGame game)
        {
            Hud = ParentGame.GetScreen<HudScreen>();

            int floorHeight = 60;
            Physics = new Gnomic.Physics.PhysicsSystem(this);
            Physics.CreateBorder(ParentGame.ScreenWidth,
                                 ParentGame.ScreenHeight,
                                 new Vector2(0.0f, -floorHeight),
                                 /*friction*/ 0.0f);

            // Create a 2D camera
            base.Camera2D = camera = new Camera2D(ParentGame.GraphicsDevice.Viewport);

            CreateBackground();

            Vector2 vehiclePos = new Vector2(ParentGame.ScreenWidth / 5, ParentGame.ScreenHeight - floorHeight);
            Vector2 vehicleSizePhysicsCoords = new Vector2(2.7f, 4.5f);
            Vector2 vehicleOffsetPhysicsCoords = new Vector2(0.0f, -vehicleSizePhysicsCoords.Y / 2.0f);
   
            Vector2 princessPos = vehiclePos - ConvertUnits.ToDisplayUnits(new Vector2(0.0f, vehicleSizePhysicsCoords.Y));
            lilMissBadAss = Princess.CreatePrincess(princessPos, new Vector2(0.6f, 1.2f), new Vector2(0.0f, -0.6f));
            base.AddEntity(lilMissBadAss); // sets ParentScreen

            Vehicle = PrincessVehicle.CreateDefaultEntity(vehiclePos, vehicleSizePhysicsCoords, vehicleOffsetPhysicsCoords);
            base.AddEntity(Vehicle);

            
            units = new UnitManager(this);

            float startX = 0.7f;

            for (int i = 0; i < 20; ++i)
            {
                AddUnit(UnitType.Grunt, new Vector2(startX+(i*0.05f), 0.7f));
            }

            AddUnit(UnitType.Knight, new Vector2(0.5f, 0.7f));

            projectiles = new ProjectileManager(this);

            base.Initialize(game);

            standingJoint = new RevoluteJoint(lilMissBadAss.Physics.Bodies[0], Vehicle.Physics.Bodies[0], Vector2.Zero, new Vector2(0.0f, -vehicleSizePhysicsCoords.Y));
            Physics.World.AddJoint(standingJoint);
        }

        void AddUnit(UnitType et, Vector2 offsets)
        {
            units.AddUnitToScene(et, Camera2D.Position + new Vector2(ParentGame.ScreenWidth * offsets.X, ParentGame.ScreenHeight * offsets.Y));
        }

        public void FireProjectile(Vector2 startPos, Vector2 impulse)
        {
            projectiles.Launch(startPos, impulse);
        }

        private void CreateBackground()
        {
            base.Layers.Add(new Layer2D(new Vector2(1.0f, 1.0f)));
            base.Layers.Add(new Layer2D(new Vector2(0.4f, 1.0f)));
            base.Layers.Add(new Layer2D(new Vector2(0.2f, 1.0f)));
            base.Layers.Add(new Layer2D(new Vector2(0.0f, 1.0f)));

            for (int i = 0; i < 20; ++i)
            {
                SpriteEntity background = new SpriteEntity();
                background.SpriteState =
                    new Gnomic.Anim.SpriteState(Content.Load<Texture2D>("Background_Ground"));
                background.SpriteState.Transform.Origin = Vector2.Zero;
                background.LayerID = 0;
                background.Position = new Vector2(ParentGame.ScreenWidth * i, 0.0f);
                base.AddEntity(background);
            
                background = new SpriteEntity();
                background.SpriteState =
                    new Gnomic.Anim.SpriteState(Content.Load<Texture2D>("Background_Clouds"));
                background.SpriteState.Transform.Origin = Vector2.Zero;
                background.LayerID = 1;
                background.Position = new Vector2(ParentGame.ScreenWidth * i, 0.0f);
                base.AddEntity(background);

                background = new SpriteEntity();
                background.SpriteState =
                    new Gnomic.Anim.SpriteState(Content.Load<Texture2D>("Background_Mountains"));
                background.SpriteState.Transform.Origin = Vector2.Zero;
                background.LayerID = 2;
                background.Position = new Vector2(ParentGame.ScreenWidth * i, 0.0f);
                base.AddEntity(background);

                background = new SpriteEntity();
                background.SpriteState =
                    new Gnomic.Anim.SpriteState(Content.Load<Texture2D>("Background_Sky"));
                background.SpriteState.Transform.Origin = Vector2.Zero;
                background.LayerID = 3;
                background.Position = new Vector2(ParentGame.ScreenWidth * i, 0.0f);
                base.AddEntity(background);
            }
        }

        public override void Update(float dt)
        {
            if (Gnomic.Input.KeyJustDown(Microsoft.Xna.Framework.Input.Keys.F1))
            {
                base.EnablePhysicsDebug = !base.EnablePhysicsDebug;
            }
            base.Update(dt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
