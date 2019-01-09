namespace LoESoft.GameServer.realm.mapsetpiece
{
    internal class Maurth : MapSetPiece
    {
        public override int Size => 3;

        public override void RenderSetPiece(World world, IntPoint pos)
        {
            var entity = Entity.Resolve("Maurth the Succubus Princess");
            entity.Move(pos.X + 2.5f, pos.Y + 2.5f);
            world.EnterWorld(entity);
        }
    }
}