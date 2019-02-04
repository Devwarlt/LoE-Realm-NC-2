#region

using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity.player;
using Mono.Game;
using System;

#endregion

namespace LoESoft.GameServer.logic.behaviors.Drakes
{
    internal class DrakeFollow : CycleBehavior
    {
        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            FollowState s;
            if (state == null)
                s = new FollowState();
            else
                s = (FollowState)state;

            Status = CycleStatus.NotStarted;

            Player player = host.GetPlayerOwner();
            if (player.Owner == null)
            {
                host.Owner.LeaveWorld(host);
                return;
            }

            Vector2 vect;
            switch (s.State)
            {
                case F.DontKnowWhere:
                    if (s.RemainingTime > 0)
                        s.RemainingTime -= time.ElapsedMsDelta;
                    else
                        s.State = F.Acquired;
                    break;

                case F.Acquired:
                    if (player == null)
                    {
                        s.State = F.DontKnowWhere;
                        s.RemainingTime = 0;
                        break;
                    }
                    if (s.RemainingTime > 0)
                        s.RemainingTime -= time.ElapsedMsDelta;

                    vect = new Vector2(player.X - host.X, player.Y - host.Y);
                    if (vect.Length > 20)
                    {
                        host.Move(player.X, player.Y);
                        host.UpdateCount++;
                    }
                    else if (vect.Length > 1)
                    {
                        float dist = host.EntitySpeed(player.Stats[4] / 10, time);

                        if (vect.Length > 2 && vect.Length <= 3.5)
                            dist *= 1.75f;
                        else if (vect.Length > 3.5 && vect.Length <= 5)
                            dist *= 2f;
                        else if (vect.Length > 5 && vect.Length <= 6)
                            dist *= 2.25f;
                        else if (vect.Length > 6 && vect.Length <= 7)
                            dist *= 2.75f;
                        else if (vect.Length > 7 && vect.Length <= 10)
                            dist *= 3f;
                        else if (vect.Length > 10 && vect.Length <= 20)
                            dist *= 3.25f;

                        Status = CycleStatus.InProgress;
                        vect.X -= Random.Next(-2, 2) / 2f;
                        vect.Y -= Random.Next(-2, 2) / 2f;
                        vect.Normalize();
                        host.ValidateAndMove(host.X + vect.X * dist, host.Y + vect.Y * dist);
                        host.UpdateCount++;
                    }

                    break;
            }

            state = s;
        }

        private enum F
        {
            DontKnowWhere,
            Acquired,
        }

        private class FollowState
        {
            public int RemainingTime;
            public F State;
        }
    }
}