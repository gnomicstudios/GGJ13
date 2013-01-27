using System.Collections.Generic;
using FarseerPhysics.Dynamics.Joints;
using Gnomic;
using Gnomic.Audio;
using Gnomic.Entities;
using Gnomic.Graphics;
using Gnomic.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spineless.Entities;

namespace Spineless
{
    public class LevelScreen : Gnomic.GameScreen
    {
        Camera2D camera;
        UnitManager units;
        public UnitManager Units { get { return units; } }
        ProjectileManager projectiles;
        public HudScreen Hud;
        public PrincessVehicle Vehicle;
        public Princess lilMissBadAss;
        RevoluteJoint standingJoint;
        public AudioManager Audio;

        const int PLAY_AREA_WIDTH_IN_SCREENS = 20;
        const int PLAY_AREA_HEIGHT_IN_SCREENS = 30;

        public LevelScreen()
        {
        }
        
        public override void Initialize(GnomicGame game) 
        {
            Hud = ParentGame.GetScreen<HudScreen>();

            int floorHeight = 60;
            Physics = new Gnomic.Physics.PhysicsSystem(this);
            Physics.CreateBorder(ParentGame.ScreenWidth * PLAY_AREA_WIDTH_IN_SCREENS,
                                 ParentGame.ScreenHeight, // * PLAY_AREA_HEIGHT_IN_SCREENS,
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
                        
            this.Audio = this.ParentGame.Audio;

            base.Initialize(game);

            standingJoint = new RevoluteJoint(lilMissBadAss.Physics.Bodies[0], Vehicle.Physics.Bodies[0], Vector2.Zero, new Vector2(0.0f, -vehicleSizePhysicsCoords.Y));
            Physics.World.AddJoint(standingJoint);
        }

        private void AddUnit(UnitType et, Vector2 offsets)
        {
            units.AddUnitToScene(et, Camera2D.Position + new Vector2(ParentGame.ScreenWidth * offsets.X, ParentGame.ScreenHeight * offsets.Y));
        }

        private void CreateBackground()
        {
            base.Layers.Add(new Layer2D(new Vector2(1.0f, 1.0f)));
            base.Layers.Add(new Layer2D(new Vector2(0.05f, 1.0f)));
            base.Layers.Add(new Layer2D(new Vector2(0.2f, 0.2f)));
            base.Layers.Add(new Layer2D(new Vector2(0.0f, 0.1f)));

            for (int i = 0; i < PLAY_AREA_WIDTH_IN_SCREENS; ++i)
            {
                SpriteEntity background = new SpriteEntity();
                background.SpriteState =
                    new Gnomic.Anim.SpriteState(Content.Load<Texture2D>("Background_Ground"));
                background.SpriteState.Transform.Origin = Vector2.Zero;
                background.LayerID = 0;
                background.Position = new Vector2(ParentGame.ScreenWidth * (i - 1), 0.0f);
                base.AddEntity(background);

                background = new SpriteEntity();
                background.SpriteState =
                    new Gnomic.Anim.SpriteState(Content.Load<Texture2D>("Background_Clouds"));
                background.SpriteState.Transform.Origin = Vector2.Zero;
                background.LayerID = 1;
                background.Position = new Vector2(ParentGame.ScreenWidth * (i - 1), 0.0f);
                base.AddEntity(background);

                background = new SpriteEntity();
                background.SpriteState =
                    new Gnomic.Anim.SpriteState(Content.Load<Texture2D>("Background_Mountains"));
                background.SpriteState.Transform.Origin = Vector2.Zero;
                background.LayerID = 2;
                background.Position = new Vector2(ParentGame.ScreenWidth * (i - 1), 0.0f);
                base.AddEntity(background);
            }

            SpriteEntity sky = new SpriteEntity();
            sky.SpriteState =
                new Gnomic.Anim.SpriteState(Content.Load<Texture2D>("Background_Sky"));
            sky.SpriteState.Transform.Origin = Vector2.Zero;
            sky.LayerID = 3;
            sky.Position = new Vector2(0.0f, 0.0f);
            base.AddEntity(sky);
        }

        public void FireProjectile(Vector2 startPos, Vector2 impulse, float angle, ProjectileType type)
        {
            projectiles.Launch(startPos, impulse, angle, type);
        }

        public void Splash(Vector2 pos, float radius, float maxDamage)
        {
            foreach (List<Unit> us in units.UnitLists.Values)
            {
                foreach (Unit u in us)
                {
                    if (u.Health > 0)
                    {
                        float distance = Vector2.Distance(pos, u.Position);

                        if (distance <= radius)
                        {
                            // push
                            Vector2 blastVector = (u.Position - pos) * 0.5f;

                            if(blastVector.Y > 0)
                                blastVector.Y *= -1; // make things always fly

                            u.Physics.Bodies[0].ApplyLinearImpulse(blastVector);

                            // remove health
                            u.Health -= (1.0f - distance / radius) * maxDamage; 
                        }
                    }
                }
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
