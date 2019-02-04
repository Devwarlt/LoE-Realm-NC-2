#region

using Mono.Game;
using System;

#endregion

namespace LoESoft.GameServer.realm.entity
{
    partial class Decoy
    {
        public void Damage(int dmg, Entity chr, bool NoDef, bool manaDrain = false)
        {
        }

        public bool IsVisibleToEnemy()
        {
            return true;
        }

        private Vector2 GetRandDirection()
        {
            double angle = rand.NextDouble() * 2 * Math.PI;
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
    }
}