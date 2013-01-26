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
using Gnomic;
using Gnomic.Graphics;
using Gnomic.Entities;

using Spineless.Entities;

namespace Spineless
{
    public class LevelScreen : Gnomic.GameScreen
    {
        Camera2D camera;
        
        public LevelScreen()
        {
        }
        
        public override void Initialize(GnomicGame game)
        {
            Physics = new Gnomic.Physics.PhysicsSystem(this);

            // Create a 3D camera
            base.Camera2D = camera = new Camera2D(ParentGame.GraphicsDevice.Viewport);

            CreateBackground();

            SpinelessEntitySettings settings = new SpinelessEntitySettings();
            settings.ClipFile = "player_player.clipxml";
            settings.Position = new Vector2(ParentGame.ScreenWidth / 2,
                                            ParentGame.ScreenHeight / 2);
            settings.DefaultAnimName = "run-right";
            base.AddEntity(settings.CreateEntity());

            
            ClipEntitySettings princessClipSettings = new ClipEntitySettings();
            princessClipSettings.ClipFile           = "player_player.clipxml";
            princessClipSettings.Position           = new Vector2(120, 156);
            princessClipSettings.DefaultAnimName    = "idle-right";
            princessClipSettings.EntityClass        = "Spineless.Princess, Spineless";
            Princess lilMissBadAss                  = (Princess)princessClipSettings.CreateEntity();
            base.AddEntity(lilMissBadAss); // sets ParentScreen
            lilMissBadAss.AimTexture                = new Texture2D(lilMissBadAss.ParentScreen.ParentGame.GraphicsDevice, 1, 1);
            lilMissBadAss.AimTexture.SetData<Color>(new Color[] { Color.White });


            base.Initialize(game);
        }

        private void CreateBackground()
        {
            base.Layers.Add(new Layer2D(new Vector2(1.0f, 1.0f)));
            base.Layers.Add(new Layer2D(new Vector2(0.7f, 1.0f)));
            base.Layers.Add(new Layer2D(new Vector2(0.3f, 1.0f)));
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
            // TODO: remove hacky scroll
            //Camera2D.Position = new Vector2(100.0f * (float)TotalTime, 0.0f);

            base.Update(dt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
