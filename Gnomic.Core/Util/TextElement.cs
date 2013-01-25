using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Gnomic.Util
{
    public enum VerticalAlignment
    {
        Top, 
        Middle,
        Bottom
    }

    public enum HorizontalAlignment
    {
        Left,
        Middle,
        Right
    }

    /// <summary>
    /// Represents formatted text with inline button glyphs, clipping and new lines.
    /// </summary>
    public class TextElement
    {
        private SpriteFont font, buttonFont;
        private Dictionary<string, string> buttonGlyphs = new Dictionary<string, string>();
        private float lineHeight = 0;
        float buttonYOffset;

        public TextElement()
        {
            ButtonScale = 0.9f;
            Position = Vector2.Zero;
            Text = "";
            Color = Color.Black;
            Size = Vector2.Zero;
            ClipText = false;
        }

        #region Properties
        private string text;
        public string Text
        {
            get 
            { 
                return text; 
            }
            set
            {
                text = value;
                WrapText();
            }
        }
        public bool ClipText
        {
            get;
            set;
        }
        public SpriteFont Font
        {
            get { return font; }
            set
            {
                font = value;
                // Strictly speaking we should re-wrap the text, however this
                // is inefficient and if the font is the same size, unnecessary!
                // Our particular use-case always had the same sized fonts.
                //WrapText();
            }
        }
        private string fontName;
        public string FontName
        {
            get { return fontName; }
            set { fontName = value; }
        }
        public Vector2 Position
        {
            get;
            set;
        }
        public Color Color
        {
            get;
            set;
        }
        public float Rotation 
        { 
            get; 
            set; 
        }
        public Vector2 Origin 
        { 
            get; 
            set; 
        }
        Vector2 scale = new Vector2(1.0f, 1.0f);
        public Vector2 Scale 
        { 
            get { return scale;}
            set { scale = value; }
        }
        public float LayerDepth 
        { 
            get; 
            set; 
        }

        private Vector2 size;
        public Vector2 Size
        {
            get { return size; }
            set
            {
                size = value;
                if (size != Vector2.Zero)
                    WrapText();
            }
        }
        public float ButtonScale
        {
            get;
            set;
        }

        public HorizontalAlignment AlignH = HorizontalAlignment.Middle;
        public VerticalAlignment AlignV = VerticalAlignment.Middle;
        
        #endregion

        public void LoadContent(ContentManager content, GraphicsDevice device)
        {
            buttonGlyphs = new Dictionary<string, string>();
            buttonGlyphs.Add("[LTHUMB]", " ");
            buttonGlyphs.Add("[RTHUMB]", "\"");
            buttonGlyphs.Add("[DPAD]", "!");
            buttonGlyphs.Add("[BACK]", "#");
            buttonGlyphs.Add("[GUIDE]", "$");
            buttonGlyphs.Add("[START]", "%");
            buttonGlyphs.Add("[X]", "&");
            buttonGlyphs.Add("[Y]", "(");
            buttonGlyphs.Add("[A]", "'");
            buttonGlyphs.Add("[B]", ")");
            buttonGlyphs.Add("[LB]", "-");
            buttonGlyphs.Add("[RB]", "*");
            buttonGlyphs.Add("[RT]", "+");
            buttonGlyphs.Add("[LT]", ",");

            font = content.Load<SpriteFont>(FontName);
            buttonFont = content.Load<SpriteFont>("UI/xboxControllerSpriteFont");

            buttonYOffset = ((1.1f * ButtonScale * (float)buttonFont.LineSpacing / 2.0f) - (float)font.LineSpacing / 2.0f);
            
            WrapText();
        }

        object drawLock = new object();
        public void Draw(SpriteBatch sb)
        {

            lineHeight = font.LineSpacing;
            if (ClipText)
            {
                sb.GraphicsDevice.RasterizerState.ScissorTestEnable = true;
                sb.GraphicsDevice.ScissorRectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
            }

            lock (drawLock)
            {
                if (size != Vector2.Zero)
                    DrawLines(sb);
                else
                    DrawText(text, sb, Position);
            }

            if (ClipText)
            {
                sb.End();
                sb.Begin();
                sb.GraphicsDevice.RasterizerState.ScissorTestEnable = false;
            }
        }

        private void DrawText(string text, SpriteBatch sb, Vector2 position)
        {
            int tagStart = text.IndexOf('[');
            int tagEnd = text.IndexOf(']', tagStart != -1 ? tagStart : 0);
            
            if (tagStart > 0)
                position += DrawString(text.Substring(0, tagStart), sb, position);
            
            if (tagStart != -1 && tagEnd != -1)
                position += DrawButton(text.Substring(tagStart, tagEnd - tagStart + 1), sb, position);
            
            if (tagEnd != -1)
                DrawText(text.Substring(tagEnd + 1), sb, position);
            else
                DrawString(text, sb, position);
        }

        private void DrawLines(SpriteBatch sb)
        {
            Vector2 pos = Position;

            switch (AlignV)
            {
                case VerticalAlignment.Top:
                    // already at top
                    break;
                case VerticalAlignment.Bottom:
                    pos.Y += Size.Y - linesHeight;
                    break;
                case VerticalAlignment.Middle:
                    pos.Y += (Size.Y - linesHeight) * 0.5f;
                    break;
            }

            for (int i = 0; i < lines.Count; i++)
            {
                switch (AlignH)
                {
                    case HorizontalAlignment.Left:
                        // already at left
                        break;
                    case HorizontalAlignment.Right:
                        pos.X = Position.X + Size.X - lineSizes[i].X;
                        break;
                    case HorizontalAlignment.Middle:
                        pos.X = Position.X + (Size.X - lineSizes[i].X) * 0.5f;
                        break;
                }

                DrawText(lines[i], sb, pos);
                pos.Y += lineSizes[i].Y;
            }
        }

        private Vector2 DrawString(string text, SpriteBatch sb, Vector2 position)
        {
            //sb.DrawString(font, text, position, Color);
            sb.DrawString(font, text, position, Color, 
                          Rotation, Origin, scale, SpriteEffects.None, LayerDepth );

            return new Vector2(font.MeasureString(text).X * scale.X, 0);
        }

        private Vector2 DrawButton(string tag, SpriteBatch sb, Vector2 position)
        {
            //Convert tag into button font text
            if (buttonGlyphs.ContainsKey(tag))
                tag = buttonGlyphs[tag];
            else if (tag == "[NL]")
            {
                Vector2 r = new Vector2(Position.X - position.X, lineHeight);
                lineHeight = font.LineSpacing;
                return r;
            }
            else
                return DrawString(tag, sb, position);

            Vector2 size = buttonFont.MeasureString(tag) * ButtonScale * scale;

            sb.DrawString(buttonFont, tag, position,
                new Color(255, 255, 255, Color.A), 0f, 
                new Vector2(0f, buttonYOffset), 
                ButtonScale * scale, 
                SpriteEffects.None, LayerDepth);

            return new Vector2(size.X, 0);
        }

        public bool wrapText = true;
        List<Vector2> lineSizes = new List<Vector2>(2);
        List<String> lines = new List<string>(2);
        float linesHeight = 0f;
        private void WrapText()
        {
            lock (drawLock)
            {
                linesHeight = 0f;
                lineSizes.Clear();
                lines.Clear();

                if (Text == "" || font == null ||
                    Size == Vector2.Zero || !wrapText)
                    return;

                float lineWidth = 0f;
                float spaceWidth = font.MeasureString(" ").X * scale.X;
                float lineHeight = 0f;
                string currentLine = "";


                StringBuilder sb = new StringBuilder();
                string[] tmpLines = Text.Split('\n');

                linesHeight = 0f;

                for (int iLine = 0; iLine < tmpLines.Length; ++iLine)
                {
                    string lineText = tmpLines[iLine].Trim();
                    if (lineText == "")
                    {
                        lineSizes.Add(new Vector2(lineWidth, lineHeight));
                        lines.Add(currentLine);
                        linesHeight += lineHeight;
                    }
                    else
                    {
                        string[] words = lineText.Split(' ');
                        int wordInLine = 0;
                        for (int iWord = 0; iWord < words.Length; iWord++)
                        {
                            Vector2 wordSize = MeasureWord(words[iWord]);
                            lineHeight = wordSize.Y; // Math.Max(lineHeight, size.Y);

                            if (wordInLine > 0 && lineWidth + wordSize.X > Size.X)
                            {
                                // Wrap to next line
                                lineSizes.Add(new Vector2(lineWidth, lineHeight));
                                lines.Add(currentLine);
                                linesHeight += lineHeight;

                                currentLine = words[iWord];
                                wordInLine = 1;
                                lineWidth = wordSize.X;
                            }
                            else
                            {
                                // Append space if first
                                if (wordInLine > 0)
                                {
                                    currentLine += " ";
                                    lineWidth += spaceWidth;
                                }

                                // Continue this line
                                currentLine += words[iWord];
                                wordInLine++;
                                lineWidth += wordSize.X;
                            }
                        }

                        lineSizes.Add(new Vector2(lineWidth, lineHeight));
                        lines.Add(currentLine);
                        linesHeight += lineHeight;
                        currentLine = "";
                        lineWidth = 0f;
                    }
                }
            }
        }

        private Vector2 MeasureWord(string text)
        {
            Vector2 tmp = Vector2.Zero;
            Vector2 total = Vector2.Zero;
            
            int startIndex = text.IndexOf('[');
            int endIndex = text.IndexOf(']', startIndex != -1 ? startIndex : 0);
            
            if (startIndex > 0)
                total = font.MeasureString(text.Substring(0, startIndex)) * scale;
            
            if (startIndex != -1 && endIndex != -1)
            {
                tmp = MeasureTag(text.Substring(startIndex, endIndex - startIndex + 1));
                total = new Vector2(total.X + tmp.X, Math.Max(total.Y, tmp.Y));
            }
            
            if (endIndex != -1)
            {
                tmp = MeasureWord(text.Substring(endIndex + 1));
                total = new Vector2(total.X + tmp.X, Math.Max(total.Y, tmp.Y));
            }
            else
                return font.MeasureString(text) * scale;

            if (total.X < 0) total.X = 0;
            return total;
        }

        private Vector2 MeasureTag(string tag)
        {
            if (buttonGlyphs.ContainsKey(tag))
                return buttonFont.MeasureString(buttonGlyphs[tag]) * scale;
            else if (tag == "[NL]")
                return new Vector2(float.NegativeInfinity);
            else
                return font.MeasureString(tag) * scale;
        }
    }
}

