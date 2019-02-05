using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
namespace Viewer_for_Xymon
{
    public class TextFix
    {
        //public static string regexDate = "^(?:\\s*(Sun|Mon|Tue|Wed|Thu|Fri|Sat),\\s*)?(0?[1-9]|[1-2][0-9]|3[01])\\s+(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\\s+(19[0-9]{2}|[2-9][0-9]{3}|[0-9]{2})\\s+(2[0-3]|[0-1][0-9]):([0-5][0-9])(?::(60|[0-5][0-9]))?\\s+([-\\+][0-9]{2}[0-5][0-9]|(?:UT|GMT|(?:E|C|M|P)(?:ST|DT)|[A-IK-Z]))(\\s*\\((\\\\(|\\\\)|(?<=[^\\])\\((?<C>)|(?<=[^\\])\\)(?<-C>)|[^\\(\\)]*)*(?(C)(?!))\\))*\\s*$";
        public static string regexDate = "(Sun|Mon|Tue|Wed|Thu|Fri|Sat).(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec).(0?[1-9]|[1-2][0-9]|3[01]).[012]\\d{1}:[012345]\\d{1}:[012345]\\d{1}.*20\\d{2} *:*";
        public static char[] trimChars = " -".ToCharArray();

        public static string line1(string input)
	    {
            string t = input;
            //if (t.Length > 0)
            //{
            //    string[] colorEnum = new string[6] { "green", "red", "yellow", "purple", "blue", "clear" };
            //    foreach (string c in colorEnum)
            //    {

            //        int i = t.IndexOf(c, 0);
            //        //Debug.WriteLine("index: " + i + " color: " + c + " input: " + t);
            //        if (i > -1)
            //        {
            //            t = t.Remove(i, c.Length);
            //            break;
            //        }
            //    }

            //    Match m = Regex.Match(t, regexDate);
            //    if (m.Success)
            //    {
            //        int i = m.Index;
            //        int len = m.Length;
            //        t = t.Remove(i, len);
            //    }
            //    t = t.Trim(trimChars);

            //} 
            return t;
	    }
        public static string stats(string input)
        {
            string t = input;
            if (t.Length > 0)
            {
                t = t.Substring(14);   //statuschanges=10
            }
            return t;
        }

        public static string msg(string input)
        {
            string t = input;
            if (t.Length > 0)
            {
                string[] colorEnum = new string[6] { "green", "red", "yellow", "purple", "blue", "clear" };
                foreach (string c in colorEnum)
                {

                    int i = t.LastIndexOf(c, 0);
                    //Debug.WriteLine("index: " + i + " color: " + c + " input: " + t);
                    if (i > -1)
                    {
                        t = t.Remove(0, i);
                        //Debug.WriteLine(t);
                        break;
                    }
                }
                /*
                Match m = Regex.Match(t, regexDate);
                if (m.Success)
                {
                    int i = m.Index;
                    int len = m.Length;
                    t = t.Remove(i, len);
                    Debug.WriteLine(t);
                }
                else Debug.WriteLine("regex miss");
                */
                
                t = t.Trim(trimChars);

            }
            return t;
        }

