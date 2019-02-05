using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Viewer_for_Xymon
{
    static class Status
    {
        public static bool firstUpdate = true;
        public static bool refreshActive = false;
        public static bool processing = false;
        public static string processingCmd = String.Empty;
        public static string processingType = String.Empty;  //backlog, noMsg, error, manual, lastchange, ack, retry
        public static bool processingFullMsg = true;
        public static bool processingCheckFirst = false;
        public static bool errorOnPrevious = false;
        public static int errorCount = 0;
        public static string failedCmd = String.Empty;
        public static List<string> errorRow;

        public static int linesReceived;
        public static int linesToProcess;
        public static int dupeCount;
        public static int newCount;
        public static int rowsProcessed;
        public static int rowsError;
        public static int rowsNotTest;
        public static int ignoreColsCount;
        public static int rowsFilteredOut;

        public static bool changedXymondAddr = false;
        public static bool gotXymonName = false;
        public static bool gotXymonConn = false;
        public static bool gotXymonCfg = false;
        public static bool gotWebConn = false;
        public static bool gotUserSign = false;
        public static bool configWWWfailed = false;
        public static bool xymondWWWfailed = false;

        public static string successfulUpdateTime = "Not connected";
        public static string successfulUpdateTimeEpoch = "Not connected";

        public static int createCaseStatus = -1;
        // int createCaseStatus = -1; // -1: No attempt, 0: Failure, 1: Success, 2: Found existing cases

        public static ObservableCollection<string> consoleLog = new ObservableCollection<string>();
        public static void log(string str)
        {
            consoleLog.Add(str);
        }

        public static bool lockedPane = false;
        public static string openPane = "none";

    }
}
