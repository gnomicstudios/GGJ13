using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Gnomic.Anim
{
    public enum TweenType
    {
        none,
        motion
    }

    public class JointAnimSpatial : JointAnim
    {
        public struct Frame
        {
            public int FrameNumber;
            public Transform2D Transform;

			[ContentSerializer(Optional = true)]
			public TweenType Tween;
			[ContentSerializer(Optional = true)]
			public int EaseValue;
        }

        [ContentSerializer(FlattenContent = true, CollectionItemName = "Frame")]
        public Frame[] Frames;

        public override int FrameCount { get { return Frames.Length; } }
        public override int GetFrameNumber(int i) { return Frames[i].FrameNumber; }

        public override void ApplySate(int currentKeyframeIndex, int nextKeyframeIndex, float lerpValue, ref SpriteState jointState)
        {
			TweenType tweenType = Frames[currentKeyframeIndex].Tween;
			if (tweenType == TweenType.none)
            {
                jointState.Transform = Frames[currentKeyframeIndex].Transform;
            }
            else
            {
				int tweenEase = Frames[currentKeyframeIndex].EaseValue;
				//if (tweenType == TweenType.motion)
				//{
					if (tweenEase > 0)
					{
						lerpValue = Easing.QuadraticEaseOut(lerpValue, (float)tweenEase / 100.0f);
					}
					else if (tweenEase < 0)
					{
						lerpValue = Easing.QuadraticEaseIn(lerpValue, (float)tweenEase / -100.0f);
					}
				//}
				//else 
				//{
				//    if (tweenType == TweenType.motionCustomAll)
				//    {
				//        Vector2[] bezier = Frames[currentKeyframeIndex].CustomEaseAll;
				//        // determine segment id
				//        int segId = 3;
				//        while (segId < bezier.Length && lerpValue > bezier[segId].X)
				//        {
				//            segId += 3;
				//        }
				//        float endTime = bezier[segId].X;
				//        float startTime = bezier[segId - 3].X;
				//        float lerpInternal = (lerpValue - startTime) / (endTime - startTime);

				//        Easing.CubicBezierCurve(
				//    }
				//}

                Transform2D.Lerp(
                    ref Frames[currentKeyframeIndex].Transform, 
                    ref Frames[nextKeyframeIndex].Transform,
                    lerpValue, ref jointState.Transform);
            }
        }

        public override JointAnimState<JointAnim> CreateState()
        {
            return new JointAnimState<JointAnim>(this);
        }
    }
}
