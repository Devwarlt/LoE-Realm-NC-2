#region

using LoESoft.Core;
using LoESoft.Core.database;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace LoESoft.GameServer.logic
{
    public class FameCounter
    {
        protected Player Player { get; set; }

        public FameStats Stats { get; private set; }
        public DbClassStats ClassStats { get; private set; }

        public FameCounter(Player player)
        {
            Player = player;
            Stats = FameStats.Read(player.Client.Character.FameStats);
            ClassStats = new DbClassStats(player.Client.Account);
        }

        private HashSet<Projectile> projs = new HashSet<Projectile>();

        public void Shoot(Projectile proj)
        {
            Stats.Shots++;
            projs.Add(proj);
        }

        public void Hit(Projectile proj, Enemy enemy)
        {
            if (projs.Contains(proj))
            {
                projs.Remove(proj);
                Stats.ShotsThatDamage++;
            }
        }

        public void Killed(Enemy enemy, bool killer)
        {
            if (enemy.ObjectDesc.God)
                Stats.GodAssists++;
            else
                Stats.MonsterAssists++;
            if (Player.Quest == enemy)
                Stats.QuestsCompleted++;
            if (killer)
            {
                if (enemy.ObjectDesc.God)
                    Stats.GodKills++;
                else
                    Stats.MonsterKills++;

                if (enemy.ObjectDesc.Cube)
                    Stats.CubeKills++;

                if (enemy.ObjectDesc.Oryx)
                    Stats.OryxKills++;
            }

            // process task below
            if (Player.ActualTask != null)
            {
                var monsterData = Player.MonsterCaches.FirstOrDefault(monster => monster.ObjectId == enemy.ObjectDesc.ObjectId);

                if (monsterData == null)
                    Player.MonsterCaches.Add(new MonsterCache()
                    {
                        ObjectId = enemy.ObjectDesc.ObjectId,
                        TaskCount = -1,
                        TaskLimit = -1,
                        Total = 1
                    });
                else
                {
                    if (monsterData.TaskCount != -1 && monsterData.TaskLimit != -1)
                    {
                        if (monsterData.TaskCount == monsterData.TaskLimit)
                            monsterData.Total++;
                        else
                        {
                            foreach (var i in Player.Task.MonsterDatas)
                                if (i.ObjectId == monsterData.ObjectId)
                                {
                                    monsterData.TaskCount++;
                                    monsterData.Total++;
                                    break;
                                }

                            Player.SendHelp($"[{monsterData.TaskCount}/{monsterData.TaskLimit}] {monsterData.ObjectId}.");
                        }
                    }
                    else
                        monsterData.Total++;
                }
            }
        }

        public void LevelUpAssist(int count)
        {
            Stats.LevelUpAssists += count;
        }

        public void TileSent(int num)
        {
            Stats.TilesUncovered += num;
        }

        public void Teleport()
        {
            Stats.Teleports++;
        }

        public void UseAbility()
        {
            Stats.SpecialAbilityUses++;
        }

        public void DrinkPot()
        {
            Stats.PotionsDrunk++;
        }

        private int elapsed = 0;

        public void Tick(RealmTime time)
        {
            elapsed += time.ElapsedMsDelta;

            if (elapsed > 1000 * 60)
            {
                elapsed -= 1000 * 60;
                Stats.MinutesActive++;
            }
        }
    }
}