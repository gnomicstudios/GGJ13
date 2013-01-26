using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spineless.Entities;
using Microsoft.Xna.Framework;

using Spineless.AI;

namespace Spineless
{
    public class UnitManager
    {
        LevelScreen screen;
        BehaviourManager behaviours;

        Dictionary<UnitType, string> enemyClipNames =
            new Dictionary<UnitType, string>(new UnitTypeComparer());

        Dictionary<UnitType, List<Unit>> unitLists =
            new Dictionary<UnitType, List<Unit>>(new UnitTypeComparer());

        public List<Unit> ActiveEnemies = new List<Unit>();

        public UnitManager(LevelScreen screen)
        {
            this.screen = screen;
            behaviours  = new BehaviourManager();

            enemyClipNames[UnitType.Grunt] = "enemy";
            unitLists[UnitType.Grunt] = new List<Unit>(20);

            enemyClipNames[UnitType.Captain] = "enemy";
            unitLists[UnitType.Captain] = new List<Unit>(20);

            enemyClipNames[UnitType.Boss] = "enemy";
            unitLists[UnitType.Boss] = new List<Unit>(20);

            enemyClipNames[UnitType.Knight] = "knight";
            unitLists[UnitType.Knight] = new List<Unit>(20);

            for (int i = 0; i < 10; ++i)
            {
                Create(UnitType.Grunt);
                Create(UnitType.Captain);
                Create(UnitType.Boss);
                Create(UnitType.Knight);
            }
        }

        public Unit AddUnitToScene(UnitType et, Vector2 pos)
        {
            List<Unit> enemies = unitLists[et];
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
            es.DefaultAnimName = "walk";
            es.ClipFile = enemyClipNames[et];
            es.Physics = new SpinelessPhysicsSettings();
            es.Physics.Width = 0.6f;
            es.Physics.Height = 1f;
            es.Physics.Offset = new Vector2(0.0f, -0.5f);
            es.ActivateByDefault = false;
            var e = (Unit)es.CreateEntity();
            e.UnitType = et;
            e.Activated += UnitActivated;
            e.Deactivated += UnitDeactivated;
            e.Initialize(screen);

            e.UnitManager = this;
            e.Behaviour   = behaviours.Create<IBaseUnit>(et);
            
            unitLists[et].Add(e);
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

        public Unit FindNearestTypeTo(UnitType type, Unit start)
        {
            Unit nearestUnit = null;
            float nearestDist = Single.MaxValue;

            foreach (Unit unit in unitLists[type])
            {
                if (start == unit)
                    continue;

                float dist = (unit.Position - start.Position).Length();

                if (dist < nearestDist || nearestUnit == null)
                {
                    nearestUnit = unit;
                    nearestDist = dist;
                }
            }

            return nearestUnit;
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
