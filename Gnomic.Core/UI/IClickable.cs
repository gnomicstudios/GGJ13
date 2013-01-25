using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gnomic.UI
{
    public interface IClickable
    {
        Action<Vector2> ClickedAction { get; set; }
    }
}
