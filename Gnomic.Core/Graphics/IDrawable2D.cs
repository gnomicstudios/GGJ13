using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Gnomic.Graphics
{
    public interface IDrawable2D
    {
        void Draw2D(SpriteBatch spriteBatch);
        int LayerID { get; }
        bool IsVisible { get; }
    }
}
