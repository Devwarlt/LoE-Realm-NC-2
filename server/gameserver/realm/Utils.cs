#region

using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer.realm
{
    internal static class EntityUtils
    {
        public static double DistSqr(this Entity a, Entity b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return dx * dx + dy * dy;
        }

        public static double Dist(this Entity a, Entity b)
        {
            return Math.Sqrt(a.DistSqr(b));
        }

        public static bool AnyPlayerNearby(this Entity entity)
        {
            foreach (Player i in entity.Owner.PlayersCollision.HitTest(entity.X, entity.Y, 16).OfType<Player>())
            {
                double d = i.Dist(entity);
                if (d < 16 * 16)
                    return true;
            }
            return false;
        }

        public static bool AnyPlayerNearby(this World world, double x, double y)
        {
            foreach (Player i in world.PlayersCollision.HitTest(x, y, 16).OfType<Player>())
            {
                double d = MathsUtils.Dist(i.X, i.Y, x, y);
                if (d < 16 * 16)
                    return true;
            }
            return false;
        }

        public static Entity GetNearestEntity(this Entity entity, double dist, ushort? objType) //Null for player
        {
            Entity[] entities = entity.GetNearestEntities(dist, objType).ToArray();
            if (entities.Length <= 0)
                return null;
            return entities.Aggregate((curmin, x) => (curmin == null || x.DistSqr(entity) < curmin.DistSqr(entity) ? x : curmin));
        }

        public static Entity GetEntity(this Entity entity, int entityId)
        {
            return entity.Owner.GetEntity(entityId);
        }

        /// <summary>
        /// Only for enemys
        /// </summary>
        public static IEnumerable<Entity> GetNearestEntities(this Entity entity, double dist)
        {
            if (entity.Owner == null)
                yield break;
            foreach (Entity i in entity.Owner.EnemiesCollision.HitTest(entity.X, entity.Y, (float)dist))
            {
                double d = i.Dist(entity);
                if (d < dist)
                    yield return i;
            }
        }

        public static IEnumerable<Entity> GetNearestEntities(this Entity entity, double dist, ushort? objType, bool pets = false)
        //Null for player
        {
            if (entity.Owner == null)
                yield break;
            if (objType == null)
                foreach (Entity i in entity.Owner.PlayersCollision.HitTest(entity.X, entity.Y, (float)dist))
                {
                    if (!(i as IPlayer).IsVisibleToEnemy())
                        continue;
                    double d = i.Dist(entity);
                    if (d < dist)
                        yield return i;
                }
            else
                foreach (Entity i in entity.Owner.EnemiesCollision.HitTest(entity.X, entity.Y, (float)dist))
                {
                    if (i.ObjectType != objType.Value)
                        continue;
                    double d = i.Dist(entity);
                    if (d < dist)
                        yield return i;
                }
        }

        public static Entity GetNearestEntity(this Entity entity, double dist, bool players,
            Predicate<Entity> predicate = null)
        {
            if (entity.Owner == null)
                return null;
            Entity ret = null;
            if (players)
                foreach (Entity i in entity.Owner.PlayersCollision.HitTest(entity.X, entity.Y, (float)dist))
                {
                    if (!(i as IPlayer).IsVisibleToEnemy() ||
                        i == entity)
                        continue;
                    double d = i.Dist(entity);
                    if (d < dist)
                    {
                        if (predicate != null && !predicate(i))
                            continue;
                        dist = d;
                        ret = i;
                    }
                }
            else
                foreach (Entity i in entity.Owner.EnemiesCollision.HitTest(entity.X, entity.Y, (float)dist))
                {
                    if (i == entity)
                        continue;
                    double d = i.Dist(entity);
                    if (d < dist)
                    {
                        if (predicate != null && !predicate(i))
                            continue;
                        dist = d;
                        ret = i;
                    }
                }
            return ret;
        }

        public static Entity GetNearestEntityByGroup(this Entity entity, double dist, string group)
        {
            return entity.GetNearestEntitiesByGroup(dist, group).FirstOrDefault();
        }

        public static IEnumerable<Entity> GetNearestEntitiesByGroup(this Entity entity, double dist, string group)
        {
            if (entity.Owner == null)
                yield break;
            foreach (Entity i in entity.Owner.EnemiesCollision.HitTest(entity.X, entity.Y, (float)dist))
            {
                if (i.ObjectDesc == null || i.ObjectDesc.Group != group)
                    continue;
                double d = i.Dist(entity);
                if (d < dist)
                    yield return i;
            }
        }

        public static int CountEntity(this Entity entity, double dist, ushort? objType)
        {
            if (entity.Owner == null)
                return 0;
            int ret = 0;
            if (objType == null)
                foreach (Entity i in entity.Owner.PlayersCollision.HitTest(entity.X, entity.Y, (float)dist))
                {
                    if (!(i as IPlayer).IsVisibleToEnemy())
                        continue;
                    double d = i.Dist(entity);
                    if (d < dist)
                        ret++;
                }
            else
                foreach (Entity i in entity.Owner.EnemiesCollision.HitTest(entity.X, entity.Y, (float)dist))
                {
                    if (i.ObjectType != objType.Value)
                        continue;
                    double d = i.Dist(entity);
                    if (d < dist)
                        ret++;
                }
            return ret;
        }

        public static int CountEntity(this Entity entity, double dist, string group)
        {
            if (entity.Owner == null)
                return 0;
            int ret = 0;
            foreach (Entity i in entity.Owner.EnemiesCollision.HitTest(entity.X, entity.Y, (float)dist))
            {
                if (i.ObjectDesc == null || i.ObjectDesc.Group != group)
                    continue;
                double d = i.Dist(entity);
                if (d < dist)
                    ret++;
            }
            return ret;
        }

        public static void Aoe(this Entity entity, float radius, ushort? objType, Action<Entity> callback)
        //Null for player
        {
            if (objType == null)
                foreach (Entity i in entity.Owner.PlayersCollision.HitTest(entity.X, entity.Y, radius))
                {
                    double d = i.Dist(entity);
                    if (d < radius)
                        callback(i);
                }
            else
                foreach (Entity i in entity.Owner.EnemiesCollision.HitTest(entity.X, entity.Y, radius))
                {
                    if (i.ObjectType != objType.Value)
                        continue;
                    double d = i.Dist(entity);
                    if (d < radius)
                        callback(i);
                }
        }

        public static void Aoe(this Entity entity, float radius, bool players, Action<Entity> callback)
        //Null for player
        {
            if (players)
                foreach (Entity i in entity.Owner.PlayersCollision.HitTest(entity.X, entity.Y, radius))
                {
                    double d = i.Dist(entity);
                    if (d < radius)
                        callback(i);
                }
            else
                foreach (Entity i in entity.Owner.EnemiesCollision.HitTest(entity.X, entity.Y, radius))
                {
                    if (!(i is Enemy))
                        continue;
                    double d = i.Dist(entity);
                    if (d < radius)
                        callback(i);
                }
        }

        public static void Aoe(this World world, Position pos, float radius, bool players, Action<Entity> callback)
        //Null for player
        {
            if (players)
                foreach (Entity i in world.PlayersCollision.HitTest(pos.X, pos.Y, radius))
                {
                    double d = MathsUtils.Dist(i.X, i.Y, pos.X, pos.Y);
                    if (d < radius)
                        callback(i);
                }
            else
                foreach (Entity i in world.EnemiesCollision.HitTest(pos.X, pos.Y, radius))
                {
                    if (!(i is Enemy))
                        continue;
                    double d = MathsUtils.Dist(i.X, i.Y, pos.X, pos.Y);
                    if (d < radius)
                        callback(i);
                }
        }
    }

    internal static class ItemUtils
    {
        public static bool AuditItem(this IContainer container, Item item, int slot)
        {
            if (container is Container || container is OneWayContainer)
                return true;
            return item == null || container.SlotTypes[slot] == 0 || item.SlotType == container.SlotTypes[slot];
        }
    }
}