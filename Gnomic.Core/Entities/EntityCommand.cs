using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Gnomic;
using Gnomic.Util;
using Gnomic.Entities;

namespace Gnomic.Entities
{
    /// <summary>
    /// Command types for entities to execute
    /// </summary>
    public enum CommandType
    {
        None,
        Activate,
        Deactivate,
        Enable,
        Disable,
        ScheduleDeactivate,
        SetState,
        Delegate
    }

    public class EntityCommand
    {
        public delegate void Execute();

        [ContentSerializer(Optional = true)]
        public CommandType CommandType;

        [ContentSerializer(Optional = true)]
        public int CommandTypeCustom = -1;

        [ContentSerializer(Optional = true)]
        public string TargetName;

        [ContentSerializer(Optional = true)]
        public float TimeLeft;

        [ContentSerializer(Optional=true)]
        public object[] args;

        [ContentSerializerIgnore()]
        public int TargetId;
        
        [ContentSerializerIgnore()]
        public float DelayTime;

        [ContentSerializerIgnore()]
        public Execute OnExecute;

        internal GameScreen screen;

        public void Initialize(GameScreen screen)
        {
            this.screen = screen;
            if (!string.IsNullOrEmpty(TargetName))
            {
                TargetId = -1;
                for (int i = 0; i < screen.AllEntities.Count; ++i)
                {
                    if (screen.AllEntities[i].Name == TargetName)
                    {
                        TargetId = i;
                        break;
                    }
                }

#if DEBUG
                if (TargetId < 0)
                    throw new ArgumentException(string.Format("Command could not find Entity with name '{0}'", TargetName));

                int count = 0;
                foreach (Entity e in screen.AllEntities)
                {
                    if (e.Name == TargetName)
                        count++;
                }
                if (count > 1)
                    throw new ArgumentException(string.Format("Command detected multiple target entities with name '{0}'", TargetName));
#endif
            }

            TimeLeft = DelayTime;
        }
        
        bool NameIsTargetName(Entity c)
        {
            return c.Name == TargetName;
        }

        public static EntityCommand CreateCommand(GameScreen screen, string targetName, CommandType command, object[] args)
        {
            return EntityCommand.CreateCommand(screen, targetName, command, args, 0f);
        }
        public static EntityCommand CreateCommand(GameScreen screen, string targetName, CommandType command, object[] args, float delayTime)
        {
            EntityCommand cc = new EntityCommand();
            cc.TargetName = targetName;
            cc.Initialize(screen);
            cc.CommandType = command;
            cc.args = args;
            cc.TimeLeft = delayTime;
            cc.DelayTime = delayTime;

            return cc;
        }

        internal static Cache<EntityCommand> commandCache = new Cache<EntityCommand>();

        public static EntityCommand CreateCommandRuntime(GameScreen screen, int targetId, CommandType command, object[] args, float delayTime)
        {
            EntityCommand cc = null;
            if (commandCache.Count > 0)
                cc = commandCache.Fetch();
            else
                cc = new EntityCommand();

            cc.TargetName = "";
            cc.screen = screen;
            cc.TargetId = targetId;
            cc.CommandType = command;
            cc.args = args;
            cc.TimeLeft = delayTime;

            return cc;
        }
    }
}
