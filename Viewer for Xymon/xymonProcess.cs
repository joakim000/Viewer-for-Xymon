using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace Viewer_for_Xymon
{
    public static class xymonProcess
    {
        
        public static void updateAndAdd(List<List<string>> ListOfRows)
        {
            List<List<string>> newFountsToBe = new List<List<string>>();
            foreach (List<string> xr in ListOfRows)
            {
                bool found = false;
                foreach (Fount f in Model.fc)
                {
                    if (f.hostname == xr[0] && f.testname == xr[1])
                    {
                        found = true;
                        //ThreadPool.RunAsync((workItem) => updateFount(f, xr));
                        //if (Status.processingType != "cache")
                        if (Status.processingFullMsg == true)
                            updateFount(f, xr);
                    }
                }
                if (!found) newFountsToBe.Add(xr);
            }
            foreach (List<string> xr in newFountsToBe)
            {
                Model.fc.Add(updateFount(new Fount(xr[0], xr[1]), xr));

                //ThreadPool.RunAsync((workItem) =>
                // {
                //    fc.Add(updateFount(new Fount(xr[0], xr[1]), xr));
                //});

            }
            Status.linesToProcess = ListOfRows.Count();
            Status.newCount = newFountsToBe.Count();
            Status.dupeCount = Status.linesToProcess - Status.newCount;
            if (Settings.verboseLog)
                Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " Rows:" + Status.linesToProcess + " Dupes:" + Status.dupeCount + " New:" + Status.newCount);

            //string pCmd = Status.processingCmd.Remove(Status.processingCmd.Length - Settings.fields.Length).Replace("xymondboard ", "");
            //Status.log(pCmd);
            if (Settings.verboseLog)
                Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " +
                "Received:" + Status.linesReceived.ToString() +
                " Filtered:" + Status.rowsFilteredOut +
                " Processed:" + Status.rowsProcessed +
                " Dupe:" + Status.dupeCount +
                " New:" + Status.newCount +
                " Error:" + Status.rowsError
                );
            // End of processing
            if (Status.errorOnPrevious)
            {
                Status.errorCount++;
                Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " Field count error in final row - likely interrupted transfer");
                Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " Field count error in final row - likely interrupted transfer");
                if (Status.errorCount < Settings.maxRetry)
                {
                    //Debug.WriteLine("Retry " + Status.errorCount);
                    //Status.log("Retry " + Status.errorCount);
                    Status.processingType = "error";
                }
                else
                {
                       Debug.WriteLine("Reached max retries: " + Settings.maxRetry + ", giving up." );
                       Status.log("Reached max retries: " + Settings.maxRetry + ", giving up.");
                }
                    
            }
            Status.processing = false;
        }


        public static List<List<string>> processXymonList(string xymonRecv)
        {
            int fieldCount;
            if (Status.processingFullMsg) fieldCount = Settings.fieldCount;
            else fieldCount = Settings.fieldCount - 1;

            List<string> xymonList = xymonRecv.ToString().Split('\n').ToList();
            int lastindex = xymonList.Count() - 1;
            if (xymonList[lastindex].Length == 0) xymonList.RemoveAt(lastindex);

            if (Settings.verboseLog)
                Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " Lines received: " + lastindex);
            if (Settings.verboseLog)
                Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " Lines received: " + lastindex);
            Status.linesReceived = lastindex;
            Status.rowsProcessed = 0; Status.rowsFilteredOut = 0; Status.rowsNotTest = 0; Status.rowsError = 0; Status.ignoreColsCount = 0; // Debugging / logging


            List<List<string>> ListOfRows = new List<List<string>>();
            foreach (string el in xymonList)
            {
                Status.rowsProcessed++;
                List<string> xymonRow = el.Split('|').ToList();

                if (Status.processingFullMsg == false)
                {
                    try
                    {
                        xymonRow.Insert(24, String.Empty);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Out of range adding empty message to non-full row " + e);
                        Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Out of range adding empty message to non-full row " + e);
                    }
                    //fieldCount = Settings.fieldCount;
                }

                if (xymonRow.Count() < fieldCount)
                {
                    Status.rowsError++;
                    try
                    {
                        Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Line has too few fields! " + xymonRow[0] + " : " + xymonRow[1]);
                        Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Line has too few fields! " + xymonRow[0] + " : " + xymonRow[1]);
                        Status.errorOnPrevious = true;
                        Status.errorRow = xymonRow;
                        Status.failedCmd = Status.processingCmd;
                        continue;
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Line has too few fields! Even field 0 and 1 is missing.\nException: " + e);
                        Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Line has too few fields! Even field 0 and 1 is missing.\nException: " + e);
                        Status.errorOnPrevious = true;
                        Status.failedCmd = Status.processingCmd;
                        continue;
                    }
                }


                // Filter out nonongreen / nobb2 (tillsvidare)
                try
                {
                    if (!Settings.showNoNonGreen)
                    {
                        if (xymonRow[23].IndexOf("nobb2") > -1 || xymonRow[23].IndexOf("nonongreen") > -1)
                        { Status.rowsFilteredOut++; continue; }
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Out of range filtering nobb2: " + e);
                    Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Out of range filtering nobb2: " + e);
                }


                try
                {
                    // NOPROPRED / YELLOW / PURPLE
                    if (!Settings.showNoProp)
                    {
                        if (xymonRow[2] == "yellow" && (xymonRow[20].IndexOf(xymonRow[1]) > -1 || xymonRow[20] == "*"))
                        { Status.rowsFilteredOut++; continue; }
                        if (xymonRow[2] == "red" && (xymonRow[21].IndexOf(xymonRow[1]) > -1 || xymonRow[21] == "*"))
                        { Status.rowsFilteredOut++; continue; }
                        if (xymonRow[2] == "purple" && (xymonRow[22].IndexOf(xymonRow[1]) > -1 || xymonRow[22] == "*"))
                        { Status.rowsFilteredOut++; continue; }
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Out of range filtering noprop: " + e);
                    Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Out of range filtering noprop: " + e);
                }


                // Special to handle XMH_RAW
                int xrf = (Settings.fieldCount - 1); // XMH_RAW_field
                if (xymonRow.Count() > Settings.fieldCount)
                {
                    string allraw = String.Empty;
                    for (int i = xrf; i < (xymonRow.Count()); i++)
                    {
                        allraw = allraw + xymonRow[i];
                        allraw = allraw + ",";
                    }
                    xymonRow[xrf] = allraw.TrimEnd(',');
                    //TODO Remove orphaned list-elements
                }

                

                // Filter out NOCOLUMNS (tillsvidare)
                try
                {
                    if (!Settings.showNoNonGreen)  // Visa NOCOLUMNS-rader om nobb2 visas
                    {
                        bool filterNoCol = false;
                        int NoColsIndex = xymonRow[xrf].IndexOf("NOCOLUMNS:");
                        String[] NoCols;
                        if (NoColsIndex > -1)
                        {
                            int NoColsEnd = xymonRow[xrf].IndexOf('|', NoColsIndex);
                            string NoColsString;
                            if (NoColsEnd > -1)
                                NoColsString = xymonRow[xrf].Substring(NoColsIndex + 10, NoColsEnd - NoColsIndex + 10); // 10 is length of "NOCOLUMNS:"
                            else
                                NoColsString = xymonRow[xrf].Substring(NoColsIndex + 10);
                            NoCols = NoColsString.Split(',');
                            foreach (String s in NoCols)
                            {
                                //Debug.WriteLine("NOCOLUMNS-entry: " + s + " test: " + xymonRow[1]);
                                if (s == xymonRow[1])
                                    filterNoCol = true;
                                break;
                            }
                            if (filterNoCol)
                            {
                                Status.rowsFilteredOut++;
                                Debug.WriteLine("Filtered NOCOLUMNS: " + xymonRow[0] + " : " + xymonRow[1]);
                                continue;
                            }
                        }
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Out of range filtering NOCOLUMNS: " + e);
                    Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Out of range filtering NOCOLUMNS: " + e);
                }


                ListOfRows.Add(xymonRow);
            }
            if (Settings.verboseLog)
                Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " Processed:" + Status.rowsProcessed + " Error:" + Status.rowsError + " Non-test:" + Status.rowsNotTest + " ignoreColsCount:" + Status.ignoreColsCount + " Filtered out:" + Status.rowsFilteredOut);
            if (Status.rowsError == 0)
            {
                Status.errorOnPrevious = false;
                Status.errorCount = 0;
            }
            return ListOfRows;
        }

        public static Fount updateFount(Fount f, List<string> xymonRow)
        {
            try
            {
                // Update only if logtime updated or acked  
                int logtime;
                if (Int32.TryParse(xymonRow[5], out logtime))
                    if (f.logtime_epoch >= logtime && xymonRow[7] == "0" )
                    {
                       //Debug.WriteLine("Logtimecheck: returned unmodified fount");
                       return f;
                    }        

                // Debug
                f.rawFields = xymonRow.Count();
                f.rawElement = String.Join("|", xymonRow);
                
                // Save previous color
                if (f.color != xymonRow[2]) 
                    f.previousColor = f.color;

                // No processing on these
                f.color = xymonRow[2];
                f.flags = xymonRow[3];
                f.sender = xymonRow[9];
                f.cookie = xymonRow[10];
                f.line1 = xymonRow[11];
                f.client = xymonRow[15];
                f.flapinfo = xymonRow[17];
                f.XMH_NOPROPYELLOW = xymonRow[20];
                f.XMH_NOPROPRED = xymonRow[21];
                f.XMH_NOPROPPURPLE = xymonRow[22];
                f.XMH_FLAG_NONONGREEN = xymonRow[23];
                if (xymonRow[24].Length > 0) f.msg = xymonRow[24];  //Don't write over a perfectly good msg when caching
                    f.msg = xymonRow[24];


                // Process ack message
                
                if (xymonRow[13].Length > 0 && f.color != "green")
                {
                    f.ackmsg = xymonRow[13];
                    Match ackMatch = Regex.Match(f.ackmsg, Settings.ackPattern);
                    if (ackMatch.Success)
                    {
                        f.ackuser = ackMatch.Groups["user"].Value; //Debug.WriteLine("Ackuser: " + ackMatch.Groups["user"].Value + " : " + f.ackuser);
                        f.ackmsg_processed = ackMatch.Groups["msg"].Value;
                    }
                    else
                        f.ackmsg_processed = f.ackmsg;                      
                    if (f.ackmsg_processed != null) f.ackmsg_processed = TextFix.lineWrap(f.ackmsg_processed);
                    //else Debug.WriteLine("ackMatch fail on " + f.hostname + " : " + f.testname + " ackmsg:" + ackmsg);
                }

                // onGreenDelay
                //if ((f.color == "red" || f.color == "yellow" || f.color == "purple" ) && xymonRow[2] == "green")
                //{
                //    f.greenDelay = Settings.onGreenDelay;
                //}
                //else
                if (xymonRow[2] == "blue") // Process dismMsg on change to blue, else keep in Fount
                {
                    // Process disable message
                    f.dismsg = xymonRow[14];
                    if (f.dismsg.Length > 0)
                    {
                        //string dismsg = f.dismsg;
                        Match disMatch = Regex.Match(f.dismsg, Settings.disPattern);
                        if (disMatch.Success)
                        {
                            f.disuser = disMatch.Groups["user"].Value;   //Debug.WriteLine(f.disuser);
                            f.dismsg_processed = disMatch.Groups["msg"].Value;

                        }
                        else
                            f.dismsg_processed = f.dismsg;
                        if (f.dismsg_processed != null) f.dismsg_processed = TextFix.lineWrap(f.dismsg_processed);
                        //else Debug.WriteLine("disMatch fail on " + f.hostname + " : " + f.testname + " dismsg:" + dismsg);
                    }
                    
                }

                
                

                // Convert times in string to int
                int utime;
                if (Int32.TryParse(xymonRow[4], out utime))
                    f.lastchange_epoch = utime;
                else f.lastchange_epoch = 0;
                if (Int32.TryParse(xymonRow[5], out utime))
                    f.logtime_epoch = utime;
                else f.logtime_epoch = 0;
                if (Int32.TryParse(xymonRow[6], out utime))
                    f.validtime_epoch = utime;
                else f.validtime_epoch = 0;
                if (Int32.TryParse(xymonRow[7], out utime))
                    f.acktime_epoch = utime;
                else f.acktime_epoch = 0;
                if (Int32.TryParse(xymonRow[8], out utime))
                    f.disabletime_epoch = utime;
                else f.disabletime_epoch = 0;
                if (Int32.TryParse(xymonRow[16], out utime))
                    f.clntstamp_epoch = utime;
                else f.clntstamp_epoch = 0;

                //Convert time: 4:lastchange, 5:logtime, 6:validtime, 7:acktime, 8:disabletime, 16:clntstamp
                f.lastchange = TextFix.convert_utime(xymonRow[4]);
                f.logtime = TextFix.convert_utime(xymonRow[5]);
                f.validtime = TextFix.convert_utime(xymonRow[6]);
                if (xymonRow[7] == "0") // No acktime
                    f.acktime = "";
                else
                    f.acktime = TextFix.convert_utime(xymonRow[7]);
                if (xymonRow[8] == "-1")  //Indefinte timespan
                    f.disabletime = "Until OK";
                else
                    f.disabletime = TextFix.convert_utime(xymonRow[8]);
                f.clntstamp = TextFix.convert_utime(xymonRow[16]);

                // updateColor from line1
                f.updateColor = TextFix.getUpdateColor(f.line1);

                // updateTime: Ta clntstamp först. Om icke-lila och clntstamp ej finns ta logtime. Om lila (eller lila bakom blå) och ej clntstamp, försök parsa från line1.
                if (f.clntstamp_epoch > 0)
                {
                    f.updateTime = f.clntstamp;
                }
                else if (f.color == "blue" && f.previousColor != "purple")
                {
                    f.updateTime = f.logtime;
                }
                else if (f.color != "purple")
                {
                    f.updateTime = f.logtime;
                }
                else // color is purple and no clntstamp available, try parsing from line1 (returns "na" if not able to parse)
                {
                    f.updateTime = TextFix.getUpdateTime(f.line1);
                }


                // Special processing
                f.stats = TextFix.stats(xymonRow[18]);
                f.XMH_DGNAME = TextFix.lineWrap(xymonRow[19]);
                f.XMH_RAW = TextFix.lineWrap(xymonRow[25]);
                f.description = DescFilter.description(f.color, f.testname, f.line1, f.msg, f.updateColor);

               


                // Process derived props if source changed (or new)
                if (!String.Equals(f.XMH_PAGEPATHTITLE, xymonRow[12]))
                {
                    f.XMH_PAGEPATHTITLE = xymonRow[12];
                    int pathSnip = f.XMH_PAGEPATHTITLE.IndexOf("/");
                    if (pathSnip > -1)
                    {
                        f.pageroot = f.XMH_PAGEPATHTITLE.Remove(pathSnip);
                        f.pagepath = f.XMH_PAGEPATHTITLE.Substring(pathSnip + 1);
                        f.pagepath = TextFix.lineWrap(f.pagepath);
                    }
                    else
                    {
                        f.pageroot = f.XMH_PAGEPATHTITLE;
                        f.pagepath = "";
                    }
                    f.pageroot = TextFix.lineWrap(f.pageroot);
                }

                // Format flapinfo
                int utime_flap;
                List<string> flap = f.flapinfo.Split('/').ToList();
                if (String.Equals(flap[0], "0"))
                {
                    f.flapinfo = "";
                }
                else
                {
                    if (Int32.TryParse(flap[2], out utime_flap))
                    {
                        flap[2] = Settings.refTime.AddSeconds(utime).ToLocalTime().ToString("yyMMdd HH:mm:ss");
                    }
                    else Debug.WriteLine("parsing failed");
                    f.flapinfo = flap[2] + Environment.NewLine + flap[3] + " <> " + flap[4];
                }


            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Out of range updating Fount (missing fields): "
                                + f.hostname + " : " + f.testname + " " + e);
                Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Out of range updating Fount (missing fields): "
                                + f.hostname + " : " + f.testname + " " + e);
            }
            return f;
        }


    }
}
