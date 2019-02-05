using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media;

namespace Viewer_for_Xymon
{
    public static class Settings
    {
        public static string appName = "Viewer for Xymon";
        public static string appVariant = "";
        public static string appVersion = "1.3.2";

        /* Saved settings */
        public static string XymondAddr = String.Empty; //= "xymon.example.net"; 
        public static int XymondPort = 1984;
        public static string userSignType = "Custom"; // "Custom", "Display name", "Domain user", "RFC 822"
        public static string userSign = String.Empty;
        //public static string userSign = "Viewer for Xymon";
        public static string disPattern = @"^(Disabled by:? ?)(?<user>\w+(\s\w+)*)( @ )?.*\\nReason: (?<msg>.*)\\n"; 
        public static string ackPattern = @"^\s*(?<msg>.*)\\nAcked by: (?<user>\w+(\s\w+)*)";

        public static string casePattern;
        //public static string casePattern = @"(INC|CHG)\d{7}"; //INC0076290
        public static string showCaseURL; // = "https://ticket.example.net/incident?$ticket";  
        public static string showCaseMethod;
        public static ObservableCollection<ValuePair> showCasePairs = new ObservableCollection<ValuePair>();
        public static string showCaseWebUser;
        public static string showCaseWebPw;

        public static string createCaseURL;  //public static string createCaseURL  script...
        public static string createCaseMethod;
        public static string createCasePostKey;
        public static ObservableCollection<ValuePair> createCasePairs = new ObservableCollection<ValuePair>();
        public static string createCaseWebUser;
        public static string createCaseWebPw;
        public static string caseResponseSuccess = String.Empty; // = @"Created case";
        public static string caseResponseExists = String.Empty; // = @"Existing active";
        public static string caseResponseFailed = String.Empty; // = @"Could not create";

        public static string printFriendlyPattern = String.Empty; // = @"docs\.365-24\.se";
        public static string printFriendlyReq = String.Empty; // = "?cover=print";
        


        // Set these to defaults for now
        public static bool[] startView = { false, true, true, true, false, false, false };    // ack, red, yellow, purple, clear, blue, green
        //public static bool[] startView = { false, true, true, true, true, true, true };    // For use with test-server
        public static string defaultAckDuration = "1"; // 1 week
        public static string defaultDisDuration = "4"; // 4 hours
        public static string httpScheme = "https"; // Set by webtester

        /* From xymonserver.cfg */
        public static string XymonWWWName = String.Empty;
        public static string XymonWWWURL = String.Empty;
        public static string XymonCGIURL = String.Empty;
        public static string XymonSecureCGIURL = String.Empty;
        public static string InfoCol = String.Empty;
        public static string TrendsCol = String.Empty;
        public static string ClientCol = String.Empty;
        public static string XymonGenOpts = String.Empty;
        public static string ColumnDocURL = String.Empty;
        public static string docsURL = String.Empty; 

        
        /* Application settings */
        public static int maxRetry = 5;
        public static bool statusRowEpoch = false;
        public static bool verboseLog = true;
        //public static bool verboseLog = false;

        public static List<string> ignoreCols = new List<string>();
        private static string[] defaultIgnoreTags = { "nobb2", "nonongreen" };
        public static List<string> ignoreTags = new List<string>(defaultIgnoreTags);
        private static string[] defaultUserSignTypes = { "Custom", "Display name", "Domain user", "RFC 822" };
        public static ObservableCollection<string> userSignTypes = new ObservableCollection<string>(defaultUserSignTypes);

