using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Gnomic.Entities;
using Gnomic.Util;
using Gnomic.UI;
using Gnomic.Graphics;
using Gnomic.Physics;

namespace Gnomic
{
    public class GameScreen
    {
        // A content reference to the 
        public string ObjectSettingsFile;
        public List<EntitySettings> ObjectSettings = new List<EntitySettings>();

        public Random Rand = new Random();

        RenderBatch renderBatch;
        public RenderBatch RenderBatch { get { return renderBatch; } }

        Matrix renderBatchWorldTransform = Matrix.Identity;
        public Matrix RenderBatchWorldTransform
        {
            get { return renderBatchWorldTransform; }
            set { renderBatchWorldTransform = value; }
        }

        ICamera3D camera3D;
        public ICamera3D Camera3D
        { 
            get { return camera3D; }
            set { camera3D = value; }
        }

        Camera2D camera2D;
        public Camera2D Camera2D
        {
            get { return camera2D; }
            set { camera2D = value; }
        }

        PhysicsSystem physics;
        public PhysicsSystem Physics
        {
            get { return physics; }
            protected set { physics = value; }
        }

        public GameScreen PreviousScreen;
        internal GnomicGame parentGame;
        public GnomicGame ParentGame { get { return parentGame; } }

        public ReadOnlyCollection<Entity> ActiveEntities { get { return activeEntitiesReadOnly; } }
        public ReadOnlyCollection<Entity> AllEntities { get { return allEntitiesReadOnly; } }
        
        protected Action<int> PreDrawGroup;
        protected Action<int> PostDrawGroup;
        
        private Dictionary<int, List<IDrawable3D>> DrawGroups = new Dictionary<int, List<IDrawable3D>>(10);
        private List<IDrawable2D> drawable2DEntities = new List<IDrawable2D>(20);

        // Active entities
        private List<Entity> activeEntities = new List<Entity>(200);
        private ReadOnlyCollection<Entity> allEntitiesReadOnly;
        
        // All entities. Contains pre-initialised triggerable entities
        private List<Entity> allEntities = new List<Entity>(200);
        private ReadOnlyCollection<Entity> activeEntitiesReadOnly;
        //private ReadOnlyCollection<Entity> drawableEntitiesReadOnly;

        private List<Entity> toRemove = new List<Entity>(20);
        private List<Entity> toAdd = new List<Entity>(20);

        public Dictionary<Type, Action<Entity>> EntityAddedCallbacks = new Dictionary<Type,Action<Entity>>();
        public Dictionary<Type, Action<Entity>> EntityRemovedCallbacks = new Dictionary<Type, Action<Entity>>();

        List<EntityCommand> Commands = new List<EntityCommand>(20);

        public bool CanNavigateBack = true;

        protected double totalTime;
        public double TotalTime { get { return totalTime; } }

        protected ContentManager content;
        public ContentManager Content
        {
            get 
            { 
                // Screen can override content manager
                if (content == null)
                {
                    return parentGame.Content;
                }
                return content; 
            }
            set { content = value;}
        }

        public IClickable CurrentClickable;
        public bool ClickEnabled = true;
        
        public string Name { get; set; }
        public bool PassUpdate { get; set; }
        public bool PassDraw { get; set; }
        public bool IsInitialized { get; internal set; }
        public bool IsActivated { get; internal set; }
        public bool HandleInput { get; set; }
        
        public GameScreen()
        {
            PassDraw = true;
            PassUpdate = true;
            activeEntitiesReadOnly = new ReadOnlyCollection<Entity>(activeEntities);
            allEntitiesReadOnly = new ReadOnlyCollection<Entity>(allEntities);
        }

        public virtual object SaveState() { return null; }
        public virtual void RestoreState(object state) { }

        public void AddToAllEntities(Entity e)
        {
            // Entities store their Id in the AllEntities list
            e.Id = allEntities.Count;
            allEntities.Add(e);

            // Allocating at runtime is to be avoided where possible. This
            // call will happen during load/initialisation most of the time.
            // Note that we're not actually inserting the object into the list, 
            // just ensure a list exists for it's requested bucket.
            var drawable3D = e as IDrawable3D;
            if (drawable3D != null)
            {
                // Ensure the group is set up for this IDrawable3D
                List<IDrawable3D> group;
                if (!DrawGroups.TryGetValue(drawable3D.DrawGroup, out group))
                {
                    group = new List<IDrawable3D>();
                    DrawGroups.Add(drawable3D.DrawGroup, group);
                }

                if (renderBatch == null)
                {
                    renderBatch = new RenderBatch(parentGame.GraphicsDevice);
                }
            }
        }

