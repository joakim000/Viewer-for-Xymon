using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Telerik.UI.Xaml.Controls.Grid.View;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.Web.Http;


namespace Viewer_for_Xymon
{
    public sealed partial class MainPage : Page
    {
        PropertySortDescriptor psd = new PropertySortDescriptor();
        ColorFilters cf = new ColorFilters();
        CompositeFilterDescriptor colorFilters = new CompositeFilterDescriptor();
        CompositeFilterDescriptor dummyFilter = new CompositeFilterDescriptor();

        // Properties
        public WebView Web1 { get => webView1; set => webView1 = value; }
        public RadDataGrid Grid { get => DataGrid; set => DataGrid = value; }
        //public TextBlock CollSizeTextBlockProp { get => CollSizeTextBlock;  }

        public MainPage()
        {
            DataContext = Model.fc;
            this.InitializeComponent();
            
            //DataGrid.DataContext = Model.fc;

            

            ConsoleListView.DataContext = Status.consoleLog;

            // Event handlers
            webView1.NavigationCompleted += webView_NavigationCompleted;
            webView1.NavigationStarting += WebView_NavigationStarting;
            DataGrid.SelectionChanged += DataGrid_SelectionChanged;
            DataGrid.DataContextChanged += DataGrid_DataContextChanged;
            this.SizeChanged += MainPage_SizeChanged;
            DataGrid.Tapped += DataGrid_Tapped;
            //DataGrid.KeyUp += DataGrid_KeyUp;
            

            // General setup
            disableBtns();
            InitColorFilters();
            DataGrid.UseSystemFocusVisuals = true;
            DataGrid.SelectionMode = DataGridSelectionMode.Extended;
            //DataGrid.IsSynchronizedWithCurrentItem = false;

            setup();
        }


