using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Gnomic.Util;
using Gnomic.Entities;
using Gnomic.Graphics;

namespace Gnomic.UI
{
    public class TextSettings : OverlaySettings
    {
        public TextSettings()
        {
            this.EntityClass = "Gnomic.UI.Text";
        }

        public string FontName;
        public string Text;
        [ContentSerializer(Optional = true)]
        public string TranslationTag;

        [ContentSerializer(Optional = true)]
        public Vector2 FontScreenHeight = new Vector2(0.04f, 0.04f);
        [ContentSerializer(Optional = true)]
        public HorizontalAlignment AlignH = HorizontalAlignment.Middle;
        [ContentSerializer(Optional = true)]
        public VerticalAlignment AlignV = VerticalAlignment.Top;

    }

    public class Text : Entity, IDrawable2D
    {
        public new TextSettings Settings;
        public override void ApplySettings(EntitySettings settings)
        {
            base.ApplySettings(settings);
            this.Settings = CastSettings<TextSettings>(settings);
            HandleSettingsChanged();
        }

        TextElement textElem;

        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set
            {
                destRectPosDirty = true;
                position = value;
                if (textElem != null)
                    textElem.Position = value;
            }
        }


        float rotation;
        public float RotRadians
        {
            get { return rotation; }
            set
            {
                rotation = value;
                Settings.Rotation = MathHelper.ToDegrees(rotation);

                if (textElem != null)
                    textElem.Rotation = value;
            }
        }

        private Vector2 scale = Vector2.One;
        public Vector2 Scale
        {
            get { return scale; }
            set 
            {
                scale = value;
                if (textElem != null)
                    textElem.Scale = value;
                else
                    destRectSizeDirty = true;
            }
        }

        private Vector2 origin;
        public Vector2 Origin
        {
            get { return origin; }
            set 
            { 
                origin = value;
                if (textElem != null)
                    textElem.Origin = value;
            }
        }

        //public string fontName;
        public string FontName
        {
            get { return Settings.FontName; }
            set 
            {
                Settings.FontName = value;
                if (IsInitialized)
                {
                    font = ParentScreen.ParentGame.Content.Load<SpriteFont>(Settings.FontName);
                    if (textElem != null)
                    {
                        textElem.FontName = value;
                        textElem.Font = Font;
                    }
                }
            }
        }
        SpriteFont font;
        public SpriteFont Font
        {
            get { return font; }
            set
            {
                font = value;
                if (textElem != null)
                {
                    textElem.Font = value;
                }
            }
        }
 
        StringBuilder builder = new StringBuilder();

        public void ClearBuilder()
        {
            builder.Length = 0;
        }

        static char[] numberBuffer = new char[16];
        public void AppendNumber(float number, int dp)
        {
            int intPart = (int)number;
            int floatPart = (int)((number - (float)intPart) * Math.Pow(10, dp));
            AppendNumber(intPart);
            builder.Append('.');
            AppendNumber(floatPart, dp);
        }
        public void AppendNumber(int number)
        {
            AppendNumber(number, 0);
        }
        public void AppendNumber(int number, int minDigits)
        {
            if (number < 0)
            {
                builder.Append('-');
                number = -number;
            }
            int index = 0;
            do
            {
                int digit = number % 10;
                numberBuffer[index] = (char)('0' + digit);
                number /= 10;
                ++index;
            } while (number > 0 || index < minDigits);
            for (--index; index >= 0; --index)
            {
                builder.Append(numberBuffer[index]);
            }
        } 

        public string TextValue 
        {
            get { return builder.ToString(); }
            set
            {
                ClearBuilder();
                builder.Append(value);

                if (textElem != null)
                    textElem.Text = value;
            }
        }

        private bool destRectPosDirty = true;
        private bool destRectSizeDirty = true;
        private Rectangle destRect;
        public Rectangle DestRect 
        {
            get 
            {
                if (destRectPosDirty)
                {
                    destRect.X = (int)Position.X;
                    destRect.Y = (int)Position.Y;
                    destRectPosDirty = false;
                }
                if (destRectSizeDirty)
                {
                    UpdateDestRectSize();
                }
                return destRect; 
            }
            set 
            {
                position.X = destRect.X;
                position.Y = destRect.Y;
                
                if (textElem != null)
                {
                    textElem.Position = Position;
                    textElem.Size = new Vector2(destRect.Width, destRect.Height);
                }
            }
        }

