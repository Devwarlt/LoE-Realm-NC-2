using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.mapsetpiece;
using System.Collections.Generic;

namespace LoESoft.GameServer.realm
{
    internal partial class Realm
    {
        public readonly double RealmEventProbability = 0.75; // 75%

        public readonly List<RealmEvent> RealmEventCache = new List<RealmEvent>
        {
            new RealmEvent("Skull Shrine",  new SkullShrine(), "Your futile efforts are no match for a Skull Shrine!"),
            new RealmEvent("Pentaract",  new Pentaract(), "Behold my Pentaract, and despair!"),
            new RealmEvent("Grand Sphinx",  new Sphinx(), "At last, a Grand Sphinx will teach you to respect!"),
            new RealmEvent("Cube God",  new CubeGod(), "Your meager abillities cannot possibly challenge a Cube God!"),
            new RealmEvent("Dream Island Horde",  new DreamIsle(), "Fools! your futile efforts are no match for a Dream Island Horde!"),
            new RealmEvent("Maurth the Succubus Princess", new Maurth(), "Haha!! My Maurth the Succubus Princess will SUCC the Life out of you!"),
            new RealmEvent("Undertaker the Great Juggernaut", new Undertaker(), "You Humans are fools! My Undertaker the Great Juggernaut will take care to crush your spines!")
        };

        public void HandleRealmEvent(Enemy enemy, Player killer)
        {
            if (enemy.ObjectDesc != null)
            {
                TauntData? dat = null;

                foreach (var i in criticalEnemies)
                    if ((enemy.ObjectDesc.DisplayId ?? enemy.ObjectDesc.ObjectId) == i.Item1)
                    {
                        dat = i.Item2;
                        break;
                    }

                if (dat == null)
                    return;

                if (dat.Value.killed != null)
                {
                    string[] arr = dat.Value.killed;
                    string msg = arr[rand.Next(0, arr.Length)];

                    while (killer == null && msg.Contains("{PLAYER}"))
                        msg = arr[rand.Next(0, arr.Length)];

                    msg = msg.Replace("{PLAYER}", killer.Name);

                    BroadcastMsg(msg);
                }

                if (rand.NextDouble() < RealmEventProbability)
                {
                    RealmEvent evt = RealmEventCache[rand.Next(0, RealmEventCache.Count)];

                    try
                    {
                        // only for few events.
                        if (GameServer.Manager.GameData.ObjectDescs[GameServer.Manager.GameData.IdToObjectType[evt.Name]].PerRealmMax == 1)
                            RealmEventCache.Remove(evt);
                    }
                    catch { }

                    SpawnEvent(evt.Name, evt.MapSetPiece);

                    BroadcastMsg(evt.Message);

                    dat = null;

                    foreach (var i in criticalEnemies)
                        if (evt.Name == i.Item1)
                        {
                            dat = i.Item2;
                            break;
                        }

                    if (dat == null)
                        return;

                    if (dat.Value.spawn != null)
                    {
                        string[] arr = dat.Value.spawn;
                        string msg = arr[rand.Next(0, arr.Length)];

                        BroadcastMsg(msg);
                    }
                }
            }
        }

        public class RealmEvent
        {
            public string Name { get; set; }
            public MapSetPiece MapSetPiece { get; set; }
            public string Message { get; set; }

            public RealmEvent(
                string Name,
                MapSetPiece MapSetPiece,
                string Message
                )
            {
                this.Name = Name;
                this.MapSetPiece = MapSetPiece;
                this.Message = Message;
            }
        }
    }
}