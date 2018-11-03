#region

using LoESoft.Core;
using LoESoft.Core.config;
using LoESoft.GameServer.networking;
using LoESoft.GameServer.networking.incoming;
using LoESoft.GameServer.networking.outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using static LoESoft.GameServer.networking.Client;

#endregion

namespace LoESoft.GameServer.realm.entity.player
{
    partial class Player
    {
        public bool Activate(RealmTime time, Item item, USEITEM pkt)
        {
            bool endMethod = false;

            Position target = pkt.ItemUsePos;
            MP -= item.MpCost;

            IContainer con = Owner?.GetEntity(pkt.SlotObject.ObjectId) as IContainer;
            if (con == null)
                return true;
            if (CheatEngineDetectSlot(item, pkt, con))
                CheatEngineDetect(item, pkt);
            if (item.IsBackpack)
                Backpack();
            if (item.XpBooster) //XpBooster(item);
                return true;
            if (item.LootDropBooster) //LootDropBooster(item);
                return true;
            if (item.LootTierBooster) //LootTierBooster(item);
                return true;

            Random rnd = new Random();

            foreach (ActivateEffect eff in item.ActivateEffects)
            {
                switch (eff.Effect)
                {
                    case ActivateEffects.BulletNova:
                        {
                            Projectile[] prjs = new Projectile[20];
                            ProjectileDesc prjDesc = item.Projectiles[0];
                            var batch = new Message[21];

                            for (var i = 0; i < 20; i++)
                            {
                                Projectile proj = CreateProjectile(prjDesc, item.ObjectType, Random.Next(prjDesc.MinDamage, prjDesc.MaxDamage), time.TotalElapsedMs, target, (float) (i * (Math.PI * 2) / 20));

                                Owner.AddProjectileFromId(Id, proj.ProjectileId, proj);

                                Owner?.EnterWorld(proj);
                                FameCounter.Shoot(proj);

                                batch[i] = new SERVERPLAYERSHOOT()
                                {
                                    BulletId = proj.ProjectileId,
                                    OwnerId = Id,
                                    ContainerType = item.ObjectType,
                                    StartingPos = target,
                                    Angle = proj.Angle,
                                    Damage = (short) proj.Damage
                                };
                                prjs[i] = proj;
                            }

                            batch[20] = new SHOWEFFECT()
                            {
                                EffectType = EffectType.Line,
                                PosA = target,
                                TargetId = Id,
                                Color = new ARGB(0xFFFF00AA)
                            };

                            foreach (Player plr in Owner?.Players.Values.Where(p => p?.DistSqr(this) < RadiusSqr))
                                plr?.Client.SendMessage(batch);
                        }
                        break;

                    case ActivateEffects.Shoot:
                        Shoot(time, item, target);
                        break;

                    case ActivateEffects.Heal:
                        {
                            List<Message> pkts = new List<Message>();
                            ActivateHealHp(this, eff.Amount, pkts);
                            Owner?.BroadcastMessage(pkts, null);
                        }
                        break;

                    case ActivateEffects.HealNova:
                        {
                            var amountHN = eff.Amount;
                            var rangeHN = eff.Range;
                            if (eff.UseWisMod)
                            {
                                amountHN = (int) UseWisMod(eff.Amount, 0);
                                rangeHN = UseWisMod(eff.Range);
                            }

                            List<Message> pkts = new List<Message>();
                            this?.Aoe(rangeHN, true, player => { ActivateHealHp(player as Player, amountHN, pkts); });
                            pkts.Add(new SHOWEFFECT
                            {
                                EffectType = EffectType.Nova,
                                TargetId = Id,
                                Color = new ARGB(0xffffffff),
                                PosA = new Position { X = rangeHN }
                            });
                            BroadcastSync(pkts, p => this.Dist(p) < 25);
                        }
                        break;

                    case ActivateEffects.Magic:
                        {
                            List<Message> pkts = new List<Message>();
                            ActivateHealMp(this, eff.Amount, pkts);
                            Owner?.BroadcastMessage(pkts, null);
                        }
                        break;

                    case ActivateEffects.MagicNova:
                        {
                            List<Message> pkts = new List<Message>();
                            this?.Aoe(eff.Range / 2, true, player => ActivateHealMp(player as Player, eff.Amount, pkts));
                            pkts.Add(new SHOWEFFECT
                            {
                                EffectType = EffectType.Nova,
                                TargetId = Id,
                                Color = new ARGB(0xffffffff),
                                PosA = new Position { X = eff.Range }
                            });
                            Owner?.BroadcastMessage(pkts, null);
                        }
                        break;

                    case ActivateEffects.Teleport:
                        {
                            if (Database.Names.Contains(Name))
                            {
                                SendInfo("Players without valid name couldn't use this feature. Please name your character to continue.");
                                return true;
                            }

                            Move(target.X, target.Y);
                            UpdateCount++;
                            Owner?.BroadcastMessage(new Message[]
                            {
                                new GOTO
                                {
                                    ObjectId = Id,
                                    Position = new Position
                                    {
                                        X = X,
                                        Y = Y
                                    }
                                },
                                new SHOWEFFECT
                                {
                                    EffectType = EffectType.Teleport,
                                    TargetId = Id,
                                    PosA = new Position
                                    {
                                        X = X,
                                        Y = Y
                                    },
                                    Color = new ARGB(0xFFFFFFFF)
                                }
                            }, null);
                        }
                        break;

                    case ActivateEffects.VampireBlast:
                        {
                            List<Message> pkts = new List<Message>
                            {
                                new SHOWEFFECT
                                {
                                    EffectType = EffectType.Line,
                                    TargetId = Id,
                                    PosA = target,
                                    Color = new ARGB(0xFFFF0000)
                                },
                                new SHOWEFFECT
                                {
                                    EffectType = EffectType.Burst,
                                    Color = new ARGB(0xFFFF0000),
                                    TargetId = Id,
                                    PosA = target,
                                    PosB = new Position { X = target.X + eff.Radius, Y = target.Y }
                                }
                            };

                            int totalDmg = 0;
                            List<Enemy> enemies = new List<Enemy>();
                            Owner?.Aoe(target, eff.Radius, false, enemy =>
                            {
                                enemies.Add(enemy as Enemy);
                                totalDmg += (enemy as Enemy).Damage(this, time, eff.TotalDamage, false);
                            });
                            List<Player> players = new List<Player>();
                            this.Aoe(eff.Radius, true, player =>
                            {
                                players?.Add(player as Player);
                                ActivateHealHp(player as Player, totalDmg, pkts);
                            });

                            if (enemies.Count > 0)
                            {
                                Random rand = new Random();
                                for (int i = 0; i < 5; i++)
                                {
                                    Enemy a = enemies[rand.Next(0, enemies.Count)];
                                    Player b = players[rand.Next(0, players.Count)];
                                    pkts.Add(new SHOWEFFECT
                                    {
                                        EffectType = EffectType.Flow,
                                        TargetId = b.Id,
                                        PosA = new Position { X = a.X, Y = a.Y },
                                        Color = new ARGB(0xffffffff)
                                    });
                                }
                            }

                            BroadcastSync(pkts, p => this.Dist(p) < 25);
                        }
                        break;

                    case ActivateEffects.Trap:
                        {
                            BroadcastSync(new SHOWEFFECT
                            {
                                EffectType = EffectType.Throw,
                                Color = new ARGB(0xff9000ff),
                                TargetId = Id,
                                PosA = target
                            }, p => this.Dist(p) < 25);
                            Owner?.Timers.Add(new WorldTimer(1500, (world, t) =>
                            {
                                Trap trap = new Trap(
                                    this,
                                    eff.Radius,
                                    eff.TotalDamage,
                                    eff.ConditionEffect ?? ConditionEffectIndex.Slowed,
                                    eff.EffectDuration);
                                trap?.Move(target.X, target.Y);
                                world?.EnterWorld(trap);
                            }));
                        }
                        break;

                    case ActivateEffects.StasisBlast:
                        {
                            List<Message> pkts = new List<Message>
                            {
                                new SHOWEFFECT
                                {
                                    EffectType = EffectType.Collapse,
                                    TargetId = Id,
                                    PosA = target,
                                    PosB = new Position { X = target.X + 3, Y = target.Y },
                                    Color = new ARGB(0xFF00D0)
                                }
                            };
                            Owner?.Aoe(target, 3, false, enemy =>
                            {
                                if (IsSpecial(enemy.ObjectType))
                                    return;

                                if (enemy.HasConditionEffect(ConditionEffectIndex.StasisImmune))
                                {
                                    if (!enemy.HasConditionEffect(ConditionEffectIndex.Invincible))
                                    {
                                        pkts.Add(new NOTIFICATION
                                        {
                                            ObjectId = enemy.Id,
                                            Color = new ARGB(0xff00ff00),
                                            Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"Immune\"}}"
                                        });
                                    }
                                }
                                else if (!enemy.HasConditionEffect(ConditionEffectIndex.Stasis))
                                {
                                    enemy.ApplyConditionEffect(new ConditionEffect
                                    {
                                        Effect = ConditionEffectIndex.Stasis,
                                        DurationMS = eff.DurationMS
                                    });
                                    Owner.Timers.Add(new WorldTimer(eff.DurationMS, (world, t) =>
                                    {
                                        enemy.ApplyConditionEffect(new ConditionEffect
                                        {
                                            Effect = ConditionEffectIndex.StasisImmune,
                                            DurationMS = 3000
                                        });
                                    }));
                                    pkts.Add(new NOTIFICATION
                                    {
                                        ObjectId = enemy.Id,
                                        Color = new ARGB(0xffff0000),
                                        Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"Stasis\"}}"
                                    });
                                }
                            });
                            BroadcastSync(pkts, p => this.Dist(p) < 25);
                        }
                        break;

                    case ActivateEffects.Decoy:
                        {
                            Decoy decoy = new Decoy(this, eff.DurationMS, StatsManager.GetSpeed());
                            decoy?.Move(X, Y);
                            Owner?.EnterWorld(decoy);
                        }
                        break;

                    case ActivateEffects.Lightning:
                        {
                            Enemy start = null;
                            double angle = Math.Atan2(target.Y - Y, target.X - X);
                            double diff = Math.PI / 3;
                            Owner?.Aoe(target, 6, false, enemy =>
                            {
                                if (!(enemy is Enemy))
                                    return;
                                double x = Math.Atan2(enemy.Y - Y, enemy.X - X);
                                if (Math.Abs(angle - x) < diff)
                                {
                                    start = enemy as Enemy;
                                    diff = Math.Abs(angle - x);
                                }
                            });
                            if (start == null)
                                return true;

                            Enemy current = start;
                            Enemy[] targets = new Enemy[eff.MaxTargets];
                            for (int i = 0; i < targets.Length; i++)
                            {
                                targets[i] = current;
                                Enemy next = current.GetNearestEntity(8, false,
                                    enemy =>
                                        enemy is Enemy &&
                                        Array.IndexOf(targets, enemy) == -1 &&
                                        this.Dist(enemy) <= 6) as Enemy;

                                if (next == null)
                                    break;
                                current = next;
                            }

                            List<Message> pkts = new List<Message>();
                            for (int i = 0; i < targets.Length; i++)
                            {
                                if (targets[i] == null)
                                    break;
                                if (targets[i].HasConditionEffect(ConditionEffectIndex.Invincible))
                                    continue;
                                Entity prev = i == 0 ? (Entity) this : targets[i - 1];
                                targets[i]?.Damage(this, time, eff.TotalDamage, false);
                                if (eff.ConditionEffect != null)
                                    targets[i].ApplyConditionEffect(new ConditionEffect
                                    {
                                        Effect = eff.ConditionEffect.Value,
                                        DurationMS = (int) (eff.EffectDuration * 1000)
                                    });
                                pkts.Add(new SHOWEFFECT
                                {
                                    EffectType = EffectType.Lightning,
                                    TargetId = prev.Id,
                                    Color = new ARGB(0xffff0088),
                                    PosA = new Position
                                    {
                                        X = targets[i].X,
                                        Y = targets[i].Y
                                    },
                                    PosB = new Position { X = 350 }
                                });
                            }
                            BroadcastSync(pkts, p => this?.Dist(p) < 25);
                        }
                        break;

                    case ActivateEffects.PoisonGrenade:
                        {
                            try
                            {
                                BroadcastSync(new SHOWEFFECT
                                {
                                    EffectType = EffectType.Throw,
                                    Color = new ARGB(0xffddff00),
                                    TargetId = Id,
                                    PosA = target
                                }, p => this?.Dist(p) < 25);
                                Placeholder x = new Placeholder(1500);
                                x.Move(target.X, target.Y);
                                Owner?.EnterWorld(x);
                                try
                                {
                                    Owner.Timers.Add(new WorldTimer(1500, (world, t) =>
                                    {
                                        world.BroadcastMessage(new SHOWEFFECT
                                        {
                                            EffectType = EffectType.Nova,
                                            Color = new ARGB(0xffddff00),
                                            TargetId = x.Id,
                                            PosA = new Position { X = eff.Radius }
                                        }, null);
                                        world.Aoe(target, eff.Radius, false,
                                            enemy => PoisonEnemy(enemy as Enemy, eff));
                                    }));
                                }
                                catch (Exception) { }
                            }
                            catch (Exception) { }
                        }
                        break;

                    case ActivateEffects.Donor:
                        {
                            if (Database.Names.Contains(Name))
                            {
                                SendInfo("Players without valid name couldn't use this feature. Please name your character to continue.");
                                return true;
                            }

                            if (!Owner.Name.Contains("Vault"))
                            {
                                SendInfo("You have to be in Vault to use this item.");
                                return false;
                            }

                            List<ushort> rewards = new List<ushort> { 0x5016, 0x5017, 0x5018, 0x5019, 0x5020, 0x5021, 0x5022, 0x5023, 0x5024, 0x5025, 0x5026, 0x5027, 0x5028, 0x5029, 0x5030, 0x5031, 0x5032, 0x5033, 0x5034, 0x5035, 0x5036, 0x5037, 0x5038, 0x5039, 0x5040, 0x5041, 0x5042 };

                            ushort reward = rewards[rnd.Next(0, rewards.Count)];

                            if (Inventory[4] != null && Inventory[5] != null && Inventory[6] != null && Inventory[7] != null &&
                                Inventory[8] != null && Inventory[9] != null && Inventory[10] != null && Inventory[11] != null)
                            {
                                SendHelp("Your inventory need free space to enchant this item.");
                                return true;
                            }
                            else
                                for (int i = 4; i < 12; i++)
                                    if (Inventory[i] == null)
                                    {
                                        Inventory[i] = GameServer.Manager.GameData.Items[reward];
                                        UpdateCount++;
                                        SaveToCharacter();
                                        return false;
                                    }
                            return true;
                        }
                    case ActivateEffects.RemoveNegativeConditions:
                        {
                            this?.Aoe(eff.Range / 2, true, player => ApplyConditionEffect(NegativeEffs));
                            BroadcastSync(new SHOWEFFECT
                            {
                                EffectType = EffectType.Nova,
                                TargetId = Id,
                                Color = new ARGB(0xffffffff),
                                PosA = new Position { X = eff.Range / 2 }
                            }, p => this?.Dist(p) < 25);
                        }
                        break;

                    case ActivateEffects.RemoveNegativeConditionsSelf:
                        {
                            ApplyConditionEffect(NegativeEffs);
                            Owner?.BroadcastMessage(new SHOWEFFECT
                            {
                                EffectType = EffectType.Nova,
                                TargetId = Id,
                                Color = new ARGB(0xffffffff),
                                PosA = new Position { X = 1 }
                            }, null);
                        }
                        break;

                    case ActivateEffects.IncrementStat:
                        {
                            if (Database.Names.Contains(Name))
                            {
                                SendInfo("Players without valid name couldn't use this feature. Please name your character to continue.");
                                return true;
                            }

                            int idx = -1;

                            if (eff.Stats == StatsType.MAX_HP_STAT)
                                idx = 0;
                            else if (eff.Stats == StatsType.MAX_MP_STAT)
                                idx = 1;
                            else if (eff.Stats == StatsType.ATTACK_STAT)
                                idx = 2;
                            else if (eff.Stats == StatsType.DEFENSE_STAT)
                                idx = 3;
                            else if (eff.Stats == StatsType.SPEED_STAT)
                                idx = 4;
                            else if (eff.Stats == StatsType.VITALITY_STAT)
                                idx = 5;
                            else if (eff.Stats == StatsType.WISDOM_STAT)
                                idx = 6;
                            else if (eff.Stats == StatsType.DEXTERITY_STAT)
                                idx = 7;

                            Stats[idx] += eff.Amount;
                            int limit =
                                int.Parse(
                                    GameServer.Manager.GameData.ObjectTypeToElement[ObjectType].Element(
                                        StatsManager.StatsIndexToName(idx))
                                        .Attribute("max")
                                        .Value);
                            if (Stats[idx] > limit)
                                Stats[idx] = limit;
                        }
                        break;

                    case ActivateEffects.UnlockPortal:
                        {
                            if (Database.Names.Contains(Name))
                            {
                                SendInfo("Players without valid name couldn't use this feature. Please name your character to continue.");
                                return true;
                            }

                            Portal portal = this.GetNearestEntity(5, GameServer.Manager.GameData.IdToObjectType[eff.LockedName]) as Portal;

                            Message[] packets = new Message[3];
                            packets[0] = new SHOWEFFECT
                            {
                                EffectType = EffectType.Nova,
                                Color = new ARGB(0xFFFFFF),
                                PosA = new Position { X = 5 },
                                TargetId = Id
                            };

                            if (portal == null)
                                return true;

                            portal.Unlock(eff.DungeonName);

                            packets[1] = new NOTIFICATION
                            {
                                Color = new ARGB(0x00FF00),
                                Text =
                                    "{\"key\":\"blank\",\"tokens\":{\"data\":\"Unlocked by " +
                                    Name + "\"}}",
                                ObjectId = Id
                            };

                            packets[2] = new TEXT
                            {
                                BubbleTime = 0,
                                Stars = -1,
                                Name = "",
                                Text = eff.DungeonName + " Unlocked by " + Name + ".",
                                NameColor = 0x123456,
                                TextColor = 0x123456
                            };

                            BroadcastSync(packets);
                        }
                        break;

                    case ActivateEffects.Create:
                        {
                            if (Database.Names.Contains(Name))
                            {
                                SendInfo("Players without valid name couldn't use this feature. Please name your character to continue.");
                                return true;
                            }

                            if (Stars >= 10 || AccountPerks.ByPassKeysRequirements())
                            {
                                if (!GameServer.Manager.GameData.IdToObjectType.TryGetValue(eff.Id, out ushort objType) || !GameServer.Manager.GameData.Portals.ContainsKey(objType))
                                {
                                    SendHelp("Dungeon not implemented yet.");
                                    return true;
                                }
                                Entity entity = Resolve(objType);
                                World w = GameServer.Manager.GetWorld(Owner.Id); //can't use Owner here, as it goes out of scope
                                int TimeoutTime = GameServer.Manager.GameData.Portals[objType].TimeoutTime;
                                string DungName = GameServer.Manager.GameData.Portals[objType].DungeonName;

                                ARGB c = new ARGB(0x00FF00);

                                entity?.Move(X, Y);
                                w?.EnterWorld(entity);

                                w.BroadcastMessage(new NOTIFICATION
                                {
                                    Color = c,
                                    Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"Opened by " + Name + "\"}}",
                                    ObjectId = Client.Player.Id
                                }, null);

                                w.BroadcastMessage(new TEXT
                                {
                                    BubbleTime = 0,
                                    Stars = -1,
                                    Name = "",
                                    Text = DungName + " opened by " + Name,
                                    NameColor = 0x123456,
                                    TextColor = 0x123456
                                }, null);
                                w?.Timers.Add(new WorldTimer(TimeoutTime * 1000,
                                    (world, t) => //default portal close time * 1000
                                    {
                                        try
                                        {
                                            w?.LeaveWorld(entity);
                                        }
                                        catch (Exception) { }
                                    }));
                                return false;
                            }
                            else
                            {
                                SendInfo("You need at least 10 stars to use Keys! Complete class quests to earn additional stars.");
                                return true;
                            }
                        }
                    case ActivateEffects.Dye:
                        {
                            SendHelp("Feature temporarly disabled until further notice from LoESoft Games.");
                            return true;
                            //if (Database.Names.Contains(Name))
                            //{
                            //    SendInfo("Players without valid name couldn't use this feature. Please name your character to continue.");
                            //    return true;
                            //}

                            //if (item.Texture1 != 0)
                            //{
                            //    Texture1 = item.Texture1;
                            //}
                            //if (item.Texture2 != 0)
                            //{
                            //    Texture2 = item.Texture2;
                            //}

                            //SaveToCharacter();
                        }
                    case ActivateEffects.ShurikenAbility:
                        {
                            if (!ninjaShoot)
                            {
                                ApplyConditionEffect(new ConditionEffect
                                {
                                    Effect = ConditionEffectIndex.Speedy,
                                    DurationMS = -1
                                });
                                ninjaFreeTimer = true;
                                ninjaShoot = true;
                            }
                            else
                            {
                                ApplyConditionEffect(new ConditionEffect
                                {
                                    Effect = ConditionEffectIndex.Speedy,
                                    DurationMS = 0
                                });
                                GameServer.Manager.GameData.IdToObjectType.TryGetValue(item.ObjectId, out ushort obj);
                                if (MP >= item.MpEndCost)
                                {
                                    Shoot(time, item, pkt.ItemUsePos);
                                    MP -= (int) item.MpEndCost;
                                }
                                Targetlink = target;
                                ninjaShoot = false;
                            }
                        }
                        break;

                    case ActivateEffects.UnlockSkin:
                        {
                            if (Database.Names.Contains(Name))
                            {
                                SendInfo("Players without valid name couldn't use this feature. Please name your character to continue.");
                                return true;
                            }

                            if (Client.Player.Owner.Name == "Vault")
                            {
                                if (!Client.Account.OwnedSkins.Contains(item.ActivateEffects[0].SkinType))
                                {
                                    GameServer.Manager.Database.AddSkin(Client.Account, item.ActivateEffects[0].SkinType);
                                    SendInfo("New skin unlocked successfully. Change skins in your Vault, or start a new character to use.");
                                    Client.SendMessage(new RESKIN_UNLOCK
                                    {
                                        SkinID = item.ActivateEffects[0].SkinType
                                    });
                                    endMethod = false;
                                    return false;
                                }
                                SendInfo("Error.alreadyOwnsSkin");
                                endMethod = true;
                                return true;
                            }
                            else
                            {
                                SendInfo("Skin items can only be used from the Vault!");
                                endMethod = true;
                                return true;
                            }
                        }
                    case ActivateEffects.Pet:
                        {
                            if (Database.Names.Contains(Name))
                            {
                                SendInfo("Players without valid name couldn't use this feature. Please name your character to continue.");
                                return true;
                            }

                            Entity en = Resolve(eff.ObjectId);
                            en?.Move(X, Y);
                            en?.SetPlayerOwner(this);
                            Owner?.EnterWorld(en);
                            Owner?.Timers.Add(new WorldTimer(30 * 1000, (w, t) => w.LeaveWorld(en)));
                        }
                        break;

                    case ActivateEffects.MysteryPortal:
                        {
                            if (Database.Names.Contains(Name))
                            {
                                SendInfo("Players without valid name couldn't use this feature. Please name your character to continue.");
                                return true;
                            }

                            string[] dungeons = new[]
                            {
                                "Pirate Cave Portal",
                                "Forest Maze Portal",
                                "Spider Den Portal",
                                "Snake Pit Portal",
                                "Glowing Portal",
                                "Forbidden Jungle Portal",
                                "Candyland Portal",
                                "Haunted Cemetery Portal",
                                "Undead Lair Portal",
                                "Davy Jones' Locker Portal",
                                "Manor of the Immortals Portal",
                                "Abyss of Demons Portal",
                                "Lair of Draconis Portal",
                                "Mad Lab Portal",
                                "Ocean Trench Portal",
                                "Tomb of the Ancients Portal",
                                "Beachzone Portal",
                                "The Shatters",
                                "Deadwater Docks",
                                "Woodland Labyrinth",
                                "The Crawling Depths",
                                "Treasure Cave Portal",
                                "Battle Nexus Portal",
                                "Belladonna's Garden Portal",
                                "Lair of Shaitan Portal"
                            };

                            PortalDesc[] descs = GameServer.Manager.GameData.Portals.Where(_ => dungeons.Contains(_.Value.ObjectId)).Select(_ => _.Value).ToArray();
                            PortalDesc portalDesc = descs[Random.Next(0, descs.Count())];
                            Entity por = Resolve(portalDesc.ObjectId);
                            por?.Move(X, Y);
                            Owner?.EnterWorld(por);

                            Client?.SendMessage(new NOTIFICATION
                            {
                                Color = new ARGB(0x00FF00),
                                Text =
                                    "{\"key\":\"blank\",\"tokens\":{\"data\":\"Opened by " +
                                    Client.Account.Name + "\"}}",
                                ObjectId = Client.Player.Id
                            });

                            Owner?.BroadcastMessage(new TEXT
                            {
                                BubbleTime = 0,
                                Stars = -1,
                                Name = "",
                                Text = portalDesc.ObjectId + " opened by " + Name,
                                NameColor = 0x123456,
                                TextColor = 0x123456
                            }, null);

                            Owner?.Timers.Add(new WorldTimer(portalDesc.TimeoutTime * 1000, (w, t) => //default portal close time * 1000
                            {
                                try
                                {
                                    w?.LeaveWorld(por);
                                }
                                catch (Exception) { }
                            }));
                        }
                        break;

                    case ActivateEffects.GenericActivate:
                        {
                            bool targetPlayer = eff.Target.Equals("player");
                            bool centerPlayer = eff.Center.Equals("player");
                            int duration = (eff.UseWisMod) ? (int) (UseWisMod(eff.DurationSec) * 1000) : eff.DurationMS;
                            float range = (eff.UseWisMod) ? UseWisMod(eff.Range) : eff.Range;

                            Owner?.Aoe((eff.Center.Equals("mouse")) ? target : new Position { X = X, Y = Y }, range, targetPlayer, entity =>
                            {
                                if (IsSpecial(entity.ObjectType))
                                    return;
                                if (!entity.HasConditionEffect(ConditionEffectIndex.Stasis) &&
                                    !entity.HasConditionEffect(ConditionEffectIndex.Invincible))
                                {
                                    entity.ApplyConditionEffect(
                                    new ConditionEffect()
                                    {
                                        Effect = eff.ConditionEffect.Value,
                                        DurationMS = duration
                                    });
                                }
                            });

                            // replaced this last bit with what I had, never noticed any issue with it. Perhaps I'm wrong?
                            BroadcastSync(new SHOWEFFECT()
                            {
                                EffectType = (EffectType) eff.VisualEffect,
                                TargetId = Id,
                                Color = new ARGB(eff.Color ?? 0xffffffff),
                                PosA = centerPlayer ? new Position { X = range } : target,
                                PosB = new Position(target.X - range, target.Y) //Its the range of the diffuse effect
                            }, p => this?.DistSqr(p) < 25);
                        }
                        break;

                    case ActivateEffects.SpecialPet:
                        {
                            /// Probability:
                            /// This is a simple probability formula based in chance between
                            /// 0 to 100 when player hatch egg.

                            if (Database.Names.Contains(Name))
                            {
                                SendInfo("Players without valid name couldn't use this feature. Please name your character to continue.");
                                return true;
                            }

                            int chance = eff.Chance;
                            int minStars = item.MinStars;

                            if (Stars >= minStars || AccountPerks.ByPassEggsRequirements())
                            {
                                if (Owner.Name == "Vault")
                                {
                                    int min = 0;
                                    int max = 100;
                                    int newPetID;
                                    string message = null;

                                    newPetID = chance == 100 ? eff.PetType : (rnd.Next(min, max) <= chance ? eff.PetType : 0);

                                    if (PetID == 0)
                                    {
                                        if (newPetID != 0)
                                        {
                                            PetID = newPetID;
                                            HatchlingPet = true;
                                            message = "Congratulations! You received a new pet.";
                                            Entity petResolve = Resolve(eff.PetType);
                                            petResolve.Move(X, Y);
                                            petResolve.SetPlayerOwner(this);
                                            Owner.EnterWorld(petResolve);
                                            SendInfo(message);
                                        }
                                        else
                                        {
                                            message = "Oh no! Your egg hatched and pet escaped.";
                                            SendHelp(message);
                                        }

                                        SaveToCharacter();
                                        UpdateCount++;

                                        return false;
                                    }
                                    else
                                    {
                                        if (newPetID != 0)
                                        {
                                            PetID = newPetID;
                                            HatchlingPet = true;
                                            Owner.LeaveWorld(Pet);
                                            message = "Congratulations! You received a new pet.";
                                            Pet = Resolve(eff.PetType);
                                            Pet.Move(X, Y);
                                            Pet.SetPlayerOwner(this);
                                            Owner.EnterWorld(Pet);
                                            SendInfo(message);
                                        }
                                        else
                                        {
                                            message = "Oh no! Your egg hatched and pet escaped.";
                                            SendHelp(message);
                                        }

                                        SaveToCharacter();
                                        UpdateCount++;

                                        return false;
                                    }
                                }
                                else
                                {
                                    SendInfo("You have to be in Vault to use this item.");
                                    return true;
                                }
                            }
                            else
                            {
                                SendInfo($"You need {minStars} star{(minStars > 1 ? "s" : "")} to use this item.");
                                return true;
                            }
                        }
                    case ActivateEffects.CreatePet:
                        {
                            /// <summary>
                            /// EGG CONVERSION ALGORITHM
                            ///
                            /// This feature was used before to manage FSoD pets i.e create new pet type,
                            /// now we are converting all exist eggs for new pet system. This algorithm
                            /// added convert FSoD pet egg to randomly pet egg of new system depending by
                            /// player's luck and egg tier.
                            ///
                            /// by Devwarlt
                            /// </summary>

                            /// <summary>
                            /// Replace all current eggs, when consumed for new pet system
                            /// </summary>

                            if (Database.Names.Contains(Name))
                            {
                                SendInfo("Players without valid name couldn't use this feature. Please name your character to continue.");
                                return true;
                            }

                            int success = 20; // 20% chance to get perfect pet type
                            int min = 0;
                            int _min = 0;
                            int __min = 0;
                            int ___min = 3;
                            int max = 100;
                            int _max = 2;
                            int __max = 1;
                            int ___max = 5;

                            List<ushort> _common = new List<ushort> { 0x4000, 0x4001, 0x4002 };
                            List<ushort> _uncommon = new List<ushort> { 0x4003, 0x4004, 0x4005, 0x4006, 0x4007, 0x4008 };
                            List<ushort> _rare = new List<ushort> { 0x4009, 0x400a, 0x400b, 0x400c, 0x400d, 0x400e };
                            List<ushort> _legendary = new List<ushort> { 0x400f, 0x4010, 0x4011, 0x4012, 0x4013, 0x4014 };
                            List<ushort> _divine = new List<ushort> { 0x4015, 0x4016, 0x4017 };

                            IEnumerable<ushort> common = _common;
                            IEnumerable<ushort> uncommon = _uncommon.Concat(common);
                            IEnumerable<ushort> rare = _rare.Concat(uncommon);
                            IEnumerable<ushort> legendary = _legendary.Concat(rare);
                            IEnumerable<ushort> divine = _divine.Concat(legendary);

                            /// <summary>
                            /// Probability used in this case for randomly egg type
                            /// </summary>

                            int chance = rnd.Next(min, max);
                            int minorVariance = rnd.Next(_min, _max);
                            int middleVariance = rnd.Next(_min, _max);
                            int _rangedVariance = rnd.Next(__min, __max); // values between 0-1
                            int __rangedVariance = rnd.Next(___min, ___max); // values between 3-5

                            List<int> candidates = new List<int>
                            {
                                _rangedVariance,
                                __rangedVariance
                            };

                            int possibleCandidate = rnd.Next(min, candidates.Count);

                            ushort convertedEgg;

                            // Default represent divine rarity
                            switch (item.Tier)
                            {
                                case (int) EggRarity.Common:
                                    {
                                        convertedEgg =
                                                chance <= success ?
                                                    common.ToList()[2] : common.ToList()[minorVariance];
                                    }
                                    break;

                                case (int) EggRarity.Uncommon:
                                    {
                                        convertedEgg =
                                            chance <= success ?
                                                uncommon.ToList()[middleVariance] : uncommon.ToList()[possibleCandidate];
                                    }
                                    break;

                                case (int) EggRarity.Rare:
                                    {
                                        convertedEgg =
                                            chance <= success ?
                                                rare.ToList()[middleVariance] : rare.ToList()[possibleCandidate];
                                    }
                                    break;

                                case (int) EggRarity.Legendary:
                                    {
                                        convertedEgg =
                                            chance <= success ?
                                                legendary.ToList()[middleVariance] : legendary.ToList()[possibleCandidate];
                                    }
                                    break;

                                default:
                                    {
                                        convertedEgg =
                                            chance <= success ?
                                                divine.ToList()[2] : divine.ToList()[minorVariance];
                                    }
                                    break;
                            }
                            if (!Owner.Name.Contains("Vault"))
                            {
                                SendInfo("You have to be in Vault to use this item.");
                                return true;
                            }

                            if (Inventory[4] != null && Inventory[5] != null && Inventory[6] != null && Inventory[7] != null &&
                                Inventory[8] != null && Inventory[9] != null && Inventory[10] != null && Inventory[11] != null)
                            {
                                SendHelp("Your inventory need free space to enchant this item.");
                                return true;
                            }
                            else
                                for (int i = 4; i < 12; i++)
                                    if (Inventory[i] == null)
                                    {
                                        Inventory[i] = GameServer.Manager.GameData.Items[convertedEgg];
                                        UpdateCount++;
                                        SaveToCharacter();
                                        return false;
                                    }
                            return true;
                        }
                    // TODO ?
                    case ActivateEffects.StatBoostSelf:
                        {
                            int idx = -1;

                            if (eff.Stats == StatsType.MAX_HP_STAT)
                                idx = 0;
                            else if (eff.Stats == StatsType.MAX_MP_STAT)
                                idx = 1;
                            else if (eff.Stats == StatsType.ATTACK_STAT)
                                idx = 2;
                            else if (eff.Stats == StatsType.DEFENSE_STAT)
                                idx = 3;
                            else if (eff.Stats == StatsType.SPEED_STAT)
                                idx = 4;
                            else if (eff.Stats == StatsType.DEXTERITY_STAT)
                                idx = 5;
                            else if (eff.Stats == StatsType.VITALITY_STAT)
                                idx = 6;
                            else if (eff.Stats == StatsType.WISDOM_STAT)
                                idx = 7;

                            List<Message> pkts = new List<Message>();

                            ActivateBoostStat(this, idx, pkts);
                            int OGstat = Oldstat;
                            int bit = idx + 39;

                            int s = eff.Amount;
                            Boost[idx] += s;
                            ApplyConditionEffect(new ConditionEffect
                            {
                                DurationMS = eff.DurationMS,
                                Effect = (ConditionEffectIndex) bit
                            });
                            UpdateCount++;
                            Owner?.Timers.Add(new WorldTimer(eff.DurationMS, (world, t) =>
                            {
                                Boost[idx] = OGstat;
                                UpdateCount++;
                            }));
                            Owner?.BroadcastMessage(new SHOWEFFECT
                            {
                                EffectType = EffectType.Heal,
                                TargetId = Id,
                                Color = new ARGB(0xffffffff)
                            }, null);

                            return false;
                        }
                    case ActivateEffects.StatBoostAura:
                        {
                            int idx = -1;

                            if (eff.Stats == StatsType.MAX_HP_STAT)
                                idx = 0;
                            if (eff.Stats == StatsType.MAX_MP_STAT)
                                idx = 1;
                            if (eff.Stats == StatsType.ATTACK_STAT)
                                idx = 2;
                            if (eff.Stats == StatsType.DEFENSE_STAT)
                                idx = 3;
                            if (eff.Stats == StatsType.SPEED_STAT)
                                idx = 4;
                            if (eff.Stats == StatsType.DEXTERITY_STAT)
                                idx = 5;
                            if (eff.Stats == StatsType.VITALITY_STAT)
                                idx = 6;
                            if (eff.Stats == StatsType.WISDOM_STAT)
                                idx = 7;

                            int bit = idx + 39;

                            int amountSBA = eff.Amount;
                            int durationSBA = eff.DurationMS;
                            float rangeSBA = eff.Range;
                            bool noStack = eff.NoStack;
                            if (eff.UseWisMod)
                            {
                                amountSBA = (int) UseWisMod(eff.Amount, 0);
                                durationSBA = (int) (UseWisMod(eff.DurationSec) * 1000);
                                rangeSBA = UseWisMod(eff.Range);
                            }

                            this?.Aoe(rangeSBA, true, player =>
                            {
                                // TODO support for noStack StatBoostAura attribute (paladin total hp increase / insta heal)
                                if (!noStack)
                                {
                                    ApplyConditionEffect(new ConditionEffect
                                    {
                                        DurationMS = durationSBA,
                                        Effect = (ConditionEffectIndex) bit
                                    });
                                    ActivateBoost[idx].Push(amountSBA);
                                    HP += amountSBA;
                                    CalculateBoost();
                                    player.UpdateCount++;
                                    Owner?.Timers.Add(new WorldTimer(durationSBA, (world, t) =>
                                    {
                                        ActivateBoost[idx].Pop(amountSBA);
                                        CalculateBoost();
                                        player.UpdateCount++;
                                    }));
                                }
                                else
                                {
                                    if (!player.HasConditionEffect(ConditionEffectIndex.HPBoost))
                                    {
                                        ActivateBoost[idx].Push(amountSBA);
                                        HP += amountSBA;
                                        CalculateBoost();
                                        ApplyConditionEffect(new ConditionEffect
                                        {
                                            DurationMS = durationSBA,
                                            Effect = (ConditionEffectIndex) bit
                                        });
                                        player.UpdateCount++;
                                        Owner?.Timers.Add(new WorldTimer(durationSBA, (world, t) =>
                                        {
                                            ActivateBoost[idx].Pop(amountSBA);
                                            CalculateBoost();
                                            player.UpdateCount++;
                                        }));
                                    }
                                }
                            });
                            BroadcastSync(new SHOWEFFECT()
                            {
                                EffectType = EffectType.Nova,
                                TargetId = Id,
                                Color = new ARGB(0xffffffff),
                                PosA = new Position() { X = rangeSBA }
                            }, p => this?.Dist(p) < 25);
                        }
                        break;

                    case ActivateEffects.ConditionEffectAura:
                        {
                            int durationCEA = eff.DurationMS;

                            float rangeCEA = eff.Range;

                            if (eff.UseWisMod)
                            {
                                durationCEA = (int) (UseWisMod(eff.DurationSec) * 1000);
                                rangeCEA = UseWisMod(eff.Range);
                            }

                            this?.Aoe(rangeCEA, true, player =>
                            {
                                player.ApplyConditionEffect(new ConditionEffect
                                {
                                    Effect = eff.ConditionEffect.Value,
                                    DurationMS = durationCEA
                                });
                            });

                            uint color = 0xffffffff;

                            switch (eff.ConditionEffect.Value)
                            {
                                case ConditionEffectIndex.Damaging:
                                    color = 0xffff0000;
                                    break;

                                case ConditionEffectIndex.Berserk:
                                    color = 0x808080;
                                    break;
                            }

                            BroadcastSync(new SHOWEFFECT
                            {
                                EffectType = EffectType.Nova,
                                TargetId = Id,
                                Color = new ARGB(color),
                                PosA = new Position { X = rangeCEA }
                            }, p => this?.Dist(p) < 25);
                        }
                        break;

                    case ActivateEffects.ConditionEffectSelf:
                        {
                            int durationCES = eff.DurationMS;

                            if (eff.UseWisMod)
                                durationCES = (int) (UseWisMod(eff.DurationSec) * 1000);

                            uint color = 0xffffffff;

                            switch (eff.ConditionEffect.Value)
                            {
                                case ConditionEffectIndex.Damaging:
                                    color = 0xffff0000;
                                    break;

                                case ConditionEffectIndex.Berserk:
                                    color = 0x808080;
                                    break;
                            }

                            ApplyConditionEffect(new ConditionEffect
                            {
                                Effect = eff.ConditionEffect.Value,
                                DurationMS = durationCES
                            });

                            Owner?.BroadcastMessage(new SHOWEFFECT
                            {
                                EffectType = EffectType.Nova,
                                TargetId = Id,
                                Color = new ARGB(color),
                                PosA = new Position { X = 2F }
                            }, null);
                        }
                        break;

                    case ActivateEffects.AccountLifetime:
                        {
                            if (Database.Names.Contains(Name))
                            {
                                SendInfo("Players without valid name couldn't use this feature. Please name your character to continue.");
                                return true;
                            }

                            if (AccountType == (int) Core.config.AccountType.VIP_ACCOUNT)
                            {
                                SendInfo($"You can only use {item.DisplayId} when your VIP account lifetime over.");
                                return true;
                            }

                            if (AccountType >= (int) Core.config.AccountType.LEGENDS_OF_LOE_ACCOUNT)
                            {
                                SendInfo($"Only VIP account type can use {item.DisplayId}.");
                                return true;
                            }

                            List<Message> _outgoing = new List<Message>();
                            World _world = GameServer.Manager.GetWorld(Owner.Id);
                            DbAccount acc = Client.Account;
                            int days = eff.Amount;

                            SendInfo($"Success! You received {eff.Amount} day{(eff.Amount > 1 ? "s" : "")} as account lifetime to your VIP account type when {item.DisplayId} was consumed!");

                            NOTIFICATION _notification = new NOTIFICATION
                            {
                                Color = new ARGB(0xFFFFFF),
                                ObjectId = Id,
                                Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"Success!\"}}"
                            };

                            _outgoing.Add(_notification);

                            SHOWEFFECT _showeffect = new SHOWEFFECT
                            {
                                Color = new ARGB(0xffddff00),
                                EffectType = EffectType.Nova,
                                PosA = new Position { X = 2 }
                            };

                            _outgoing.Add(_showeffect);

                            Owner.BroadcastMessage(_outgoing, null);

                            acc.AccountLifetime = DateTime.Now;
                            acc.AccountLifetime = acc.AccountLifetime.AddDays(days);
                            acc.AccountType = (int) Core.config.AccountType.VIP_ACCOUNT;
                            acc.Flush();
                            acc.Reload();

                            UpdateCount++;

                            SendInfo("Reconnecting...");

                            RECONNECT _reconnect = new RECONNECT
                            {
                                GameId = (int) WorldID.NEXUS_ID, // change to Drasta Citadel in future versions!
                                Host = string.Empty,
                                Key = Empty<byte>.Array,
                                Name = "Nexus",
                                Port = Settings.GAMESERVER.PORT
                            };

                            _world.Timers.Add(new WorldTimer(2000, (w, t) => Client.Reconnect(_reconnect)));
                        }
                        break;

                    case ActivateEffects.Gold:
                        {
                            if (Database.Names.Contains(Name))
                                return true;
                            Credits += eff.Amount;
                            GameServer.Manager.Database.UpdateCredit(Client.Account, eff.Amount);
                            UpdateCount++;
                        }
                        break;

                    case ActivateEffects.Exchange:
                        {
                            if (Database.Names.Contains(Name))
                            {
                                SendInfo("Players without valid name couldn't use this feature. Please name your character to continue.");
                                return true;
                            }

                            if (Owner.Name.Contains("Vault"))
                            {
                                SendInfo("You cannot use this feature in Vault.");
                                return true;
                            }

                            Action action = () =>
                            {
                                switch (item.DisplayId)
                                {
                                    case "Succubus Horn x 20":
                                        var umbral = new List<string> { "Umbral Staff", "Umbral Wand", "Umbral Sword", "Umbral Bow", "Umbral Dagger", "Umbral Katana" };
                                        var egg = new List<string> { "lcp egg t4-75", "lcp egg t5-75", "lcp egg t6-75", "lcp egg t7-75" };
                                        var gifts = Client.Account.Gifts.ToList();
                                        gifts.Add(GameServer.Manager.GameData.IdToObjectType[umbral[new Random().Next(0, umbral.Count)]]);
                                        gifts.Add(GameServer.Manager.GameData.IdToObjectType[egg[new Random().Next(0, egg.Count)]]);

                                        Experience += 200000;
                                        GameServer.Manager.Database.UpdateFame(Client.Account, 1000);
                                        GameServer.Manager.Database.UpdateCredit(Client.Account, 200);
                                        Fame = Client.Account.Fame;
                                        Credits = Client.Account.Credits;
                                        Client.Account.Gifts = gifts.ToArray();
                                        Client.Account.Flush();
                                        Client.Account.Reload();
                                        break;
                                }
                            };

                            if (Inventory[4] != null && Inventory[5] != null && Inventory[6] != null && Inventory[7] != null &&
                                Inventory[8] != null && Inventory[9] != null && Inventory[10] != null && Inventory[11] != null)
                            {
                                SendHelp("Your inventory need free space to enchant this item.");
                                return true;
                            }
                            else
                                for (int i = 4; i < 12; i++)
                                    if (Inventory[i] == null)
                                    {
                                        Inventory[i] = GameServer.Manager.GameData.Items[(ushort) Utils.FromString(eff.Id)];
                                        action.Invoke();
                                        UpdateCount++;
                                        SaveToCharacter();
                                        return false;
                                    }
                            return true;
                        }
                    case ActivateEffects.PermaPet:
                    case ActivateEffects.PetSkin:
                    case ActivateEffects.Unlock:
                        {
                            if (Owner.Id == (int) WorldID.VAULT_ID)
                            {
                                SendInfo("You cannot use this item in Vault!");
                                return true;
                            }

                            if (eff.Slot == null)
                            {
                                SendInfo($"There is no unlock action for this item.");
                                return true;
                            }

                            string message;

                            switch (eff.Slot)
                            {
                                case "char":
                                    message = "Character Slot";
                                    GameServer.Manager.Database.AddChar(Client.Account);
                                    break;
                                case "vault":
                                    message = "Vault Chest";
                                    GameServer.Manager.Database.AddChest(Client.Account);
                                    break;
                                default:
                                    SendInfo($"There is no unlock action to slot '{eff.Slot}' in game actions.");
                                    return true;
                            }

                            SendInfo($"You have successfully received +1 {message}!");
                            UpdateCount++;
                            SaveToCharacter();
                            return false;
                        }
                    case ActivateEffects.MysteryDyes:
                    default:
                        return true;
                }
            }
            UpdateCount++;
            return endMethod;
        }

