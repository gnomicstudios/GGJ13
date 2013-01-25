using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics;
using FarseerPhysics.Dynamics;

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

        public PhysicsSystem(GameScreen screen)
        {
            this.screen = screen;

            // Todo: pass in a PhysicsSystemSetting with gravity, worldMin and worldMax
            world = new World(
                new Vector2(0.0f, -10.0f), 
                new FarseerPhysics.Collision.AABB(Vector2.One * -500.0f, Vector2.One * 500.0f));
        }


    }
}
