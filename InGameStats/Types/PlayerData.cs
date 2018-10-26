using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;


namespace Types
{
    public class PlayerData
    {
        public int Number { get; }
        public TimeSpan Minutes { get; private set; }
        public int Rate { get; private set; }

        public PlayerData(int number)
        {
            this.Number = number;
        }


        public static IList<PlayerData> GetData(IEnumerable<GameSnapshot> snapShotsCollection)
        {   
            var players = new List<PlayerData>();
            var snapShots = snapShotsCollection.ToList();
            if (snapShots.Count < 1)
                return players;
            players =
                snapShots.SelectMany(snp => snp.PlayerNumbers)
                .Distinct()
                .Select(pn => new PlayerData(pn)).ToList();

            var me = players.Where(p => p.Number == 11).FirstOrDefault();
            for (int i = 0; i < snapShots.Count; i++)
            {
                var snp = snapShots[i];
                GameSnapshot prevSnap = i > 0 ?
                                        snapShots[i - 1]
                                        : GameSnapshot.GetBeginingOfGameSnapshot();

                var currPlayers = players.Where(p => snp.PlayerNumbers.Contains(p.Number)).ToList();
                var subbedOutPlayers = players.Where(p => prevSnap.PlayerNumbers.Contains(p.Number))
                                              .Where(p => !currPlayers.Contains(p)).ToList();
                var playerInPrevSnap = players.Where(p => prevSnap.PlayerNumbers.Contains(p.Number)).ToList();


                
                foreach (var player in playerInPrevSnap)
                {
                    player.Rate += snp.ScoreDiff;
                }

                ////For end of game
                //if (snp.TotalTimeLeft.TotalMinutes == 0)
                //{
                //    foreach (var player in currPlayers)
                //    {
                //        {
                //            player.Rate += snp.ScoreDiff;
                //        }
                //    }
                //}
                var timeDiff = prevSnap.Elapsed;
                if (timeDiff.HasValue)
                {
                    var nonSubbedPlayers = snp.PlayerNumbers.Intersect(prevSnap.PlayerNumbers)
                                        .Select(n => players.Where(p => p.Number == n).FirstOrDefault())
                                        .ToList();
                    var playersToIncreasMinutes = nonSubbedPlayers.Union(subbedOutPlayers).Distinct().ToList();
                    foreach (var p in playersToIncreasMinutes)
                    {
                        p.Minutes += timeDiff.Value;
                    }
                }
            }

            snapShots.ToString();

            return players;
        }

        public static List<GameSnapshot> GetGameSnapShots(string csvText)
        {
            var snapShots = new List<GameSnapshot>();

            var lines = csvText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            if (lines.Length < 2)
                return snapShots;

            var header = lines.FirstOrDefault();
            var hData = header.Split(',').ToList();
            Func<string, string[], string> getData =
                (headerText, dataLine) =>
                {
                    var idx = hData.IndexOf(headerText);
                    var val = idx >= 0 && dataLine.Length > hData.IndexOf(headerText) ?
                    dataLine[idx] : "";
                    return val;
                };

            var data = lines.Skip(1).ToArray();


            foreach (var l in data)
            {
                var arr = l.Split(',');
                var isEmpty = arr.All(s => String.IsNullOrWhiteSpace(s));
                if (isEmpty)
                {
                    continue;
                }
                if (l.Length < 5)
                {
                    continue;
                }
                var p1 = getData(CsvHandler.H_P1, arr);
                var p2 = getData(CsvHandler.H_P2, arr);
                var p3 = getData(CsvHandler.H_P3, arr);
                var p4 = getData(CsvHandler.H_P4, arr);
                var p5 = getData(CsvHandler.H_P5, arr);

                var playerNumbersStr = new List<string> { p1, p2, p3, p4, p5 };
                var playerNumbers = new List<int>();
                foreach (var nStr in playerNumbersStr)
                {
                    int num;
                    if (int.TryParse(nStr, out num))
                        playerNumbers.Add(num);

                }


                var scoreStr = getData(CsvHandler.H_TEAM_SCORE, arr);
                var op_scoreStr = getData(CsvHandler.H_OPONENT_SCORE, arr);
                var quarterStr = getData(CsvHandler.H_QUARTER, arr);
                var timeLeftInQuarterStr = getData(CsvHandler.H_TIME_LEFT, arr);


                int score = -1;
                int.TryParse(scoreStr, out score);
                int op_score = -1;
                int.TryParse(op_scoreStr, out op_score);
                int quarter = -1;
                int.TryParse(quarterStr, out quarter);
                TimeSpan timeLeftInQuarter = TimeSpan.FromSeconds(0);
                timeLeftInQuarterStr = timeLeftInQuarterStr.Replace(" ","");
                var timeFormats = new string[] { "mm\\:ss", "m\\:ss", "mmss", "mm:ss" };

                var tsCulture = System.Globalization.CultureInfo.InvariantCulture;
                bool success = false;
                foreach (var f in timeFormats)
                {
                    success = TimeSpan.TryParseExact(timeLeftInQuarterStr, f, tsCulture , out timeLeftInQuarter);
                    if (success)
                        break;

                }
                if (!success)
                {
                    throw new InvalidDataException($"Failed to parse time data (timeLeftInQuarter):\n {timeLeftInQuarter}");
                }


                //We can know this only after gathering all information
                TimeSpan elapsed = TimeSpan.FromMinutes(0);
                var snapshot = new GameSnapshot(playerNumbers, quarter, timeLeftInQuarter, score, op_score, elapsed);
                snapShots.Add(snapshot);

            }
            snapShots = snapShots.OrderBy(snp => snp.Quarter).ThenByDescending(snp => snp.TimeLeft).ToList();

            var fixedSnapshots = new List<GameSnapshot>();
            for (int i = 0; i < snapShots.Count; i++)
            {
                var oldSN = snapShots[i];
                TimeSpan? elapsed;
                int? snapshotScoreDiff = null;

                if (i < snapShots.Count - 1)
                {
                    var nextSN = snapShots[i + 1];
                    elapsed = oldSN.TotalTimeLeft - nextSN.TotalTimeLeft;
                }
                else
                    elapsed = null;

                var previousSN = fixedSnapshots.LastOrDefault();
                if (previousSN != null)
                {
                    snapshotScoreDiff = oldSN.GameScoreDiff - previousSN.GameScoreDiff;
                }
                var fixedSN = new GameSnapshot(oldSN.PlayerNumbers, oldSN.Quarter, oldSN.TimeLeft, oldSN.TeamScore, oldSN.OponentScore,
                                elapsed, snapshotScoreDiff:snapshotScoreDiff);

                fixedSnapshots.Add(fixedSN);
            }
            


            return fixedSnapshots;
        }

