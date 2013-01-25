using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gnomic.Anim;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gnomic.Graphics;

namespace Gnomic.Entities
{
	public class SpriteEntity : Entity, IDrawable2D
	{
		protected SpriteState spriteState = new SpriteState();

        public Vector2 Position
        {
            get { return spriteState.Transform.Pos; }
            set { spriteState.Transform.Pos = value; }
        }

        bool isVisible = true;
        public bool IsVisible
        {
            get { return isVisible; }
        }

		public void Draw2D(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(
				spriteState.Texture,
				spriteState.Transform.Pos,
				new Rectangle?(spriteState.TextureRect),
				spriteState.Color,
				spriteState.Transform.Rot,
				spriteState.Transform.Origin,
				spriteState.Transform.Scale,
				spriteState.FlipState,
				0.0f);
		}
	}
}