        public virtual void AddEntity(Entity e)
        {
            e.ParentScreen = this;
            if (e.Id < 0)
            {
                AddToAllEntities(e);
            }

            if (!toRemove.Remove(e))
                toAdd.Add(e);
        }

        public virtual void RemoveEntity(Entity e)
        {
            if (!toAdd.Remove(e))
                toRemove.Add(e);
        }

        public void Activate()
        {
            HandleInput = true;
            deactivating = false;

            ParentGame.AddActiveScreen(this.Name);

            if (!this.IsInitialized)
                Initialize(ParentGame);

            IsActivated = true;
            OnActivate();
        }

        public void Activate(int insertPosFromEnd)
        {
            HandleInput = true;
            deactivating = false;

            ParentGame.InsertScreen(this.Name, insertPosFromEnd);

            if (!this.IsInitialized)
                Initialize(ParentGame);

            IsActivated = true;
            OnActivate();

            ClickEnabled = false;
            CurrentClickable = null;
#if WINDOWS_PHONE
            TouchTwinStick.ClearTaps();
#endif
        }

        protected virtual void OnActivate()
        {
        }

        public void Deactivate()
        {
            ParentGame.RemoveActiveScreen(this.Name);
            IsActivated = false;
            PreviousScreen = null;
            OnDeactivate();
        }

        protected virtual void OnDeactivate()
        {
            IsActivated = false;
        }

        protected float deactivateTime = -1.0f;
        protected bool deactivating;
        public virtual void Deactivate(float timer)
        {
            ClickEnabled = false;
            CurrentClickable = null;

            deactivating = true;
            deactivateTime = timer;
            HandleInput = false;
        }


        public Image AddSprite(string textureName, Vector2 size, Vector2 pos, Vector2 pivot, bool visible)
        {
            return AddSprite(textureName, size, pos, pivot, 0f, visible);
        }
        public Image AddSprite(string textureName, Vector2 size, Vector2 pos, Vector2 pivot, bool visible, Vector4 uvRect)
        {
            return AddSprite(textureName, size, pos, pivot, 0f, visible, uvRect, null);
        }

        public Image AddSprite(string textureName, Vector2 size, Vector2 pos, Vector2 pivot, float rotation, bool visible)
        {
            return AddSprite(textureName, size, pos, pivot, rotation, visible, new Vector4(0f, 0f, 1f, 1f), null);
        }
        public Image AddSprite(string textureName, Vector2 size, Vector2 pos, Vector2 pivot, float rotation, bool visible, Vector4 uvRect, string className)
        {
            ImageSettings img = new ImageSettings();
            if (!string.IsNullOrEmpty(className))
                img.EntityClass = className;

            img.TextureName = textureName;
            img.Size = size;
            img.Position = pos;
            img.Pivot = pivot;
            img.Visible = visible;
            img.UVRect = uvRect;
            img.Rotation = rotation;
            Image imgObj = (Image)img.CreateEntity();
            this.AddEntity(imgObj);
            return imgObj;
        }


        public Text AddText(string fontName, string text, Color tint, Vector2 fontHeight, Vector2 pos, bool visible)
        {
            return AddText(fontName, text, tint, fontHeight, pos, Vector2.Zero, HorizontalAlignment.Middle, true, "Gnomic.Entities.TextEntity");
        }
        public Text AddText(string fontName, string text, Color tint, Vector2 fontHeight, Vector2 pos, Vector2 screenSize, HorizontalAlignment alignH, bool visible)
        {
            return AddText(fontName, text, tint, fontHeight, pos, screenSize, HorizontalAlignment.Middle, visible, "Gnomic.Entities.TextEntity");
        }
        public Text AddText(string fontName, string text, Color tint, Vector2 fontHeight, Vector2 pos, Vector2 screenSize, HorizontalAlignment alignH, bool visible, string className)
        {
            TextSettings txt = new TextSettings();
            txt.EntityClass = className;
            txt.FontName = fontName;
            txt.Size = screenSize;
            txt.AlignH = alignH;
            txt.Position = pos;
            txt.FontScreenHeight = fontHeight;
            txt.Visible = visible;
            txt.Text = text;
            txt.Tint = tint;
            Text textObj = (Text)txt.CreateEntity();
            this.AddEntity(textObj);
            return textObj;
        }

