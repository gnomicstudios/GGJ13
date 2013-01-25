using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Gnomic.Anim
{
    public class ClipAnim
    {
        /// <summary>
        /// Name of the clip anim
        /// </summary>
        public string Name;

        /// <summary>
        /// Duration in frames
        /// </summary>
        public int Duration;

        /// <summary>
        /// Number of frames per second
        /// </summary>
        public float Framerate;

        /// <summary>
        /// The flip state of this animation.
        /// Flipping.X = -1 for horizontal flipping
        /// Flipping.Y = -1 for vertical flipping
        /// </summary>
        [ContentSerializer(Optional=true)]
        public Vector2 Flipping = Vector2.One;

        /// <summary>
        /// List of joint animations
        /// </summary>
        [ContentSerializer(FlattenContent = true, CollectionItemName = "JointAnim")] 
        public List<JointAnim> JointAnims = new List<JointAnim>();


        protected Clip parentClip;
        [ContentSerializerIgnore()]
        public Clip ParentClip { get { return parentClip; } }

        public void Init(ContentManager content, Clip clip)
        {
            parentClip = clip;

            foreach (JointAnim ja in JointAnims)
            {
                ja.Init(content, this);
            }
        }
    }

}
