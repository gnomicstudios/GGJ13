using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Gnomic.Anim
{
    public class JointAnimColour : JointAnim
    {
        public struct Frame
        {
            public int FrameNumber;
            public int R;
            public int G;
            public int B;
            public int A;

			[ContentSerializer(Optional = true)]
			public TweenType Tween;
			[ContentSerializer(Optional = true)]
			public int EaseValue;
            [ContentSerializerIgnore()]
            public Color Color;

            public void Init()
            {
                Color = new Color(R, G, B, A);
            }
        }

        [ContentSerializer(FlattenContent = true, CollectionItemName="Frame")]
        public Frame[] Frames;
        
        public override int FrameCount { get { return Frames.Length; } }
        public override int GetFrameNumber(int i) { return Frames[i].FrameNumber; }

        public override void Init(ContentManager content, ClipAnim clipAnim)
        {
            base.Init(content, clipAnim);

            for (int i = 0; i < Frames.Length; ++i)
            {
                Frames[i].Init();
            }
        }


        public override void ApplySate(int currentKeyframeIndex, int nextKeyframeIndex, float lerpValue, ref SpriteState jointState)
        {
			if (Frames[currentKeyframeIndex].Tween == TweenType.none)
			{
				jointState.Color = Frames[currentKeyframeIndex].Color;
			}
			else
			{
				int tweenEase = Frames[currentKeyframeIndex].EaseValue;
				if (tweenEase > 0)
				{
					lerpValue = Easing.QuadraticEaseOut(lerpValue, (float)tweenEase / 100.0f);
				}
				else if (tweenEase < 0)
				{
					lerpValue = Easing.QuadraticEaseIn(lerpValue, (float)tweenEase / -100.0f);
				}

				jointState.Color = Color.Lerp(
					Frames[currentKeyframeIndex].Color,
					Frames[nextKeyframeIndex].Color,
					lerpValue);
			}
        }

        public override JointAnimState<JointAnim> CreateState()
        {
            return new JointAnimState<JointAnim>(this);
        }
    }
}
