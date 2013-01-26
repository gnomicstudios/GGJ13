using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spineless.Entities;
using Microsoft.Xna.Framework;

namespace Spineless
{
    class UnitManager
    {
        LevelScreen screen;

        Dictionary<UnitType, string> enemyClipNames =
            new Dictionary<UnitType, string>(new UnitTypeComparer());

        Dictionary<UnitType, List<Unit>> enemyLists =
            new Dictionary<UnitType, List<Unit>>(new UnitTypeComparer());

        public List<Unit> ActiveEnemies = new List<Unit>();

        public UnitManager(LevelScreen screen)
        {
            this.screen = screen;

            enemyClipNames[UnitType.Grunt] = "player_player";
            enemyLists[UnitType.Grunt] = new List<Unit>(20);

            enemyClipNames[UnitType.Captain] = "player_player";
            enemyLists[UnitType.Captain] = new List<Unit>(20);

            enemyClipNames[UnitType.Boss] = "player_player";
            enemyLists[UnitType.Boss] = new List<Unit>(20);

            for (int i = 0; i < 10; ++i)
            {
                Create(UnitType.Grunt);
                Create(UnitType.Captain);
                Create(UnitType.Boss);
            }
        }

        public Unit AddUnitToScene(UnitType et, Vector2 pos)
        {
            List<Unit> enemies = enemyLists[et];
            foreach (Unit e in enemies)
            {
                if (!e.IsAdded)
                {
                    e.IsAdded = true;
                    e.Physics.Position = pos;
                    e.Physics.Enabled = true;
                    screen.AddEntity(e);
                    return e;
                }
            }
            return null;
        }

        Unit Create(UnitType et)
        {
            SpinelessEntitySettings es = new SpinelessEntitySettings();
            es.EntityClass = "Spineless.Entities.Unit,Spineless";
            es.DefaultAnimName = "run-left";
            es.Scale = new Vector2(0.5f, 0.5f);
            es.ClipFile = enemyClipNames[et];
            es.Physics = new SpinelessPhysicsSettings();
            es.Physics.Width = 2f;
            es.Physics.Height = 2.75f;
            es.ActivateByDefault = false;
            var e = (Unit)es.CreateEntity();
            e.UnitType = et;
            e.Activated += UnitActivated;
            e.Deactivated += UnitDeactivated;
            e.Initialize(screen);
            
            enemyLists[et].Add(e);
            return e;
        }

        void UnitActivated(Gnomic.Entities.Entity ent)
        {
            ActiveEnemies.Add((Unit)ent);
        }


        void UnitDeactivated(Gnomic.Entities.Entity ent)
        {
            var e =(Unit)ent;
            ActiveEnemies.Remove(e);
            e.IsAdded = false;
        }
    }

    class UnitTypeComparer : IEqualityComparer<UnitType>
    {
        public bool Equals(UnitType x, UnitType y)
        {
 	        return x == y;
        }

        public int GetHashCode(UnitType obj)
        {
            return ((int)obj).GetHashCode();
        }
    }
}