        public void UpdateDestRectSize()
        {
            if (textElem != null)
            {
                destRect.Width = (int)(Settings.Size.X * (float)ParentScreen.ParentGame.ScreenHeight);
                destRect.Height = (int)(Settings.Size.Y * (float)ParentScreen.ParentGame.ScreenHeight);
            }
            else
            {
                Vector2 size = font.MeasureString(builder);
                destRect.Width = (int)(size.X * scale.X);
                destRect.Height = (int)(size.Y * scale.Y);
            }
            destRectSizeDirty = false;
        }

        private Color colorWithAlpha = Color.White;
        //private int alpha = 255;
        public byte Alpha
        {
            get { return (byte)Settings.Alpha; }
            set
            {
                Settings.Alpha = value;
                if (colorWithAlpha.A != value)
                {
                    colorWithAlpha = new Color(Settings.Tint.R, Settings.Tint.G, Settings.Tint.B,
                                               (byte)((float)(Settings.Tint.A * Settings.Alpha * 255) / 65025.0f));
                }

                if (textElem != null)
                    textElem.Color = colorWithAlpha;
            }
        }

        //private Color tint = Color.White;
        public Color Tint
        {
            get { return Settings.Tint; }
            set
            {
                Settings.Tint = value;
                colorWithAlpha = new Color(Settings.Tint.R, Settings.Tint.G, Settings.Tint.B,
                                           (byte)((float)(Settings.Tint.A * Settings.Alpha * 255) / 65025.0f));

                if (textElem != null)
                    textElem.Color = colorWithAlpha;
            }
        }

        //private float layerDepth;
        public float LayerDepth
        {
            get { return Settings.LayerDepth; }
            set 
            {
                Settings.LayerDepth = value;

                if (textElem != null)
                    textElem.LayerDepth = value;
            }
        }

        public Text() 
        {
        }
        public Text(string fontName, string text, Vector2 position)
            : this()
        {
            Vector2 screenSize = new Vector2(ParentScreen.ParentGame.ScreenWidth, ParentScreen.ParentGame.ScreenHeight);
            Settings = new TextSettings();
            Settings.FontName = fontName;
            Settings.Text = text;
            Settings.Position = (position - screenSize * 0.5f) / screenSize.Y;

            ApplySettings(Settings);
        }

        public Text(string fontName, string text, Vector2 position, Vector2 size)
            : this()
        {
            Vector2 screenSize = new Vector2(ParentScreen.ParentGame.ScreenWidth, ParentScreen.ParentGame.ScreenHeight);
            Settings.FontName = fontName;
            Settings.Text = text;
            Settings.Position = (position - screenSize * 0.5f) / screenSize.Y;
            Settings.Size = size / screenSize.Y;

            ApplySettings(Settings);
        }

        public override void Initialize(GameScreen parentScreen)
        {
            base.Initialize(parentScreen);

            font = ParentScreen.ParentGame.Content.Load<SpriteFont>(Settings.FontName);

            HandleSettingsChanged();
        
            ParentScreen.ParentGame.OnScreenSizeChanged += HandleScreenSizeChange;
        }

        public override void Deinitialize()
        {
            base.Deinitialize();
            ParentScreen.ParentGame.OnScreenSizeChanged -= HandleScreenSizeChange;
        }

        public void SetPositionFromScreenCoords(Vector2 screenPos)
        {
            destRectPosDirty = true;

            position = (float)ParentScreen.ParentGame.ScreenHeight * screenPos +
                        new Vector2((float)ParentScreen.ParentGame.ScreenWidth * 0.5f,
                                    (float)ParentScreen.ParentGame.ScreenHeight * 0.5f);
            
            if (textElem != null)
            {
                textElem.Position = position;
            }
        }

        float fontScreenHeightDefault;
        void HandleScreenSizeChange(int w, int h)
        {
            if (!IsInitialized)
                return;

            SetPositionFromScreenCoords(Settings.Position);

            destRectSizeDirty = true;
            fontScreenHeightDefault = (float)font.LineSpacing / (float)h;
            if (Settings.FontScreenHeight == Vector2.Zero)
            {
                scale = Vector2.One;
            }
            else
            {
                scale = Settings.FontScreenHeight / fontScreenHeightDefault;
            }

            if (textElem != null)
            {
                textElem.AlignH = Settings.AlignH;
                textElem.AlignV = Settings.AlignV;
                textElem.Scale = scale;
                textElem.Size = Settings.Size * (float)h;
                textElem.Color = this.colorWithAlpha;
                textElem.ClipText = false;
                textElem.Rotation = rotation;
 
                textElem.FontName = Settings.FontName;
                textElem.LoadContent(ParentScreen.ParentGame.Content, ParentScreen.ParentGame.GraphicsDevice);
            }
        }