        public async void setup()
        {
            getSavedSettings();  // Get local settings

            while (!Status.gotXymonName)
            {
                await Task.Delay(400);
                Task<ContentDialogResult> settingsTask = OpenSettingsDialog(null, null);
                await settingsTask;
                if (!(Settings.XymondAddr == null || Settings.XymondAddr == ""))
                {
                    Status.gotXymonName = true;
                }
            }
            Debug.WriteLine("Got xymon name: " + Settings.XymondAddr);
            Status.log("Got xymon name: " + Settings.XymondAddr);

            while (!Status.gotXymonConn)
            {
                Task<string> pingTask = xymonConnect.connect("ping");
                await pingTask;
                if (pingTask.Result.IndexOf("xymond") != -1)
                {
                    Status.gotXymonConn = true;
                    Debug.WriteLine("Got xymon connection: " + pingTask.Result);
                    Status.log("Got xymon connection: " + pingTask.Result);
                }
                else
                {
                    //Task<ContentDialogResult> errorTask = OpenErrorDialog(null, null, pingTask.Result);

                    //try
                    //{
                    //    await errorTask;
                    //}
                    //catch (Exception e)
                    //{
                    //    Debug.WriteLine("Opening ErrorDialog" + e);
                    //}

                    //if (errorTask.Result == ContentDialogResult.Primary)
                    //{
                    //    setup();
                    //}
                    //else if (errorTask.Result == ContentDialogResult.Secondary)
                    //{
                    //    await OpenSettingsDialog(null, null);
                    //}

                    Status.refreshActive = false;
                    try
                    {
                        string myError = String.Empty;
                        var ed = new ErrorDialog(pingTask.Result);
                        //Task<ContentDialogResult> errorTask = ed.ShowAsync();
                        var result = await ed.ShowAsync();
                        ed.Hide();
                        if (result == ContentDialogResult.Primary)
                        {
                            //startUpdates();
                            setup();
                        }
                        if (result == ContentDialogResult.Secondary)
                        {
                            await OpenSettingsDialog(null, null);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Opening ErrorDialog" + e);
                    }
                    await Task.Delay(Settings.refreshDelay);
                }
            }

            while (!Status.gotXymonCfg)
            {
                Task<string> getConfigTask = xymonConnect.xymonGetConfig();
                await getConfigTask;
                Debug.WriteLine("Get xymon config - " + getConfigTask.Result);
                Status.log("Get xymon config - " + getConfigTask.Result);
                if (getConfigTask.Result == "success")
                    Status.gotXymonCfg = true;
                else
                    await Task.Delay(Settings.refreshDelay);
            }

            // Webtest 
            //Task<string> webTestTask = new XWeb().testWeb();
            //await webTestTask;
            //if (webTestTask.Result != "failed")
            //{
            //    Status.gotWebConn = true;
            //    Debug.WriteLine("Web test ok with scheme: " + webTestTask.Result);
            //    Status.log("Web test ok with scheme: " + webTestTask.Result);
            //}
            //else
            //{
            //    Debug.WriteLine("Web test failed with both schemes. ");
            //    Status.log("Web test failed with both schemes. ");
            //    //await Task.Delay(Settings.refreshDelay);
            //    ViewModel.Alert(null, null, "Failed to connect to Xymon web services:" + Environment.NewLine + "Status, History, Test and Trends will be disabled.");
            //}
            Task<string> webTestTask = new XWeb().testWeb();
            await webTestTask;
            if (webTestTask.Result != "failed")
            {
                Status.gotWebConn = true;
                Debug.WriteLine("Web test ok with scheme: " + webTestTask.Result);
                Status.log("Web test ok with scheme: " + webTestTask.Result);
            }
            else
            {
                Settings.XymonWWWName = Settings.XymondAddr;
                Task<string> webTestTask2 = new XWeb().testWeb();
                await webTestTask2;
                if (webTestTask2.Result != "failed")
                {
                    Status.gotWebConn = true;
                    Debug.WriteLine("Using xymond-name instead of www name from conf: Web test ok with scheme: " + webTestTask2.Result);
                    Status.log("Using xymond-name instead of www name from conf: Web test ok with scheme: " + webTestTask2.Result);
                }
                else
                {
                    Debug.WriteLine("Web test failed with both schemes. ");
                    Status.log("Web test failed with both schemes. ");
                    ViewModel.Alert(null, null, "Failed to connect to Xymon web services:" + Environment.NewLine + "Status, History, Test, Trends and Log will be disabled.");
                }
            }


            if (true)  // Ready to start - more tests here?
            {
                //xymonProcess.greenDelayLoop();
                greenDelayLoop();
                updateLoop();
                Status.refreshActive = true;
                startUpdates();
                if (Settings.cacheOnStart) cacheOnStart();
            }
        }

        public void startUpdates()
        {
            //DateTime currentTime = DateTime.Now;
            //long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
            //long refreshScope = unixTime - 2592000; // Get statuschanges from last month
            //long refreshScope = unixTime - 7776000; // Get statuschanges from last 3 months
            //long refreshScope = unixTime - 15724800; // Get statuschanges from last 6 months
            //long refreshScope = unixTime - 31536000; // Get statuschanges from last year
            //long refreshScope = 0; // Get from beginning of time

            // Setup start view
            string colorScope = " color=";
            //if (Settings.startView[0]) toggleAck.IsChecked = true;
            if (Settings.startView[1]) { toggleRed.IsChecked = true; colorScope = colorScope + "red,"; } else toggleRed.IsChecked = false;
            if (Settings.startView[2]) { toggleYellow.IsChecked = true; colorScope = colorScope + "yellow,"; } else toggleYellow.IsChecked = false;
            if (Settings.startView[3]) { togglePurple.IsChecked = true; colorScope = colorScope + "purple,"; } else togglePurple.IsChecked = false;
            if (Settings.startView[4]) { toggleClear.IsChecked = true; colorScope = colorScope + "clear,"; } else toggleClear.IsChecked = false;
            if (Settings.startView[5]) { toggleBlue.IsChecked = true; colorScope = colorScope + "blue,"; } else toggleBlue.IsChecked = false;
            if (Settings.startView[6]) { toggleGreen.IsChecked = true; colorScope = colorScope + "green,"; } else toggleGreen.IsChecked = false;
            colorScope = colorScope.TrimEnd(',');

            psd.PropertyName = "lastchange";
            psd.SortOrder = SortOrder.Descending;
            DataGrid.SortDescriptors.Clear();
            DataGrid.SortDescriptors.Add(psd);

            Status.firstUpdate = true;
            xymonRefresh(colorScope);
           
        }



        public void DisableWebButtons()
        {
            List<AppBarButton> toolBarEnum = new List<AppBarButton>
            {
                SideStatus, SideHistory, SideTest, SideTrends, docsBtn, caseBtn
            };
            foreach (AppBarButton btn in toolBarEnum)
            {
                btn.IsCompact = true;
            }
        }

        private async void DefaultLaunch(Uri uri)
        {
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            if (success)
            {
                // URI launched
            }
            else
            {
                // URI launch failed
            }
        }

        private void WebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (sender.Source != null)
            {
                try
                {
                    Match m = Regex.Match(args.Uri.ToString(), Settings.printFriendlyPattern);
                    if (m.Success)
                    {
                        if (args.Uri.ToString().IndexOf(Settings.printFriendlyReq) == -1) // Avoid endless loop
                        {
                            Uri navPage = new Uri(args.Uri + Settings.printFriendlyReq);
                            HttpRequestMessage request = new XWeb().RequestMessage("GET", navPage, Settings.webUser, Settings.webPw);
                            webView1.NavigateWithHttpRequestMessage(request);
                        }
                    }
                }
                catch (NullReferenceException) { }
            }
        }