        //public static string fields = " fields=hostname,testname,color,flags,lastchange,logtime,validtime,acktime,disabletime,sender,cookie,line1,XMH_PAGEPATHTITLE,ackmsg,dismsg,client,clntstamp,flapinfo,stats,XMH_DGNAME,XMH_NOPROPYELLOW,XMH_NOPROPRED,XMH_NOPROPPURPLE,msg,XMH_RAW\0\n";
        public static string fields = " fields=hostname,testname,color,flags,lastchange,logtime,validtime,acktime,disabletime,sender,cookie,line1,XMH_PAGEPATHTITLE,ackmsg,dismsg,client,clntstamp,flapinfo,stats,XMH_DGNAME,XMH_NOPROPYELLOW,XMH_NOPROPRED,XMH_NOPROPPURPLE,XMH_FLAG_NONONGREEN,msg,XMH_RAW";
        public static int fieldCount = 26;
        public static DateTime refTime = new DateTime(1970, 01, 01, 00, 00, 00);
        public static bool showNoNonGreen = false;
        public static bool showNoProp = false;
        public static string StatusURL = "/svcstatus.sh?HOST=%HOSTNAME%&SERVICE=%TESTNAME%";
        public static string HistURL = "/history.sh?HISTFILE=%HOSTNAME%.%TESTNAME%&BARSUMS=0";
        public static string TrendsURL = "/svcstatus.sh?HOST=%HOSTNAME%&SERVICE=%TRENDSCOL%";
        // For history-pane 
        public static string ackHistURL = "https://opmon.qbs.se/reports/ackhist.html";
        public static string histPattern = @"NOWRAP>(?<date>\w{3} \w{3} \d{1,2} .{8}) ?\w* (?<year>\d{2,4}).*<\/TD>\r?\n.*(?<link>historylog.sh\?HOST=.*?)"">.*ALT=""(?<color>.*?)"".*\r?\n<TD.*?>(?<duration>.*)<\/TD>";
        public static string ackHistPattern = @"<TR BGCOLOR=#000000>\r?\n.*<FONT COLOR=white>(?<date>\w{3} \w{3} \d{1,2} .{8}) \w+ (?<year>\d{2,4}).*\r?\n<\/FONT><\/TD>\r?\n<TD ALIGN=CENTER BGCOLOR=(?<color>.*)><FONT COLOR=black>(%HOSTNAME%)<\/FONT><\/TD>\r?\n.*<FONT COLOR=white>\s*(%TESTNAME%)\s*<\/FONT><\/TD>\r?\n.*\r?\n.*BGCOLOR=#000033>\s*(?<ackuser>.*)\s*\r?\n<\/TD>\r?\n<TD ALIGN=LEFT>\s*(?<message>(<a href=.*?>)?.*?)\s*<\/";
        public static int histLimit = 50;

        /* Options */
        public static int refreshDelay = 5000;  // (ms) - Time to delay before next refresh
        public static int refreshSpan = 20; // (s) - How far to look back with each refresh
        public static int onGreenDelay = 15;
        public static string emptyPaneFill = "https://c.xkcd.com/random/comic/";
        public static int maxLineLength = 80;
        public static int maxLines = 5;
        public static int textSize = 14;
        public static bool CommandLabels = true; 
        public static bool CommandTextBox = false; 
        public static bool ColumnHeaderFlyout = false; // Deprecated
        public static bool newBold = true;
        public static bool newSaturate = false;
        public static bool ackBold = false;
        public static bool ackSaturate = true;
        public static bool showManualBlue = false;
        public static bool showConsole = false;
        public static bool disableUntilGreen = true;
        public static bool cacheOnStart = true;
        public static int cacheHours = 72;

        public static bool pane_status = true;
        public static bool pane_history = true;
        public static bool pane_docs = true;
        public static bool pane_case = true;
        public static bool pane_test = true;
        public static bool pane_trends = true;
        public static bool pane_log = true;
        public static bool pane_info = true;
        public static bool pane_debug = false;


        public static string webUser;
        public static string webPw;

        public static string red_hex = "#f1937a";
        public static string strongRed_hex = "#e9582f";
        public static string yellow_hex = "#fee599";
        public static string strongYellow_hex = "#fdcb35";
        public static string purple_hex = "#b2a1c7";
        public static string strongPurple_hex = "##9780b3";
        public static string clear_hex = "#d3d3d3";
        public static string blue_hex = "#9cc3e5";
        public static string green_hex = "#a8d08d";
        public static SolidColorBrush red_brush = Jlib.GetSolidColorBrush(red_hex);
        public static SolidColorBrush strongRed_brush = Jlib.GetSolidColorBrush(strongRed_hex);
        public static SolidColorBrush yellow_brush = Jlib.GetSolidColorBrush(yellow_hex);
        public static SolidColorBrush strongYellow_brush = Jlib.GetSolidColorBrush(strongYellow_hex);
        public static SolidColorBrush purple_brush = Jlib.GetSolidColorBrush(purple_hex);
        public static SolidColorBrush strongPurple_brush = Jlib.GetSolidColorBrush(strongPurple_hex);
        public static SolidColorBrush clear_brush = Jlib.GetSolidColorBrush(clear_hex);
        public static SolidColorBrush blue_brush = Jlib.GetSolidColorBrush(blue_hex);
        public static SolidColorBrush green_brush = Jlib.GetSolidColorBrush(green_hex);
        public static SolidColorBrush black_brush = Jlib.GetSolidColorBrush("#000000");
        public static SolidColorBrush dimgray_brush = Jlib.GetSolidColorBrush("#696969");

