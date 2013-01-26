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
        private Body edgeLeft, edgeTop, edgeRight, edgeBottom;
                
        public Border(World world,
                      float width, float height, // display units
                      Vector2 offset,
                      float friction)
        {
            float simWidth = ConvertUnits.ToSimUnits(width);
            float simHeight = ConvertUnits.ToSimUnits(height);

            float simOffsetWidth = ConvertUnits.ToSimUnits(offset.X);
            float simOffsetHeight = ConvertUnits.ToSimUnits(offset.Y);

            edgeLeft = BodyFactory.CreateEdge(world, new Vector2(simOffsetWidth, simOffsetHeight), new Vector2(simOffsetWidth, simOffsetHeight + simHeight));
            edgeTop = BodyFactory.CreateEdge(world, new Vector2(simOffsetWidth, simOffsetHeight), new Vector2(simOffsetWidth + simWidth, simOffsetHeight));
            edgeRight = BodyFactory.CreateEdge(world, new Vector2(simOffsetWidth + simWidth, simOffsetHeight), new Vector2(simOffsetWidth + simWidth, simOffsetHeight + simHeight));
            edgeBottom = BodyFactory.CreateEdge(world, new Vector2(simOffsetWidth, simOffsetHeight + simHeight), new Vector2(simOffsetWidth + simWidth, simOffsetHeight + simHeight));

            edgeLeft.CollisionCategories    = Category.Cat8; //}
            edgeTop.CollisionCategories     = Category.Cat8; //} i.e. Border
            edgeRight.CollisionCategories   = Category.Cat8; //}
            edgeBottom.CollisionCategories  = Category.Cat4; // i.e. Terrain

            edgeBottom.Friction = 0;
        }
    }
}