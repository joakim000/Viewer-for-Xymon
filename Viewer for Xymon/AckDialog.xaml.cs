using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;




namespace Viewer_for_Xymon
{
    public sealed partial class AckDialog : ContentDialog
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(AckDialog), new PropertyMetadata(default(string)));
     
        ObservableCollection<object> sel;
        private AckJob job;
        private List<string> timeUnits = new List<string>();

        public AckDialog(ObservableCollection<object> selected)
        {
            this.InitializeComponent();
            Status.createCaseStatus = -1; // -1: No attempt, 0: Failure, 1: Success, 2: Found existing cases

            if (String.IsNullOrEmpty(Settings.createCaseURL))
            {
                CaseBtn.IsEnabled = false;
                CaseText.Text = "";
            }

            timeUnits.Add("minutes");
            timeUnits.Add("hours");
            timeUnits.Add("days");
            timeUnits.Add("weeks");
            timeUnitCombo.DataContext = timeUnits;
            timeUnitCombo.SelectedItem = "weeks";

            sel = selected;
            job = new AckJob(sel);
            DataContext = job;
            SelectionGrid.SelectionChanged += targetListView_SelectionChanged;
            //TODO Sätt första i listan som selected
            if (job.targets.Count == 1)
            {
                SelectionGrid.SelectedItem = job.targets.First();
            }

        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if ((bool)rb1.IsChecked)
            {
                //Debug.WriteLine(job.targets.ToString() + job.minutes + job.message);
                Int32.TryParse(job.minutes, out int min);
                if ((string)timeUnitCombo.SelectedItem == "hours") min = min * 60;
                if ((string)timeUnitCombo.SelectedItem == "days") min = min * 60 * 24;
                if ((string)timeUnitCombo.SelectedItem == "weeks") min = min * 60 * 24 * 7;
                job.minutes = min.ToString();
            }
            if ((bool)rb2.IsChecked)
            {
                var dt = datepicker.Date.Date;
                dt = dt.AddHours(timepicker.Time.Hours);
                dt = dt.AddMinutes(timepicker.Time.Minutes);
                var span = dt - DateTime.Now;
                var min = Math.Round(span.TotalMinutes);
                job.minutes = min.ToString();
            }
            xymonConnect.xymonAckAsync(job);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void CopyButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dp = new DataPackage();
            string str = String.Empty;
            foreach (AckTarget t in job.targets)
            {
                str = str + t.hostname + " : " + t.testname + Environment.NewLine;
            }
            dp.SetText(str);
            Clipboard.SetContent(dp);
        }

        private async void CaseButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            busyIndicator.IsActive = true;
            busyIndicator.Visibility = Windows.UI.Xaml.Visibility.Visible;
            string caseRef;
            Task<string> t = new XWeb().CreateCaseCall(sel.First() as Fount);
            //Task<string> t = new XWeb().CreateServiceNowCall(sel.First() as Fount);
            await t;
            var result_nobreaks = t.Result.Replace("\\n", "");
            result_nobreaks = result_nobreaks.Replace("\\r", "");
            string p = String.Format("((?<success>{0})|(?<exists>{1})|(?<failed>{2}))", Settings.caseResponseSuccess, Settings.caseResponseExists, Settings.caseResponseFailed);
            Match m = Regex.Match(result_nobreaks, p);
            if (!m.Success)
            {
                Status.createCaseStatus = 0;
                Debug.WriteLine("No readable response when creating case:\n" + t.Result);
                ViewModel.Alert(null, null, "No readable response when creating case:\n" + t.Result);
            }
            else
            {
                if (m.Groups["success"].Success)
                {
                    Status.createCaseStatus = 1;
                    Match refMatch = Regex.Match(result_nobreaks, String.Format("({0})(?!.*\\1)", Settings.casePattern));
                    if (!refMatch.Success)
                    {
                        ViewModel.Alert(null, null, "Case create successful: Couldn't find ticket in response.");
                    }
                    Status.createCaseStatus = 1;
                    caseRef = refMatch.Value;
                    CaseText.Text = "Created " + caseRef;
                    CaseText2.Text = "";
                    CaseBtn.IsEnabled = false;
                    //if (job.message == null) job.message = "";
                    //job.message = job.message.Insert(0, caseRef + " ");
                    // Update msg
                    if (job.message == null) job.message = caseRef + " ";
                    else job.message = caseRef + " " + job.message;
                    MessageBox.Text = job.message; // Temp fix

                    if (Settings.createdCaseToClipboard)
                    {
                        var dp = new DataPackage();
                        dp.SetText(caseRef);
                        Clipboard.SetContent(dp);
                    }

                }
                else if (m.Groups["exists"].Success)
                {
                    Status.createCaseStatus = 2;
                    MatchCollection refMatches = Regex.Matches(result_nobreaks, String.Format("({0})(?!.*\\1)", Settings.casePattern));

                    if (refMatches.Count <= 0)
                    {
                        ViewModel.Alert(null, null, "Existing case(s): Couldn't find ticket(s) in response.");
                    }
                    else
                    {
                        string str = "Open: ";
                        string previousMatch = "";
                        foreach (Match ticket in refMatches)
                        {
                            if (!String.Equals(ticket.Value, previousMatch))
                            {
                                str += ticket.Value + " ";
                            }
                            previousMatch = ticket.Value;
                        }
                        str = str.Remove(str.Length - 1);
                        CaseText.Text = str;
                        CaseText2.Text = "";
                        CaseBtn.IsEnabled = false;
                        // Update msg
                        caseRef = str;
                        if (job.message == null) job.message = caseRef + " ";
                        else job.message = job.message + " " + caseRef;
                        MessageBox.Text = job.message; // Temp fix
                    }
                }
                else if (m.Groups["failed"].Success)
                {
                    Status.createCaseStatus = 0;
                    CaseText.Text = "No server mapping";
                    CaseText2.Text = "";
                    CaseBtn.IsEnabled = false;
                }
            }

            busyIndicator.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            busyIndicator.IsActive = false;

            /*
            Match existing = Regex.Match(t.Result, Settings.caseResponseExists);
            if (existing.Success)
            {
                createCaseStatus = 2;
                MatchCollection oldCases = Regex.Matches(t.Result, "(?<=value=)" + Settings.casePattern);
                string str = "Open cases: ";
                foreach (Match oldCase in oldCases)
                {
                    str += oldCase.Value + " ";
                }
                str = str.Remove(str.Length - 1);
                CaseText.Text = str;
                // Update msg
                caseRef = oldCases[0].Value;
                if (job.message == null) job.message = caseRef + " ";
                else job.message = caseRef + " " + job.message;
                MessageBox.Text = job.message; // Temp fix
            }
            else
            { 
                Match m = Regex.Match(t.Result, Settings.casePattern);
                if (m.Success)
                {
                    createCaseStatus = 1;
                    caseRef = m.Value;
                    CaseText.Text = "Created " + caseRef;
                    CaseBtn.IsEnabled = false;
                    //if (job.message == null) job.message = "";
                    //job.message = job.message.Insert(0, caseRef + " ");
                    // Update msg
                    if (job.message == null) job.message = caseRef + " ";
                    else job.message = caseRef + " " + job.message;
                    MessageBox.Text = job.message; // Temp fix
                }
                else
                {
                    // CreateCaseCall successful but no case found in return string -> integration-script failed.
                    createCaseStatus = 0;
                    Debug.WriteLine("CreateCaseCall successful but no case found in return string -> integration-script failed.");
                    CaseText.Text = "No server mapping";

                }
            } */
        }


        private void targetListView_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
            if (Settings.verboseLog) Debug.WriteLine("Ack targets selection changed");
            //if (String.IsNullOrEmpty(Settings.createCaseURL))
            if (Settings.createCaseURL != "" && Settings.createCaseURL != null)
            {
                if (Status.createCaseStatus == -1)
                {
                    //if (SelectionGrid.SelectedItems.Count < 1)
                    //{
                    //    CaseBtn.IsEnabled = false;
                    //    CaseText.Text = "Select to create";
                    //}
                    if (SelectionGrid.SelectedItems.Count == 1)
                    {
                        CaseBtn.IsEnabled = true;
                        var target = SelectionGrid.SelectedItems.First() as AckTarget;
                        CaseText.Text = target.hostname;
                        CaseText2.Text = target.testname;
                    }
                    else if (SelectionGrid.SelectedItems.Count > 1)
                    {
                        CaseBtn.IsEnabled = false;
                        CaseText.Text = "Select one";
                    }
                }
            }
        }

        public async void OpenErrorDialog(object sender, RoutedEventArgs e, string error)
        {
            var ed = new ErrorDialog(error);
            ContentDialogResult result;
            try
            {
                result = await ed.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                }
                if (result == ContentDialogResult.Secondary)
                {
                }
            }
            catch (Exception generalException) { Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + generalException); }

        }

    }
}
