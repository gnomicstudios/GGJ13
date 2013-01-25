using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gnomic.Anim
{
    public class Joint
    {
        public int ParentId;
        public string Name;
		[ContentSerializer(Optional=true)]
        public Transform2D Transform = Transform2D.Identity;
    }
}
