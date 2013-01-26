using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gnomic;
using Gnomic.UI;
using Gnomic.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spineless
{
    public class HudScreen : GameScreen
    {
        LevelScreen levelScreen;
        public Image IconMove;

        public override void Initialize(GnomicGame game)
        {
            levelScreen = ParentGame.GetScreen<LevelScreen>();
            
            Vector2 imageSize = new Vector2(64.0f / 720.0f);
            Vector2 topRight = new Vector2(ParentGame.ScreenWidth, -ParentGame.ScreenHeight) * (0.5f / ParentGame.ScreenHeight) - new Vector2(imageSize.X, -imageSize.Y);

            IconMove = base.AddSprite("IconMove", imageSize, topRight, new Vector2(0.5f), true);

            base.Initialize(game);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }
    }
}
