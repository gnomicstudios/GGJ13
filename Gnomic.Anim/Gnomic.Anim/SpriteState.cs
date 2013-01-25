using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Gnomic.Anim
{
	public struct SpriteState
	{
		public Transform2D Transform;
		public Color Color;
		[ContentSerializerIgnore()]
		public Texture2D Texture;
		public Rectangle TextureRect;
		public SpriteEffects FlipState;
		public bool Visible;

        public SpriteState(Texture2D texture)
        {
            Texture = texture;
            Transform = Transform2D.Identity;
            Transform.Origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Color = Color.White;
            TextureRect = new Rectangle(0, 0, texture.Width, texture.Height);
            FlipState = SpriteEffects.None;
            Visible = true;
        }
	}
}
