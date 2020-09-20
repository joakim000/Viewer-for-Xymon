using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.Web.Http;
using Telerik.Data.Core;

namespace Viewer_for_Xymon
{
    public sealed partial class MainPage : Page
    {

     

    /*****************************/
    /* Toolbar                   */
    /*****************************/
    private void GetBacklog(object sender, RoutedEventArgs e)
        {
            var btn = sender as AppBarToggleButton;
            //btn.IsChecked = true;
            string colorScope = " color=";
            switch (btn.Name)
            {
                case "toggleRed":        colorScope += "red"; break;
                case "toggleYellow":     colorScope += "yellow"; break;
                case "togglePurple":     colorScope += "purple"; break;
                case "toggleClear":      colorScope += "clear"; break;
                case "toggleBlue":       colorScope += "blue"; break;
                case "toggleManualBlue": colorScope += "blue"; break;
                case "toggleGreen":      colorScope += "green"; break;
                default:                 colorScope += "none"; break;
            }
            if (colorScope != " color=none")
            {
                Status.processing = true;
                Status.processingFullMsg = true;
                Status.processingType = "backlog";
                

                long refreshScope = 0; // Get from beginning of time
                string xymonCmd = "xymondboard " + xymonConnect.nonTestsPatternBuilder() + "lastchange>" + refreshScope.ToString() + colorScope + Settings.fields;
                Debug.WriteLine(xymonCmd + Environment.NewLine + Environment.NewLine);
                
                xymonGetAsync(xymonCmd);
            }
            else Debug.WriteLine("GetBacklog called by unknown element ");
        }

            

        private void ToggleAckOn(object sender, RoutedEventArgs e)
        {
            colorFilters.Descriptors.Add(cf.showAck);
            RefreshColorFilters();
        }

        private void ToggleAckOff(object sender, RoutedEventArgs e)
        {
            colorFilters.Descriptors.Remove(cf.showAck);
            RefreshColorFilters();
        }
        private void ToggleGreenOn(object sender, RoutedEventArgs e)
        {
            colorFilters.Descriptors.Add(cf.showGreen);
            RefreshColorFilters();
        }

        private void ToggleGreenOff(object sender, RoutedEventArgs e)
        {
            colorFilters.Descriptors.Remove(cf.showGreen);
            RefreshColorFilters();
        }
        private void ToggleClearOn(object sender, RoutedEventArgs e)
        {
            colorFilters.Descriptors.Add(cf.showClear);
            RefreshColorFilters();
        }

        private void ToggleClearOff(object sender, RoutedEventArgs e)
        {
            colorFilters.Descriptors.Remove(cf.showClear);
            RefreshColorFilters();
        }
        private void ToggleBlueOn(object sender, RoutedEventArgs e)
        {
            colorFilters.Descriptors.Add(cf.showBlue);
            toggleManualBlue.IsChecked = false;
            this.dismsgCol.IsVisible = true;
            this.disuserCol.IsVisible = true;
            //this.disabletimeCol.IsVisible = true;
            RefreshColorFilters();
        }

        private void ToggleBlueOff(object sender, RoutedEventArgs e)
        {
            colorFilters.Descriptors.Remove(cf.showBlue);
            this.dismsgCol.IsVisible = false;
            this.disuserCol.IsVisible = false;
            //this.disabletimeCol.IsVisible = false;
            RefreshColorFilters();
        }
        private void ToggleManualBlueOn(object sender, RoutedEventArgs e)
        {
            colorFilters.Descriptors.Add(cf.showManualBlue);
            toggleBlue.IsChecked = false;
            this.dismsgCol.IsVisible = true;
            this.disuserCol.IsVisible = true;
            RefreshColorFilters();
        }

        private void ToggleManualBlueOff(object sender, RoutedEventArgs e)
        {
            colorFilters.Descriptors.Remove(cf.showManualBlue);
            this.dismsgCol.IsVisible = false;
            this.disuserCol.IsVisible = false;
            RefreshColorFilters();
        }
        private void ToggleRedOn(object sender, RoutedEventArgs e)
        {
            colorFilters.Descriptors.Add(cf.showRed);
            RefreshColorFilters();
        }