        List<Entity> toInit = new List<Entity>(10);
        List<EntityCommand> toHandle = new List<EntityCommand>(10);

        public virtual void Update(float dt)
        {
            totalTime += dt;

            if (physics != null)
            {
                physics.World.Step(dt);
            }

            // Clicking/tapping is disabled when a screen activates
            // This stops IClickables from being clicked by a touch that was started on a different screen
            if (!ClickEnabled && !deactivating)
            {
#if WINDOWS_PHONE
                if (TouchTwinStick.JustTouched)
                    ClickEnabled = true;
#else
                if (Input.MouseJustDown(MouseButton.Left))
                    ClickEnabled = true;
#endif
            }

            ProcessAddRemoves();

            foreach (Entity c in activeEntities)
            {
                if (c.IsEnabled)
                {
                    c.Update(dt);
                }
            }

            ProcessCommands(dt);

            if (deactivating)
            {
                deactivateTime -= dt;
                if (deactivateTime < 0.0f)
                {
                    deactivating = false;
                    Deactivate();
                }
            }
        }
        
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // Draw 3D groups
            if (DrawGroups.Count > 0)
            {
                // Set default states as using spritebatch can bork them
                GraphicsDevice dvc = parentGame.GraphicsDevice;
                dvc.SamplerStates[0] = SamplerState.LinearWrap;
                dvc.RasterizerState = RasterizerState.CullNone;
                dvc.BlendState = BlendState.AlphaBlend;
                dvc.DepthStencilState = DepthStencilState.Default;

                // Draw 3D buckets
                foreach (KeyValuePair<int, List<IDrawable3D>> kvp in DrawGroups)
                {
                    // Call the group pre-draw if it exists
                    if (PreDrawGroup != null)
                    {
                        PreDrawGroup(kvp.Key);
                    }

                    foreach (IDrawable3D drawable3D in kvp.Value)
                    {
                        if (drawable3D.IsVisible)
                        {
                            drawable3D.Draw3D();
                        }
                    }

                    // After the IDrawable3D objects have beend drawn,
                    // call RenderBatch.Render() to flush any added triangles
                    if (renderBatch != null && renderBatch.HasTriangles)
                    {
                        Debug.Assert(camera3D != null);

                        Matrix view = camera3D.MatrixView;
                        Matrix proj = camera3D.MatrixProj;

                        renderBatch.Render(ref renderBatchWorldTransform, ref view, ref proj);
                        renderBatch.Reset();
                    }

                    // Call the group post-draw if it exists
                    if (PostDrawGroup != null)
                    {
                        PostDrawGroup(kvp.Key);
                    }
                }
            }

