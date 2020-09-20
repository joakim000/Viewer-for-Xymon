using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Viewer_for_Xymon
{
    public sealed partial class MainPage : Page
    {

        public async void OpenDisableDialog(object sender, RoutedEventArgs e)
        {
            //PrintSelected();
            var selected = DataGrid.SelectedItems;
            var dd = new DisableDialog(selected);
            var result = await dd.ShowAsync();
            //DataGrid.SelectItem(DataGrid.SelectedItem); //Make sure sidebuttons are updated
            if (result == ContentDialogResult.Primary)
            {
                if (Status.createCaseStatus > 0)
                {
                    caseBtn.IsEnabled = true;
                }
            }
        }
        public async void OpenDisableDialog(object sender, AccessKeyInvokedEventArgs e)
        {
            //PrintSelected();
            var selected = DataGrid.SelectedItems;
            var dd = new DisableDialog(selected);
            var result = await dd.ShowAsync();
            //DataGrid.SelectItem(DataGrid.SelectedItem); //Make sure sidebuttons are updated
            if (result == ContentDialogResult.Primary)
            {
                if (Status.createCaseStatus > 0)
                {
                    caseBtn.IsEnabled = true;
                }
            }
        }

        public async void OpenAckDialog(object sender, RoutedEventArgs e)
        {
            //PrintSelected();
            var selected = DataGrid.SelectedItems;
            var ad = new AckDialog(selected);
            var result = await ad.ShowAsync();
            //DataGrid.SelectItem(DataGrid.SelectedItem); //Make sure sidebuttons are updated
            if (result == ContentDialogResult.Primary)
            {
                if (Status.createCaseStatus > 0)
                {
                    caseBtn.IsEnabled = true;
                }
            }
        }
        public async void OpenAckDialog(object sender, AccessKeyInvokedEventArgs e)
        {
            //PrintSelected();
            var selected = DataGrid.SelectedItems;
            var ad = new AckDialog(selected);
            var result = await ad.ShowAsync();
            //var result = await new AckDialog(DataGrid.SelectedItems).ShowAsync();
            //DataGrid.SelectItem(DataGrid.SelectedItem); //Make sure sidebuttons are updated
            if (result == ContentDialogResult.Primary)
            {
                if (Status.createCaseStatus > 0)
                {
                    caseBtn.IsEnabled = true;
                }
            }
        }

        public async Task<ContentDialogResult> OpenSettingsDialog(object sender, RoutedEventArgs e)
        {
            HamburgerPane.IsPaneOpen = false;
            var sd = new SettingsDialog();
            if (this.ActualWidth < 900) sd.MaxWidth = this.ActualWidth;
            else sd.MaxWidth = 900;
            if (this.ActualHeight < 800) sd.MaxHeight = this.ActualHeight;
            else sd.MaxHeight = 800;
            //sd.DataContext = sm;
            ContentDialogResult result = await sd.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                DataGrid.FontSize = Settings.textSize;
                if (Settings.CommandLabels)
                    ToggleToolbarLabelsOn(null, null);
                else
                    ToggleToolbarLabelsOff(null, null);
                DataGrid.FontSize = Settings.textSize;

                if (Settings.showManualBlue)
                    toggleManualBlue.Visibility = Windows.UI.Xaml.Visibility.Visible;
                else
                    toggleManualBlue.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                if (Settings.CommandTextBox)
                    cmdTextBox.Visibility = Windows.UI.Xaml.Visibility.Visible;
                else
                    cmdTextBox.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                if (Settings.showConsole)
                    ConsoleGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
                else
                    ConsoleGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

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

                    //// Custom filters, feature in progress. Show/hide with debug for now
                    //Heading_filter.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    //myAcksBtn.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    //preDef1.Visibility = Windows.UI.Xaml.Visibility.Visible;

                }
                else
                {
                    Heading_debug.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    RefreshSelectedBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    CropBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    CacheBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    togglePause.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    ClearBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                    // Custom filters, feature in progress. Show/hide with debug for now
                    Heading_filter.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    myAcksBtn.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    preDef1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }



                // In case OnGreenDelay has changed
                if (Settings.onGreenDelay <= 0)
                    cf.delayTime.Value = DateTimeOffset.Now.ToUnixTimeSeconds() + 1000;
                else
                    cf.delayTime.Value = DateTimeOffset.Now.ToUnixTimeSeconds() - (Settings.onGreenDelay * 60);

                RefreshColorFilters(); 

            }
            
            // Checks when dialog closed
            if (Settings.showCaseURL == "" || Settings.casePattern == "")
            {
                caseBtn.IsEnabled = false;
            }
            if (Settings.docsURL == "" || Settings.docsURL == null)
            {
                docsBtn.IsEnabled = false;
            }
            if (Status.changedXymondAddr)
            {
                Status.changedXymondAddr = false;

                Status.gotXymonName = true;
                Status.gotXymonConn = false;
                Status.gotXymonCfg = false;
                Status.gotWebConn = false;

                Status.refreshActive = false;
                setup();

            }

            return result;
        }

        public async Task<ContentDialogResult> OpenErrorDialog(object sender, RoutedEventArgs e, string error)
        {
            Status.refreshActive = false;
            string myError = String.Empty;
            var ed = new ErrorDialog(error);

            var result = await ed.ShowAsync();
            ed.Hide();
            if (result == ContentDialogResult.Primary)
            {
                //startUpdates();
                //setup();
            }
            if (result == ContentDialogResult.Secondary)
            {
               //await OpenSettingsDialog(null, null);
            }

            
            return result;

        }
    }
}
