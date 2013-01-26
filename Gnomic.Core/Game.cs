using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Gnomic.Audio;

namespace Gnomic
{
    public class GnomicGame
    {
        public delegate void ScreenSizeChanged(int x, int y);

        struct ScreenToInsert
        {
            public int PositionFromEnd;
            public GameScreen Screen;
        }

        public GameServiceContainer Services = new GameServiceContainer();
        
        protected AudioManager audio;
        public AudioManager Audio
        {
            get { return audio; }
        }
        protected ContentManager content;
        public ContentManager Content 
        { 
            get { return content; } 
        }

        protected SpriteBatch spriteBatch;
        protected GraphicsDevice graphicsDevice;
        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsDevice; }
        }

        public int GarbageCollectCounter = -1;

        public Dictionary<string, GameScreen> allScreens = new Dictionary<string, GameScreen>();
        public List<GameScreen> ActiveScreens = new List<GameScreen>();
        public int ActiveScreenCount
        {
            get { return ActiveScreens.Count - toPop.Count; }
        }

        public System.Resources.ResourceManager Resources { get; set; }

        private List<GameScreen> toPush = new List<GameScreen>(4);
        private List<ScreenToInsert> toInsert = new List<ScreenToInsert>(4);
        private List<GameScreen> toPop = new List<GameScreen>(4);


        public int ScreenWidth
        {
            get { return graphicsDevice.Viewport.Width; }
        }
        public int ScreenHeight
        {
            get { return graphicsDevice.Viewport.Height; }
        }

        public int BackBufferWidth
        {
            get { return graphicsDevice.PresentationParameters.BackBufferWidth; }
        }
        public int BackBufferHeight
        {
            get { return graphicsDevice.PresentationParameters.BackBufferHeight; }
        }

        public double TotalTime = 0;

        public ScreenSizeChanged OnScreenSizeChanged;
        public void TriggerScreenSizeChange()
        {
            if (OnScreenSizeChanged != null)
            {
                OnScreenSizeChanged(ScreenWidth, ScreenHeight);
            }
        }


        public virtual void Initialize(IGraphicsDeviceService deviceSvc)
        {
            graphicsDevice = deviceSvc.GraphicsDevice;
            Services.AddService(typeof(IGraphicsDeviceService), deviceSvc);
            deviceSvc.DeviceReset += new EventHandler<EventArgs>(deviceSvc_DeviceReset);

            content = new Gnomic.Core.ContentTracker(Services);
            Content.RootDirectory = "Content";
#if WINDOWS
            ((Gnomic.Core.ContentTracker)content).UseSourceAssets = true;
#endif
            
            audio = new AudioManager(Content);

            spriteBatch = new SpriteBatch(graphicsDevice);

            Input.Initialize(graphicsDevice);

            foreach (GameScreen gs in this.ActiveScreens)
            {
                if (!gs.IsInitialized)
                    gs.Initialize(this);
            }
        }
        
        public virtual void Update(float dt)
        {
            TotalTime += dt;

            Input.Update(dt);

            ProcessAddRemoveInserts();

            // Update from most recent screen
            // back until it is blocked
            int i = ActiveScreens.Count - 1;
            while (i > 0)
            {
                if (ActiveScreens[i].PassUpdate)
                    i--;
                else
                    break;
            }

            for (i = Math.Max(0, i); i < ActiveScreens.Count; i++)
            {
                ActiveScreens[i].Update(dt);
            }

            Audio.Update(dt);
            
            // Allow garbage to be collected after a given number of updates
            if (GarbageCollectCounter > 0)
            {
                GarbageCollectCounter--;
                if (GarbageCollectCounter == 0)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }

        public virtual void Draw()
        {
            // Determine last screen
            // that doesn't pass draw
            int i = ActiveScreens.Count - 1;
            while (i > 0)
            {
                if (ActiveScreens[i].PassDraw)
                    i--;
                else
                    break;
            }

            for (i = Math.Max(0, i); i < ActiveScreens.Count; i++)
            {
                ActiveScreens[i].Draw(spriteBatch);
            }
        }
        void deviceSvc_DeviceReset(object sender, EventArgs e)
        {
            if (OnScreenSizeChanged != null)
                OnScreenSizeChanged(ScreenWidth, ScreenHeight);
        }

        public void AddScreen(string name, GameScreen screen)
        {
            screen.Name = name;
            screen.parentGame = this;
            allScreens[name] = screen;
        }

        public void RemoveScreen(string name)
        {
            GameScreen gs;
            if (allScreens.TryGetValue(name, out gs))
            {
                if (gs.IsActivated)
                {
                    gs.Deactivate();
                }

                if (gs.IsInitialized)
                {
                    gs.Deinitialize();
                }

                allScreens.Remove(name);
            }
        }

        public GameScreen GetScreen(string key)
        {
            if (allScreens.ContainsKey(key))
                return allScreens[key];

            return null;
        }

        public GameScreen GetTopScreen()
        {
            if (ActiveScreens.Count - 1 - toPop.Count >= ActiveScreens.Count)
            {
                return null;
            }
            if (ActiveScreens.Count > 0)
                return ActiveScreens[ActiveScreens.Count - 1 - toPop.Count];
            else
                return null;
        }

        public void AddActiveScreen(string screenName)
        {
            if (allScreens.ContainsKey(screenName))
            {
                GameScreen gs = allScreens[screenName];
                if (!ActiveScreens.Contains(gs) || toPop.Contains(gs))
                    toPush.Add(gs);
            }
        }

        public void InsertScreen(string screenName, int posFromEnd)
        {
            if (allScreens.ContainsKey(screenName))
            {
                GameScreen gs = allScreens[screenName];
                if (!ActiveScreens.Contains(gs))
                {
                    ScreenToInsert inserter = new ScreenToInsert();
                    inserter.PositionFromEnd = posFromEnd;
                    inserter.Screen = gs;
                    toInsert.Add(inserter);
                }
            }
        }
        public void RemoveActiveScreen(string screenName)
        {
            if (allScreens.ContainsKey(screenName))
            {
                GameScreen scrn = allScreens[screenName];
                scrn.IsActivated = false;
                if (ActiveScreens.Contains(scrn))
                {
                    // Should never pop the same screen twice!
                    System.Diagnostics.Debug.Assert(!toPop.Contains(scrn));

                    toPop.Add(scrn);
                }
            }
        }

        public void ProcessAddRemoveInserts()
        {

            // Remove outgoing screens
            if (toPop.Count > 0)
            {
                for (int i = 0; i < toPop.Count; i++)
                {
                    ActiveScreens.Remove(toPop[i]);
                    toPop[i].IsActivated = false;
                }
                toPop.Clear();
            }

            // Add new screens
            if (toPush.Count > 0)
            {
                for (int i = 0; i < toPush.Count; i++)
                {
                    // Assign parent screen
                    GameScreen scrn = toPush[i];
                    //scrn.ParentScreen = (ActiveScreens.Count == 0) ?
                    //                         null : ActiveScreens[ActiveScreens.Count - 1];

                    ActiveScreens.Add(scrn);

                    if (!scrn.IsInitialized)
                    {
                        scrn.Initialize(this);
                    }

                    scrn.IsActivated = true;

                }
                toPush.Clear();
            }

            // Insert new screens
            if (toInsert.Count > 0)
            {
                for (int i = 0; i < toInsert.Count; i++)
                {
                    ScreenToInsert inserter = toInsert[i];
                    if (inserter.PositionFromEnd > ActiveScreens.Count)
                    {
                        throw new InvalidOperationException(string.Format("Tried to insert screen '{0}' {1} from end when only {2} screens are active", inserter.Screen.Name, inserter.PositionFromEnd, ActiveScreens.Count));
                        //continue;
                    }

                    // Assign parent screen
                    int insertPos = ActiveScreens.Count - inserter.PositionFromEnd;
                    //if (insertPos > 0)
                    //    inserter.ParentScreen = ActiveScreens[insertPos - 1];

                    // Insert the screen
                    if (insertPos < ActiveScreens.Count)
                        ActiveScreens.Insert(insertPos, inserter.Screen);
                    else
                        ActiveScreens.Add(inserter.Screen);

                    // Ensure initialized
                    if (!inserter.Screen.IsInitialized)
                    {
                        inserter.Screen.Initialize(this);
                    }

                    inserter.Screen.IsActivated = true;
                }
                toInsert.Clear();
            }
        }

        public T GetScreen<T>(string name) where T : GameScreen
        {
            GameScreen returnScreen;
            if (allScreens.TryGetValue(name, out returnScreen))
                return (T)returnScreen;
            else
                return null;
        }

        public T GetScreen<T>() where T : GameScreen
        {
            T returnScreen = null;
            foreach (GameScreen gs in allScreens.Values)
            {
                returnScreen = gs as T;
                if (returnScreen != null)
                    break;
            }
            return returnScreen;
        }

        public T GetActiveScreen<T>() where T : GameScreen
        {
            T returnScreen = null;
            foreach (GameScreen gs in ActiveScreens)
            {
                returnScreen = gs as T;
                if (returnScreen != null)
                    break;
            }
            return returnScreen;
        }
    }
}
