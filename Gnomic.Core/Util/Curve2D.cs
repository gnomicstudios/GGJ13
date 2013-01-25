using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Gnomic.Util
{
    public class CurveKey2D
    {
        public float Time;
        public float Tension = 0.5f;
        public Vector2 Position { get; set; }

        internal Vector2 tanIn;
        public Vector2 TangentIn { get { return tanIn; } }
        
        internal Vector2 tanOut;
        public Vector2 TangentOut { get { return tanOut; } }

        public CurveKey2D(float time, Vector2 pos)
        {
            Time = time;
            Position = pos;
        }
    }

    public class Curve2D
    {
        public List<CurveKey2D> Keys = new List<CurveKey2D>();

        public bool IsClosed;
        bool needsUpdate = true;
        int currSeg = 0;
        float currTime = 0.0f;

        //q(t) = 0.5 *( (2 * P1) +
  	    //                (-P0 + P2) * t +
        //                (2*P0 - 5*P1 + 4*P2 - P3) * t2 +
        //                (-P0 + 3*P1- 3*P2 + P3) * t3)

        //q'(t) = 0.5 *( (2 * P1) +
        //                (-P0 + P2) * t +
        //                (2*P0 - 5*P1 + 4*P2 - P3) * t2 +
        //                (-P0 + 3*P1- 3*P2 + P3) * t3)

        // http://www.cubic.org/docs/hermite.htm
        // http://en.wikipedia.org/wiki/Catmull-Rom_spline#Catmull.E2.80.93Rom_spline

        public CurveKey2D AddKey(float time, Vector2 point, float tension)
        {
            CurveKey2D newKey = AddKey(time, point);
            newKey.Tension = tension;
            return newKey;
        }

        public CurveKey2D AddKey(float time, Vector2 point)
        {
            // find insertion point
            int pos = Keys.Count;
            while (pos > 0 && time < Keys[pos - 1].Time)
                pos--;

            // add new key to list
            CurveKey2D newKey = new CurveKey2D(time, point);
            if (pos == Keys.Count)
                Keys.Add(newKey);
            else
                Keys.Insert(pos, newKey);

            needsUpdate = true;
            return newKey;
        }

        public float TotalTime
        {
            get 
            {
                if (Keys.Count == 0)
                    return 0.0f;
                else
                    return Keys[Keys.Count - 1].Time;
            }
        }

        public void BuildTangents()
        {
            Vector2 lastPoint;
            Vector2 nextPoint;

            for (int i = 0; i < Keys.Count; i++)
            {
                if (i == 0)
                    lastPoint = Keys[i].Position;
                else
                    lastPoint = Keys[i - 1].Position;

                if (i == Keys.Count - 1)
                    nextPoint = Keys[i].Position;
                else
                    nextPoint = Keys[i + 1].Position;

                float factor = (1.0f - Keys[i].Tension) * 0.5f;
                Keys[i].tanOut.X = factor * (nextPoint.X - lastPoint.X);
                Keys[i].tanOut.Y = factor * (nextPoint.Y - lastPoint.Y);
                Keys[i].tanIn = -Keys[i].tanOut;
            }

            needsUpdate = false;
            currSeg = 0;
            currTime = 0;
        }

        public Vector2 GetValueAt(float time)
        {
            if (needsUpdate)
                BuildTangents();

            // Ensure segment is correct. Assumes sequential lookup. 
            if (time > currTime)
            {
                while (currSeg < Keys.Count - 2 &&
                       time > Keys[currSeg + 1].Time)
                {
                    currSeg++;
                }
            }
            else if (time < currTime)
            {
                while (currSeg > 0 && 
                       time < Keys[currSeg].Time)
                {
                    currSeg--;
                }
            }
            
            currTime = time;

            // t = proportion through current segment
            float t = (time - Keys[currSeg].Time) / 
                      (Keys[currSeg + 1].Time - Keys[currSeg].Time);

            // exponentials of t
            float t2 = t * t;
            float t3 = t2 * t;

            // Calculate Hermite cubic:
            return Keys[currSeg].Position * (2.0f * t3 - 3.0f * t2 + 1) +
                   Keys[currSeg].TangentOut * (t3 - 2.0f * t2 + t) +
                   Keys[currSeg + 1].Position * (-2.0f * t3 + 3.0f * t2) +
                   Keys[currSeg + 1].TangentOut * (t3 - t2);
        }


        public Vector2 GetTangentAt(float time)
        {
            if (needsUpdate)
                BuildTangents();

            // Ensure segment is correct. 
            // Optimised for sequential lookup
            if (time > currTime)
            {
                while (currSeg < Keys.Count - 2 &&
                       time > Keys[currSeg + 1].Time)
                {
                    currSeg++;
                }
            }
            else if (time < currTime)
            {
                while (currSeg > 0 &&
                       time < Keys[currSeg].Time)
                {
                    currSeg--;
                }
            }

            currTime = time;

            // t = proportion through current segment
            float t = (time - Keys[currSeg].Time) /
                      (Keys[currSeg + 1].Time - Keys[currSeg].Time);

            // exponentials of t
            float t2 = t * t;
            //float t3 = t2 * t;

            // Calculate Hermite cubic (DERIVATIVE FOR TANGENT):
            return Keys[currSeg].Position * (6.0f * t2 - 6.0f * t) +
                   Keys[currSeg].TangentOut * (3.0f * t2 - 4.0f * t + 1) +
                   Keys[currSeg + 1].Position * (-6.0f * t2 + 6.0f * t) +
                   Keys[currSeg + 1].TangentOut * (3.0f * t2 - 2.0f * t);
        }

    }
}