        #region "Activate Code Assit"

        private static void ActivateBoostStat(Player player, int idxnew, List<Message> pkts)
        {
            int OriginalStat = 0;
            OriginalStat = player.Stats[idxnew] + OriginalStat;
            Oldstat = OriginalStat;
        }

        private void ActivateHealHp(Player player, int amount, List<Message> pkts)
        {
            int maxHp = player.Stats[0] + player.Boost[0];
            int newHp = Math.Min(maxHp, player.HP + amount);
            if (newHp != player.HP)
            {
                pkts.Add(new SHOWEFFECT
                {
                    EffectType = EffectType.Heal,
                    TargetId = player.Id,
                    Color = new ARGB(0xffffffff)
                });
                pkts.Add(new NOTIFICATION
                {
                    Color = new ARGB(0xff00ff00),
                    ObjectId = player.Id,
                    Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"+" + (newHp - player.HP) + "\"}}"
                });
                player.HP = newHp;
                player.UpdateCount++;
            }
        }

        private bool ActivateHealMp(Player player, int amount, List<Message> pkts)
        {
            int maxMp = player.Stats[1] + player.Boost[1];
            int newMp = Math.Min(maxMp, player.MP + amount);
            if (newMp != player.MP)
            {
                pkts.Add(new SHOWEFFECT
                {
                    EffectType = EffectType.Heal,
                    TargetId = player.Id,
                    Color = new ARGB(0x6084e0)
                });
                pkts.Add(new NOTIFICATION
                {
                    Color = new ARGB(0x6084e0),
                    ObjectId = player.Id,
                    Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"+" + (newMp - player.MP) + "\"}}"
                });
                player.MP = newMp;
                player.UpdateCount++;
            }

            return false;
        }

