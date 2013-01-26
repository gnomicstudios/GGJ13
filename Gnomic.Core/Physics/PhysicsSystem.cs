using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Factories;

using Gnomic;

namespace Gnomic.Physics
{
    public class PhysicsSystem
    {
        GameScreen screen;
        
        World world;
        public World World
        {
            get { return world; }
        }

        DebugViewXNA debugView;
        public DebugViewXNA DebugView
        {
            get { return debugView; }
        }

        Matrix projection;
        public void RenderDebugView()
        {
            DebugView.RenderDebugData(ref projection);
        }

        public void CreateBorder(
            float width, float height, Vector2 offset, float friction)
        {
            var border = new Border(world, width, height, offset, friction);
        }

        public PhysicsSystem(GameScreen screen)
        {
            this.screen = screen;

            GnomicGame game = screen.ParentGame;

            // Todo: pass in a PhysicsSystemSetting with gravity, worldMin and worldMax
            world = new World(
                new Vector2(0.0f, 20.0f),
                new FarseerPhysics.Collision.AABB(Vector2.One * -500.0f, Vector2.One * 500.0f));

            debugView = new DebugViewXNA(world);
            debugView.AppendFlags(DebugViewFlags.Shape);
            debugView.DefaultShapeColor = Color.White;
            debugView.SleepingShapeColor = Color.LightGray;
            debugView.LoadContent(game.GraphicsDevice, game.Content, "GameFont");

            projection = Matrix.CreateOrthographicOffCenter(
                0f,
                ConvertUnits.ToSimUnits(game.ScreenWidth),
                ConvertUnits.ToSimUnits(game.ScreenHeight),
                0f, 0f, 1f);
        }
    }
}
