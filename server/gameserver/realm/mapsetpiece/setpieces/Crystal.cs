using System;

namespace LoESoft.GameServer.realm.mapsetpiece
{
    internal class Crystal : MapSetPiece
    {
        public override int Size => 5;

        public override void RenderSetPiece(World world, IntPoint pos)
        {
            var rnd = new Random();
            var crystal = Entity.Resolve("Mysterious Crystal");
            crystal.Move(rnd.Next(900, 1000) + 0.5f, rnd.Next(900, 1000) + 0.5f);
            world.EnterWorld(crystal);
        }
    }
}