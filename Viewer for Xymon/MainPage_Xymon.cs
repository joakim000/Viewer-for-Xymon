using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Viewer_for_Xymon
{
    public sealed partial class MainPage : Page
    {

        private async void cacheOnStart()
        {
            while (true)
            {
                if (Status.processing)
                {
                    await Task.Delay(100);
                }
                else
                {
                    Status.processing = true;
                    GetCache(-1, "noGreen", false);
                    break;
                }
            }

            while (true)
            {
                if (Status.processing)
                {
                    await Task.Delay(100);
                }
                else
                {
                    Status.processing = true;
                    GetCache(Settings.cacheHours, "green", false);
                    break;
                }
            }
        }


        private async void xymonRefresh(string refreshColorScope)
        {
			//Status.refreshActive = true;
            while (Status.refreshActive)
            {
                if (!Status.processing)
                {
                    
                    Status.processing = true;
                    string xymonCmd;
                    DateTime currentTime = DateTime.Now;
                    long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
                    long refreshScope = unixTime - Settings.refreshSpan;

                    if (Status.processingType == "error")// Previous run needs to be retried 
                    {
                        Status.processingType = "retry";
                        xymonCmd = Status.failedCmd;
                        Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " Retry " + Status.errorCount + ", command: " + xymonCmd);
                        //int attempt = Status.errorCount + 1;
                        Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " Retry " + Status.errorCount + ", command: " + xymonCmd);
                        xymonGetAsync(xymonCmd);
                    }

                    else if (Status.firstUpdate)
                    {
                        Status.processingType = "firstupdate";
                        Status.processingFullMsg = true;
                        refreshScope = 0; // Get from beginning of time
                        xymonCmd = "xymondboard " + xymonConnect.nonTestsPatternBuilder() + "lastchange>" + refreshScope.ToString() + refreshColorScope + Settings.fields;
                        xymonGetAsync(xymonCmd);
                    }


                    else if (Status.processingType == "lastchange") // Previous run was lastchange
                    {
                        // Get all acked from any time
                        Status.processingFullMsg = true;
                        Status.processingType = "ack";
                        xymonCmd = "xymondboard " + xymonConnect.nonTestsPatternBuilder() + " acktime>0 " + Settings.fields;
                        xymonGetAsync(xymonCmd);
                    }
                    else
                    {
                        // Get all with lastchange within refreshScope
                        Status.processingFullMsg = true;
                        Status.processingType = "lastchange";
                        xymonCmd = "xymondboard " + xymonConnect.nonTestsPatternBuilder() + " lastchange>" + refreshScope.ToString() + Settings.fields;
                        xymonGetAsync(xymonCmd);

                        await Task.Delay(Settings.refreshDelay);
                    }
                }
                else
                {
                    await Task.Delay(100);
                }
            }
        }

        public async void xymonGetAsync(string xymonCmd)
        {
            if (Status.processingCheckFirst)
            {
                Task<int> checkTask = xymonLineCheck(xymonCmd);
                await checkTask;
                Debug.WriteLine("Linecheck returned " + checkTask.Result.ToString() + " for command: " + xymonCmd);
                Status.processingCheckFirst = false;
            }

        Task<string> t = xymonConnect.connect(xymonCmd);
            await t;
            if (t.Result.IndexOf("Error ") == 0)
            {
                Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + t.Result);
                var errorLines = new StringReader(t.Result);
                errorLines.ReadLine(); 
                var errorText = errorLines.ReadLine();
                var eIndex = errorText.IndexOf("Exception: ");
                if (eIndex != -1)
                {
                    errorText = errorText.Substring(eIndex + 11);
                }
                CollSizeTextBlock.Text = "Xymon error: " + errorText;
                UpdateTextBlock.Text = "Last update " + Status.successfulUpdateTime + " Size " + Model.fc.Count.ToString();
                CollSizeTextBlock.Foreground = Settings.strongRed_brush;
                Status.processing = false;
            }
            else
            {
                List<List<string>> ListOfRows = xymonProcess.processXymonList(t.Result);
                xymonProcess.updateAndAdd(ListOfRows);
                //Task u = WindowsRuntimeSystemExtensions.AsTask(updateAndAdd(ListOfRows), default(CancellationToken);
                //ThreadPool.RunAsync(updateAndAdd(ListOfRows));
                //IAsyncAction aa = Windows.System.Threading.ThreadPool.RunAsync( (workItem) => updateAndAdd(ListOfRows));
                //IAsyncAction a = Windows.System.Threading.ThreadPool.RunAsync( (workItem) => updateAndAdd(ListOfRows));

                Status.successfulUpdateTime = DateTime.Now.ToString("HH:mm:ss");
                Status.successfulUpdateTimeEpoch = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

                if (Status.errorCount > 0)
                {
                    CollSizeTextBlock.Foreground = Settings.strongRed_brush;
                    CollSizeTextBlock.Text = Status.rowsError.ToString() + " errors. Retry " + Status.errorCount.ToString() + ". Size " + Model.fc.Count.ToString();
                }
                else
                {
                    CollSizeTextBlock.Foreground = Settings.black_brush;
                    CollSizeTextBlock.Text = DataGrid.GetDataView().Items.Count.ToString() + "/" + Model.fc.Count.ToString();
                }

                if (Settings.statusRowEpoch)
                {
                    UpdateTextBlock.Text = Status.successfulUpdateTimeEpoch;
                }
                else
                {
                    UpdateTextBlock.Text = Status.successfulUpdateTime;
                }

                if (Status.firstUpdate)
                {
                    RefreshColorFilters();
                    Status.firstUpdate = false;
                } 
            }
        }

        public async Task<int> xymonLineCheck(string xymonCmd)
        {
            string lineCheckCmd = String.Empty;
            int index = xymonCmd.IndexOf("fields=");
            if (index > -1)
            {
                lineCheckCmd = xymonCmd.Remove(index);
                lineCheckCmd = lineCheckCmd + "fields=stats";
            }
            
            Task<string> t = xymonConnect.connect(lineCheckCmd);
            await t;
            if (t.Result.IndexOf("Error ") == 0)
            {
                Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + t.Result);
                var errorLines = new StringReader(t.Result);
                errorLines.ReadLine();
                var errorText = errorLines.ReadLine();
                var eIndex = errorText.IndexOf("Exception: ");
                if (eIndex != -1)
                {
                    errorText = errorText.Substring(eIndex + 11);
                }
                CollSizeTextBlock.Text = "Xymon error: " + errorText;
                UpdateTextBlock.Text = "Last update " + Status.successfulUpdateTime + " Size " + Model.fc.Count.ToString();
                CollSizeTextBlock.Foreground = Settings.strongRed_brush;
                Status.processing = false;
                return 0;
            }
            else
            {
                List<string> xymonList = t.Result.ToString().Split('\n').ToList();
                int lastindex = xymonList.Count() - 1;
                Debug.WriteLine("Linecheck returned " + lastindex.ToString() + " for command: " + xymonCmd);
                return lastindex;

            }
        }


        //public string nonTestsPatternBuilder()
        //{
        //    string nonTestsPattern = " test=^(?!((";
        //    foreach (string nonTest in Settings.ignoreCols)
        //    {
        //        nonTestsPattern += nonTest;
        //        nonTestsPattern += "|";
        //    }
        //    //nonTestsPattern = nonTestsPattern.Remove(nonTestsPattern.Length - 1); // Remove the final |
        //    nonTestsPattern = nonTestsPattern.Trim('|');
        //    nonTestsPattern += ")$)).*$ ";
        //    return nonTestsPattern;
        //}


        public async void greenDelayLoop()
        {
            while (true)
            {

                //long delayTime = DateTimeOffset.Now.ToUnixTimeSeconds() - (Settings.onGreenDelay * 60); 

                if (Settings.onGreenDelay <= 0)
                    cf.delayTime.Value = DateTimeOffset.Now.ToUnixTimeSeconds() + 1000;
                else
                    cf.delayTime.Value = DateTimeOffset.Now.ToUnixTimeSeconds() - (Settings.onGreenDelay * 60);

                //Debug.WriteLine("Delaytime: " + cf.delayTime.Value);
                RefreshColorFilters();
                await Task.Delay(60000);
            }
        }

        public async void updateLoop()
        {
            await Task.Delay(60000);
            while (true)
            {
                if (!Status.processing)
                {
                    //update
                    Status.processing = true;

                    string colorScope = " color=";
                    if (toggleRed.IsChecked.Value.Equals(true)) colorScope = colorScope + "red,";
                    if (toggleYellow.IsChecked.Value.Equals(true)) colorScope = colorScope + "yellow,";
                    if (togglePurple.IsChecked.Value.Equals(true)) colorScope = colorScope + "purple,";
                    if (toggleClear.IsChecked.Value.Equals(true)) colorScope = colorScope + "clear,";
                    if (toggleBlue.IsChecked.Value.Equals(true)) colorScope = colorScope + "blue,";
                    if (toggleGreen.IsChecked.Value.Equals(true)) colorScope = colorScope + "green,";
                    colorScope = colorScope.TrimEnd(',');

                    DateTime currentTime = DateTime.Now;
                    long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
                    long refreshScope = unixTime - 180000; // Look 3 min back

                    string xymonCmd = "xymondboard " + xymonConnect.nonTestsPatternBuilder() + "logtime>" + refreshScope.ToString() + colorScope + Settings.fields;
                    Debug.WriteLine("Running updates: " + xymonCmd);
                    xymonGetAsync(xymonCmd);

                    //RefreshColorFilters();
                    await Task.Delay(60000);
                }
                else
                    await Task.Delay(200);
            }
        }


    }
}
