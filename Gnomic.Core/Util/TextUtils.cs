using System;
using Microsoft.Xna.Framework.Graphics;

namespace Gnomic.Util
{
    public static class TextUtils
    {
        /// <summary>
        /// Ensure text is safe for the given SpriteFont
        /// </summary>
        public static string GetSupportedString(SpriteFont font, string inputStr, char replaceUnsupported)
        {
            char[] safeName = inputStr.ToCharArray();
            for (int c = 0; c < safeName.Length; ++c)
            {
                if (!font.Characters.Contains(safeName[c]))
                {
                    safeName[c] = replaceUnsupported;
                }
            }
            return new string(safeName);
        }
    }
}