        private void ToggleRedOff(object sender, RoutedEventArgs e)
        {
            colorFilters.Descriptors.Remove(cf.showRed);
            RefreshColorFilters();
        }
        private void ToggleYellowOn(object sender, RoutedEventArgs e)
        {
            colorFilters.Descriptors.Add(cf.showYellow);
            RefreshColorFilters();
        }

        private void ToggleYellowOff(object sender, RoutedEventArgs e)
        {
            colorFilters.Descriptors.Remove(cf.showYellow);
            RefreshColorFilters();
        }
        private void TogglePurpleOn(object sender, RoutedEventArgs e)
        {
            colorFilters.Descriptors.Add(cf.showPurple);
            RefreshColorFilters();
        }

        private void TogglePurpleOff(object sender, RoutedEventArgs e)
        {
            colorFilters.Descriptors.Remove(cf.showPurple);
            RefreshColorFilters();
        }
        
        private void ToggleLockOn(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(webView1.AccessKey);
            Debug.WriteLine(webView1.AccessKeyScopeOwner);
            Debug.WriteLine(webView1.ActualHeight);
            Debug.WriteLine(webView1.ActualWidth);
            Debug.WriteLine(webView1.AllowDrop);
            //Debug.WriteLine(webView1.AllowedScriptNotifyUris);
            Debug.WriteLine(webView1.AllowFocusOnInteraction);
            Debug.WriteLine(webView1.AllowFocusWhenDisabled);
            Debug.WriteLine(webView1.BaseUri);
            //Debug.WriteLine(webView1);


            // Fill with stuff if empty
            if (DataGrid.SelectedItems.Count < 1)
            {
                webView1.Navigate(new Uri(Settings.emptyPaneFill));
            }
            else if (DataGrid.SelectedItems.Count > 1)
            {
                SelectPane();
            }
            
            else if (webView1.Source == null)
            {
                if (DataGrid.SelectedItems.Count == 1)
                {
                    //e = new RoutedEventArgs();
                    //SideStatusBtn(sender, e);
                    OpenStatus(0);
                }
                DataGrid_SelectionChanged(null, null);
            }


            // Actual main things to for this function
            SplitMain.DisplayMode = SplitViewDisplayMode.CompactInline;
            SplitMain.IsPaneOpen = true;
            Status.lockedPane = true;

            DataGrid.InvalidateArrange();
            DataGrid.UpdateLayout();
            //RefreshColorFilters(); // To refresh column widths
            RefreshGridView();

            Debug.WriteLine("Lock pane on. webView1.Source = " + webView1.Source);

        }
        private void ToggleLockOff(object sender, RoutedEventArgs e)
        {
            SplitMain.DisplayMode = SplitViewDisplayMode.CompactOverlay;
            SplitMain.IsPaneOpen = false;
            Status.lockedPane = false;
        }

        private void ToggleMultiOn(object sender, RoutedEventArgs e)
        {
            DataGrid.SelectionMode = DataGridSelectionMode.Multiple;
            //SelectAllBtn.IsEnabled = true;
            DataGrid_SelectionChanged(null, null);
        }
        private void ToggleMultiOff(object sender, RoutedEventArgs e)
        {
            DataGrid.SelectionMode = DataGridSelectionMode.Extended;
            //SelectAllBtn.IsEnabled = false;
        }


        private void SelectAllBtnTapped(object sender, RoutedEventArgs e)
        {
            ToggleMultiBtn.IsChecked = true;
            //DataGrid.SelectionMode = DataGridSelectionMode.Multiple;
            DataGrid.SelectAll();
        }
        private void SelectAllBtnTapped(object sender, AccessKeyInvokedEventArgs e)
        {
            ToggleMultiBtn.IsChecked = true;
            //DataGrid.SelectionMode = DataGridSelectionMode.Multiple;
            DataGrid.SelectAll();
        }