        private void Shoot(RealmTime time, Item item, Position target)
        {
            double arcGap = item.ArcGap * Math.PI / 180;
            double startAngle = Math.Atan2(target.Y - Y, target.X - X) - (item.NumProjectiles - 1) / 2 * arcGap;
            ProjectileDesc prjDesc = item.Projectiles[0]; //Assume only one

            for (int i = 0; i < item.NumProjectiles; i++)
            {
                Projectile proj = CreateProjectile(prjDesc, item.ObjectType,
                    (int) StatsManager.GetAttackDamage(prjDesc.MinDamage, prjDesc.MaxDamage),
                    time.TotalElapsedMs, new Position { X = X, Y = Y }, (float) (startAngle + arcGap * i));
                Owner?.EnterWorld(proj);
                FameCounter.Shoot(proj);
            }
        }

        private void PoisonEnemy(Enemy enemy, ActivateEffect eff)
        {
            if (enemy.ObjectDesc.Pet)
                return;

            try
            {
                if (eff.ConditionEffect != null)
                    enemy?.ApplyConditionEffect(new[]
                    {
                        new ConditionEffect
                        {
                            Effect = (ConditionEffectIndex) eff.ConditionEffect,
                            DurationMS = (int) eff.EffectDuration
                        }
                    });
                int remainingDmg = (int) StatsManager.GetDefenseDamage(enemy, eff.TotalDamage, enemy.ObjectDesc.Defense);
                int perDmg = remainingDmg * 1000 / eff.DurationMS;
                WorldTimer tmr = null;
                int x = 0;
                tmr = new WorldTimer(100, (w, t) =>
                {
                    if (enemy.Owner == null)
                        return;
                    w.BroadcastMessage(new SHOWEFFECT
                    {
                        EffectType = EffectType.Poison,
                        TargetId = enemy.Id,
                        Color = new ARGB(0xffddff00)
                    }, null);

                    if (x % 10 == 0)
                    {
                        int thisDmg;
                        if (remainingDmg < perDmg)
                            thisDmg = remainingDmg;
                        else
                            thisDmg = perDmg;

                        enemy.Damage(this, t, thisDmg, true);
                        remainingDmg -= thisDmg;
                        if (remainingDmg <= 0)
                            return;
                    }
                    x++;

                    tmr.Reset();

                    GameServer.Manager.Logic.AddPendingAction(_ => w.Timers.Add(tmr), PendingPriority.Creation);
                });
                Owner?.Timers.Add(tmr);
            }
            catch (Exception) { }
        }

