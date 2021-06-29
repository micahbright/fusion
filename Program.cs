using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fusion
{
    class Program
    {
        static void Main(string[] args)
        {
            var tbl = readFWTable("C:\\Users\\mbright\\Downloads\\football.dat", new string[] { "------" });

            //printTable(tbl);

            string team = findMinValue(tbl, 10000, 5, 6, 0);

            Console.WriteLine("Team with min F/A spread: " + team);






            tbl = readFWTable("C:\\Users\\mbright\\Downloads\\weather.dat", new string[] { });

            string day = findMinValue(tbl, 10000, 1, 2, 0);

            Console.WriteLine("Day with min temp spread: " + day);
        }

        static string findMinValue(List<List<string>> tbl, float largeSpread, int leftCol, int rightCol, int valCol)
        {
            float minspread = largeSpread;
            string minspreadval = "";
            foreach (var row in tbl)
            {
                string max = row[leftCol].Replace("-", "").Replace("*","").Trim();
                string min = row[rightCol].Replace("-", "").Replace("*","").Trim();
                float spread = Math.Abs(float.Parse(max) - float.Parse(min));
                if (spread < minspread)
                {
                    minspread = spread;
                    minspreadval = row[valCol];
                }
            }
            return minspreadval;
        }

        static void printTable(List<List<string>> tbl)
        {
            foreach (var row in tbl)
            {
                foreach (var itm in row)
                {
                    Console.Write(itm + " * ");
                }
                Console.WriteLine("");
            }
        }

        static List<List<string>> readFWTable(string path, string[] excludeLinesWith)
        {
            var lines = File.ReadLines(path);
            var headerline = lines.First();
            List<Tuple<string, int, int>> header = new List<Tuple<string, int, int>>();
            int? fieldTextStart = null;
            int fieldStart = 0;
            string fieldText;
            for (var i = 0; i < headerline.Length; ++i)
            {
                if (headerline[i] != ' ')
                {
                    fieldTextStart = i;
                }
                if (fieldTextStart != null && char.IsWhiteSpace(headerline[i]))
                {
                    int fieldLength = i - fieldStart;
                    fieldText = headerline.Substring(fieldStart, fieldLength);
                    fieldTextStart = null;
                    header.Add(new Tuple<string, int, int>(fieldText.Trim(), fieldLength, fieldStart));
                    fieldStart = i;
                }
            }
            fieldText = headerline.Substring(fieldStart);
            header.Add(new Tuple<string, int, int>(fieldText.Trim(), headerline.Length - fieldStart, fieldStart));
            List<List<string>> table = new List<List<string>>();

            int prevextend = 0;
            foreach (var line in lines.Skip(1))
            {
                bool excluded = false;
                foreach(string str in excludeLinesWith)
                {
                    if (line.Contains(str)) excluded = true;
                }
                if (excluded) continue;
                if (line.Length == 0) continue;

                List<string> row = new List<string>();
                int extend = 0;
                foreach (var c in header)
                {
                    int start = c.Item3 + prevextend;
                    int fieldlen = c.Item2;
                    if (start > line.Length)
                    {
                        row.Add("");
                    }
                    else
                    {
                        if (line.Length - 1 > start + fieldlen && char.IsWhiteSpace(line[start + fieldlen - 1]))
                            extend = 0;
                        else if (line.Length - 1 > start + fieldlen && !char.IsWhiteSpace(line[start + fieldlen]))
                            while (line.Length > start + fieldlen + extend && !char.IsWhiteSpace(line[start + fieldlen + extend])) extend++;
                        var val = line.Length > start + fieldlen + extend ? line.Substring(start, fieldlen + extend) : line.Substring(start);
                        row.Add(val.Trim());
                        prevextend = extend + 1;
                    }
                }
                prevextend = 0;
                table.Add(row);
            }
            return table;
        }

    }

}
