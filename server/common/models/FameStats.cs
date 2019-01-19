#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion

namespace LoESoft.Core
{
    public class FameStats
    {
        public int Shots { get; set; }
        public int ShotsThatDamage { get; set; }
        public int SpecialAbilityUses { get; set; }
        public int TilesUncovered { get; set; }
        public int Teleports { get; set; }
        public int PotionsDrunk { get; set; }
        public int MonsterKills { get; set; }
        public int MonsterAssists { get; set; }
        public int GodKills { get; set; }
        public int GodAssists { get; set; }
        public int CubeKills { get; set; }
        public int OryxKills { get; set; }
        public int QuestsCompleted { get; set; }
        public int PirateCavesCompleted { get; set; }
        public int UndeadLairsCompleted { get; set; }
        public int AbyssOfDemonsCompleted { get; set; }
        public int SnakePitsCompleted { get; set; }
        public int SpiderDensCompleted { get; set; }
        public int SpriteWorldsCompleted { get; set; }
        public int LevelUpAssists { get; set; }
        public int MinutesActive { get; set; }
        public int TombsCompleted { get; set; }
        public int TrenchesCompleted { get; set; }
        public int JunglesCompleted { get; set; }
        public int ManorsCompleted { get; set; }

        public static FameStats Read(byte[] bytes)
        {
            var ret = new FameStats();
            var reader = new NReader(new MemoryStream(bytes));
            byte id;
            while (reader.PeekChar() != -1)
            {
                id = reader.ReadByte();
                switch (id)
                {
                    case 0:
                        ret.Shots = reader.ReadInt32();
                        break;

                    case 1:
                        ret.ShotsThatDamage = reader.ReadInt32();
                        break;

                    case 2:
                        ret.SpecialAbilityUses = reader.ReadInt32();
                        break;

                    case 3:
                        ret.TilesUncovered = reader.ReadInt32();
                        break;

                    case 4:
                        ret.Teleports = reader.ReadInt32();
                        break;

                    case 5:
                        ret.PotionsDrunk = reader.ReadInt32();
                        break;

                    case 6:
                        ret.MonsterKills = reader.ReadInt32();
                        break;

                    case 7:
                        ret.MonsterAssists = reader.ReadInt32();
                        break;

                    case 8:
                        ret.GodKills = reader.ReadInt32();
                        break;

                    case 9:
                        ret.GodAssists = reader.ReadInt32();
                        break;

                    case 10:
                        ret.CubeKills = reader.ReadInt32();
                        break;

                    case 11:
                        ret.OryxKills = reader.ReadInt32();
                        break;

                    case 12:
                        ret.QuestsCompleted = reader.ReadInt32();
                        break;

                    case 13:
                        ret.PirateCavesCompleted = reader.ReadInt32();
                        break;

                    case 14:
                        ret.UndeadLairsCompleted = reader.ReadInt32();
                        break;

                    case 15:
                        ret.AbyssOfDemonsCompleted = reader.ReadInt32();
                        break;

                    case 16:
                        ret.SnakePitsCompleted = reader.ReadInt32();
                        break;

                    case 17:
                        ret.SpiderDensCompleted = reader.ReadInt32();
                        break;

                    case 18:
                        ret.SpriteWorldsCompleted = reader.ReadInt32();
                        break;

                    case 19:
                        ret.LevelUpAssists = reader.ReadInt32();
                        break;

                    case 20:
                        ret.MinutesActive = reader.ReadInt32();
                        break;

                    case 21:
                        ret.TombsCompleted = reader.ReadInt32();
                        break;

                    case 22:
                        ret.TrenchesCompleted = reader.ReadInt32();
                        break;

                    case 23:
                        ret.JunglesCompleted = reader.ReadInt32();
                        break;

                    case 24:
                        ret.ManorsCompleted = reader.ReadInt32();
                        break;
                }
            }
            return ret;
        }