            // Draw 2D layer with SpriteBatch
            if (drawable2DEntities.Count > 0)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null);
                foreach (IDrawable2D e in drawable2DEntities)
                {
                    if (e.IsVisible) e.Draw2D(spriteBatch);
                }
                spriteBatch.End();
            }
        }

        protected void ProcessCommands(float dt)
        {
            // Apply commands
            if (Commands.Count > 0)
            {
                foreach (EntityCommand cmd in Commands)
                {
                    cmd.TimeLeft -= dt;
                    if (cmd.TimeLeft < 0f)
                    {
                        toHandle.Add(cmd);
                    }
                }

                foreach (EntityCommand cmd in toHandle)
                {
                    if (cmd.CommandType != CommandType.Delegate)
                    {
                        AllEntities[cmd.TargetId].HandleCommand(cmd);
                    }

                    if (cmd.OnExecute != null)
                    {
                        cmd.OnExecute();
                        cmd.OnExecute = null;
                    }

                    cmd.screen = null;
                    EntityCommand.commandCache.Insert(cmd);
                }
                toHandle.Clear();

                for (int i = Commands.Count - 1; i >= 0; i--)
                {
                    if (Commands[i].TimeLeft <= 0f)
                    {
                        Commands.RemoveAt(i);
                    }
                }
            }
        }

        public void ProcessAddRemoves()
        {
            if (toRemove.Count > 0)
            {
                foreach (Entity e in toRemove)
                {
                    Debug.Assert(activeEntities.Contains(e) && e.IsActivated);

                    activeEntities.Remove(e);
                    onEntityRemoved(e);

                    var drawble2D = e as IDrawable2D;
                    if (drawble2D != null)
                    {
                        drawable2DEntities.Remove(drawble2D);
                    }
                    var drawble3D = e as IDrawable3D;
                    if (drawble3D != null)
                    {
                        DrawGroups[drawble3D.DrawGroup].Remove(drawble3D);
                    }
                }
                toRemove.Clear();
            }

            if (toAdd.Count > 0)
            {
                foreach (Entity e in toAdd)
                {
                    Debug.Assert(!activeEntities.Contains(e) && !e.IsActivated);
                    
                    activeEntities.Add(e);
                    onEntityAdded(e);

                    if (this.IsInitialized && !e.IsInitialized)
                        toInit.Add(e);

                    var drawble2D = e as IDrawable2D;
                    if (drawble2D != null)
                    {
                        drawable2DEntities.Add(drawble2D);
                    }
                    var drawble3D = e as IDrawable3D;
                    if (drawble3D != null)
                    {
                        DrawGroups[drawble3D.DrawGroup].Add(drawble3D);
                    }
                }
                toAdd.Clear();

                foreach (Entity c in toInit)
                {
                    c.Initialize(this);
                }
                toInit.Clear();
            }
        }
        
        void onEntityAdded(Entity e)
        {
            Action<Entity> callback;
            if (EntityAddedCallbacks.TryGetValue(e.GetType(), out callback))
            {
                callback(e);
            }
        }
        void onEntityRemoved(Entity e)
        {
            Action<Entity> callback;
            if (EntityRemovedCallbacks.TryGetValue(e.GetType(), out callback))
            {
                callback(e);
            }
        }

        public bool ContainsEntity(Entity e)
        {
            return !toRemove.Contains(e) &&
                    (activeEntities.Contains(e) || toAdd.Contains(e));
        }

        public virtual void Initialize(GnomicGame game)
        {
            this.parentGame = game;
            Debug.Assert(!IsInitialized);

            // Append any entity settings from specified file
            if (!string.IsNullOrEmpty(ObjectSettingsFile))
            {
                var entitySettings = Content.Load<List<EntitySettings>>(ObjectSettingsFile);
                ObjectSettings.AddRange(entitySettings);
            }

            // Create entities from settings
            foreach (EntitySettings cs in ObjectSettings)
            {
                AddEntity(cs.CreateEntity());
            }

            // Initialise created entities
            foreach (Entity e in activeEntities)
            {
                if (!e.IsInitialized)
                {
                    e.Initialize(this);
                }
            }

            // toAdd may contain new entities that were added 
            // during the initialisation of the other entities
            foreach (Entity c in toAdd)
            {
                if (!c.IsInitialized)
                {
                    c.Initialize(this);
                }
            }

            IsInitialized = true;
        }

        public virtual void Deinitialize()
        {
            if (IsInitialized)
            {
                if (IsActivated)
                {
                    OnDeactivate();
                }

                foreach (Entity c in AllEntities)
                {
                    if (c.IsInitialized)
                    {
                        c.Deinitialize();
                    }
                }

                IsInitialized = false;
            }
        }

        protected virtual void NavigateBack()
        {
            if (!CanNavigateBack)
                return;

            Deactivate();
        }

        public T Get<T>(string name) where T : Entity
        {
            foreach (Entity c in activeEntities)
            {
                T testcmp = c as T;
                if (testcmp != null && testcmp.Name == name)
                    return testcmp;
            }
            foreach (Entity c in toAdd)
            {
                T testcmp = c as T;
                if (testcmp != null && testcmp.Name == name)
                    return testcmp;
            }
            return null;
        }
        public T Get<T>() where T : Entity
        {
            foreach (Entity c in activeEntities)
            {
                T testcmp = c as T;
                if (testcmp != null)
                    return testcmp;
            }
            foreach (Entity c in toAdd)
            {
                T testcmp = c as T;
                if (testcmp != null)
                    return testcmp;
            }
            return null;
        }
    }


    //public class DistanceComparer : IComparer<IDrawable3D>
    //{
    //    #region IComparer Members

    //    int IComparer<IDrawable3D>.Compare(IDrawable3D x, IDrawable3D y)
    //    {
    //        float xToCam = (BoundingBoxEx.Middle(x.BoundingBox) - Engine.Instance.Camera.Position).LengthSquared();
    //        float yToCam = (BoundingBoxEx.Middle(y.BoundingBox) - Engine.Instance.Camera.Position).LengthSquared();

    //        if (xToCam < yToCam)
    //            return -1;
    //        else if (xToCam > yToCam)
    //            return 1;
    //        else
    //            return 0;
    //    }

    //    #endregion
    //}
}
