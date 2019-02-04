#region

using System;

#endregion

namespace LoESoft.GameServer.realm.entity.player
{
    partial class Player
    {
        private static readonly ConditionEffect[] NegativeEffs =
        {
            new ConditionEffect { Effect = ConditionEffectIndex.Slowed, DurationMS = 0 },
            new ConditionEffect { Effect = ConditionEffectIndex.Paralyzed, DurationMS = 0 },
            new ConditionEffect { Effect = ConditionEffectIndex.Weak, DurationMS = 0 },
            new ConditionEffect { Effect = ConditionEffectIndex.Stunned, DurationMS = 0 },
            new ConditionEffect { Effect = ConditionEffectIndex.Confused, DurationMS = 0 },
            new ConditionEffect { Effect = ConditionEffectIndex.Blind, DurationMS = 0 },
            new ConditionEffect { Effect = ConditionEffectIndex.Quiet, DurationMS = 0 },
            new ConditionEffect { Effect = ConditionEffectIndex.ArmorBroken, DurationMS = 0 },
            new ConditionEffect { Effect = ConditionEffectIndex.Bleeding, DurationMS = 0 },
            new ConditionEffect { Effect = ConditionEffectIndex.Dazed, DurationMS = 0 },
            new ConditionEffect { Effect = ConditionEffectIndex.Sick, DurationMS = 0 },
            new ConditionEffect { Effect = ConditionEffectIndex.Drunk, DurationMS = 0 },
            new ConditionEffect { Effect = ConditionEffectIndex.Hallucinating, DurationMS = 0 },
            new ConditionEffect { Effect = ConditionEffectIndex.Hexed, DurationMS = 0 },
            new ConditionEffect { Effect = ConditionEffectIndex.Unstable, DurationMS = 0  }
        };

        private void HandleEffects(RealmTime time)
        {
            if (HasConditionEffect(ConditionEffectIndex.Healing))
            {
                if (healing > 1)
                {
                    HP = Math.Min(Stats[0] + Boost[0], HP + (int)healing);
                    healing -= (int)healing;
                    UpdateCount++;
                    healCount++;
                }
                healing += 28 * (time.ElapsedMsDelta / 1000f);
            }
            if (HasConditionEffect(ConditionEffectIndex.Quiet) &&
                MP > 0)
            {
                MP = 0;
                UpdateCount++;
            }
            if (HasConditionEffect(ConditionEffectIndex.Bleeding) &&
                HP > 1)
            {
                if (bleeding > 1)
                {
                    HP -= (int)bleeding;
                    bleeding -= (int)bleeding;
                    UpdateCount++;
                }
                bleeding += 28 * (time.ElapsedMsDelta / 1000f);
            }

            if (newbieTime > 0)
            {
                newbieTime -= time.ElapsedMsDelta;
                if (newbieTime < 0)
                    newbieTime = 0;
            }
            if (CanTPCooldownTime > 0)
            {
                CanTPCooldownTime -= time.ElapsedMsDelta;
                if (CanTPCooldownTime < 0)
                    CanTPCooldownTime = 0;
            }
        }
    }
}