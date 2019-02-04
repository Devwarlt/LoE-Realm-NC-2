#region

using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public class Flashing : Behavior
    {
        private readonly uint color;
        private readonly float flashPeriod;
        private readonly int flashRepeats;

        public Flashing(
            uint color,
            double flashPeriod,
            int flashRepeats
            )
        {
            this.color = color;
            this.flashPeriod = (float)flashPeriod;
            this.flashRepeats = flashRepeats;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            host.Owner.BroadcastMessage(new SHOWEFFECT
            {
                EffectType = EffectType.Flash,
                PosA = new Position { X = flashPeriod, Y = flashRepeats },
                TargetId = host.Id,
                Color = new ARGB(color)
            }, null);
        }
    }
}