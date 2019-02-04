using LoESoft.GameServer.realm.terrain;
using System.Collections.Generic;

namespace LoESoft.GameServer.realm
{
    internal partial class Realm
    {
        public class Spawn
        {
            public WmapTerrain WmapTerrain { get; set; }
            public int Density { get; set; }
            public List<KeyValuePair<string, double>> Entities { get; set; }

            public Spawn(
                WmapTerrain WmapTerrain,
                int Density,
                List<KeyValuePair<string, double>> Entities
                )
            {
                this.WmapTerrain = WmapTerrain;
                this.Density = Density;
                this.Entities = Entities;
            }
        }

        private static KeyValuePair<string, double> Add(string name, double probability) =>
            new KeyValuePair<string, double>(name, probability);

        #region "Spawn data"

        public readonly List<Spawn> RealmSpawnCache = new List<Realm.Spawn>
        {
            new Spawn(WmapTerrain.ShoreSand, 750,
                new List<KeyValuePair<string, double>>
                {
                    Add("Pirate", 0.3),
                    Add("Piratess", 0.1),
                    Add("Snake", 0.2),
                    Add("Scorpion Queen", 0.4)
                }),
            new Spawn(WmapTerrain.ShorePlains, 325,
                new List<KeyValuePair<string, double>>
                {
                    Add("Bandit Leader", 0.4),
                    Add("Red Gelatinous Cube", 0.2),
                    Add("Purple Gelatinous Cube", 0.2),
                    Add("Green Gelatinous Cube", 0.2)
                }),
            new Spawn(WmapTerrain.LowPlains, 750,
                new List<KeyValuePair<string, double>>
                {
                    Add("Hobbit Mage", 0.5),
                    Add("Undead Hobbit Mage", 0.4),
                    Add("Sumo Master", 0.1)
                }),
            new Spawn(WmapTerrain.LowForest, 1250,
                new List<KeyValuePair<string, double>>
                {
                    Add("Elf Wizard", 0.2),
                    Add("Goblin Mage", 0.2),
                    Add("Easily Enraged Bunny", 0.3),
                    Add("Forest Nymph", 0.3)
                }),
            new Spawn(WmapTerrain.LowSand, 1875,
                new List<KeyValuePair<string, double>>
                {
                    Add("Sandsman King", 0.4),
                    Add("Giant Crab", 0.2),
                    Add("Sand Devil", 0.4)
                }),
            new Spawn(WmapTerrain.MidPlains, 1875,
                new List<KeyValuePair<string, double>>
                {
                    Add("Fire Sprite", 0.1),
                    Add("Ice Sprite", 0.1),
                    Add("Magic Sprite", 0.1),
                    Add("Pink Blob", 0.07),
                    Add("Gray Blob", 0.07),
                    Add("Earth Golem", 0.04),
                    Add("Paper Golem", 0.04),
                    Add("Big Green Slime", 0.08),
                    Add("Swarm", 0.05),
                    Add("Wasp Queen", 0.2),
                    Add("Shambling Sludge", 0.03),
                    Add("Orc King", 0.06)
                }),
            new Spawn(WmapTerrain.MidForest, 1250,
                new List<KeyValuePair<string, double>>
                {
                    Add("Dwarf King", 0.3),
                    Add("Metal Golem", 0.05),
                    Add("Clockwork Golem", 0.05),
                    Add("Werelion", 0.1),
                    Add("Horned Drake", 0.3),
                    Add("Red Spider", 0.1),
                    Add("Black Bat", 0.1)
                }),
            new Spawn(WmapTerrain.MidSand, 250,
                new List<KeyValuePair<string, double>>
                {
                    Add("Desert Werewolf", 0.25),
                    Add("Fire Golem", 0.1),
                    Add("Darkness Golem", 0.1),
                    Add("Sand Phantom", 0.2),
                    Add("Nomadic Shaman", 0.25),
                    Add("Great Lizard", 0.1)
                }),
            new Spawn(WmapTerrain.HighPlains, 875,
                new List<KeyValuePair<string, double>>
                {
                    Add("Shield Orc Key", 0.2),
                    Add("Urgle", 0.2),
                    Add("Undead Dwarf God", 0.6)
                }),
            new Spawn(WmapTerrain.HighForest, 1250,
                new List<KeyValuePair<string, double>>
                {
                    Add("Ogre King", 0.4),
                    Add("Dragon Egg", 0.1),
                    Add("Lizard God", 0.5)
                }),
            new Spawn(WmapTerrain.HighSand, 125,
                new List<KeyValuePair<string, double>>
                {
                    Add("Minotaur", 0.4),
                    Add("Flayer God", 0.4),
                    Add("Flamer King", 0.2)
                }),
            new Spawn(WmapTerrain.Mountains, 100,
                new List<KeyValuePair<string, double>>
                {
                    Add("White Demon", 0.1),
                    Add("Sprite God", 0.09),
                    Add("Medusa", 0.1),
                    Add("Ent God", 0.1),
                    Add("Beholder", 0.1),
                    Add("Flying Brain", 0.1),
                    Add("Slime God", 0.09),
                    Add("Ghost God", 0.09),
                    Add("Rock Bot", 0.05),
                    Add("Djinn", 0.09),
                    Add("Leviathan", 0.09),
                    Add("Arena Headless Horseman", 0.01)
                })
        };

        #endregion "Spawn data"
    }
}