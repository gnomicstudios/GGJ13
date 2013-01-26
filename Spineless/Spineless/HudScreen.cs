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
        public Image IconMoveLeft;
        public Image IconMoveRight;

        public ClipEntity Heart;
        public ClipEntity HeartBG;
        string[] heartAnims = new string[] { "beatA", "beatB", "beatC" };
        public int heartAnimId = 0;

        public override void Initialize(GnomicGame game)
        {
            levelScreen = ParentGame.GetScreen<LevelScreen>();
            
            Vector2 imageSize = new Vector2(64.0f / 720.0f);
            Vector2 topRight = new Vector2(ParentGame.ScreenWidth, -ParentGame.ScreenHeight) * (0.5f / ParentGame.ScreenHeight) - new Vector2(imageSize.X, -imageSize.Y);

            IconMoveRight = base.AddSprite("IconMove", imageSize, topRight, new Vector2(0.5f), true);
            IconMoveLeft = base.AddSprite("IconMove", imageSize, topRight - new Vector2(imageSize.X, 0.0f), new Vector2(0.5f), true);
            IconMoveLeft.Settings.SpriteEffects = SpriteEffects.FlipHorizontally;
            
            ClipEntitySettings settings;

            settings = new ClipEntitySettings();
            settings.ClipFile = "heart";
            settings.Scale = Vector2.One;
            settings.DefaultAnimName = "heartBG";
            settings.Position = new Vector2(90, 90);
            HeartBG = (ClipEntity)settings.CreateEntity();
            base.AddEntity(HeartBG);

            settings = new ClipEntitySettings();
            settings.ClipFile = "heart";
            settings.Scale = Vector2.One * 0.5f;
            settings.DefaultAnimName = "beatA";
            settings.Position = new Vector2(90, 90);
            Heart = (ClipEntity)settings.CreateEntity();
            base.AddEntity(Heart);
            
            base.Initialize(game);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            
            float fear = levelScreen.lilMissBadAss.FearFactor;
            int animId = 0;
            
            if (fear < 0.5f) animId = 0;
            else if (fear < 0.85f) animId = 1;
            else animId = 2;

            if (animId != heartAnimId)
            {
                Heart.ClipInstance.Play(heartAnims[animId]);
                heartAnimId = animId;
            }

            Heart.Scale = Vector2.One * (0.5f + fear * 0.8f);
            Heart.ClipInstance.CurrentAnim.SpeedModifier = 0.5f + 2.0f * fear;
        }
    }
}