        private void ClearSelectionBtn_Tapped(object sender, RoutedEventArgs e)
        {
            DataGrid.DeselectAll();
            //DataGrid.SelectionMode = DataGridSelectionMode.Single;
            ToggleMultiBtn.IsChecked = false;

        }
        private void ClearSelectionBtn_Tapped(object sender, AccessKeyInvokedEventArgs e)
        {

            DataGrid.DeselectAll();
            //DataGrid.SelectionMode = DataGridSelectionMode.Single;
            ToggleMultiBtn.IsChecked = false;

        }

        private void ResetFiltersBtn_Tapped(object sender, RoutedEventArgs e)
        {
            HamburgerPane.IsPaneOpen = false;
            InitColorFilters();
            //DataGrid.SelectionMode = DataGridSelectionMode.Single;
        }
        private void ResetFiltersBtn_Tapped(object sender, AccessKeyInvokedEventArgs e)
        {
            HamburgerPane.IsPaneOpen = false;
            InitColorFilters();
            //DataGrid.SelectionMode = DataGridSelectionMode.Single;
        }

        private void CropBtn_Tapped(object sender, RoutedEventArgs e)
        {
            HamburgerPane.IsPaneOpen = false;
            var selected = DataGrid.SelectedItems;
            foreach (Fount f in selected)
            {
                Model.fc.Remove(f);
            }
            DataGrid.DeselectAll();
        }

        private void CropBtn_RightTapped(object sender, RoutedEventArgs e)
        {
            HamburgerPane.IsPaneOpen = false;
            var toCrop = new List<Fount>();       
            foreach (Fount f in Model.fc) if (f.color == "green") toCrop.Add(f);
            foreach (Fount f in toCrop) Model.fc.Remove(f);
            //foreach (Fount f in Model.fc) if (f.color == "green") Model.fc.Remove(f);
            DataGrid.DeselectAll();
            //toCrop.Finalize();
        }

        private void CropBtn_RightTapped(object sender, AccessKeyInvokedEventArgs e)
        {
            HamburgerPane.IsPaneOpen = false;
            var selected = DataGrid.SelectedItems;
            foreach (Fount f in selected)
            {
                Model.fc.Remove(f);
            }
            DataGrid.DeselectAll();
        }

        /*****************************/
        /* Details pane (right side) */
        /*****************************/
        private void DrawerBtn(object sender, RoutedEventArgs e)  //Deprecated
        {
            SplitMain.IsPaneOpen = !SplitMain.IsPaneOpen;
        }

        public  void SideStatusBtn(object sender, RoutedEventArgs e)
        {
            OpenStatus(0);
        }
        public void SideStatusBtn(object sender, AccessKeyInvokedEventArgs e)
        {
            OpenStatus(0);
        }

        private void SideStatusSecondary(object sender, RoutedEventArgs e)
        {
            OpenStatus(1);
        }


        private void SideStatusDbl(object sender, RoutedEventArgs e)
        {
            OpenStatus(2);
        }

        private void SideHistoryBtn(object sender, RoutedEventArgs e)
        {
            OpenHistory(Settings.histLimit);
        }
        private void SideHistoryBtn(object sender, AccessKeyInvokedEventArgs e)
        {
            OpenHistory(Settings.histLimit);
        }
        private void SideHistorySecondary(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItems;
            if (selected.Count() >= 1)
            {
                Fount f = selected.Last() as Fount;
                Uri navPage = new XWeb().buildUri(Settings.XymonCGIURL + "/history.sh?HISTFILE=" + f.hostname + "." + f.testname + "&BARSUMS=15");
                DefaultLaunch(navPage);
            }
        }
        private void SideHistoryDbl(object sender, RoutedEventArgs e)
        {
            OpenHistory(0);
        }

        private void SideDocsBtn(object sender, RoutedEventArgs e)
        {
            OpenDocs();
        }
        private void SideDocsBtn(object sender, AccessKeyInvokedEventArgs e)
        {
            OpenDocs();
        }
        private void SideDocsDbl(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItems;
            if (selected.Count() >= 1)
            {
                Fount f = selected.Last() as Fount;
                //Uri navPage = new XWeb().buildUri(Settings.XymonCGIURL + "/notes?" + f.hostname);
                Uri navPage = new XWeb().buildUri(Settings.docsURL.Replace("%HOSTNAME%", f.hostname));
                DefaultLaunch(navPage);
            }
        }

