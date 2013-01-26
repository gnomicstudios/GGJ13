using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gnomic.Anim
{
    public struct JointAnimState<T> where T : JointAnim
    {
        public Transform2D CurrentKeyframe;
        public T JointAnim;

        private int currentKeyframeIndex;
        private int nextKeyframeIndex;
        private float currentSectionLength;
        private float timeSinceFrameChange;

        public JointAnimState(T anim)
            : this()
        {
            this.JointAnim = anim;
            Reset();
        }
        public void Reset()
        {
            timeSinceFrameChange = 0;
            currentKeyframeIndex = 0;
            nextKeyframeIndex = 0;
            currentSectionLength = 0;

            if (JointAnim.FrameCount > 1)
            {
                currentSectionLength = (JointAnim.GetFrameNumber(1) - JointAnim.GetFrameNumber(0)) / JointAnim.ParentClipAnim.Framerate;
                nextKeyframeIndex = 1;
            }
        }

        public void Update(float elapsedSeconds, ref SpriteState jointState)
        {
            if (JointAnim.FrameCount > 1)
            {
                timeSinceFrameChange += elapsedSeconds;

                while (timeSinceFrameChange > currentSectionLength)
                {
                    timeSinceFrameChange -= currentSectionLength;
                    currentKeyframeIndex = nextKeyframeIndex;
                    nextKeyframeIndex = (nextKeyframeIndex + 1) % JointAnim.FrameCount;

                    int fn1 = JointAnim.GetFrameNumber(currentKeyframeIndex);
                    int fn2 = JointAnim.GetFrameNumber(nextKeyframeIndex);
                    int frameDifference = fn2 > fn1 ? fn2 - fn1 : (JointAnim.ParentClipAnim.Duration - fn1) + fn2;
                    currentSectionLength = frameDifference / JointAnim.ParentClipAnim.Framerate;
                }
            }

            float lerpValue = currentSectionLength == 0 ? 0 : timeSinceFrameChange / currentSectionLength;
            JointAnim.ApplySate(currentKeyframeIndex, nextKeyframeIndex, lerpValue, ref jointState);

            //Transform2D key1 = JointAnim.Frames[currentKeyframeIndex].Transform;
            //Transform2D key2 = JointAnim.Frames[nextKeyframeIndex].Transform;
            //Transform2D.Lerp(ref key1, ref key2, timeSinceFrameChange / currentSectionLength, ref CurrentKeyframe);
        }

    }
}
