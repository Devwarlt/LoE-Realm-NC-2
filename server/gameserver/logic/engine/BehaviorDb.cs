#region

using LoESoft.Core;
using LoESoft.Core.models;
using LoESoft.GameServer.logic.loot;
using LoESoft.GameServer.realm;
using LoESoft.GameServer.realm.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

#endregion

namespace LoESoft.GameServer.logic
{
    public partial class BehaviorDb
    {
        private static wRandom rand = new wRandom();

        private static int initializing;
        internal static BehaviorDb InitDb;
        private static int randCount = 0;

        internal static wRandom Random
        {
            get
            {
                if (randCount > 10)
                {
                    rand = new wRandom();
                    randCount = 0;
                }
                randCount++;
                return rand;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public BehaviorDb()
        {
            Definitions = new Dictionary<ushort, Tuple<State, Loot>>();

            if (Interlocked.Exchange(ref initializing, 1) == 1)
            {
                Log.Error("Attempted to initialize multiple BehaviorDb at the same time.");
                throw new InvalidOperationException("Attempted to initialize multiple BehaviorDb at the same time.");
            }
            InitDb = this;

            FieldInfo[] fields = GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.FieldType == typeof(_))
                .ToArray();

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                ((_)field.GetValue(this))();
                field.SetValue(this, null);
            }

            Log._("Behavior", fields.Length);

            InitDb = null;
            initializing = 0;
        }

        internal static EmbeddedData InitGameData
        { get { return GameServer.Manager.GameData; } }

        public Dictionary<ushort, Tuple<State, Loot>> Definitions
        { get; private set; }

        public void ResolveBehavior(Entity entity)
        {
            if (Definitions.TryGetValue(entity.ObjectType, out Tuple<State, Loot> def))
                entity.SwitchTo(def.Item1);
        }

        public static Ctor Behav() => new Ctor();

        public delegate Ctor _();

        public struct Ctor
        {
            public Ctor Init(string objType, State rootState, params ILootDef[] defs)
            {
                var d = new Dictionary<string, State>();

                rootState.Resolve(d);
                rootState.ResolveChildren(d);

                var dat = GameServer.Manager.GameData;

                try
                {
                    if (InitDb.Definitions.ContainsKey(dat.IdToObjectType[objType]))
                        Log.Warn($"Duplicated behavior for entity '{objType}'.");
                    else
                    {
                        if (defs.Length > 0)
                        {
                            var loot = new Loot(defs);
                            rootState.Death += (sender, e) => loot.Handle((Enemy)e.Host, e.Time);
                            InitDb.Definitions.Add(dat.IdToObjectType[objType], new Tuple<State, Loot>(rootState, loot));
                        }
                        else
                            InitDb.Definitions.Add(dat.IdToObjectType[objType], new Tuple<State, Loot>(rootState, null));
                    }
                }
                catch (KeyNotFoundException)
                { Log.Warn($"[State: {rootState.ToString()}] There is no definition for entity '{objType}' in game assets."); }

                return this;
            }
        }
    }
}