        private void SideTrendsBtn(object sender, RoutedEventArgs e)
        {
            OpenTrends();
        }
        private void SideTrendsBtn(object sender, AccessKeyInvokedEventArgs e)
        {
            OpenTrends();
        }
        private void SideTrendsDbl(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItems;
            if (selected.Count() >= 1)
            {
                Fount f = selected.Last() as Fount;
                Uri navPage = new XWeb().buildUri(Settings.XymonCGIURL + "/svcstatus.sh?HOST=" + f.hostname + "&SERVICE=" + Settings.TrendsCol);
                DefaultLaunch(navPage);
            }
        }

        private void SideTestBtn(object sender, RoutedEventArgs e)
        {
            OpenTest();
        }
        private void SideTestBtn(object sender, AccessKeyInvokedEventArgs e)
        {
            OpenTest();
        }
        private void SideTestDbl(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItems;
            if (selected.Count() >= 1)
            {
                Fount f = selected.Last() as Fount;
                Uri navPage = new XWeb().buildUri(Settings.ColumnDocURL.Replace("%TESTNAME%", f.testname));
                DefaultLaunch(navPage);
            }
        }

        private void SideCaseBtn(object sender, RoutedEventArgs e)
        {
            OpenCase();
        }
        private void SideCaseBtn(object sender, AccessKeyInvokedEventArgs e)
        {
            OpenCase();
        }
        private void SideCaseDbl(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItems;
            if (selected.Count() >= 1)
            {
                bool matchInAck = true;
                string caseRef = String.Empty;
                Fount f = selected.Last() as Fount;
                try
                {
                    Match m = Regex.Match(f.ackmsg, Settings.casePattern);
                    if (!m.Success)
                    {
                        matchInAck = false;
                        m = Regex.Match(f.dismsg, Settings.casePattern);
                    }
                    if (m.Success && matchInAck) caseRef = f.ackmsg.Substring(m.Index, m.Length);
                    if (m.Success && !matchInAck) caseRef = f.dismsg.Substring(m.Index, m.Length);
                    if (m.Success)
                    {
                        Uri navPage = new Uri(Settings.showCaseURL.Replace("%TICKET%", caseRef));
                        DefaultLaunch(navPage);
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " Error matching casepattern: " + exception);
                    Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " Error matching casepattern: " + exception);
                }
                //Debug.WriteLine(f.ackmsg + " " + Settings.casePattern + " " + m.Value);
                                
            }
        }

        private void logsBtn_Tapped(object sender, RoutedEventArgs e)
        {
            OpenLog();
        }
        private void logsBtn_Tapped(object sender, AccessKeyInvokedEventArgs e)
        {
            OpenLog();
        }
        private void logsBtn_RightTapped(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItems;
            if (selected.Count() >= 1)
            {
                Fount f = selected.Last() as Fount;
                Uri navPage = new XWeb().buildUri(Settings.XymonCGIURL + "/svcstatus.sh?HOST=" + f.hostname + "&SERVICE=" + Settings.ClientCol.Replace(" ", "%20"));
                DefaultLaunch(navPage);
            }
        }

        private void infoBtn_Tapped(object sender, RoutedEventArgs e)
        {
            OpenInfo();
        }
        private void infoBtn_Tapped(object sender, AccessKeyInvokedEventArgs e)
        {
            OpenInfo();
        }
        private void infoBtn_RightTapped(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItems;
            if (selected.Count() >= 1)
            {
                Fount f = selected.Last() as Fount;
                Uri navPage = new XWeb().buildUri(Settings.XymonCGIURL + "/svcstatus.sh?HOST=" + f.hostname + "&SERVICE=" + Settings.InfoCol.Replace(" ", "%20"));
                DefaultLaunch(navPage);
            }
        }

