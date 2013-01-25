using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;
using System.Xml.Serialization;
using Gnomic.Util;

namespace Gnomic.Entities
{
    public class EntitySettings
    {
        [ContentSerializer(Optional = true)]
        public string EntityClass;
        [ContentSerializer(Optional = true)]
        public string Name;
        [ContentSerializer(Optional = true)]
        public bool ActivateByDefault = true;
        
        [ContentSerializerIgnore]
        public Entity CreatedEntity { get; private set; }

        public Entity CreateEntity()
        {
            Entity inst = null;
            
            if (inst == null)
            {
                Type entityType = Type.GetType(this.EntityClass);
                inst = (Entity)Activator.CreateInstance(entityType);
            }
            
            inst.ApplySettings(this);
            CreatedEntity = inst;
            return inst;
        }
    }
}
