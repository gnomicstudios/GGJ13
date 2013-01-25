using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Gnomic.Graphics
{
    public interface IDrawable3D
    {
        void Draw3D();

        int DrawGroup { get; }

        bool IsVisible { get; }
    }
}
