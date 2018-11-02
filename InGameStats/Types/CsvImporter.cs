using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Types
{
    public class CsvImporter
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


        public static List<GameSnapshot> GetGameSnapShots(string csvText)
        {
            IList<RawEntry> entries = CsvImporter.GetEntries(csvText);
            List<GameSnapshot> snapshots = GameSnapshot.Factory(entries);
            return snapshots;
        }

        public static IList<RawEntry> GetEntries(string csvText)
        {
            var entries = new List<RawEntry>();

            var lines = csvText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            if (lines.Length < 2)
                return entries;

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
                var p1 = getData(CsvImporter.H_P1, arr);
                var p2 = getData(CsvImporter.H_P2, arr);
                var p3 = getData(CsvImporter.H_P3, arr);
                var p4 = getData(CsvImporter.H_P4, arr);
                var p5 = getData(CsvImporter.H_P5, arr);

                var playerNumbersStr = new List<string> { p1, p2, p3, p4, p5 };
                var playerNumbers = new List<int>();
                foreach (var nStr in playerNumbersStr)
                {
                    int num;
                    if (int.TryParse(nStr, out num))
                        playerNumbers.Add(num);

                }


                var scoreStr = getData(CsvImporter.H_TEAM_SCORE, arr);
                var op_scoreStr = getData(CsvImporter.H_OPONENT_SCORE, arr);
                var quarterStr = getData(CsvImporter.H_QUARTER, arr);
                var timeLeftInQuarterStr = getData(CsvImporter.H_TIME_LEFT, arr);


                int score = -1;
                int.TryParse(scoreStr, out score);
                int op_score = -1;
                int.TryParse(op_scoreStr, out op_score);
                int quarter = -1;
                int.TryParse(quarterStr, out quarter);
                TimeSpan timeLeftInQuarter = TimeSpan.FromSeconds(0);
                timeLeftInQuarterStr = timeLeftInQuarterStr.Replace(" ", "");
                var timeFormats = new string[] { "mm\\:ss", "m\\:ss", "mmss", "mm:ss" };

                var tsCulture = System.Globalization.CultureInfo.InvariantCulture;
                bool success = false;
                foreach (var f in timeFormats)
                {
                    success = TimeSpan.TryParseExact(timeLeftInQuarterStr, f, tsCulture, out timeLeftInQuarter);
                    if (success)
                        break;

                }
                if (!success)
                {
                    throw new InvalidDataException($"Failed to parse time data (timeLeftInQuarter):\n {timeLeftInQuarterStr}");
                }
                var snapshot = new RawEntry(playerNumbers, quarter, timeLeftInQuarter, score, op_score);
                entries.Add(snapshot);
            }
            entries = entries.OrderBy(snp => snp.Quarter).ThenByDescending(snp => snp.TimeLeft).ToList();
            return entries;
        }
    }
}