        public static string lineWrap(string input)
        {
            int max = Settings.maxLineLength; // Max linelength (soft)
            int maxLines = Settings.maxLines;
            int lines = 0;
            string line = null;
            var reader = new StringReader(input);
            var builder = new StringBuilder();
            while (true)
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    lines++;
                    while (line.Length > max && lines < maxLines)
                    {
                        int i = -1;
                        lines++;
                        try
                        {
                            if (line.Length > max + 10)
                            {
                                i = line.IndexOf(" ", max, 10);
                            }
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Out of range during lineWrap: " + e);
                            Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Out of range during lineWrap: " + e);
                        }
                        if (i == -1)
                        {
                            builder.AppendLine(line.Remove(max));
                            line = line.Substring(max).Trim(" ".ToCharArray());
                        }
                        else
                        {
                            builder.AppendLine(line.Remove(i));
                            line = line.Substring(i).Trim(" ".ToCharArray());
                        }
                    }
                    if (lines < maxLines)
                    {
                        builder.AppendLine(line); //Append what is left of line
                    }
                    else
                    {
                        builder.Append("...");
                        break;
                    }
                }
                else
                {
                    break; 
                }
            }
            return builder.ToString().Trim();
        }

        // Convert unixtime to human readable date format. 
        public static string convert_utime(string utimeString)
        {
            int utime;
            try
            {
                if (utimeString == "0" || utimeString == "-1")
                {
                    return utimeString;
                }
                else if (Int32.TryParse(utimeString, out utime))
                {
                    return Settings.refTime.AddSeconds(utime).ToLocalTime().ToString("yyMMdd HH:mm:ss");
                }
                else
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Time parsing failed");
                    Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Time parsing failed");
                    return utimeString;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " Parsing time: " + utimeString + "\nException: " + e);
                Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " Parsing time: " + utimeString + "\nException: " + e);
                return utimeString;
            }
        }

        public static string getUpdateColor(string input)
        {
            string color = "na";
            if (input.Length > 0)
            {
                string colorRegex = @"(green|red|yellow|purple|blue|clear)";
                Match m = Regex.Match(input, colorRegex);
                if (m.Success)
                    color = m.Value;
            }
            return color;
        }

        public static string getUpdateTime(string input)
        {
            //string dateRegex = @"(Sun|Mon|Tue|Wed|Thu|Fri|Sat).(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec).(0?[1-9]|[1-2][0-9]|3[01]).([012]?\d{1}):[012345]\d{1}:[012345]\d{1}.*20\d{2}";
            string dateRegex = @"(Sun|Mon|Tue|Wed|Thu|Fri|Sat)\s+(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s+(0?[1-9]|[1-2][0-9]|3[01]).([012]?\d{1}):([012345]\d{1}):([012345]\d{1})(\s?\w*\s?)(20\d{2})";
            DateTime dt;
            string dateString = String.Empty;
            string updateTime = "na";

            try
            {
                Match m = Regex.Match(input, dateRegex);
                if (m.Success)
                {
                    dateString = m.Value;

                    //TODO: Clean this up with regex
                    dateString = dateString.Replace(" CEST ", " ");
                    dateString = dateString.Replace(" CET ", " ");
                    dateString = dateString.Replace(" UTC ", " ");
                    dateString = dateString.Replace(" GMT ", " ");
                    dateString = dateString.Replace(" PDT ", " ");

                    dateString = dateString.Replace("  ", " ");
                    dateString = dateString.Replace("   ", " ");

                    dateString = dateString.Replace(" 1 ", " 01 ");
                    dateString = dateString.Replace(" 2 ", " 02 ");
                    dateString = dateString.Replace(" 3 ", " 03 ");
                    dateString = dateString.Replace(" 4 ", " 04 ");
                    dateString = dateString.Replace(" 5 ", " 05 ");
                    dateString = dateString.Replace(" 6 ", " 06 ");
                    dateString = dateString.Replace(" 7 ", " 07 ");
                    dateString = dateString.Replace(" 8 ", " 08 ");
                    dateString = dateString.Replace(" 9 ", " 09 ");
                }
                else
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "dateString not found: " + input);
                    return updateTime;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Finding timestring in line1-time: " + e);
                Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Finding timestring in line1-time: " + e);
            }

            try
            {
                string longFormat = "ddd MMM dd HH:mm:ss yyyy";
                dt = DateTime.ParseExact(dateString, longFormat, CultureInfo.InvariantCulture);
                //dt = DateTime.Parse(dateString, CultureInfo.InvariantCulture);
                updateTime = dt.ToString("yyMMdd HH:mm:ss");
            }
            catch (Exception e)
            {
                Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Parsing line1-time with string: " + dateString + "\n" + e);
                Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Parsing line1-time with string: " + dateString + "\n" + e);
            }
            return updateTime;
        }


    }
}