        private void backBtn_Tapped(object sender, RoutedEventArgs e)
        {
            if (Web1.CanGoBack) Web1.GoBack();
        }
        private void backBtn_Tapped(object sender, AccessKeyInvokedEventArgs e)
        {
            if (Web1.CanGoBack) Web1.GoBack();
        }
        private void backBtn_RightTapped(object sender, RoutedEventArgs e)
        {
            if (Web1.CanGoForward) Web1.GoForward();
        }

        private void debugBtn_Tapped(object sender, RoutedEventArgs e)
        {
            OpenDebug();
        }
        private void debugBtn_Tapped(object sender, AccessKeyInvokedEventArgs e)
        {
            OpenDebug();
        }


        /*****************************/
        /* Function menu (left side) */
        /*****************************/
        private void HamburgerBtn_Tapped(object sender, RoutedEventArgs e)
        {
            HamburgerPane.IsPaneOpen = !HamburgerPane.IsPaneOpen;
        }

        private void SettingsBtn_PointerExit(object sender, RoutedEventArgs e)
        {

        }
        private async void SettingsBtn_Tapped(object sender, RoutedEventArgs a)
        {
            await OpenSettingsDialog(sender, a);
        }

        
        private void RefreshSelectedBtn_Tapped(object sender, RoutedEventArgs e)
        {
            HamburgerPane.IsPaneOpen = false;
            var selected = DataGrid.SelectedItems;

            //Status.processing = true;
            //Status.processingType = "manual";
            //DataGrid.Focus(FocusState.Pointer);

            Status.processing = true;
            Status.processingFullMsg = true;
            foreach (Fount f in selected)
            {

                xymonGetAsync("xymondboard " + xymonConnect.nonTestsPatternBuilder() + " host=" + f.hostname + " test=" + f.testname + Settings.fields);
            }
            

        }

        private void enableBtn_Tapped(object sender, RoutedEventArgs e)
        {
            HamburgerPane.IsPaneOpen = false;
            var selected = DataGrid.SelectedItems;
            DisableJob enableJob = new DisableJob(selected);
            xymonConnect.xymonEnableAsync(enableJob);
        }

        private void TabPane_Closed(object sender, RoutedEventArgs e)
        {
            //HamburgerBtn.IsChecked = false;
        }
        private void TogglePauseOn(object sender, RoutedEventArgs e)
        {
            HamburgerPane.IsPaneOpen = false;
            Status.refreshActive = false;
        }

        private void TogglePauseOff(object sender, RoutedEventArgs e)
        {
            HamburgerPane.IsPaneOpen = false;
            Status.processing = false;
            Status.refreshActive = true;
            Status.firstUpdate = true;
            string colorScope = " color=";
            if ( toggleRed.IsChecked.Value.Equals(true) ) colorScope = colorScope + "red,";
            if (toggleYellow.IsChecked.Value.Equals(true)) colorScope = colorScope + "yellow,";
            if (togglePurple.IsChecked.Value.Equals(true)) colorScope = colorScope + "purple,";
            if (toggleClear.IsChecked.Value.Equals(true)) colorScope = colorScope + "clear,";
            if (toggleBlue.IsChecked.Value.Equals(true)) colorScope = colorScope + "blue,";
            if (toggleGreen.IsChecked.Value.Equals(true)) colorScope = colorScope + "green,";
            colorScope = colorScope.TrimEnd(',');
            xymonRefresh(colorScope); 
        }

       
        private void ToggleToolbarLabelsOn(object sender, RoutedEventArgs e)
        {
            //ToolBarRowDef.Height = GridLength 60;
            var gl = new GridLength(60);
            MainGrid.RowDefinitions.First().Height = gl;
            toggleDrawerSide.Height = 60;
            List<AppBarToggleButton> toolBarEnum = new List<AppBarToggleButton>
            {
                toggleRed, toggleYellow, togglePurple, toggleClear, toggleBlue, toggleGreen, ToggleMultiBtn, toggleManualBlue, toggleDrawerSide
            };
            List<AppBarButton> toolBarEnum2 = new List<AppBarButton>
            {
                SelectAllBtn, ClearSelectionBtn, AckBtn, DisableBtn, CropBtn, enableBtn
            };
            foreach (AppBarToggleButton btn in toolBarEnum)
            {
                btn.IsCompact = false;
            }
            foreach (AppBarButton btn in toolBarEnum2)
            {
                btn.IsCompact = false;
            }
            //RefreshColorFilters();
            RefreshGridView();
        }