        /* SSH settings 
        public static string SshAddr = "192.168.0.113";
        public static int SshPort = 22;
        public static string SshUser = "xymon";
        public static string SshPw = "sqqfe";

        public static string GatewayAddr = "192.168.0.111";
        public static int GatewayPort = 22;
        public static string GatewayUser = "joakim";
        public static string GatewayPw = "sdqfe";

        // These can be got from xymonserver.cfg
        public static string AlleventsPath = "/home/xymond02/data/hist/allevents";
        public static string LogDir = "/home/bb/bbvar/logs";
        public static string XymonBin = "/home/xymond02/server/bin/xymon";
        public static string HostsPath = "/home/hobbitcmn/etc/bb-hosts";
        */

        public static Dictionary<string, string> descFiltersDict = new Dictionary<string, string>();


    }

    public class SettingsManager : BindableBase
    {
        private string _XymondAddr;
        private int _XymondPort;
        private ObservableCollection<string> _userSignTypes;
        private string _userSignType;
        private string _userSign;
        private string _docsURL;
        private string _ColumnDocURL;
        private string _disPattern;
        private string _ackPattern;
        private int _refreshDelay;
        private int _refreshSpan;
        private string _webUser;
        private string _webPw;
        private string _printFriendlyPattern;
        private string _printFriendlyReq;

        private bool _cacheOnStart;
        private int _cacheHours;

        private bool _CommandLabels;
        private bool _CommandTextBox;
        private bool _ColumnHeaderFlyout;
        private bool _ColumnHeaderInline;
        private bool _newBold;
        private bool _newSaturate;
        private bool _ackBold;
        private bool _ackSaturate;
        private bool _showManualBlue;
        private bool _showConsole;
        private bool _disableUntilGreen;
        private string _emptyPaneFill;
        private int _maxLineLength;
        private int _maxLines;
        private int _textSize;
        private int _onGreenDelay;

        private bool _pane_status;
        private bool _pane_history;
        private bool _pane_docs;
        private bool _pane_case;
        private bool _pane_test;
        private bool _pane_trends;
        private bool _pane_log;
        private bool _pane_info;
        private bool _pane_debug;

        private bool _verboseLog;
        private int _maxRetry;
        private string _histPattern;
        private string _ackHistURL;
        private string _ackHistPattern;
        private int _histLimit;


        private string _showCaseURL;
        private string _showCaseWebUser;
        private string _showCaseWebPw;

        private string _casePattern;
        private string _createCaseURL;
        private string _createCaseMethod;
        private string _createCasePostKey;
        private string _caseWebUser;
        private string _caseWebPw;
        private string _caseResponseSuccess;
        private string _caseResponseExists;
        private string _caseResponseFailed;
        private ObservableCollection<ValuePair> _createCasePairs;
        private Dictionary<string, string> _descFiltersDict;



        public string XymondAddr { get => _XymondAddr; set => SetProperty(ref _XymondAddr, value); }
        public int XymondPort { get => _XymondPort; set => SetProperty(ref _XymondPort, value); }
        public string userSignType { get => _userSignType; set => SetProperty(ref _userSignType, value); }
        public ObservableCollection<string> userSignTypes { get => _userSignTypes; set => SetProperty(ref _userSignTypes, value); }
        public string userSign { get => _userSign; set => SetProperty(ref _userSign, value); }
        public string docsURL { get => _docsURL; set => SetProperty(ref _docsURL, value); }
        public string ColumnDocURL { get => _ColumnDocURL; set => SetProperty(ref _ColumnDocURL, value); }
        public string disPattern { get => _disPattern; set => SetProperty(ref _disPattern, value); }
        public string ackPattern { get => _ackPattern; set => SetProperty(ref _ackPattern, value); }
        public int refreshDelay { get => _refreshDelay; set => SetProperty(ref _refreshDelay, value); }
        public int refreshSpan { get => _refreshSpan; set => SetProperty(ref _refreshSpan, value); }
        public string webUser { get => _webUser; set => SetProperty(ref _webUser, value); }
        public string webPw { get => _webPw; set => SetProperty(ref _webPw, value); }
        public string printFriendlyPattern { get => _printFriendlyPattern; set => SetProperty(ref _printFriendlyPattern, value); }
        public string printFriendlyReq { get => _printFriendlyReq; set => SetProperty(ref _printFriendlyReq, value); }

