using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Viewer_for_Xymon
{
    public sealed partial class MainPage : Page
    {
        public async void getSavedSettings()
        {
            //bool missingXymondAddr = true;
            bool missinguserSign = true;
            bool missingdocsURL = true;
            bool missingcasePattern = true;
            bool missingshowCaseURL = true;

            Windows.Storage.ApplicationDataContainer ls = Windows.Storage.ApplicationData.Current.LocalSettings;
            bool hasContainer = ls.Containers.ContainsKey("xyvContainer");
            if (hasContainer)
            {
                Debug.WriteLine("Found localsetting:s container");
                var c = ls.Containers["xyvContainer"];

                //string[] se = {"XymondAddr", "userSign", "docsURL", "refreshDelay", "refreshSpan",
                //               "ackPattern", "disPattern", "printFriendlyPattern", "printFriendlyReq",
                //               "webUser", "webPw",
                //               "CommandLabels", "CommandTestBox", "ColumnHeaderFlyout", "ColoumnHeaderInline",
                //               "emptyPaneFill", "maxLineLength", "maxLines", "onGreenDelay",
                //               "showCaseURL", "casePattern", "createCaseURL", "createCaseMethod", "createCasePostKey"};
                //List<string> settingsEnum = new List<string>(se);
                //foreach (string s in settingsEnum)
                //{
                //    if (c.Values.ContainsKey(s))
                //    {
                //        Debug.WriteLine("Found localsetting:: " + s);
                //        Settings.XymondAddr = c.Values[s] as string;
                //    }
                //}

                if (c.Values.ContainsKey("XymondAddr"))
                {
                    Debug.WriteLine("Found localsetting: XymondAddr");
                    Settings.XymondAddr = c.Values["XymondAddr"] as string;
                    //missingXymondAddr = false; //Deprecated
                    if (!(Settings.XymondAddr == null || Settings.XymondAddr == ""))
                    {
                        Status.gotXymonName = true;
                    }
                }
                if (c.Values.ContainsKey("XymondPort"))
                {
                    Settings.XymondPort = (int)c.Values["XymondPort"];
                }
                if (c.Values.ContainsKey("userSignType"))
                {
                    Debug.WriteLine("Found localsetting: userSignType");
                    Settings.userSignType = c.Values["userSignType"] as string;
                }
                if (c.Values.ContainsKey("userSign"))
                {
                    Settings.userSign = c.Values["userSign"] as string;
                    missinguserSign = false; // Deprecated
                }
                if (c.Values.ContainsKey("disPattern"))
                {
                    Debug.WriteLine("Found localsetting: disPattern");
                    Settings.disPattern = c.Values["disPattern"] as string;
                }
                if (c.Values.ContainsKey("ackPattern"))
                {
                    Debug.WriteLine("Found localsetting: ackPattern");
                    Settings.ackPattern = c.Values["ackPattern"] as string;
                }
                if (c.Values.ContainsKey("casePattern"))
                {
                    Settings.casePattern = c.Values["casePattern"] as string;
                    missingcasePattern = false;
                }
                if (c.Values.ContainsKey("showCaseURL"))
                {
                    Settings.showCaseURL = c.Values["showCaseURL"] as string;
                    missingshowCaseURL = false;
                }
                if (c.Values.ContainsKey("createCaseURL"))
                {
                    Debug.WriteLine("Found localsetting: createCaseURL");
                    Settings.createCaseURL = c.Values["createCaseURL"] as string;
                }
                if (c.Values.ContainsKey("createCaseMethod"))
                {
                    Debug.WriteLine("Found localsetting: createCaseMethod");
                    Settings.createCaseMethod = c.Values["createCaseMethod"] as string;
                }
                if (c.Values.ContainsKey("createCasePostKey"))
                {
                    Debug.WriteLine("Found localsetting: createCasePostKey");
                    Settings.createCasePostKey = c.Values["createCasePostKey"] as string;
                }
                if (c.Values.ContainsKey("createCaseWebUser"))
                {
                    Debug.WriteLine("Found localsetting: createCaseWebUser");
                    Settings.createCaseWebUser = c.Values["createCaseWebUser"] as string;
                }
                if (c.Values.ContainsKey("createCaseWebPw"))
                {
                    Debug.WriteLine("Found localsetting: createCaseWebPw");
                    Settings.createCaseWebPw = c.Values["createCaseWebPw"] as string;
                }
                if (c.Values.ContainsKey("showCaseWebUser"))
                {
                    Debug.WriteLine("Found localsetting: showCaseWebUser");
                    Settings.showCaseWebUser = c.Values["showCaseWebUser"] as string;
                }
                if (c.Values.ContainsKey("showCaseWebPw"))
                {
                    Debug.WriteLine("Found localsetting: showCaseWebPw");
                    Settings.showCaseWebPw = c.Values["showCaseWebPw"] as string;
                }
                if (c.Values.ContainsKey("caseResponseSuccess"))
                {
                    Debug.WriteLine("Found localsetting: caseResponseSuccess");
                    Settings.caseResponseSuccess = c.Values["caseResponseSuccess"] as string;
                }
                if (c.Values.ContainsKey("caseResponseExists"))
                {
                    Debug.WriteLine("Found localsetting: caseResponseExists");
                    Settings.caseResponseExists = c.Values["caseResponseExists"] as string;
                }
                if (c.Values.ContainsKey("caseResponseFailed"))
                {
                    Debug.WriteLine("Found localsetting: caseResponseFailed");
                    Settings.caseResponseFailed = c.Values["caseResponseFailed"] as string;
                }
                if (c.Values.ContainsKey("printFriendlyPattern"))
                {
                    Debug.WriteLine("Found localsetting: printFriendlyPattern");
                    Settings.printFriendlyPattern = c.Values["printFriendlyPattern"] as string;
                }
                if (c.Values.ContainsKey("printFriendlyReq"))
                {
                    Debug.WriteLine("Found localsetting: printFriendlyReq");
                    Settings.printFriendlyReq = c.Values["printFriendlyReq"] as string;
                }
                if (c.Values.ContainsKey("ColumnDocURL"))
                {
                    Debug.WriteLine("Found localsetting: ColumnDocURL");
                    Settings.ColumnDocURL = c.Values["ColumnDocURL"] as string;
                }
                if (c.Values.ContainsKey("docsURL"))
                {
                    Settings.docsURL = c.Values["docsURL"] as string;
                    missingdocsURL = false;
                }
                if (c.Values.ContainsKey("onGreenDelay"))
                {
                    Debug.WriteLine("Found localsetting: onGreenDelay");
                    Settings.onGreenDelay = (int)c.Values["onGreenDelay"];
                }
                if (c.Values.ContainsKey("refreshDelay"))
                {
                    Debug.WriteLine("Found localsetting: refreshDelay");
                    Settings.refreshDelay = (int)c.Values["refreshDelay"] * 1000;
                }
                if (c.Values.ContainsKey("refreshSpan"))
                {
                    Debug.WriteLine("Found localsetting: refreshSpan");
                    Settings.refreshSpan = (int)c.Values["refreshSpan"];
                }
                if (c.Values.ContainsKey("emptyPaneFill"))
                {
                    Debug.WriteLine("Found localsetting: emptyPaneFill");
                    Settings.emptyPaneFill = c.Values["emptyPaneFill"] as string;
                }
                if (c.Values.ContainsKey("maxLineLength"))
                {
                    Debug.WriteLine("Found localsetting: maxLineLength");
                    Settings.maxLineLength = (int)c.Values["maxLineLength"];
                }
                if (c.Values.ContainsKey("maxLines"))
                {
                    Debug.WriteLine("Found localsetting: maxLines");
                    Settings.maxLines = (int)c.Values["maxLines"];
                }
                if (c.Values.ContainsKey("textSize"))
                {
                    Debug.WriteLine("Found localsetting: textSize");
                    Settings.textSize = (int)c.Values["textSize"];
                    DataGrid.FontSize = Settings.textSize;
                }

                if (c.Values.ContainsKey("CommandLabels"))
                {
                    Debug.WriteLine("Found localsetting: CommandLabels");
                    Settings.CommandLabels = (bool)c.Values["CommandLabels"];
                }
                if (Settings.CommandLabels) ToggleToolbarLabelsOn(null, null);
                else ToggleToolbarLabelsOff(null, null);

                if (c.Values.ContainsKey("CommandTextBox"))
                {
                    Debug.WriteLine("Found localsetting: CommandTextBox");
                    Settings.CommandTextBox = (bool)c.Values["CommandTextBox"];
                }
                if (Settings.CommandTextBox) cmdTextBox.Visibility = Windows.UI.Xaml.Visibility.Visible;
                else cmdTextBox.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                if (c.Values.ContainsKey("showManualBlue"))
                {
                    Debug.WriteLine("Found localsetting: showManualBlue");
                    Settings.showManualBlue = (bool)c.Values["showManualBlue"];
                }
                if (Settings.showManualBlue) toggleManualBlue.Visibility = Windows.UI.Xaml.Visibility.Visible;
                else toggleManualBlue.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                if (c.Values.ContainsKey("showConsole"))
                {
                    Debug.WriteLine("Found localsetting: showConsole");
                    Settings.showConsole = (bool)c.Values["showConsole"];
                }
                if (Settings.showConsole) ConsoleGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
                else ConsoleGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                if (c.Values.ContainsKey("disableUntilGreen"))
                {
                    Debug.WriteLine("Found localsetting: disableUntilGreen");
                    Settings.disableUntilGreen = (bool)c.Values["disableUntilGreen"];
                }

                //if (c.Values.ContainsKey("ColumnHeaderFlyout"))
                //{
                //    Debug.WriteLine("Found localsetting: ColumnHeaderFlyout");
                //    Settings.ColumnHeaderFlyout = c.Values["ColumnHeaderFlyout"] as string;
                //}
                if (c.Values.ContainsKey("newSaturate"))
                {
                    Debug.WriteLine("Found localsetting: newSaturate");
                    Settings.newSaturate = (bool)c.Values["newSaturate"];
                }
                if (c.Values.ContainsKey("newBold"))
                {
                    Debug.WriteLine("Found localsetting: newBold");
                    Settings.newBold = (bool)c.Values["newBold"];
                }
                if (c.Values.ContainsKey("ackSaturate"))
                {
                    Debug.WriteLine("Found localsetting: ackSaturate");
                    Settings.ackSaturate = (bool)c.Values["ackSaturate"];
                }
                if (c.Values.ContainsKey("ackBold"))
                {
                    Debug.WriteLine("Found localsetting: ackBold");
                    Settings.ackBold = (bool)c.Values["ackBold"];
                }

                if (c.Values.ContainsKey("pane_status")) { Debug.WriteLine("Found localsetting: pane_status"); Settings.pane_status = (bool)c.Values["pane_status"]; }
                if (c.Values.ContainsKey("pane_history")) { Debug.WriteLine("Found localsetting: pane_history"); Settings.pane_history = (bool)c.Values["pane_history"]; }
                if (c.Values.ContainsKey("pane_docs")) { Debug.WriteLine("Found localsetting: pane_docs"); Settings.pane_docs = (bool)c.Values["pane_docs"]; }
                if (c.Values.ContainsKey("pane_case")) { Debug.WriteLine("Found localsetting: pane_case"); Settings.pane_case = (bool)c.Values["pane_case"]; }
                if (c.Values.ContainsKey("pane_test")) { Debug.WriteLine("Found localsetting: pane_test"); Settings.pane_test = (bool)c.Values["pane_test"]; }
                if (c.Values.ContainsKey("pane_trends")) { Debug.WriteLine("Found localsetting: pane_trends"); Settings.pane_trends = (bool)c.Values["pane_trends"]; }
                if (c.Values.ContainsKey("pane_log")) { Debug.WriteLine("Found localsetting: pane_log"); Settings.pane_log = (bool)c.Values["pane_log"]; }
                if (c.Values.ContainsKey("pane_info")) { Debug.WriteLine("Found localsetting: pane_info"); Settings.pane_info = (bool)c.Values["pane_info"]; }
                if (c.Values.ContainsKey("pane_debug")) { Debug.WriteLine("Found localsetting: pane_debug"); Settings.pane_debug = (bool)c.Values["pane_debug"]; }

                if (c.Values.ContainsKey("verboseLog")) { Debug.WriteLine("Found localsetting: verboseLog"); Settings.verboseLog = (bool)c.Values["verboseLog"]; }
                if (c.Values.ContainsKey("cacheOnStart")) { Debug.WriteLine("Found localsetting: cacheOnStart"); Settings.cacheOnStart = (bool)c.Values["cacheOnStart"]; }
                if (c.Values.ContainsKey("cacheHours")) { Debug.WriteLine("Found localsetting: cacheHours"); Settings.cacheHours = (int)c.Values["cacheHours"]; }

                if (c.Values.ContainsKey("maxRetry")) { Debug.WriteLine("Found localsetting: maxRetry"); Settings.maxRetry = (int)c.Values["maxRetry"]; }

                if (c.Values.ContainsKey("histPattern")) { Debug.WriteLine("Found localsetting: histPattern"); Settings.histPattern = (string)c.Values["histPattern"]; }
                if (c.Values.ContainsKey("ackHistUrl")) { Debug.WriteLine("Found localsetting: ackHistUrl"); Settings.ackHistURL = (string)c.Values["ackHistUrl"]; }
                if (c.Values.ContainsKey("ackHistPattern")) { Debug.WriteLine("Found localsetting: ackHistPattern"); Settings.ackHistPattern = (string)c.Values["ackHistPattern"]; }
                if (c.Values.ContainsKey("histLimit")) { Debug.WriteLine("Found localsetting: histLimit"); Settings.histLimit = (int)c.Values["histLimit"]; }



                if (c.Values.ContainsKey("createCasePairs"))
                {
                    Windows.Storage.ApplicationDataCompositeValue composite = new Windows.Storage.ApplicationDataCompositeValue();
                    composite = c.Values["createCasePairs"] as Windows.Storage.ApplicationDataCompositeValue;
                    //var ccp = new List<ValuePair>();
                    Settings.createCasePairs.Clear();
                    foreach (var x in composite)
                    {
                        Settings.createCasePairs.Add(new ValuePair(x.Key as string, x.Value as string));
                    }
                    Debug.WriteLine("Found localsetting:: createCasePairs");
                }


                if (c.Values.ContainsKey("descFilters"))
                {
                    Windows.Storage.ApplicationDataCompositeValue composite = new Windows.Storage.ApplicationDataCompositeValue();
                    composite = c.Values["descFilters"] as Windows.Storage.ApplicationDataCompositeValue;
                    var dfC = new ObservableCollection<DescFilter>();
                    foreach (var x in composite)
                    {
                        dfC.Add(new DescFilter(x.Key, x.Value as string));
                    }
                    Settings.descFiltersDict = DescFilter.DescFilterC2D(dfC);
                }
                else Settings.descFiltersDict = DescFilter.DefaultDescFilters();

            }
            else Settings.descFiltersDict = DescFilter.DefaultDescFilters();

            Debug.WriteLine("Finished getting local settings");

            switch (Settings.userSignType)
            {
                case "Custom":
                    //Settings.userSign = Settings.userSign;
                    break;
                case "Display name":
                    Task<string> t_displayName = Jlib.getUserInfo("Display name");
                    await t_displayName;
                    Settings.userSign = t_displayName.Result;
                    break;
                case "Domain user":
                    Task<string> t_domainUser = Jlib.getUserInfo("Domain user");
                    await t_domainUser;
                    Settings.userSign = t_domainUser.Result;
                    break;
                case "RFC 822":
                    Task<string> t_rfc822 = Jlib.getUserInfo("RFC 822");
                    await t_rfc822;
                    Settings.userSign = t_rfc822.Result;
                    break;
                default:
                    break;
            }
            if (!(Settings.userSign == null || Settings.userSign == ""))
            {
                Status.gotUserSign = true;
            }

            //if (missingXymondAddr || Settings.XymondAddr == "" || Settings.XymondAddr == null)
            //{
            //    delayStart = true;
            //    await OpenSettingsDialog(null, null);
            //}
            if (missingshowCaseURL || missingcasePattern || Settings.showCaseURL == "" || Settings.casePattern == "")
            {
                caseBtn.IsEnabled = false;
            }
            if (missingdocsURL || Settings.docsURL == "" || Settings.docsURL == null)
            {
                docsBtn.IsEnabled = false;
            }
            if (missinguserSign || Settings.userSign == null || Settings.userSign == "")
            {
             
            }

            if (Settings.pane_test) SideTest.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else SideTest.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            if (Settings.pane_docs) docsBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else docsBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            if (Settings.pane_case) caseBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else caseBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            if (Settings.pane_log) logsBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else logsBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            if (Settings.pane_trends) SideTrends.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else SideTrends.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            if (Settings.pane_debug) debugBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else debugBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;


            if (Settings.pane_debug)
            {
                Heading_debug.Visibility = Windows.UI.Xaml.Visibility.Visible;
                RefreshSelectedBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
                CropBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
                CacheBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
                togglePause.Visibility = Windows.UI.Xaml.Visibility.Visible;
                ClearBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;

                // Custom filters, feature in progress. Show/hide with debug for now
                Heading_filter.Visibility = Windows.UI.Xaml.Visibility.Visible;
                myAcksBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
                preDef1.Visibility = Windows.UI.Xaml.Visibility.Visible;

            }
            else
            {
                Heading_debug.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                RefreshSelectedBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                CropBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                CacheBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                togglePause.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                ClearBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                //// Custom filters, feature in progress. Show/hide with debug for now
                //Heading_filter.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                //myAcksBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                //preDef1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }



        }


    }
}