        public override bool Equals(object obj)
        {
            return obj != null && this.Number == (obj as PlayerData).Number;
        }
        public override int GetHashCode()
        {
            return this.Number;
        }
        public override string ToString()
        {
            return $"#{this.Number}; Minutes: {this.Minutes}; Rate: {this.Rate};";
        }
        

        public class CsvHandler
        {
            public const string H_PLAYERPREFIX = "#";
            public const string H_P1 = H_PLAYERPREFIX + "1";
            public const string H_P2 = H_PLAYERPREFIX + "2";
            public const string H_P3 = H_PLAYERPREFIX + "3";
            public const string H_P4 = H_PLAYERPREFIX + "4";
            public const string H_P5 = H_PLAYERPREFIX + "5";

            public const string H_TEAM_SCORE = "Points";
            public const string H_OPONENT_SCORE = "Points Against";
            public const string H_QUARTER = "Quarter";
            public const string H_TIME_LEFT = "Time Left";


            public static void CreateTemplate(string fileName)
            {
                string csvStr = $"{H_P1},{H_P2},{H_P3},{H_P4},{H_P5},{H_TEAM_SCORE},{H_OPONENT_SCORE},{H_QUARTER},{H_TIME_LEFT}\n" +
                     ",,,,,0,0,1,10:00";

                File.WriteAllText(fileName, csvStr);
            }
        }

        public class GameSnapshot
        {
            public int? SnapshotScoreDiff { get; }

            public ReadOnlyCollection<int> PlayerNumbers { get; }
            public int Quarter { get; }
            public TimeSpan TimeLeft { get; }
            public TimeSpan TotalTimeLeft
            {
                get
                {
                    return GetTotalTimeLeft(this.Quarter, TimeLeft);
                }
            }

            internal static TimeSpan GetTotalTimeLeft(int quarter, TimeSpan TimeLeftInQuarter)
            {
                var minutes = (4 - quarter) * 10.0 + TimeLeftInQuarter.TotalMinutes;
                return TimeSpan.FromMinutes(minutes);
            }

            public int TeamScore { get; }
            public int OponentScore { get; }

            public int ScoreDiff
            {
                get { return this.SnapshotScoreDiff ?? 0; }
            }


            public int GameScoreDiff
            {
                get { return this.TeamScore - this.OponentScore; }
            }


            public TimeSpan? Elapsed { get; }

            public GameSnapshot(IList<int> playerNumber, int quarter, TimeSpan timeLeft, int teamScore, int oponentScore, TimeSpan? elapsed, int? snapshotScoreDiff=null)
            {
                this.PlayerNumbers = (playerNumber ?? new int[0]).ToList().AsReadOnly();
                this.Quarter = quarter;
                this.TimeLeft = timeLeft;
                this.TeamScore = teamScore;
                this.OponentScore = oponentScore;
                this.Elapsed = elapsed;
                this.SnapshotScoreDiff = snapshotScoreDiff;
            }

            public static GameSnapshot GetBeginingOfGameSnapshot()
            {
                return new GameSnapshot(new int[0], 1, TimeSpan.FromMinutes(10), 0, 0, TimeSpan.FromSeconds(0));
            }
            public override string ToString()
            {
                return $"Quarter:{this.Quarter} {this.TimeLeft}; {this.TeamScore}:{this.OponentScore}; ({String.Join(",", this.PlayerNumbers)})";
            }
        }
    }
}
