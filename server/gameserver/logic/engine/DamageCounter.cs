#region

using LoESoft.Core;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.world;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer.logic
{
    public class DamageCounter : IDisposable
    {
        private readonly WeakDictionary<Player, int> hitters = new WeakDictionary<Player, int>();
        private Enemy enemy;

        public DamageCounter(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public Enemy Host
        {
            get { return enemy; }
        }

        public Projectile LastProjectile { get; private set; }
        public Player LastHitter { get; private set; }

        public DamageCounter Corpse { get; set; }
        public DamageCounter Parent { get; set; }

        public void UpdateEnemy(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public void HitBy(Player player, RealmTime time, Projectile projectile, int dmg)
        {
            if (!hitters.TryGetValue(player, out int totalDmg))
                totalDmg = 0;

            totalDmg += dmg;
            hitters[player] = totalDmg;

            LastProjectile = projectile;
            LastHitter = player;

            player.FameCounter.Hit(projectile, enemy);
        }

        public Tuple<Player, int>[] GetPlayerData()
        {
            if (Parent != null)
                return Parent.GetPlayerData();

            var dat = new List<Tuple<Player, int>>();

            foreach (var i in hitters)
            {
                if (i.Key.Owner == null)
                    continue;

                dat.Add(new Tuple<Player, int>(i.Key, i.Value));
            }

            return dat.ToArray();
        }

        public void Death(RealmTime time)
        {
            if (Corpse != null)
            {
                Corpse.Parent = this;
                return;
            }

            var players = new List<Player>();

            foreach (var i in (Parent ?? this).hitters)
            {
                if (i.Key.Owner == null)
                    continue;

                players.Add(i.Key);
            }

            var exp = 0d;
            var newmethod = enemy.ObjectDesc.NewExperience;

            if (newmethod)
                exp = enemy.ObjectDesc.Experience;
            else
                exp = ProcessExperience(enemy.ObjectDesc.MaxHP);

            exp *= GroupBonus(players.Count);

            if (players.Count != 0)
                foreach (var player in players.Where(p => p != null))
                    player.CalculateExp(enemy, (int)exp);

            players.Clear();

            if (enemy.Owner is IRealm)
                (enemy.Owner as GameWorld).EnemyKilled(enemy, (Parent ?? this).LastHitter);
        }

        private double GroupBonus(int amount)
        {
            if (amount == 1)
                return 1;
            else if (amount > 1 && amount <= 4)
                return 1.05;
            else if (amount > 4 && amount <= 8)
                return 1.1;
            else if (amount > 8 && amount <= 12)
                return 1.15;
            else if (amount > 12 && amount <= 16)
                return 1.2;
            else if (amount > 16)
                return 1.25;
            else
                return 0;
        }

        private double ProcessExperience(double hp)
        {
            if (hp == 0)
                return 0;
            else if (hp > 0 && hp <= 100)
                return hp / 10;
            else if (hp > 100 && hp <= 1000)
                return (hp / 10) * 1.25;
            else if (hp > 1000 && hp <= 10000)
                return (hp / 10) * 1.5;
            else
                return 2000;
        }

        public void Dispose() => enemy = null;
    }
}