        private void webView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {

            if (args.IsSuccess == true)
            {
                if (sender.Source == null)
                {
                    Debug.WriteLine("Webview loaded local source successfully.");
                }
                else
                {
                    Debug.WriteLine("Navigation to " + args.Uri.ToString() + " completed successfully.");
                }
            }
            else
            {
                if (sender.Source == null)
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Webview failed loading from local source.");
                    Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Webview failed loading from local source.");
                }
                else
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Navigation to: " + args.Uri.ToString() + " failed with error " + args.WebErrorStatus.ToString());
                    Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Navigation to: " + args.Uri.ToString() + " failed with error " + args.WebErrorStatus.ToString());
                }
            }
        }

        private void DataGrid_DataContextChanged(object sender, DataContextChangedEventArgs args)
        {
            //Debug.WriteLine("Datacontext changed - NewValue: " + args.NewValue.ToString() + " Handled: " + args.Handled.ToString());
            Debug.WriteLine("Datacontext changed");
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs a)
        {
            //DataGrid.Arrange(new Rect());
            DataGrid.InvalidateArrange();
            DataGrid.UpdateLayout();
            RefreshColorFilters();
            Debug.WriteLine("MainPage_SizeChanged. DataGrid.Width:" + DataGrid.Width + " DataGrid.ActualWidth:" + DataGrid.ActualWidth + " DataGridWrapper.Width:" + DataGridWrapper.Width + " DataGridWrapper.ActualWidth:" + DataGridWrapper.ActualWidth);
            Debug.WriteLine("decriptionCol.ActualWidth:" + descriptionCol.ActualWidth);
        }

        public void PrintSelected()         // Debug helper
        {
            foreach (Fount f in DataGrid.SelectedItems)
            {
                Debug.WriteLine(f.hostname + f.testname);
            }
            Debug.WriteLine("Total selected: " + DataGrid.SelectedItems.Count());
        }

        // Deprecated
        private void cmdTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                PopulateBtn(null, null);
            if (e.Key == VirtualKey.Escape)
            {
                cmdTextBox.Text = "";
                //DataGrid.Focus(FocusState.Programmatic);
                DataGrid.Focus(FocusState.Pointer);

            }
        }

        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            // Do stuff
        }

        private void DataGrid_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Down)
            {
                Debug.WriteLine("Datagrid arrow down");
                //DataGrid.SelectedItem = DataGrid.;
                DataGrid.GetDataView().MoveCurrentToNext();
                DataGrid.SelectedItem = DataGrid.CurrentItem;

                Fount f = (Fount)DataGrid.CurrentItem;
                Debug.WriteLine(f.hostname + " : " + f.testname);
                
            }
            if (e.Key == VirtualKey.Up)
            {
                Debug.WriteLine("Datagrid arrow up");
                //DataGrid.SelectedItem = DataGrid.SelectItem;

                Fount f = (Fount)DataGrid.CurrentItem; Debug.WriteLine("Current: " + f.hostname + " : " + f.testname);
                Fount f2 = (Fount)DataGrid.SelectedItem; Debug.WriteLine("Selected: " + f2.hostname + " : " + f2.testname);

                //DataGrid.GetDataView().MoveCurrentToPrevious();
                //DataGrid.SelectedItem = DataGrid.CurrentItem;

                foreach (var item in Grid.SelectedItems)
                {
                    //Grid.GetDataView().Items[item. ItemIndex + 1]      MasterTableView.Items[item.ItemIndex + 1].Selected = true;

                    //Grid.GetDataView().Items.
                    //Grid.GetDataView().

                    Debug.WriteLine(Grid.GetDataView().Items[0].GetType());
                }



                Fount f3 = (Fount)DataGrid.CurrentItem; Debug.WriteLine("Current: " + f3.hostname + " : " + f3.testname);
            }
        }

        private void DataGrid_RightTapped(object sender, RoutedEventArgs e)
        {
            
                Debug.WriteLine("Datagrid right tapped");
            
        }


        private void DataGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Debug.WriteLine(sender);
            //Debug.WriteLine(e);
            //Debug.WriteLine(e.Handled);
            //Debug.WriteLine(e.OriginalSource);
            //Debug.WriteLine(e.PointerDeviceType);

            Debug.WriteLine("Grid tapped - OriginalSource: " + e.OriginalSource);
            //if (e.OriginalSource.GetType == Windows.UI.Xaml.Controls.Grid)
            if (e.OriginalSource.ToString() == "Windows.UI.Xaml.Controls.Grid")
            {
                Grid.DeselectAll();
            }


        }

        private void SideStatusDbl(object sender, DoubleTappedRoutedEventArgs e)
        {

        }

        //private void ResetFiltersBtn_Tapped(object sender, TappedRoutedEventArgs e)
        //{

        //}


        ////////////////////////////////////////////////////
    }   // Här slutar MainPage - det nedanför ska flyttas ut
        ////////////////////////////////////////////////////

    public class CustomCellTapped : DataGridCommand
    {
        public CustomCellTapped()
        {
            this.Id = CommandId.CellTap;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var context = parameter as DataGridCellInfo;

            Window.Current.CoreWindow.KeyDown += (s, e) =>
            {
                var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
                if (ctrl.HasFlag(CoreVirtualKeyStates.Down))
                {
                    Debug.WriteLine("Grid ctrl-tapped");
                }
                else
                {
                    Debug.WriteLine("Grid tapped");
                    //MainPage.Grid.SelectItem(context.Item as Fount);
                }
            };

            Debug.WriteLine(context.Value);
            Debug.WriteLine(context.Item);


        }
    }

    public class CustomCellRightTapped : DataGridCommand
    {
        public CustomCellRightTapped()
        {
            this.Id = CommandId.CellTap;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var context = parameter as DataGridCellInfo;
            Debug.WriteLine("Grid right-tapped (CustomCellRightTapped)");
            Debug.WriteLine(context.Value);
            Debug.WriteLine(context.Item);
        }
    }



    public class CustomCellDoubleTapped : DataGridCommand
    {
        public CustomCellDoubleTapped()
        {
            this.Id = CommandId.CellDoubleTap;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var context = parameter as DataGridCellInfo;
            var dp = new DataPackage();
            string str = String.Empty;
            str = context.Value.ToString();
            dp.SetText(str);
            Clipboard.SetContent(dp);
            Debug.WriteLine("Copied from column: " + context.Column.Name + " Value: " + str);

            // Reloading doubleclicked row
            
            Fount f = context.Item as Fount;
            Debug.WriteLine(f.hostname + " : " + f.testname);
            //Status.processing = true;
            //Status.processingType = "manual";
            //DataGrid.Focus(FocusState.Pointer);
            //MainPage.xymonGetAsync("xymondboard " + xymonConnect.nonTestsPatternBuilder() + " " + Settings.fields);



        }
    }

    public class CustomCellKeyDown : DataGridCommand
    {
        public CustomCellKeyDown()
        {
            this.Id = CommandId.KeyDown;
            
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var context = parameter as DataGridCellInfo;
            var keyEvent = parameter as KeyRoutedEventArgs;

            if (keyEvent.Key == VirtualKey.Down)
            {
                Debug.WriteLine("Datagrid arrow down (custom)");
                
                //DataGrid.SelectedItem = DataGrid.SelectItem;
            }
            if (keyEvent.Key == VirtualKey.Up)
            {
                Debug.WriteLine("Datagrid arrow up  (custom)");
                //DataGrid.SelectedItem = DataGrid.SelectItem;
            }


            Debug.WriteLine("Grid keydown");
            Debug.WriteLine(context);
            //Debug.WriteLine(context.Value);
            //Debug.WriteLine(context.Item);
        }
    }


    public class CustomCellHoldingCommand : DataGridCommand
    {
        public CustomCellHoldingCommand()
        {
            this.Id = CommandId.CellHolding;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var context = parameter as CellHoldingContext;
            //var context = parameter as DataGridCellInfo;
            //Debug.WriteLine("Copied from column: " + context.Column.Name + " Value: " + str);

            Debug.WriteLine("Grid holding");
        }
    }
}