        private void ToggleToolbarLabelsOff(object sender, RoutedEventArgs e)
        {
            //ToolBarRowDef.Height = GridLength 60;
            var gl = new GridLength(48);
            MainGrid.RowDefinitions.First().Height = gl;
            toggleDrawerSide.Height = 48;
            List<AppBarToggleButton> toolBarEnum = new List<AppBarToggleButton>
            {
                toggleRed, toggleYellow, togglePurple, toggleClear, toggleBlue, toggleGreen, ToggleMultiBtn, toggleManualBlue, toggleDrawerSide
            };
            List<AppBarButton> toolBarEnum2 = new List<AppBarButton>
            {
                SelectAllBtn, ClearSelectionBtn, AckBtn, DisableBtn, CropBtn, enableBtn
            };
            foreach (AppBarToggleButton btn in toolBarEnum)
            {
                btn.IsCompact = true;
            }
            foreach (AppBarButton btn in toolBarEnum2)
            {
                btn.IsCompact = true;
            }
            //RefreshColorFilters();
            RefreshGridView();
        }

        private void ShowHelp(object sender, RoutedEventArgs e)
        {
            HamburgerPane.IsPaneOpen = false;
            ViewModel.Alert(null, null, Settings.appName + " " + Settings.appVariant + " " + Settings.appVersion);
            
            ////ToolBarRowDef.Height = GridLength 60;
            //var gl = new GridLength(60);
            //MainGrid.RowDefinitions.First().Height = gl;
            //List<AppBarToggleButton> toolBarEnum = new List<AppBarToggleButton>
            //{
            //    toggleRed, toggleYellow, togglePurple, toggleClear, toggleBlue, toggleGreen, toggleAck, ToggleMultiBtn, //toggleDrawerSide
            //};
            //List<AppBarButton> toolBarEnum2 = new List<AppBarButton>
            //{
            //    SelectAllBtn, ClearSelectionBtn, AckBtn, DisableBtn, CropBtn
            //};
            //foreach (AppBarToggleButton btn in toolBarEnum)
            //{
            //    btn.IsCompact = false;
            //}
            //foreach (AppBarButton btn in toolBarEnum2)
            //{
            //    btn.IsCompact = false;
            //}
            //HelpGrid.Visibility = Visibility.Visible;
        }


        private void HideHelp(object sender, RoutedEventArgs e)
        {
            //ToolBarRowDef.Height = GridLength 60;
            var gl = new GridLength(48);
            MainGrid.RowDefinitions.First().Height = gl;
            List<AppBarToggleButton> toolBarEnum = new List<AppBarToggleButton>
            {
                toggleRed, toggleYellow, togglePurple, toggleClear, toggleBlue, toggleGreen,  ToggleMultiBtn, toggleDrawerSide
            };
            List<AppBarButton> toolBarEnum2 = new List<AppBarButton>
            {
                SelectAllBtn, ClearSelectionBtn, AckBtn, DisableBtn, CropBtn, enableBtn
            };
            foreach (AppBarToggleButton btn in toolBarEnum)
            {
                btn.IsCompact = true;
            }
            foreach (AppBarButton btn in toolBarEnum2)
            {
                btn.IsCompact = true;
            }
            HelpGrid.Visibility = Visibility.Collapsed;
        }