        public bool CommandLabels { get => _CommandLabels; set => SetProperty(ref _CommandLabels, value); }
        public bool CommandTextBox { get => _CommandTextBox; set => SetProperty(ref _CommandTextBox, value); }
        public bool ColumnHeaderFlyout { get => _ColumnHeaderFlyout; set => SetProperty(ref _ColumnHeaderFlyout, value); }
        public bool ColumnHeaderInline { get => _ColumnHeaderInline; set => SetProperty(ref _ColumnHeaderInline, value); }
        public bool newBold { get => _newBold; set => SetProperty(ref _newBold, value); }
        public bool newSaturate { get => _newSaturate; set => SetProperty(ref _newSaturate, value); }
        public bool ackBold { get => _ackBold; set => SetProperty(ref _ackBold, value); }
        public bool ackSaturate { get => _ackSaturate; set => SetProperty(ref _ackSaturate, value); }
        public bool showManualBlue { get => _showManualBlue; set => SetProperty(ref _showManualBlue, value); }
        public bool showConsole { get => _showConsole; set => SetProperty(ref _showConsole, value); }
        public bool disableUntilGreen { get => _disableUntilGreen; set => SetProperty(ref _disableUntilGreen, value); } 
        public string emptyPaneFill { get => _emptyPaneFill; set => SetProperty(ref _emptyPaneFill, value); }
        public int maxLineLength { get => _maxLineLength; set => SetProperty(ref _maxLineLength, value); }
        public int maxLines { get => _maxLines; set => SetProperty(ref _maxLines, value); }
        public int textSize { get => _textSize; set => SetProperty(ref _textSize, value); }
        public int onGreenDelay { get => _onGreenDelay; set => SetProperty(ref _onGreenDelay, value); }

        public bool cacheOnStart { get => _cacheOnStart; set => SetProperty(ref _cacheOnStart, value); }
        public int cacheHours { get => _cacheHours; set => SetProperty(ref _cacheHours, value); }

        public bool pane_status { get => _pane_status; set => SetProperty(ref _pane_status, value); }
        public bool pane_history { get => _pane_history; set => SetProperty(ref _pane_history, value); }
        public bool pane_docs { get => _pane_docs; set => SetProperty(ref _pane_docs, value); }
        public bool pane_case { get => _pane_case; set => SetProperty(ref _pane_case, value); }
        public bool pane_test { get => _pane_test; set => SetProperty(ref _pane_test, value); }
        public bool pane_trends { get => _pane_trends; set => SetProperty(ref _pane_trends, value); }
        public bool pane_log { get => _pane_log; set => SetProperty(ref _pane_log, value); }
        public bool pane_info { get => _pane_info; set => SetProperty(ref _pane_info, value); }
        public bool pane_debug { get => _pane_debug; set => SetProperty(ref _pane_debug, value); }

        public bool verboseLog { get => _verboseLog; set => SetProperty(ref _verboseLog, value); }
        public int maxRetry { get => _maxRetry; set => SetProperty(ref _maxRetry, value); }
        public string histPattern { get => _histPattern; set => SetProperty(ref _histPattern, value); }
        public string ackHistURL { get => _ackHistURL; set => SetProperty(ref _ackHistURL, value); }
        public string ackHistPattern { get => _ackHistPattern; set => SetProperty(ref _ackHistPattern, value); }
        public int histLimit { get => _histLimit; set => SetProperty(ref _histLimit, value); }




        public string showCaseURL { get => _showCaseURL; set => SetProperty(ref _showCaseURL, value); }
        public string showCaseWebUser { get => _showCaseWebUser; set => SetProperty(ref _showCaseWebUser, value); }
        public string showCaseWebPw { get => _showCaseWebPw; set => SetProperty(ref _showCaseWebPw, value); }
        public string casePattern { get => _casePattern; set => SetProperty(ref _casePattern, value); }
        public string createCaseURL { get => _createCaseURL; set => SetProperty(ref _createCaseURL, value); }
        public string createCaseMethod { get => _createCaseMethod; set => SetProperty(ref _createCaseMethod, value); }
        public string createCasePostKey { get => _createCasePostKey; set => SetProperty(ref _createCasePostKey, value); }
        public string caseWebUser { get => _caseWebUser; set => SetProperty(ref _caseWebUser, value); }
        public string caseWebPw { get => _caseWebPw; set => SetProperty(ref _caseWebPw, value); }
        public string caseResponseSuccess { get => _caseResponseSuccess; set => SetProperty(ref _caseResponseSuccess, value); }
        public string caseResponseExists { get => _caseResponseExists; set => SetProperty(ref _caseResponseExists, value); }
        public string caseResponseFailed { get => _caseResponseFailed; set => SetProperty(ref _caseResponseFailed, value); }
        public ObservableCollection<ValuePair> createCasePairs { get => _createCasePairs; set => SetProperty(ref _createCasePairs, value); }
        public Dictionary<string, string> descFiltersDict { get => _descFiltersDict; set => SetProperty(ref _descFiltersDict, value); }