        private void HandleSettingsChanged()
        {
            isVisible = Settings.Visible;
            Tint = Settings.Tint;
            Alpha = (byte)Settings.Alpha;
            rotation = MathHelper.ToRadians(Settings.Rotation);

            if (textElem == null && Settings.Size != Vector2.Zero)
            {
                textElem = new TextElement();
            }
            else if (Settings.Size == Vector2.Zero)
            {
                textElem = null;
            }
                        
            string actualText = Settings.Text;
            if (String.IsNullOrEmpty(actualText) &&
                !String.IsNullOrEmpty(Settings.TranslationTag) &&
                ParentScreen.ParentGame.Resources != null)
            {
                actualText = ParentScreen.ParentGame.Resources.GetString(Settings.TranslationTag);
            }

            if (actualText == null)
            {
                actualText = "";
            }

            ClearBuilder();
            builder.Append(actualText);

            if (textElem != null)
                textElem.Text = actualText;

            HandleScreenSizeChange(ParentScreen.ParentGame.ScreenWidth, ParentScreen.ParentGame.ScreenHeight);
        }

        public float GetFontScaleAtScreenHeight(float scrnHeight)
        {
            return scrnHeight / fontScreenHeightDefault;
        }


#region IDrawable2D Members

        bool isVisible = true;
        public bool IsVisible
        {
            get { return isVisible; }
        }

		public void Draw2D(SpriteBatch spriteBatch)
		{
			if (!isVisible)
                return;

            // TextElem handles wrapped and aligned text
            if (textElem != null)
                textElem.Draw(spriteBatch);
            else
                spriteBatch.DrawString( font, builder, position, colorWithAlpha,
                                        rotation, origin, scale, SpriteEffects.None, Settings.LayerDepth);
		}

#endregion

        Vector2 ToScreenSpace(Vector2 midSpace)
        {
            return new Vector2(2f * midSpace.X / ParentScreen.ParentGame.GraphicsDevice.Viewport.AspectRatio, -2f * midSpace.Y);
        }
        Vector2 ToMidSpace(Vector2 screenSpace)
        {
            return new Vector2(0.5f * screenSpace.X * ParentScreen.ParentGame.GraphicsDevice.Viewport.AspectRatio, -0.5f * screenSpace.Y);
        }

        //public void DrawDiag(Color colour)
        //{
        //    if (Settings.Size == Vector2.Zero)
        //    {
        //        Vector2 anchor = ToScreenSpace(Settings.Position);
        //        VectorRenderer.DrawLineScreenSpace(anchor.X, anchor.Y, anchor.X, anchor.Y - 2f * Settings.FontScreenHeight.Y, colour);
        //    }
        //    else
        //    {
        //        Vector2 tl = ToScreenSpace(Settings.Position);
        //        Vector2 br = ToScreenSpace(Settings.Position + Settings.Size);
        //        VectorRenderer.DrawLineScreenSpace(tl.X, tl.Y, br.X, tl.Y, colour);
        //        VectorRenderer.DrawLineScreenSpace(br.X, tl.Y, br.X, br.Y, colour);
        //        VectorRenderer.DrawLineScreenSpace(br.X, br.Y, tl.X, br.Y, colour);
        //        VectorRenderer.DrawLineScreenSpace(tl.X, br.Y, tl.X, tl.Y, colour);
        //    }
        //}

        public int EditablePointCount 
        {
            get 
            {
                // Top left and bottom right
                return 2;
            }  
        }

        public Vector2 GetEditablePoint(int i)
        {
            if (i == 0)
            {
                return ToScreenSpace(Settings.Position);
            }
            else
            {
                if (Settings.Size == Vector2.Zero)
                {
                    return ToScreenSpace(Settings.Position + new Vector2(0f, Settings.FontScreenHeight.Y));
                }
                else
                {
                    return ToScreenSpace(Settings.Position + Settings.Size);
                }
            }
        }

        public void SetEditablePoint(int i, Vector2 newPos)
        {
            if (i == 0)
            {
                Settings.Position = ToMidSpace(newPos);
            }
            else
            {
                Vector2 bottomRight = ToMidSpace(newPos);
                Settings.Size = bottomRight - Settings.Position;

                if (Settings.Size.Length() < 0.01)
                    Settings.Size = Vector2.Zero;
            }

            HandleScreenSizeChange(ParentScreen.ParentGame.ScreenWidth, ParentScreen.ParentGame.ScreenHeight);
        }
    }
}
