namespace LoESoft.GameServer.realm.mapsetpiece
{
    internal class LostSpirit : MapSetPiece
    {
        public override int Size => 3;

        public override void RenderSetPiece(World world, IntPoint pos)
        {
            var entity = Entity.Resolve("The Lost Spirit");
            entity.Move(pos.X + 2.5f, pos.Y + 2.5f);
            world.EnterWorld(entity);
        }
    }
}