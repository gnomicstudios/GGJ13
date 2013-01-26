using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using Gnomic.Core;
using Gnomic.Anim;

namespace Gnomic.Physics
{
    public class Border
    {
        private Body _anchor;

        public Vertices Corners{ get; set; }
        
        public Border(World world,
                      float width, float height, // display units
                      Vector2 offset,
                      float friction)
        {
            float simWidth = ConvertUnits.ToSimUnits(width);
            float simHeight = ConvertUnits.ToSimUnits(height);

            float simOffsetWidth = ConvertUnits.ToSimUnits(offset.X);
            float simOffsetHeight = ConvertUnits.ToSimUnits(offset.Y);

            Corners = new Vertices(4);
            Corners.Add(new Vector2(.0f + simOffsetWidth, .0f + simOffsetHeight));
            Corners.Add(new Vector2(simWidth + simOffsetWidth, .0f + simOffsetHeight));
            Corners.Add(new Vector2(simWidth + simOffsetWidth, simHeight + simOffsetHeight));
            Corners.Add(new Vector2(.0f + simOffsetWidth, simHeight + +simOffsetHeight));

            _anchor = BodyFactory.CreateLoopShape(world, Corners);
            _anchor.Friction = friction;
        }
    }
}