using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spineless.Entities;
using Microsoft.Xna.Framework;

using Spineless.AI;
using FarseerPhysics.Dynamics;
using Gnomic.Physics;

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

        const int NUM_LANES = 3;
        const float LANE_OFFSET = 0.15f;
        const float LANE_START  = -0.15f;

        Dictionary<int, float> lanes = new Dictionary<int, float>();
        Dictionary<int, Category> laneCategories = new Dictionary<int, Category>();
        Random random = new Random();
        
        const float KNIGHT_SPAWN_ACCELERATION = 0.02f;
        const float KNIGHT_SPAWN_RATE_DEFAULT = 0.2f;
        
        const float ENEMY_SPAWN_ACCELERATION = 0.05f;
        const float ENEMY_SPAWN_RATE_DEFAULT = 0.8f;


        float spawnRateEnemy = ENEMY_SPAWN_RATE_DEFAULT;
        float spawnRateKnight = KNIGHT_SPAWN_RATE_DEFAULT;

        float spawnIntervalEnemy = 1f / ENEMY_SPAWN_RATE_DEFAULT;
        float spawnIntervalKnight = 1f / ENEMY_SPAWN_RATE_DEFAULT;

        double lastSpawnTimeEnemy = 3.0f;
        double lastSpawnTimeKnight = 4.0f;

        public void Update(float dt)
        {
            spawnRateEnemy += dt * ENEMY_SPAWN_ACCELERATION;
            if (screen.TotalTime - lastSpawnTimeEnemy > spawnIntervalEnemy)
            {
                // spawn enemy
                AddUnitToScene(UnitType.Grunt, screen.Vehicle.Physics.Bodies[0].Position + new Vector2(ConvertUnits.ToSimUnits(screen.ParentGame.ScreenWidth), 0.0f));
                lastSpawnTimeEnemy = screen.TotalTime;
                spawnIntervalEnemy = 1f / spawnRateEnemy;
            }

            spawnRateKnight += dt * KNIGHT_SPAWN_ACCELERATION;
            if (screen.TotalTime - lastSpawnTimeKnight > spawnIntervalKnight)
            {
                // spawn Knight
                AddUnitToScene(UnitType.Knight, screen.Vehicle.Physics.Bodies[0].Position - new Vector2(ConvertUnits.ToSimUnits(screen.ParentGame.ScreenWidth * 0.3f), 0.0f));
                lastSpawnTimeKnight = screen.TotalTime;
                spawnIntervalKnight = 1f / spawnRateKnight;
            }
        }

        public UnitManager(LevelScreen screen)
        {
            this.screen = screen;
            behaviours  = new BehaviourManager();

            Settings unitSettings = null;

            {
                unitSettings = new Settings {
                    Health=100, Damage=30, AttackInterval=0.1f, Speed=4000
                };

                enemyClipNames[UnitType.Grunt] = "enemy";
                settings[UnitType.Grunt] = unitSettings;
                unitLists[UnitType.Grunt] = new List<Unit>(20);                
            }

            {
                unitSettings = new Settings {
                    Health=100, Damage=30, AttackInterval=0.1f, Speed=4000
                };

                enemyClipNames[UnitType.Captain] = "enemy";
                settings[UnitType.Captain] = unitSettings;
                unitLists[UnitType.Captain] = new List<Unit>(20);
            }

            {
                unitSettings = new Settings {
                    Health=100, Damage=30, AttackInterval=0.1f, Speed=4000
                };

                enemyClipNames[UnitType.Boss] = "enemy";
                settings[UnitType.Boss] = unitSettings;
                unitLists[UnitType.Boss] = new List<Unit>(20);                
            }

            {
                unitSettings = new Settings {
                    Health=200, Damage=40, AttackInterval=0.1f, Speed=4000
                };

                enemyClipNames[UnitType.Knight] = "knight";
                settings[UnitType.Knight] = unitSettings;
                unitLists[UnitType.Knight] = new List<Unit>(20);
            }

            for (int i = 0; i < NUM_LANES; ++i)
            {
                lanes[i] = LANE_START + (i * LANE_OFFSET);
            }
            laneCategories[0] = (Category)SpinelessCollisionCategories.Lane1;
            laneCategories[1] = (Category)SpinelessCollisionCategories.Lane2;
            laneCategories[2] = (Category)SpinelessCollisionCategories.Lane3;

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
                    e.LaneId = random.Next(NUM_LANES);
                    e.ClipInstance.Origin = new Vector2(e.ClipInstance.Origin.X, lanes[e.LaneId] * 50.0f);
                    e.Health = settings[et].Health;
                    e.IsAdded = true;
                    Category cat = (Category)(et == UnitType.Knight ? SpinelessCollisionCategories.Knight : SpinelessCollisionCategories.Enemy);
                    Category extraCat = (Category)(et == UnitType.Knight ? SpinelessCollisionCategories.Enemy : (SpinelessCollisionCategories.Siege | SpinelessCollisionCategories.Knight));
                    Category collidesWithCat = (Category)(SpinelessCollisionCategories.Terrain | SpinelessCollisionCategories.AllProjectiles);

                    e.Physics.Bodies[0].CollisionCategories = laneCategories[e.LaneId] & cat;
                    e.Physics.Bodies[0].CollidesWith = laneCategories[e.LaneId] | collidesWithCat | extraCat;

                    if (pos.X < 1.0f)
                        pos.X = 1.0f;

                    e.Physics.Position = ConvertUnits.ToDisplayUnits(pos);
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
            es.Physics.Offset = new Vector2(0.0f, -0.5f);
            es.Physics.Density = 2.0f;
            es.ActivateByDefault = false;
            var e = (Unit)es.CreateEntity();
            e.UnitType = et;
            e.Activated += UnitActivated;
            e.Deactivated += UnitDeactivated;
            e.Initialize(screen);
            

            e.Physics.Bodies[0].Friction = 0.1f;
            e.Physics.Bodies[0].FixtureList[0].UserData = e;

            e.UnitManager = this;
            //e.Behaviour   = behaviours.Create<IBaseUnit>(et);

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

        public Unit NearestEnemy(Unit testUnit, out float distance)
        {
            Unit nearestUnit = null;
            float nearestDist = Single.MaxValue;

            foreach (var kvp in unitLists)
            {
                // ignore units on same team
                bool isEnemyList = kvp.Key != UnitType.Knight;
                if (testUnit.IsEnemy == isEnemyList)
                    continue;

                foreach (Unit unit in kvp.Value)
                {
                    if (!unit.IsAlive)
                        continue;

                    float dist = (unit.Position - testUnit.Position).Length();

                    if (dist < nearestDist || nearestUnit == null)
                    {
                        nearestUnit = unit;
                        nearestDist = dist;
                    }
                }
            }
            distance = nearestDist;
            return nearestUnit;
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
