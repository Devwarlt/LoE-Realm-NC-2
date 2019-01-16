using LoESoft.GameServer.realm.entity;
using LoESoft.GameServer.realm.entity.player;
using LoESoft.GameServer.realm.mapsetpiece;
using System.Collections.Generic;

namespace LoESoft.GameServer.realm
{
    internal partial class Realm
    {
        public readonly List<RealmEvent> RealmEventCache = new List<RealmEvent>
        {
            new RealmEvent("Skull Shrine", 1, false, new SkullShrine(), "Your futile efforts are no match for a Skull Shrine!"),
            new RealmEvent("Pentaract",  1, false, new Pentaract(), "Behold my Pentaract, and despair!"),
            new RealmEvent("Grand Sphinx", 0.25, true, new Sphinx(), "At last, a Grand Sphinx will teach you to respect!"),
            new RealmEvent("Cube God", 1, false, new CubeGod(), "Your meager abillities cannot possibly challenge a Cube God!"),
            new RealmEvent("Maurth the Succubus Princess", 0.25, false, new Maurth(), "Haha!! My Maurth the Succubus Princess will SUCC the Life out of you!"),
            new RealmEvent("Undertaker the Great Juggernaut", 0.15, true, new Undertaker(), "You Humans are fools! My Undertaker the Great Juggernaut will take care to crush your spines!"),
            new RealmEvent("Dyno Bot", 0.3, false, new DynoBot(), "BEWARE FOOLS! My Dyno Bot mutes, kicks and bans!"),
            //new RealmEvent("The Lost Spirit", 0.15, false, new LostSpirit(), "The ancient soul of my father still presides within this realm.. and now he has awoken.. YOU ARE DOOMED MORTAL!"),
            new RealmEvent("Lucky Ent God", 1, true, new LuckyEntGod(), "Lucky Ent God has been spawned!"),
            new RealmEvent("Lucky Djinn", 1, true, new LuckyDjinn(), "Lucky Djinn has been spawned!"),
            new RealmEvent("Lord of the Lost Lands", 1, true, new LordOfTheLostLands(), "Cower in fear of my Lord of the Lost Lands!"),
            new RealmEvent("Encounter Altar", 1, true, new MountainTemple(), "Fools! The Mountain Temple is protected by the mighty Jade and Garnet statues!"),
            new RealmEvent("shtrs Defense System", 0.1, true, new Avatar(), "Attacking the Avatar of the Forgotten King would be... unwise."),
            new RealmEvent("Ghost Ship", 0.2, false, new GhostShip(), "My Ghost Ship will terrorize you pathetic peasants!")
        };

        public List<string> UniqueEvents { get; set; } = new List<string>();
        public List<string> ActualRunningEvents { get; set; } = new List<string>();

        public void HandleRealmEvent(Enemy enemy, Player killer)
        {
            if (enemy.ObjectDesc != null)
            {
                var name = enemy.ObjectDesc.DisplayId;

                if (ActualRunningEvents.Contains(name))
                    ActualRunningEvents.Remove(name);

                name = null;

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
            }
        }

        public class RealmEvent
        {
            public string Name { get; set; }
            public double Probability { get; set; }
            public bool Once { get; set; }
            public MapSetPiece MapSetPiece { get; set; }
            public string Message { get; set; }

            public RealmEvent(
                string Name,
                double Probability,
                bool Once,
                MapSetPiece MapSetPiece,
                string Message
                )
            {
                this.Name = Name;
                this.Probability = Probability;
                this.Once = Once;
                this.MapSetPiece = MapSetPiece;
                this.Message = Message;
            }
        }
    }
}