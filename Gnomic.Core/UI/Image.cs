using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Gnomic.Util;
using Gnomic.Entities;
using Gnomic.Graphics;

namespace Gnomic.UI
{
    public class ImageSettings : OverlaySettings
    {
        public ImageSettings()
        {
            this.EntityClass = "Gnomic.UI.Image";
        }

        public string TextureName;

        [ContentSerializer(Optional = true)]
        public Vector2 Pivot = new Vector2(0.5f, 0.5f);
        [ContentSerializer(Optional = true)]
        public Vector4 UVRect = new Vector4(0f, 0f, 1f, 1f);
        [ContentSerializer(Optional = true)]
        public SpriteEffects SpriteEffects = SpriteEffects.None;
    }

    public class Image : Entity, IDrawable2D
    {
        public new ImageSettings Settings = new ImageSettings();
        public override void ApplySettings(EntitySettings settings)
        {
            base.ApplySettings(settings);
            this.Settings = CastSettings<ImageSettings>(settings);
            HandleSettingsChange();
        }

        protected float texW = 1f;
        protected float texH = 1f;

        bool isVisible = true;
        public bool IsVisible
        {
            get { return isVisible; }
        }

        //string textureName;
        public string TextureName
        {
            get { return Settings.TextureName; }
            set 
            {
                Settings.TextureName = value;
#if NETFX_CORE
            // TODO
#else
                if (IsInitialized)
                    Texture = ParentScreen.ParentGame.Content.LoadTextureStream(Settings.TextureName);
#endif
            }
        }

        protected Texture2D texture;
        public Texture2D Texture
        {
            get { return texture; }
            set 
            { 
                texture = value;
                if (texture != null)
                {
                    if (sourceRect.HasValue)
                    {
                        texW = sourceRect.Value.Width;
                        texH = sourceRect.Value.Height;
                    }
                    else
                    {
                        texW = texture.Width;
                        texH = texture.Height;
                    }
                }
            }
        }

        ////Vector2 position;
        //public Vector2 Position
        //{
        //    get { return Settings.Position + Off; }
        //    set { Settings.Position = value - offset; }
        //}

        protected float rotation;
        public float RotRadians
        {
            get { return rotation; }
            set 
            {
                rotation = value;
                //Settings.Rotation = MathHelper.ToDegrees(value); 
            }
        }

        protected Vector2 offset;
        public Vector2 Offset
        {
            get { return offset; }
            set 
            {
                offset = value;
                destRectDirty = true;
            }
        }
        
        protected Vector2 origin;
        public Vector2 Pivot
        {
            get 
            {
                return origin; 
            }
            set 
            { 
                origin = value;
                destRectDirty = true;
            }
        }

        public bool Centered = true;

        protected Vector2 scale = Vector2.One;
        public Vector2 Scale
        {
            get { return scale; }
            set 
            { 
                scale = value;
                destRectDirty = true;
            }
        }
        
        //SpriteEffects spriteEffects = SpriteEffects.None;
        //float layerDepth = 0.0f;
        bool destRectDirty = true;
        Rectangle destRect;
        public Rectangle DestRect 
        { 
            get 
            {
                if (destRectDirty)
                {
                    destRectDirty = false;
                    destRect = new Rectangle(
                                        (int)(offset.X - origin.X * scale.X),
                                        (int)(offset.Y - origin.Y * scale.Y),
                                        (int)(texW * scale.X),
                                        (int)(texH * scale.Y));
                }
                return destRect;
            }
            set
            {
                // NOT SUPPORTED
                //if (texture != null)
                //{
                //    scale.X = (float)value.Width / (float)texture.Width;
                //    scale.Y = (float)value.Height / (float)texture.Height;
                //}
                //destRect = value;
                //Settings.Position.X = value.X + origin.X * scale.X;
                //Settings.Position.Y = value.Y + origin.Y * scale.Y;
            }
        }

        protected Rectangle? sourceRect;
        public Rectangle? SourceRect
        {
            get { return sourceRect; }
            set
            {
                sourceRect = value;
                if (sourceRect.HasValue)
                {
                    texW = sourceRect.Value.Width;
                    texH = sourceRect.Value.Height;
                    this.origin.X = texW * Settings.Pivot.X;
                    this.origin.Y = texH * Settings.Pivot.Y;
                }
                else if (this.Texture != null)
                {
                    texW = Texture.Width;
                    texH = Texture.Height;
                    this.origin.X = texW * Settings.Pivot.X;
                    this.origin.Y = texH * Settings.Pivot.Y;
                }
                else
                    origin = Vector2.Zero;
            }
        }

        protected Color colorWithAlpha = Color.White;
        //private int alpha = 255;
        public byte Alpha 
        {
            get { return (byte)Settings.Alpha; }
            set
            {
                Settings.Alpha = value;
                if (Settings.Alpha < 255)
                    colorWithAlpha = new Color(Settings.Tint.R, Settings.Tint.G, Settings.Tint.B,
                                           (byte)((float)(Settings.Tint.A * Settings.Alpha * 255) / 65025.0f));
                else
                    colorWithAlpha = Settings.Tint;
            }
        }
        //private Color tint = Color.White;
        public Color Tint
        {
            get { return Settings.Tint; }
            set
            {
                Settings.Tint = value;
                if (Settings.Alpha < 255)
                    colorWithAlpha = new Color(Settings.Tint.R, Settings.Tint.G, Settings.Tint.B,
                                               (byte)((float)(Settings.Tint.A * Settings.Alpha * 255) / 65025.0f));
                else
                    colorWithAlpha = Settings.Tint;
            }
        }
        
