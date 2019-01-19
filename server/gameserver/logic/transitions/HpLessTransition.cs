#region

using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;

#endregion

namespace LoESoft.GameServer.logic.transitions
{
    public class HpLessTransition : Transition, INonSkippableState
    {
        // State storage: none
        private string Name { get; set; }

        private int InvalidHP { get; set; }
        private readonly double threshold;

        public HpLessTransition(double threshold, string targetState)
            : base(targetState)
        {
            this.threshold = threshold;
            Skip = false;
            DoneAction = false;
            DoneStorage = false;
        }

        // Exclusive for new interface: INonSkipState
        public Enemy Enemy { get; set; }

        public bool DoneAction { get; set; }
        public bool DoneStorage { get; set; }
        public bool Skip { get; set; }
        public int StoreHP { get; set; }

        public void ManageHP(int stored, int threshold)
        {
            StoreHP = stored - threshold;
            DoneStorage = true;
        }

        protected override bool TickCore(Entity host, RealmTime time, ref object state)
        {
            if (threshold > 1)
                return true; // must skip
            else
            {
                if (!DoneAction)
                {
                    Enemy = host as Enemy;
                    Name = Enemy.Name;
                    StoreHP = Enemy.HP;
                    DoneAction = true;
                }

                if (Skip)
                {
                    int Damage = (int)(StoreHP - threshold * host.ObjectDesc.MaxHP);
                    InvalidHP = Enemy.HP;
                    ManageHP(StoreHP, Damage);
                    return true;
                }
                else
                {
                    if (((host as Enemy).HP / host.ObjectDesc.MaxHP) < threshold)
                    {
                        (host as Enemy).HP = StoreHP - (int)(StoreHP - threshold * host.ObjectDesc.MaxHP);
                        return true;
                    }
                }

                return false;
            }
        }
    }
}