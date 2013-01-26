using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace Gnomic.Physics
{
    public static class SimplePhysicsFactory
    {
        public static PhysicsStructureSettings CreateBox(
            Vector2 offset, // pixel-space offset from origin
            float width,    // width in pixel-space
            float height,   // height in pixel-space
            bool isSensor = false,
            int collidesWith = (int)Category.All,
            short collisionGroup = 0)
        {
            // float hw = 0.5f * ConvertUnits.ToSimUnits(width);
            // float hh = 0.5f * ConvertUnits.ToSimUnits(height);

            float hw = 0.5f * width;
            float hh = 0.5f * height;

            Vector2[] rect =
                {
                    new Vector2 (-hw, -hh), // top-left
                    new Vector2 ( hw, -hh), // top-right
                    new Vector2 ( hw,  hh), // bottom-right
                    new Vector2 (-hw,  hh)  // bottom-left
                };

            var fixture = new FixtureSettings 
            {
                Verts = rect,
                IsSensor = isSensor,
                CollisionGroup = collisionGroup,
                CollidesWith = collidesWith
            };

            var body = new BodySettings
            {
                Fixtures = { fixture },
                Offset = offset // ConvertUnits.ToSimUnits(offset),
            };

            var structure = new PhysicsStructureSettings
            {
                Bodies = { body }
            };

            return structure;
        }
    }
}
