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
		public SpriteState SpriteState = new SpriteState();

        public Vector2 Position
        {
            get { return SpriteState.Transform.Pos; }
            set { SpriteState.Transform.Pos = value; }
        }

        bool isVisible = true;
        public bool IsVisible
        {
            get { return isVisible; }
        }

		public void Draw2D(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(
				SpriteState.Texture,
				SpriteState.Transform.Pos,
				new Rectangle?(SpriteState.TextureRect),
				SpriteState.Color,
				SpriteState.Transform.Rot,
				SpriteState.Transform.Origin,
				SpriteState.Transform.Scale,
				SpriteState.FlipState,
				0.0f);
		}

        protected int layerID = 0;
        public virtual int LayerID 
        { 
            get { return layerID; } 
            set { layerID = value; }
        }

        public virtual float DrawOrder { get { return SpriteState.Transform.Pos.Y; } }
	}
}
