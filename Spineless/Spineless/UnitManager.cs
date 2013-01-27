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

        public Dictionary<UnitType, List<Unit>> UnitLists { get { return unitLists; } }

        public List<Unit> ActiveUnits = new List<Unit>();

        class Settings
        {
            public float Health;
            public float Damage;
            public float AttackInterval;
            public float Speed;
        }

        Dictionary<UnitType, Settings> settings =
            new Dictionary<UnitType, Settings>();

        const int NUM_LANES = 5;
        const float LANE_OFFSET = 0.1f;
        const float LANE_START  = -0.25f;

        Dictionary<int, float> lanes = new Dictionary<int, float>();
        Random random = new Random();

        public UnitManager(LevelScreen screen)
        {
            this.screen = screen;
            behaviours  = new BehaviourManager();

            Settings unitSettings = null;

            {
                unitSettings = new Settings {
                    Health=100, Damage=10, AttackInterval=0.1f, Speed=1000
                };

                enemyClipNames[UnitType.Grunt] = "enemy";
                settings[UnitType.Grunt] = unitSettings;
                unitLists[UnitType.Grunt] = new List<Unit>(20);                
            }

            {
                unitSettings = new Settings {
                    Health=100, Damage=10, AttackInterval=0.1f, Speed=1000
                };

                enemyClipNames[UnitType.Captain] = "enemy";
                settings[UnitType.Captain] = unitSettings;
                unitLists[UnitType.Captain] = new List<Unit>(20);
            }

            {
                unitSettings = new Settings {
                    Health=100, Damage=10, AttackInterval=0.1f, Speed=1000
                };

                enemyClipNames[UnitType.Boss] = "enemy";
                settings[UnitType.Boss] = unitSettings;
                unitLists[UnitType.Boss] = new List<Unit>(20);                
            }

            {
                unitSettings = new Settings {
                    Health=100000000, Damage=10, AttackInterval=0.1f, Speed=1000
                };

                enemyClipNames[UnitType.Knight] = "knight";
                settings[UnitType.Knight] = unitSettings;
                unitLists[UnitType.Knight] = new List<Unit>(20);
            }

            for (int i = 0; i < NUM_LANES; ++i)
            {
                lanes[i] = LANE_START + (i * LANE_OFFSET);
            }

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
                    e.Activate();
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
            es.Physics.Offset = new Vector2(0.0f, RandomLaneOffset());
            es.Physics.Density = 2.0f;
            es.ActivateByDefault = false;
            var e = (Unit)es.CreateEntity();
            e.UnitType = et;
            e.Activated += UnitActivated;
            e.Deactivated += UnitDeactivated;
            e.Initialize(screen);

            e.UnitManager = this;
            e.Behaviour   = behaviours.Create<IBaseUnit>(et);

            var unitSettings = settings[et];

            e.Health         = unitSettings.Health;
            e.Speed          = unitSettings.Speed;
            e.Damage         = unitSettings.Damage;
            e.AttackInterval = unitSettings.AttackInterval;
            
            unitLists[et].Add(e);
            return e;
        }

        float RandomLaneOffset()
        {
            return lanes[random.Next(0, NUM_LANES-1)];
        }

        void UnitActivated(Gnomic.Entities.Entity ent)
        {
            ActiveUnits.Add((Unit)ent);
        }

        void UnitDeactivated(Gnomic.Entities.Entity ent)
        {
            var e =(Unit)ent;
            ActiveUnits.Remove(e);
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
