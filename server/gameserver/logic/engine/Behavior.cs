#region

using LoESoft.GameServer.realm;
using log4net;
using System;

#endregion

namespace LoESoft.GameServer.logic
{
    public abstract class Behavior : IStateChildren
    {
        public static ILog log = LogManager.GetLogger(nameof(Behavior));

        /// <summary>
        /// Enable debug log
        /// </summary>
        public static readonly bool debug = false;

        [ThreadStatic]
        private static Random rand;

        protected static Random Random
        {
            get
            {
                if (rand == null)
                    rand = new Random();
                return rand;
            }
        }

        public void Tick(Entity host, RealmTime time)
        {
            if (!host.StoredBehaviors.TryGetValue(this, out object state))
                state = null;

            try
            {
                TickCore(host, time, ref state);

                if (state == null)
                    host.StoredBehaviors.Remove(this);
                else
                    host.StoredBehaviors[this] = state;
            }
            catch (Exception) { }
        }

        protected abstract void TickCore(Entity host, RealmTime time, ref object state);

        public void OnStateEntry(Entity host, RealmTime time)
        {
            try
            {
                if (!host.StoredBehaviors.TryGetValue(this, out object state))
                    state = null;

                OnStateEntry(host, time, ref state);

                if (state == null)
                    host.StoredBehaviors.Remove(this);
                else
                    host.StoredBehaviors[this] = state;
            }
            catch { }
        }

        protected virtual void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
        }

        public void OnStateExit(Entity host, RealmTime time)
        {
            if (!host.StoredBehaviors.TryGetValue(this, out object state))
                state = null;

            OnStateExit(host, time, ref state);

            if (state == null)
                host.StoredBehaviors.Remove(this);
            else
                host.StoredBehaviors[this] = state;
        }

        protected virtual void OnStateExit(Entity host, RealmTime time, ref object state)
        {
        }

        protected internal virtual void Resolve(State parent)
        {
        }
    }
}