        private void ClearViewBtn(object sender, RoutedEventArgs e)
        {
            HamburgerPane.IsPaneOpen = false;
            Model.fc.Clear();
        }
        private void PopulateBtn(object sender, RoutedEventArgs e)
        {
            if (cmdTextBox.Text.IndexOf("host=") != -1 ||
                cmdTextBox.Text.IndexOf("test=") != -1 ||
                cmdTextBox.Text.IndexOf("color=") != -1 ||
                cmdTextBox.Text.IndexOf("page=") != -1 ||
                cmdTextBox.Text.IndexOf("net=") != -1 ||
                cmdTextBox.Text.IndexOf("ip=") != -1 ||
                cmdTextBox.Text.IndexOf("tag=") != -1 ||
                cmdTextBox.Text.IndexOf("msg=") != -1 ||
                cmdTextBox.Text.IndexOf("ackmsg=") != -1 ||
                cmdTextBox.Text.IndexOf("dismsg=") != -1 ||
                cmdTextBox.Text.IndexOf("XMH_string=") != -1)
            {

                //// Show added host regardless of active colorscope
                //int hostIndex = cmdTextBox.Text.IndexOf("host=");
                //if (hostIndex > -1 )
                //{
                //    String host = String.Empty;
                //    int endIndex = cmdTextBox.Text.IndexOf(" ", hostIndex);
                //    if (endIndex > -1)
                //    {
                //        host = cmdTextBox.Text.Substring(hostIndex + 5, endIndex);
                //    }
                //    else
                //    {
                //        host = cmdTextBox.Text.Substring(hostIndex + 5);
                //    }
                //    Debug.WriteLine("Adding filter for: " + host);
                //    TextFilterDescriptor hostFilter = new TextFilterDescriptor();
                //    hostFilter.PropertyName = "hostname";
                //    hostFilter.Operator = TextOperator.EqualsTo;
                //    hostFilter.Value = host;
                //    colorFilters.Descriptors.Add(hostFilter);
                //    RefreshColorFilters();
                //}

                Status.processing = true;
                Status.processingFullMsg = true;
                Status.processingType = "manual";
                Status.processingCheckFirst = true;
                DataGrid.Focus(FocusState.Pointer);
                xymonGetAsync("xymondboard " + xymonConnect.nonTestsPatternBuilder() + cmdTextBox.Text + Settings.fields);
            }
        }

        private void toggleNoProp_Checked(object sender, RoutedEventArgs a)
        {
            Settings.showNoProp = true;
        }
        private void toggleNoProp_Unchecked(object sender, RoutedEventArgs a)
        {
            Settings.showNoProp = false;
        }
        private void toggleNoBb2_Checked(object sender, RoutedEventArgs a)
        {
            Settings.showNoNonGreen = true;
        }
        private void toggleNoBb2_Unchecked(object sender, RoutedEventArgs a)
        {
            Settings.showNoNonGreen = false;
        }

