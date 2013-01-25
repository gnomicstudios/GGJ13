using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Gnomic.Anim
{
    public class JointAnimTexture : JointAnim
    {
        public struct Frame
        {
            public int FrameNumber;
            public string Tex;
            public Rectangle TexRect;
            public SpriteEffects FlipState;
            public bool Visible;

            [ContentSerializerIgnore()]
            public Texture2D Texture;

            public void Init(ContentManager content)
            {
                if (!string.IsNullOrEmpty(Tex))
                    Texture = content.Load<Texture2D>(Tex);
            }
        }

        [ContentSerializer(FlattenContent = true, CollectionItemName = "Frame")]
        public Frame[] Frames;

        public override int FrameCount { get { return Frames.Length; } }
        public override int GetFrameNumber(int i) { return Frames[i].FrameNumber; }
        
        public override void Init(ContentManager content, ClipAnim clipAnim)
        {
            base.Init(content, clipAnim);

            for (int i = 0; i < Frames.Length; ++i)
            {
                Frames[i].Init(content);
            }
        }


        public override void ApplySate(int currentKeyframeIndex, int nextKeyframeIndex, float lerpValue, ref SpriteState jointState)
        {
            Frame f = Frames[currentKeyframeIndex];
            jointState.TextureRect = f.TexRect;
            jointState.Texture = f.Texture;
            jointState.FlipState = f.FlipState;
            jointState.Visible = f.Visible;
        }

        public override JointAnimState<JointAnim> CreateState()
        {
            return new JointAnimState<JointAnim>(this);
        }
    }
}
