using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spineless.Entities;
using Microsoft.Xna.Framework;

namespace Spineless
{
    class EnemyManager
    {
        LevelScreen screen;

        Dictionary<EnemyType, string> enemyClipNames =
            new Dictionary<EnemyType, string>(new EnemyTypeComparer());

        Dictionary<EnemyType, List<Enemy>> enemyLists =
            new Dictionary<EnemyType, List<Enemy>>(new EnemyTypeComparer());

        public List<Enemy> ActiveEnemies = new List<Enemy>();

        public EnemyManager(LevelScreen screen)
        {
            this.screen = screen;

            enemyClipNames[EnemyType.Grunt] = "player_player";
            enemyLists[EnemyType.Grunt] = new List<Enemy>(20);

            enemyClipNames[EnemyType.Captain] = "player_player";
            enemyLists[EnemyType.Captain] = new List<Enemy>(20);

            enemyClipNames[EnemyType.Boss] = "player_player";
            enemyLists[EnemyType.Boss] = new List<Enemy>(20);

            for (int i = 0; i < 10; ++i)
            {
                Create(EnemyType.Grunt);
                Create(EnemyType.Captain);
                Create(EnemyType.Boss);
            }
        }

        public Enemy AddEnemyToScene(EnemyType et, Vector2 pos)
        {
            List<Enemy> enemies = enemyLists[et];
            foreach (Enemy e in enemies)
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

        Enemy Create(EnemyType et)
        {
            SpinelessEntitySettings es = new SpinelessEntitySettings();
            es.EntityClass = "Spineless.Entities.Enemy,Spineless";
            es.DefaultAnimName = "run-left";
            es.Scale = new Vector2(0.5f, 0.5f);
            es.ClipFile = enemyClipNames[et];
            es.Physics = new SpinelessPhysicsSettings();
            es.Physics.Width = 2f;
            es.Physics.Height = 2.75f;
            es.ActivateByDefault = false;
            var e = (Enemy)es.CreateEntity();
            e.EnemyType = et;
            e.Activated += EnemyActivated;
            e.Deactivated += EnemyDeactivated;
            e.Initialize(screen);
            
            enemyLists[et].Add(e);
            return e;
        }

        void EnemyActivated(Gnomic.Entities.Entity ent)
        {
            ActiveEnemies.Add((Enemy)ent);
        }


        void EnemyDeactivated(Gnomic.Entities.Entity ent)
        {
            var e =(Enemy)ent;
            ActiveEnemies.Remove(e);
            e.IsAdded = false;
        }
    }

    class EnemyTypeComparer : IEqualityComparer<EnemyType>
    {
        public bool Equals(EnemyType x, EnemyType y)
        {
 	        return x == y;
        }

        public int GetHashCode(EnemyType obj)
        {
            return ((int)obj).GetHashCode();
        }
    }
}
