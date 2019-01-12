namespace LoESoft.GameServer.realm.entity
{
    public abstract class Character : Entity
    {
        public Character(ushort objType, wRandom rand)
            : base(objType)
        {
            Random = rand;

            if (ObjectDesc == null)
                return;
            Name = ObjectDesc.DisplayId ?? "";
            if (ObjectDesc.SizeStep != 0)
            {
                int step = Random.Next(0, (ObjectDesc.MaxSize - ObjectDesc.MinSize) / ObjectDesc.SizeStep + 1) *
                           ObjectDesc.SizeStep;
                Size = ObjectDesc.MinSize + step;
            }
            else
                Size = ObjectDesc.MinSize;

            HP = (int) ObjectDesc.MaxHP;
        }

        public new wRandom Random { get; private set; }

        public int HP { get; set; }
		public int MaxHP { get; set; }
	}
}