        private bool CheatEngineDetectSlot(Item item, USEITEM pkt, IContainer con) => pkt?.SlotObject.SlotId != 255 && pkt?.SlotObject.SlotId != 254 && con?.Inventory[pkt.SlotObject.SlotId] != item;

        private bool CheatEngineDetect(Item item, USEITEM pkt)
        {
            foreach (Player player in Owner?.Players.Values)
                if (player?.Client.Account.AccountType >= (int) Core.config.AccountType.TUTOR_ACCOUNT)
                    player.SendInfo(string.Format("Cheat engine detected for player {0},\nItem should be {1}, but its {2}.",
                Name, Inventory[pkt.SlotObject.SlotId].ObjectId, item.ObjectId));
            GameServer.Manager.TryDisconnect(Client, DisconnectReason.CHEAT_ENGINE_DETECTED);
            return true;
        }

        private bool Backpack()
        {
            if (HasBackpack)
                return true;
            Client.Character.Backpack = new[] { -1, -1, -1, -1, -1, -1, -1, -1 };
            HasBackpack = true;
            Client.Character.HasBackpack = true;
            Client?.Save();
            Array.Resize(ref inventory, 20);
            int[] slotTypes =
                Utils.FromCommaSepString32(
                    GameServer.Manager.GameData.ObjectTypeToElement[ObjectType].Element("SlotTypes").Value);
            Array.Resize(ref slotTypes, 20);
            for (int i = 0; i < slotTypes.Length; i++)
                if (slotTypes[i] == 0)
                    slotTypes[i] = 10;
            SlotTypes = slotTypes;
            return false;
        }