        public byte[] Write()
        {
            var dat = new Tuple<byte, int>[]
            {
                new Tuple<byte, int>(0, Shots),
                new Tuple<byte, int>(1, ShotsThatDamage),
                new Tuple<byte, int>(2, SpecialAbilityUses),
                new Tuple<byte, int>(3, TilesUncovered),
                new Tuple<byte, int>(4, Teleports),
                new Tuple<byte, int>(5, PotionsDrunk),
                new Tuple<byte, int>(6, MonsterKills),
                new Tuple<byte, int>(7, MonsterAssists),
                new Tuple<byte, int>(8, GodKills),
                new Tuple<byte, int>(9, GodAssists),
                new Tuple<byte, int>(10, CubeKills),
                new Tuple<byte, int>(11, OryxKills),
                new Tuple<byte, int>(12, QuestsCompleted),
                new Tuple<byte, int>(13, PirateCavesCompleted),
                new Tuple<byte, int>(14, UndeadLairsCompleted),
                new Tuple<byte, int>(15, AbyssOfDemonsCompleted),
                new Tuple<byte, int>(16, SnakePitsCompleted),
                new Tuple<byte, int>(17, SpiderDensCompleted),
                new Tuple<byte, int>(18, SpriteWorldsCompleted),
                new Tuple<byte, int>(19, LevelUpAssists),
                new Tuple<byte, int>(20, MinutesActive),
                new Tuple<byte, int>(21, TombsCompleted),
                new Tuple<byte, int>(22, TrenchesCompleted),
                new Tuple<byte, int>(23, JunglesCompleted),
                new Tuple<byte, int>(24, ManorsCompleted),
            };

            var ret = new MemoryStream();
            using (var wtr = new NWriter(ret))
            {
                foreach (var i in dat)
                {
                    wtr.Write(i.Item1);
                    wtr.Write(i.Item2);
                }
            }
            return ret.ToArray();
        }

        private static Tuple<string, string, Func<FameStats, DbChar, int, bool>, Func<double, double>>[] bonusDat = new[] {
            Tuple.Create("Ancestor", "First death of any of your characters",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                character.CharId < 2),
                new Func<double, double>(f => f * 0.1 + 20)
            ),