        public override void Initialize(GameScreen parentScreen)
        {
            base.Initialize(parentScreen);

#if NETFX_CORE
            // TODO
#else
            Texture = ParentScreen.ParentGame.Content.LoadTextureStream(TextureName);
#endif

            HandleSettingsChange();
            ParentScreen.ParentGame.OnScreenSizeChanged += HandleScreenSizeChange;
        }

        public override void Deinitialize()
        {
            base.Deinitialize();
            ParentScreen.ParentGame.OnScreenSizeChanged -= HandleScreenSizeChange;
        }
        
        public void SetPositionFromScreenCoords(Vector2 screenPos)
        {
            offset = (float)ParentScreen.ParentGame.ScreenHeight * screenPos +
                        new Vector2((float)ParentScreen.ParentGame.ScreenWidth * 0.5f,
                                    (float)ParentScreen.ParentGame.ScreenHeight * 0.5f);
        }

        
        

        protected Vector2 size;
        void HandleScreenSizeChange(int w, int h)
        {
            if (!IsInitialized)
                return;

            offset = (float)h * Settings.Position +
                        new Vector2((float)w * 0.5f,
                                    (float)h * 0.5f);

            size = new Vector2(Settings.Size.X * h,
                               Settings.Size.Y * h);

            scale = new Vector2(size.X / (float)texW,
                                size.Y / (float)texH);

            destRectDirty = false;
            destRect = new Rectangle(
                                (int)(offset.X - origin.X * scale.X),
                                (int)(offset.Y - origin.Y * scale.Y),
                                (int)(texW * scale.X),
                                (int)(texH * scale.Y));
        }

        private void HandleSettingsChange()
        {
            if (!IsInitialized)
                return;

            this.Visible = Settings.Visible;

            Tint = Settings.Tint;
            Alpha = Alpha;

            if (Texture != null && Settings.UVRect != (new Vector4(0f, 0f, 1f, 1f)))
            {
                sourceRect = new Rectangle?(
                    new Rectangle(
                        (int)(Texture.Width * Settings.UVRect.X),
                        (int)(Texture.Height * Settings.UVRect.Y),
                        (int)(Texture.Width * (Settings.UVRect.Z - Settings.UVRect.X)),
                        (int)(Texture.Height * (Settings.UVRect.W - Settings.UVRect.Y))));
            }
            else
            {
                sourceRect = new Rectangle?();
            }

            if (sourceRect.HasValue)
            {
                texW = sourceRect.Value.Width;
                texH = sourceRect.Value.Height;
                this.origin.X = texW * Settings.Pivot.X;
                this.origin.Y = texH * Settings.Pivot.Y;
            }
            else if (this.Texture != null)
            {
                texW = Texture.Width;
                texH = Texture.Height;
                this.origin.X = texW * Settings.Pivot.X;
                this.origin.Y = texH * Settings.Pivot.Y;
            }
            else
                origin = Vector2.Zero;

            rotation = MathHelper.ToRadians(Settings.Rotation);
            
            HandleScreenSizeChange(ParentScreen.ParentGame.ScreenWidth, ParentScreen.ParentGame.ScreenHeight);
        }

        //public override void Update() { }

        private bool visible = true;
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

		public void Draw2D(SpriteBatch spriteBatch)
		{
            spriteBatch.Draw(texture, offset, SourceRect, colorWithAlpha, rotation, origin, scale, Settings.SpriteEffects, Settings.LayerDepth);
        }


        #region IEditableDiagnosticsProvider Members
        
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
        //    Vector2 posInMidSpace = Settings.Position - Settings.Pivot * Settings.Size;
        //    Vector2 tl = ToScreenSpace(posInMidSpace);
        //    Vector2 br = ToScreenSpace(posInMidSpace + Settings.Size);
        //    VectorRenderer.DrawLineScreenSpace(tl.X, tl.Y, br.X, tl.Y, colour);
        //    VectorRenderer.DrawLineScreenSpace(br.X, tl.Y, br.X, br.Y, colour);
        //    VectorRenderer.DrawLineScreenSpace(br.X, br.Y, tl.X, br.Y, colour);
        //    VectorRenderer.DrawLineScreenSpace(tl.X, br.Y, tl.X, tl.Y, colour);
        //}

        public int EditablePointCount
        {
            get
            {
                // Top left, bottom right and pivot
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
                Vector2 bottomRight = Settings.Position - Settings.Pivot * Settings.Size + Settings.Size;
                return ToScreenSpace(bottomRight);
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
                Settings.Size = bottomRight - Settings.Position + Settings.Pivot * Settings.Size;
            }

            HandleSettingsChange();
        }
        
        #endregion
    }
}
