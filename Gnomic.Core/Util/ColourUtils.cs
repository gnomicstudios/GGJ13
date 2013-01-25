using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gnomic.Util
{
    public static class ColourUtils
    {
        public static Color ColorHSV(float h, float s, float v, float a)
        {
            float fh = h * 6.0f;
            float f = fh - (float)Math.Floor(fh);

            float p = v * (1.0f - s);
            float q = v * (1.0f - f * s);
            float t = v * (1.0f - (1.0f - f) * s);

            int ih = (int)fh;
            switch (ih)
            {
                case 0:
                    return new Color(v, t, p, a);
                case 1:
                    return new Color(q, v, p, a);
                case 2:
                    return new Color(p, v, t, a);
                case 3:
                    return new Color(p, q, v, a);
                case 4:
                    return new Color(t, p, v, a);
                default:
                    return new Color(v, p, q, a);
            }
        } 
    }
}