            Tuple.Create("Pacifist", "Never shot a bullet which hit an enemy",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                fStats.ShotsThatDamage == 0),
                new Func<double, double>(f => f * 0.25)
            ),
            //Legacy Builder???
            Tuple.Create("Thirsty", "Never drank a potion from inventory",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                fStats.PotionsDrunk == 0),
                new Func<double, double>(f => f * 0.25)
            ),
            Tuple.Create("Mundane", "Never used special ability (requires level 20)",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                character.Level == 20 && fStats.SpecialAbilityUses == 0),
                new Func<double, double>(f => f * 0.25)
            ),
            Tuple.Create("Boots on the Ground", "Never teleported",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                fStats.Teleports == 0),
                new Func<double, double>(f => f * 0.25)
            ),

            Tuple.Create("Tunnel Rat", "Completed every dungeon type",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                fStats.PirateCavesCompleted > 0 &&
                fStats.UndeadLairsCompleted > 0 &&
                fStats.AbyssOfDemonsCompleted > 0 &&
                fStats.SnakePitsCompleted > 0 &&
                fStats.SpiderDensCompleted > 0 &&
                fStats.SpriteWorldsCompleted > 0 &&
                fStats.TombsCompleted > 0 &&
                fStats.TrenchesCompleted > 0 &&
                fStats.JunglesCompleted > 0 &&
                fStats.ManorsCompleted > 0),
                new Func<double, double>(f => f * 0.1)
            ),

            Tuple.Create("Enemy of the Gods", "More than 10% of kills are gods (requires level 20)",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                character.Level == 20 && (double)fStats.GodKills / (fStats.GodKills + fStats.MonsterKills) > 0.1),
                new Func<double, double>(f => f * 0.1)
            ),
            Tuple.Create("Slayer of the Gods", "More than 50% of kills are gods (requires level 20)",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                character.Level == 20 && (double)fStats.GodKills / (fStats.GodKills + fStats.MonsterKills) > 0.5),
                new Func<double, double>(f => f * 0.1)
            ),

            Tuple.Create("Oryx Slayer", "Dealt Killing blow to Oryx",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                fStats.OryxKills > 0),
                new Func<double, double>(f => f * 0.1)
            ),

            Tuple.Create("Accurate", "Accuracy of better than 25% (requires level 20)",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                character.Level == 20 && (double)fStats.ShotsThatDamage / fStats.Shots > 0.25),
                new Func<double, double>(f => f * 0.1)
            ),
            Tuple.Create("Sharpshooter", "Accuracy of better than 50% (requires level 20)",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                character.Level == 20 && (double)fStats.ShotsThatDamage / fStats.Shots > 0.5),
                new Func<double, double>(f => f * 0.1)
            ),
            Tuple.Create("Sniper", "Accuracy of better than 75% (requires level 20)",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                character.Level == 20 && (double)fStats.ShotsThatDamage / fStats.Shots > 0.75),
                new Func<double, double>(f => f * 0.1)
            ),

            Tuple.Create("Explorer", "More than 1 million tiles uncovered",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                fStats.TilesUncovered > 1000000),
                new Func<double, double>(f => f * 0.05)
            ),
            Tuple.Create("Cartographer", "More than 4 million tiles uncovered",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                fStats.TilesUncovered > 4000000),
                new Func<double, double>(f => f * 0.05)
            ),

            Tuple.Create("Team Player", "More than 100 party member level ups",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                fStats.LevelUpAssists > 100),
                new Func<double, double>(f => f * 0.1)
            ),
            Tuple.Create("Leader of Men", "More than 1000 party member level ups",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                fStats.LevelUpAssists > 1000),
                new Func<double, double>(f => f * 0.1)
            ),

            Tuple.Create("Doer of Deeds", "More than 1000 quests completed",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                fStats.QuestsCompleted > 1000),
                new Func<double, double>(f => f * 0.05)
            ),

            Tuple.Create("Friend of the Cubes", "Never killed a cube (requires level 20)",
                new Func<FameStats, DbChar, int, bool>(
                    (fStats, character, baseFame) =>
                character.Level == 20 && fStats.CubeKills == 0),
                new Func<double, double>(f => f * 0.05)
            ),
        };

        public IEnumerable<Tuple<string, string, double>> GetBonuses(EmbeddedData data, DbChar character, bool firstBorn)
        {
            foreach (var i in bonusDat)
                if (i.Item3(this, character, character.Fame))
                    yield return Tuple.Create(i.Item1, i.Item2, i.Item4(character.Fame));

            //Well Equiped
            var bonus = character.Items.Take(4).Where(x => x != -1).Sum(x => data.Items[(ushort)x].FameBonus) / 100.0;
            if (bonus > 0)
                yield return Tuple.Create("Well Equipped", "Bonus for equipment", character.Fame * bonus);

            //First born
            if (firstBorn)
                yield return Tuple.Create("First Born", "Best fame of any of your previous incarnations", character.Fame * 0.1);
        }

        public int CalculateTotalFame(EmbeddedData data, DbClassStats stats, DbChar chr, int baseFame, out bool firstBorn)
        {
            double bonus = 0;

            //Ancestor
            if (chr.CharId < 2)
                bonus = Math.Floor(bonus) + ((baseFame + Math.Floor(bonus)) * 0.1) + 20;

            //Pacifist
            if (ShotsThatDamage == 0)
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.25;

            //Thirsty
            if (PotionsDrunk == 0)
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.25;

            //Mundane
            if (SpecialAbilityUses == 0)
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.25;

            //Boots on the Ground
            if (Teleports == 0)
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.25;

            //Tunnel Rat
            if (PirateCavesCompleted > 0 && UndeadLairsCompleted > 0 && AbyssOfDemonsCompleted > 0 && SnakePitsCompleted > 0 && SpiderDensCompleted > 0
                && SpriteWorldsCompleted > 0 && TombsCompleted > 0 && TrenchesCompleted > 0 && JunglesCompleted > 0 && ManorsCompleted > 0)
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.1;

            //Enemy of the Gods
            if ((double)GodKills / (GodKills + MonsterKills) > 0.1)
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.1;

            //Slayer of the Gods
            if ((double)GodKills / (GodKills + MonsterKills) > 0.5)
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.1;

            //Oryx Slayer
            if (OryxKills > 0)
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.1;

            //Accurate
            if ((double)ShotsThatDamage / Shots > 0.25)
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.1;

            //Sharpshooter
            if ((double)ShotsThatDamage / Shots > 0.5)
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.1;

            //Sniper
            if ((double)ShotsThatDamage / Shots > 0.75)
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.1;

            //Explorer
            if (TilesUncovered > 1000000)
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.05;

            //Cartographer
            if (TilesUncovered > 4000000)
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.05;

            //Team Player
            if (LevelUpAssists > 100)
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.1;

            //Leader of Men
            if (LevelUpAssists > 1000)
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.1;

            //Doer of Deeds
            if (QuestsCompleted > 1000)
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.1;

            //Friend of the Cubes
            if (CubeKills == 0)
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.1;

            double eq = 0;

            //Well Equipped
            for (int i = 0; i < 4; i++)
            {
                if (chr.Items[i] == -1)
                    continue;

                var b = data.Items[(ushort)chr.Items[i]].FameBonus;

                if (b > 0)
                    eq += (baseFame + Math.Floor(bonus)) * b / 100;
            }

            bonus = Math.Floor(bonus) + Math.Floor(eq);

            if (baseFame + Math.Floor(bonus) > stats.AllKeys.Select(x => stats[ushort.Parse(x)].BestFame).Max())
            {
                bonus = Math.Floor(bonus) + (baseFame + Math.Floor(bonus)) * 0.1;
                firstBorn = true;
            }
            else
                firstBorn = false;

            return (int)(baseFame + Math.Floor(bonus));
        }
    }
}