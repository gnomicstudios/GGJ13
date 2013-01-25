using System;
using Gnomic.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Gnomic.UI
{
    public class OverlaySettings : EntitySettings
    {
        public Vector2 Position = Vector2.Zero;
        public Vector2 Size = Vector2.Zero;

        [ContentSerializer(Optional = true)]
        public float Rotation = 0f;
        [ContentSerializer(Optional = true)]
        public Color Tint = Color.White;
        [ContentSerializer(Optional = true)]
        public int Alpha = 255;
        [ContentSerializer(Optional = true)]
        public float LayerDepth = 0f;
        [ContentSerializer(Optional = true)]
        public bool Visible = true;
    }

}
