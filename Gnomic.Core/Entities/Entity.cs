using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gnomic;

namespace Gnomic.Entities
{
    public class Entity
    {
        string name;
        public string Name 
        { 
            get { return name; } 
            set { name = value; } 
        }

        // index into game screen's loaded objects array
        public int Id = -1;

        public object Tag;

        public EntitySettings Settings = new EntitySettings();

        /// <summary>
        /// Attempt to cast and set a particular EntitySettings instance to the given type. 
        /// Throw readable exception if it fails.
        /// </summary>
        protected T CastSettings<T>(EntitySettings sourceSettings) where T : EntitySettings
        {
            T targetSettings = sourceSettings as T;
            if (targetSettings == null)
            {
                throw new InvalidOperationException(
                    string.Format("Invalid settings type '{0}'. Expected '{1}' for EntityClass '{2}'. Check your Xml config files.",
                        sourceSettings.GetType().FullName,
                        typeof(T).FullName,
                        sourceSettings.EntityClass));
            }
            return targetSettings;
        }

        protected GameScreen parentScreen;
        public GameScreen ParentScreen
        {
            get { return parentScreen; }
            internal set { parentScreen = value; }
        }
        
        protected bool isEnabled = true;
        public bool IsEnabled 
        { 
            get { return isEnabled;}
            set { isEnabled = value; }
        }

        protected bool isActivated = false;
        public bool IsActivated
        {
            get { return isActivated; }
        }

        public event Action<Entity> Activated;
        public event Action<Entity> Deactivated;

        public Entity() { }
        
        public virtual void ApplySettings(EntitySettings settings)
        {
            this.Settings = settings;
            this.name = settings.Name;
        }

        public void Activate()
        {
            Debug.Assert(!isActivated);
            OnActivate();
        }
        protected virtual void OnActivate()
        {
            parentScreen.AddEntity(this);

            if (!isInitialized)
                Initialize(ParentScreen);

            countingDownToDeactivate = false;
            isActivated = true;

            if (Activated != null)
                Activated(this);
        }

        public void Deactivate()
        {
            Debug.Assert(isActivated);
            OnDeactivate();
        }
        protected virtual void OnDeactivate()
        {
            isActivated = false;
            Debug.Assert(ParentScreen.ContainsEntity(this));
            ParentScreen.RemoveEntity(this);

            if (Deactivated != null)
                Deactivated(this);
        }

        protected bool countingDownToDeactivate = false;
        public bool IsCountingDownToDeactivate
        {
            get { return countingDownToDeactivate; }
        }

        float timeUntilDeactivate;
        public float TimeUntilDeactivate 
        { 
            get { return timeUntilDeactivate; } 
			set 
            { 
                timeUntilDeactivate = value;
                countingDownToDeactivate = timeUntilDeactivate >= 0.0f;
            } 
        }

        public void Deactivate(float countDown)
        {
            System.Diagnostics.Debug.Assert(isActivated);
            countingDownToDeactivate = true;
            timeUntilDeactivate = countDown;
        }

        bool isInitialized = false;
        public bool IsInitialized 
        {
            get { return isInitialized; } 
            set { isInitialized = value; } 
        }

        public virtual void Initialize(GameScreen parentScreen) 
        {
            System.Diagnostics.Debug.Assert(!isInitialized);
            this.parentScreen = parentScreen;
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
            isInitialized = true;
        }

        public virtual void Deinitialize() 
        {
            System.Diagnostics.Debug.Assert(isInitialized);
            OnDeinitialize();
        }

        protected virtual void OnDeinitialize()
        {
            isInitialized = false;
            if (isActivated)
                Deactivate();

            OnDeinitialize();
        }

        public virtual void Update(float dt) 
        {
            if (countingDownToDeactivate)
            {
                timeUntilDeactivate -= dt;
                if (timeUntilDeactivate < 0f)
                {
                    countingDownToDeactivate = false;
                    OnDeactivate();
                }
            }
        }

        public virtual void HandleCommand(EntityCommand command)
        {
            switch (command.CommandType)
            {
                case CommandType.Activate:
                    Activate();
                    break;
                case CommandType.Deactivate:
                    Deactivate();
                    break;
                case CommandType.Enable:
                    this.IsEnabled = true;
                    break;
                case CommandType.Disable:
                    this.IsEnabled = false;
                    break;
                case CommandType.ScheduleDeactivate:
                    Deactivate((float)command.args[0]);
                    break;
            }
        }
    }
}
