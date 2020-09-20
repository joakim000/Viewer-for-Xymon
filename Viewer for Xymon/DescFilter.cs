using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Viewer_for_Xymon
{
    public class DescFilter : BindableBase
    {
        private string _test;
        private string _pattern;

        public string test { get => _test; set => SetProperty(ref _test, value); }
        public string pattern { get => _pattern; set => SetProperty(ref _pattern, value); }
        //public string test { get; set; }
        //public string pattern { get; set; }

        public DescFilter() {}

        public DescFilter(string test, string pattern)
        {
            this.test = test;
            this.pattern = pattern;
        }

        public static ObservableCollection<DescFilter> DescFilterD2C(Dictionary<string, string> d)
        {
            var descFilterColl = new ObservableCollection<DescFilter>();
            var testList = d.Keys.ToList();
            var patternList = d.Values.ToList();
            var keyCount = testList.Count();
            for (var i = 0; i < keyCount; i++)
            {
                descFilterColl.Add(new DescFilter(testList[i] as string, patternList[i] as string));
            }
            return descFilterColl;
        }

        public static Dictionary<string, string> DescFilterC2D(ObservableCollection<DescFilter> c)
        {
            var descFilterDict = new Dictionary<string, string>();
            foreach (DescFilter df in c)
            {
                if (descFilterDict.ContainsKey(df.test))
                    descFilterDict[df.test] = df.pattern;
                else
                    descFilterDict.Add(df.test, df.pattern);
            }
            return descFilterDict;
        }


        public static Dictionary<string,string> DefaultDescFilters()
        {
            var d = new Dictionary<string, string>();
            d.Add("DEFAULT", @"(?<=&%COLOR% )(?<c1>.*?)(?=(\\r|\\n))");
            d.Add("EXAMPLE", @"(?<=&%COLOR% )(?<c1>.*?\%( \w+)?\)).*(?<c3> \w+)( level )\(?(?<c2>\d{1,3}\.\d%)?");
            d.Add("conn", @"(?<c1>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3} is) (?<c2>unreachable|alive)");
            d.Add("hdps", @"<tr><td>(?<c1>.*)<\/td><td>.*<\/td><td>.*<\/td><td>(&%COLOR%\s?(?<c2>.*)|.*)<\/td><td>(&%COLOR%\s?(?<c3>.*)|.*)<\/td><td>.*<\/td><\/tr>.*(Failure Reason:\sNot Applicable|Failure Reason(?<c5>:\s.*?)\s*\\r)");
            d.Add("http", @"(?<=&%COLOR% )(?<c1>.*?)(?=\\n)");
            d.Add("na_storbridge", @"(ErrorMessage\s*:\s*)(?<c1>.*)(?=\\nErrorType)");
            d.Add("svcs", @"(?<=&%COLOR% )(?<c1>.*?)(?= \(expected)");
            d.Add("dcdiag", @"(?<=&%COLOR% <\/td><td>)(?<c1>.*?)(?=<\/td>)");
            d.Add("hw", @"(&%COLOR% *(<B>)?)(?<c1>.*?)(<\/B>)?\\n?");
            d.Add("cpu", @"(?<c1>up: (\d+ days|\d+:\d+), \d+ users, \d+ procs, load=(\d+%|\d\.\d\d))(, Physical)?(?<c2>Mem: \d+MB \(\d+%\))?");
            d.Add("logs", @"(?<=<h1>)(?<c1>.*?)(?=<\/h1>.*background-color:%COLOR%"")");
            d.Add("memory", @"&%COLOR%\s*(?<c1>\w+)\s+(?<c2>\w+)\s+(?<c3>\w+)\s+(?<c4>\w+%)(\\r|\\n)");
            d.Add("procs", @"(?<=&%COLOR% )(?<c1>.*?)(?=(\\r|\\n))");
            d.Add("csv", @"(?<=&%COLOR% )(?<c1>.*?)(?=(\\r|\\n))");
            d.Add("disk", @"(?<=&%COLOR% )(?<c1>.*?\%( \w+)?\))");
            d.Add("oracle", @"(?<=&%COLOR% )(?<c1>.*?)(?=(\\r|\\n))");
            d.Add("ntpq", @"(?<=&%COLOR% )(?<c1>.*?)(?=(\\r|\\n))");
            d.Add("omADFS", @"(?<=&%COLOR% <B>)(?<c1>.*?)(?=<\/B>)");
            d.Add("TaskSched", @"(?<=&%COLOR% )(?<c1>.*?)(?=<\/td>)");
            d.Add("msgs", @"(?<=&%COLOR% )(?<c1>.*?)(?=(\\r|\\n))");
            d.Add("Azure", @"(?<=&%COLOR% <B>)(?<c1>.*?)(?=</B>)");
            d.Add("vCpu", @"(?<c1>CpuUsage: ?)\\t(?<c2>\d{1,3} %)");
            d.Add("vMem", @"(?<c1>MEMUsed: ?)\\t(?<c2>\d{1,3} %)");
            d.Add("adds-ntp", @"(?<=&%COLOR%<\/td>\\n\\t<td>)(?<c1>.*?)<\/td>\\n\\t<td>(?<c2>\d+)<\/td>\\n\\t<td>(?<c3>.*?)<\/td>");
            d.Add("mbdbloc", @"&%COLOR%<\/td><td>(?<c1>.*?)<\/td>");
            d.Add("dbpool", @"(Mon|Tue|Wed|Thu|Fri|Sat|Sun) +(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Dec) +\d{1,2}(.* - )(?<c1>.*?) ?<TABLE");
            d.Add("mssql-backup-check", @"&%COLOR%<\/td><td>(?<c1>.*?)<\/td>.*?>(?<c2>.*?)<\/td>");
            d.Add("xymonnet", @"Error output:\\n(?<c1>.*?)(\\r)?\\n");
            d.Add("ntp", @"Command:\s(.*?)\s.*\1\[\d*\]:\s(?<c1>.*?)(\\n)");
            //d.Add("", @"");  // Template
            return d;

        }

        public static string description(string color, string test, string line, string msg, string updateColor)
        {
            string p; // regex pattern
            string d = String.Empty; // description
            string c = color;
                
            //if (c == "purple") return TextFix.lineWrap(TextFix.line1(line));
            // if (c == "purple") c = "green";
            if (c == "blue" || c == "purple")
            {
                string statuscPattern = "^status(\\+\\d+)? .+ (red|yellow|green|clear)";
                Match statusMatch = Regex.Match(msg, statuscPattern);
                if (statusMatch.Success)
                {
                    c = statusMatch.Groups[2].Value;
                }
            }

            
            if (msg.Length > 0 && Settings.descFiltersDict.TryGetValue(test, out p))
            {
                p = p.Replace("%COLOR%", c);             //if (test =="http") Debug.WriteLine("Pattern:"+p);
                try
                {
                    var mColl = Regex.Matches(msg, p);          //if (test == "http") Debug.WriteLine("Matches:" + mColl.Count);
                    int mTotal = mColl.Count;
                    if (mColl.Count > 0)
                    {
                        int count = 1;
                        foreach (Match m in mColl)
                        {
                            if (m.Groups["c1"].Success) d += m.Groups["c1"].Value + " ";
                            if (m.Groups["c2"].Success) d += m.Groups["c2"].Value + " ";
                            if (m.Groups["c3"].Success) d += m.Groups["c3"].Value + " ";
                            if (m.Groups["c4"].Success) d += m.Groups["c4"].Value + " ";
                            if (m.Groups["c5"].Success) d += m.Groups["c5"].Value + " ";
                            if (count != mTotal) d += Environment.NewLine;
                        }
                    }
                    else d = TextFix.line1(line);
                }
                catch (ArgumentNullException e)
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + e);
                    Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + e);
                }
                catch (ArgumentException e)
                {
                    string eLine = new StringReader(e.ToString()).ReadLine();
                    string error = "Matching description for test " + test + ": " + Environment.NewLine + eLine;
                    Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + error);
                    Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + error);
                    string errorPattern = Settings.descFiltersDict[test];
                    Settings.descFiltersDict.Remove(test);
                    if (Settings.descFiltersDict.ContainsKey(test + "_ERROR"))
                    {
                        Settings.descFiltersDict[test + "_ERROR"] = errorPattern;
                    }
                    else
                    {
                        Settings.descFiltersDict.Add(test + "_ERROR", errorPattern);
                    }
                    error += Environment.NewLine + "Matching for this test has been disabled.";
                    ViewModel.Alert(null, null, error);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + e);
                    Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + e);
                }
            }
            else
            {
                d = TextFix.line1(line);
                Debug.WriteLine("No descFilter for test: " + test + "  Total registered descfilters: " + Settings.descFiltersDict.Count);
            }
            return TextFix.lineWrap(d);
        }
    } 
}
