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

            SpriteEntity background = new SpriteEntity();
            background.SpriteState = 
                new Gnomic.Anim.SpriteState(Content.Load<Texture2D>("Background"));
            background.SpriteState.Transform.Origin = Vector2.Zero;

            base.AddEntity(background);

            base.Initialize(game);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw 2D layer with SpriteBatch
            if (base.drawable2DEntities.Count > 0)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Camera2D.GetViewMatrix(Vector2.Zero));
                foreach (IDrawable2D e in drawable2DEntities)
                {
                    if (e.IsVisible) e.Draw2D(spriteBatch);
                }
                spriteBatch.End();
            }
        }
    }
}
