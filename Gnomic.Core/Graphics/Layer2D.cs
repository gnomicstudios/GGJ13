using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gnomic.Graphics;
using Microsoft.Xna.Framework;

namespace Gnomic.Graphics
{
    public class Layer2D
    {
        public Vector2 Parallax = Vector2.One;
        public List<IDrawable2D> Sprites = new List<IDrawable2D>(10);

        public Layer2D(Vector2 parallax)
        {
            Parallax = parallax;
        }
    }
}