        private void UpdateTextBlock_Tapped(object sender, RoutedEventArgs a)
        {
            Settings.statusRowEpoch = !Settings.statusRowEpoch;
        }
        private void CollSizeTextBlock_Tapped(object sender, RoutedEventArgs a)
        {
            Settings.showConsole = !Settings.showConsole;
            if (Settings.showConsole)
                ConsoleGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else
                ConsoleGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void preDef1_Checked(object sender, RoutedEventArgs a)
        {
            HamburgerPane.IsPaneOpen = false;

            //PredefFilter f1 = new PredefFilter();
            //f1.text1.PropertyName = "testname";
            //f1.text1.Operator = TextOperator.EqualsTo;
            //f1.text1.Value = "disk";
            //f1.text2.PropertyName = "hostname";
            //f1.text2.Operator = TextOperator.Contains;
            //f1.text2.Value = "bitsdata";
            //f1.filter1.Operator = LogicalOperator.And;
            //f1.filter1.Descriptors.Add(f1.text1);
            //f1.filter1.Descriptors.Add(f1.text2);
            //DataGrid.FilterDescriptors.Add(f1.filter1);

            PredefFilter f2 = new PredefFilter();
            f2.text1.PropertyName = "acktime";
            f2.text1.Operator = TextOperator.EqualsTo;
            f2.text1.Value = "0";
            f2.filter1.Operator = LogicalOperator.And;
            f2.filter1.Descriptors.Add(f2.text1);
            DataGrid.FilterDescriptors.Add(f2.filter1);
        }
        private void preDef1_Unchecked(object sender, RoutedEventArgs a)
        {
            HamburgerPane.IsPaneOpen = false;
            
        }

        private void myAcks_Checked(object sender, RoutedEventArgs a)
        {
            HamburgerPane.IsPaneOpen = false;

            PredefFilter myAcks = new PredefFilter();
            myAcks.text1.PropertyName = "ackuser";
            myAcks.text1.Operator = TextOperator.EqualsTo;
            myAcks.text1.Value = Settings.userSign;
            myAcks.filter1.Operator = LogicalOperator.And;
            myAcks.filter1.Descriptors.Add(myAcks.text1);
            DataGrid.FilterDescriptors.Add(myAcks.filter1);
        }
        private void myAcks_Unchecked(object sender, RoutedEventArgs a)
        {
            HamburgerPane.IsPaneOpen = false;

            

            //DataGrid.FilterDescriptors.Add(myAcks.filter1);
            Debug.WriteLine(DataGrid.FilterDescriptors.ToString());
            Debug.WriteLine(DataGrid.FilterDescriptors.Count());
            foreach (FilterDescriptorBase fd in DataGrid.FilterDescriptors)
            {
                Debug.WriteLine(fd.GetType());
                if (fd.GetType().ToString() == "CompositeFilterDescriptor")
                {
                    CompositeFilterDescriptor c = (CompositeFilterDescriptor)fd;
                    Debug.WriteLine("CFD descriptor:" + c.Descriptors.ToString());
                }
                if (fd.GetType().ToString() == "TextFilterDescriptor")
                {
                    TextFilterDescriptor t = (TextFilterDescriptor)fd;
                    Debug.WriteLine(t.ToString());
                }
            }
        }


        private void CacheBtn_Tapped(object sender, RoutedEventArgs e)
        {
            HamburgerPane.IsPaneOpen = false;
            GetCache(Settings.cacheHours, "all", false); 
        }

        private void CacheBtn_RightTapped(object sender, RoutedEventArgs e)
        {
            HamburgerPane.IsPaneOpen = false;
            GetCache(-1, "all", false); // Get from beginning of time
        }


        //private void GetCache(int hours)
        //{
        //    Status.processingFullMsg = false;
        //    Status.processing = true;
        //    Status.processingType = "cache";

        //    long refreshScope = 0; // Get from beginning of time

        //    if (hours == -1)
        //        refreshScope = 0;
        //    else
        //        refreshScope = DateTimeOffset.Now.ToUnixTimeSeconds() - (hours * 3600);
                        
        //    string fields = Settings.fields.Replace(",msg", "");
        //    string xymonCmd = "xymondboard " + xymonConnect.nonTestsPatternBuilder() + "lastchange>" + refreshScope.ToString() + fields;
        //    Debug.WriteLine(xymonCmd + Environment.NewLine + Environment.NewLine);
        //    xymonGetAsync(xymonCmd);

        //}

        private void GetCache(int hours, string colors, Boolean processFullMsg)
        {
            long refreshScope;
            string colorScope = " color=";
            string fields;
            string xymonCmd;

            if (!processFullMsg)
            {
                Status.processingFullMsg = false;
                fields = Settings.fields.Replace(",msg", "");
            }
            else
            {
                Status.processingFullMsg = true;
                fields = Settings.fields;
            }

            if (colors == "all")
            {
                colorScope = String.Empty;
            }
            else if (colors == "noGreen")
            {
                colorScope += "red,yellow,purple,clear,blue";
            }
            else colorScope += colors;

            Status.processing = true;
            Status.processingType = "cache";

            if (hours == -1)
                refreshScope = 0; // Get from beginning of time
            else
                refreshScope = DateTimeOffset.Now.ToUnixTimeSeconds() - (hours * 3600);

            xymonCmd = "xymondboard " + xymonConnect.nonTestsPatternBuilder() + "lastchange>" + refreshScope.ToString() + colorScope + fields;
            Debug.WriteLine(xymonCmd + Environment.NewLine + Environment.NewLine);
            xymonGetAsync(xymonCmd);

        }

        /*****************************/
        /*  Deprecated stuff         */
        /*****************************/
        // Settings flyout as alternative to dialog
        //private void Settings_Tapped(object sender, TappedRoutedEventArgs e)
        //{
        //    FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        //}
    }
}
