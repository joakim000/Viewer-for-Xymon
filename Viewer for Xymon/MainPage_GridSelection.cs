using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Telerik.UI.Xaml.Controls.Grid;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Viewer_for_Xymon
{
    public sealed partial class MainPage : Page
    {
        private void enableBtns(Fount f)
        {
            if (String.Equals(f.color, "red") || String.Equals(f.color, "yellow") || String.Equals(f.color, "purple")) AckBtn.IsEnabled = true;
            else AckBtn.IsEnabled = false;
            DisableBtn.IsEnabled = true;
            //CropBtn.IsEnabled = true;
            //backBtn.IsEnabled = true;

            if (Status.gotWebConn)
            {
                if (String.IsNullOrEmpty(Settings.StatusURL)) SideStatus.IsEnabled = false;
                else SideStatus.IsEnabled = true;
                if (String.IsNullOrEmpty(Settings.HistURL)) SideHistory.IsEnabled = false;
                else SideHistory.IsEnabled = true;
                if (String.IsNullOrEmpty(Settings.ColumnDocURL)) SideTest.IsEnabled = false;
                else SideTest.IsEnabled = true;
                if (String.IsNullOrEmpty(Settings.TrendsURL)) SideTrends.IsEnabled = false;
                else SideTrends.IsEnabled = true;
            }
            else
            {
                SideStatus.IsEnabled = false;
                SideHistory.IsEnabled = false;
                SideTest.IsEnabled = false;
                SideTrends.IsEnabled = false;
            }

            if (String.IsNullOrEmpty(Settings.docsURL)) docsBtn.IsEnabled = false;
            else docsBtn.IsEnabled = true;

            caseBtn.IsEnabled = false;
            //TODO if (Status.showCaseEnabled)
            if (Settings.casePattern != null && Settings.casePattern != "" && Settings.showCaseURL != null && Settings.showCaseURL != "")
            {
                if (f.ackmsg != null && f.ackmsg != "")
                {
                    Match m = Regex.Match(f.ackmsg, Settings.casePattern);
                    if (m.Success) caseBtn.IsEnabled = true;
                }
                else if (f.dismsg != null && f.dismsg != "")
                {
                    Match m = Regex.Match(f.dismsg, Settings.casePattern);
                    if (m.Success) caseBtn.IsEnabled = true;
                }
            }
            if (f.client == "Y") logsBtn.IsEnabled = true;
            else logsBtn.IsEnabled = false;
        }

        private void disableBtns()
        {
            AckBtn.IsEnabled = false;
            DisableBtn.IsEnabled = false;
            //CropBtn.IsEnabled = false;

            //backBtn.IsEnabled = false;
            SideStatus.IsEnabled = false;
            SideHistory.IsEnabled = false;
            docsBtn.IsEnabled = false;
            SideTest.IsEnabled = false;
            SideTrends.IsEnabled = false;
            caseBtn.IsEnabled = false;
            logsBtn.IsEnabled = false;
        }

        private void DataGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs args)
        {
            Debug.WriteLine("DataGrid.SelectionChanged");
            if (DataGrid.SelectedItems.Count < 1) 
            {
                disableBtns();
                try
                {
                    webView1.Navigate(new Uri(Settings.emptyPaneFill));
                }
                catch (Exception e)
                {
                    Status.log("Failed navigating to emptyPaneFill: " + e);
                }
            }
            if (DataGrid.SelectedItems.Count == 1)
            {
                

                var f = DataGrid.SelectedItem as Fount;
                enableBtns(f);

                if (Status.lockedPane)
                {
                    switch (Status.openPane)
                    {
                        case "status":
                            OpenStatus(0);
                            break;
                        case "history":
                            OpenHistory(Settings.histLimit);
                            break;
                        case "docs":
                            OpenDocs();
                            break;
                        case "case":
                            //OpenCase();
                            OpenStatus(0);
                            break;
                        case "test":
                            OpenTest();
                            break;
                        case "trends":
                            OpenTrends();
                            break;
                        case "log":
                            OpenLog();
                            break;
                        default:
                            break;
                    }
                }
            }
            if (DataGrid.SelectedItems.Count > 1)
            {
                if (Status.lockedPane) SelectPane();
            }

            //// Selection list i sidedrawer
            //if (toggleDrawerSide.IsChecked == true && ToggleMultiBtn.IsChecked == true)
            //{
            //    SelectPane();

            //}
        }
    }
}
