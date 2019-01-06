#region

using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using System;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class DropPortalOnDeath : Behavior
    {
        private readonly int despawnTime;
        private readonly int dropDelay;
        private readonly ushort portalID;
        private readonly double percent;
        private readonly string stringObjType;
        private readonly float xAdjustment;
        private readonly float yAdjustment;

        public DropPortalOnDeath(
            string portalName,
            double percent,
            int dropDelaySec = 30,
            float XAdjustment = 0,
            float YAdjustment = 0,
            int PortalDespawnTimeSec = 30
            )
        {
            stringObjType = portalName;
            this.percent = percent;
            xAdjustment = XAdjustment;
            yAdjustment = YAdjustment;
            dropDelay = dropDelaySec;
            despawnTime = PortalDespawnTimeSec;
        }

        public DropPortalOnDeath(
            ushort portalID,
            int percent,
            int dropDelaySec = 30,
            float XAdjustment = 0,
            float YAdjustment = 0,
            int PortalDespawnTimeSec = 30
            )
        {
            this.portalID = portalID;
            stringObjType = null;
            this.percent = percent;
            xAdjustment = XAdjustment;
            yAdjustment = YAdjustment;
            dropDelay = dropDelaySec;
            despawnTime = PortalDespawnTimeSec;
        }

        protected internal override void Resolve(State parent)
        {
            parent.Death += (sender, e) =>
            {
                try
                {
                    if (e.Host.Owner.Name == "Arena")
                        return;

                    if (new Random().NextDouble() <= percent)
                    {
                        Portal entity = portalID == 0
                            ? Entity.Resolve(stringObjType) as Portal
                            : Entity.Resolve(portalID) as Portal;
                        Entity en = e.Host;
                        World w = GameServer.Manager.GetWorld(e.Host.Owner.Id);
                        entity.Move(en.X + xAdjustment, en.Y + yAdjustment);
                        w.Timers.Add(new WorldTimer(dropDelay * 1000, (world, t) => { w.EnterWorld(entity); }));
                        w.Timers.Add(new WorldTimer(despawnTime * 1000, (world, t) =>
                        {
                            try
                            { w.LeaveWorld(entity); }
                            catch (Exception ex) { log.ErrorFormat("Couldn't despawn portal.\n{0}", ex); }
                        }));
                    }
                }
                catch { }
            };
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
        }
    }
}