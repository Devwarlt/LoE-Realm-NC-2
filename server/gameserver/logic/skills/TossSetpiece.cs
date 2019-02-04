#region

using LoESoft.GameServer.networking.outgoing;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.mapsetpiece;
using System;

#endregion

namespace LoESoft.GameServer.logic.behaviors
{
    public enum SpecialSetPiece : byte
    {
        ABYSS_IDOL = 0,
        ON_SELF = 255
    }

    public class TossSetpiece : Behavior
    {
        private readonly string _setpiece;
        private readonly double _range;
        private readonly uint _color;
        private Cooldown _coolDown;
        private readonly int _coolDownOffset;
        private SpecialSetPiece _special;

        public TossSetpiece(
            string setpiece,
            double range = 8,
            uint color = 0xFF0000,
            Cooldown coolDown = new Cooldown(),
            int cooldownOffset = 0,
            SpecialSetPiece special = SpecialSetPiece.ON_SELF
            )
        {
            _setpiece = setpiece;
            _range = range;
            _color = color;
            _coolDown = coolDown.Normalize();
            _coolDownOffset = cooldownOffset;
            _special = special;
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = _coolDownOffset;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            if (state == null)
                return;

            int cool = (int)state;

            ProcessType(_special, cool, host, time);

            state = cool;
        }

        private void ProcessType(SpecialSetPiece type, int cool, Entity host, RealmTime time)
        {
            if (host.HasConditionEffect(ConditionEffectIndex.Stunned))
                return;

            var piece = (MapSetPiece)Activator.CreateInstance(Type.GetType($"server.realm.mapsetpiece.{(_special == SpecialSetPiece.ON_SELF ? "" : "special.")}" + _setpiece, true, true));

            switch (type)
            {
                case SpecialSetPiece.ABYSS_IDOL:
                    {
                        if (cool <= 0)
                        {
                            Entity player = host.GetNearestEntity(_range, null);

                            if (player != null)
                            {
                                Position target = new Position
                                {
                                    X = player.X,
                                    Y = player.Y
                                };

                                host?.Owner.BroadcastMessage(new SHOWEFFECT
                                {
                                    EffectType = EffectType.Throw,
                                    Color = new ARGB(_color),
                                    TargetId = host.Id,
                                    PosA = target
                                }, null);

                                piece.RenderSetPiece(host.Owner, new IntPoint((int)target.X, (int)target.Y));
                            }

                            cool = _coolDown.Next(Random);
                        }
                        else
                            cool -= time.ElapsedMsDelta;
                    }
                    break;

                default:
                    {
                        piece.RenderSetPiece(host.Owner, new IntPoint((int)host.X, (int)host.Y));
                    }
                    break;
            }
        }
    }
}