        private bool XpBooster(Item item)
        {
            // Disable XpBoosters for now, no clue yet what is causing weird issue when player use it, maybe instances not even implemented yet.
            return true;
            //if (!XpBoosted)
            //{
            //    XpBoostTimeLeft = (float)item.Timer;
            //    XpBoosted = item.XpBooster;
            //    xpFreeTimer = (float)item.Timer == -1.0 ? false : true;
            //    return false;
            //}
            //else
            //{
            //    SendInfo("You have already an active XP Booster.");
            //    return true;
            //}
        }

        private bool LootDropBooster(Item item)
        {
            // Disable LootDropBooster for now.
            return true;
            //if (!LootDropBoost)
            //{
            //    LootDropBoostTimeLeft = (float)item.Timer;
            //    lootDropBoostFreeTimer = (float)item.Timer == -1.0 ? false : true;
            //    return false;
            //}
            //else
            //{
            //    SendInfo("You have already an active Loot Drop Booster.");
            //    return true;
            //}
        }

        private bool LootTierBooster(Item item)
        {
            // Disable LootTierBooster for now.
            return true;
            //if (!LootTierBoost)
            //{
            //    LootTierBoostTimeLeft = (float)item.Timer;
            //    lootTierBoostFreeTimer = (float)item.Timer == -1.0 ? false : true;
            //    return false;
            //}
            //else
            //{
            //    SendInfo("You have already an active Loot Tier Booster.");
            //    return true;
            //}
        }

        //TODO: Should be implemented again c:
        private bool PermaPet()
        {
            return true;
        }

        //TODO: needs implementation
        private bool PetSkin()
        {
            return true;
        }

        //TODO: needs implementation
        private bool Exchange()
        {
            return true;
        }

        //TODO: needs implementation
        private bool MysteryDyes()
        {
            return true;
        }

        //TODO: needs implementation
        private bool Unlock()
        {
            return true;
        }

        public enum EggRarity
        {
            Common,
            Uncommon,
            Rare,
            Legendary
        }

        #endregion "Activate Code Assit"
    }
}