        public SettingsManager()
        {
            XymondAddr = Settings.XymondAddr;
            XymondPort = Settings.XymondPort;
            userSign = Settings.userSign;
            userSignTypes = Settings.userSignTypes;
            userSignType = Settings.userSignType;
            docsURL = Settings.docsURL;
            ColumnDocURL = Settings.ColumnDocURL;
            disPattern = Settings.disPattern;
            ackPattern = Settings.ackPattern;

            showCaseURL = Settings.showCaseURL;
            showCaseWebUser = Settings.showCaseWebUser;
            showCaseWebPw = Settings.showCaseWebPw;

            casePattern = Settings.casePattern;
            createCaseURL = Settings.createCaseURL;
            createCaseMethod = Settings.createCaseMethod;
            createCasePostKey = Settings.createCasePostKey;
            createCasePairs = Settings.createCasePairs;

            caseWebUser = Settings.createCaseWebUser;
            caseWebPw = Settings.createCaseWebPw;
            caseResponseSuccess = Settings.caseResponseSuccess;
            caseResponseExists = Settings.caseResponseExists;
            caseResponseFailed = Settings.caseResponseFailed;

            refreshDelay = Settings.refreshDelay / 1000;  // (ms) - Time to delay before next refresh
            refreshSpan = Settings.refreshSpan; // (s) - How far to look back with each refresh
            webUser = Settings.webUser;
            webPw = Settings.webPw;
            printFriendlyPattern = Settings.printFriendlyPattern;
            printFriendlyReq = Settings.printFriendlyReq;

            CommandLabels = Settings.CommandLabels;
            CommandTextBox = Settings.CommandTextBox;
            ColumnHeaderFlyout = Settings.ColumnHeaderFlyout;
            ColumnHeaderInline = !ColumnHeaderFlyout;
            emptyPaneFill = Settings.emptyPaneFill;
            maxLineLength = Settings.maxLineLength;
            maxLines = Settings.maxLines;
            textSize = Settings.textSize;
            onGreenDelay = Settings.onGreenDelay;
            newBold = Settings.newBold;
            newSaturate = Settings.newSaturate;
            ackBold = Settings.ackBold;
            ackSaturate = Settings.ackSaturate;
            showManualBlue = Settings.showManualBlue;
            showConsole = Settings.showConsole;
            disableUntilGreen = Settings.disableUntilGreen;

            cacheOnStart = Settings.cacheOnStart;
            cacheHours = Settings.cacheHours;

            pane_status = Settings.pane_status;
            pane_history = Settings.pane_history;
            pane_docs = Settings.pane_docs;
            pane_case = Settings.pane_case;
            pane_test = Settings.pane_test;
            pane_trends = Settings.pane_trends;
            pane_log = Settings.pane_log;
            pane_info = Settings.pane_info;
            pane_debug = Settings.pane_debug;


            descFiltersDict = Settings.descFiltersDict;
        }


        public static void ResetSettings()
        {
            Windows.Storage.ApplicationDataContainer ls = Windows.Storage.ApplicationData.Current.LocalSettings;
            bool hasContainer = ls.Containers.ContainsKey("xyvContainer");
            if (!hasContainer)
                ls.DeleteContainer("xyvContainer");
        }

        public static void InitBrushes()
        {
           Settings.red_brush = Jlib.GetSolidColorBrush(Settings.red_hex);
           Settings.strongRed_brush = Jlib.GetSolidColorBrush(Settings.strongRed_hex);
           Settings.yellow_brush = Jlib.GetSolidColorBrush(Settings.yellow_hex);
           Settings.strongYellow_brush = Jlib.GetSolidColorBrush(Settings.yellow_hex);
           Settings.purple_brush = Jlib.GetSolidColorBrush(Settings.purple_hex);
           Settings.strongPurple_brush = Jlib.GetSolidColorBrush(Settings.strongPurple_hex);
           Settings.clear_brush = Jlib.GetSolidColorBrush(Settings.clear_hex);
           Settings.blue_brush = Jlib.GetSolidColorBrush(Settings.blue_hex);
           Settings.green_brush = Jlib.GetSolidColorBrush(Settings.green_hex);
        }

    }

}




