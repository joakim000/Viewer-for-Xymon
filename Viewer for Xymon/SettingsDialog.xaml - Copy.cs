﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telerik.Data.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System.Reflection;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Viewer_for_Xymon
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        public ObservableCollection<DescFilter> descFiltersC;
        ObservableCollection<string> userSignTypes = new ObservableCollection<string>();
        SettingsManager sm = new SettingsManager();

        public SettingsDialog()
        {
            descFiltersC = DescFilter.DescFilterD2C(Settings.descFiltersDict);
            sm = new SettingsManager();
            this.DataContext = sm;
            this.InitializeComponent();

            if (sm.XymondAddr == "" || sm.userSign == "")
                markReqFields();

            userSignCombo.SelectionChanged += UserSignCombo_SelectionChanged;
            XymondAddrText.TextChanged += XymondAddrText_TextChanged;
            userSignText.TextChanged += UserSignText_TextChanged;

            lineWidthBox.ValueFormat = "{0,-3}";
            maxLinesBox.ValueFormat = "{0,-3}";
            textSizeBox.ValueFormat = "{0,-3}";
            onGreenDelayBox.ValueFormat = "{0,-3}";
            refreshDelayBox.ValueFormat = "{0,-3}";
            refreshSpanBox.ValueFormat = "{0,-3}";
            cacheHoursBox.ValueFormat = "{0,-3}";

            PropertySortDescriptor sort = new PropertySortDescriptor();
            sort.PropertyName = "test";
            sort.SortOrder = SortOrder.Ascending;
            descFiltersGrid.SortDescriptors.Add(sort);

            descFiltersGrid.DataContext = descFiltersC;

            if (sm.createCaseMethod == "POST")
                createMethodPOST.IsChecked = true;
            else if (sm.createCaseMethod == "JSON")
                createMethodJSON.IsChecked = true;


        }

        private void markReqFields()
        {
            if (userSignText.Text == "" || XymondAddrText.Text == "")
            {
                this.IsPrimaryButtonEnabled = false;
            }
            else
            {
                this.IsPrimaryButtonEnabled = true;
            }
            if (XymondAddrText.Text == "") XymondAddrText.BorderBrush = Settings.strongRed_brush;
            else XymondAddrText.BorderBrush = Settings.dimgray_brush;

            if (userSignText.Text == "") userSignText.BorderBrush = Settings.strongRed_brush;
            else userSignText.BorderBrush = Settings.dimgray_brush;
        }

        private void UserSignText_TextChanged(object sender, TextChangedEventArgs e)
        {
            markReqFields();
        }

        private void XymondAddrText_TextChanged(object sender, TextChangedEventArgs e)
        {
            markReqFields();
        }

        private async void UserSignCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (sm.userSignType)
            {
                case "Custom":
                    sm.userSign = Settings.userSign;
                    break;
                case "Display name":
                    Task<string> t_displayName = Jlib.getUserInfo("Display name");
                    await t_displayName;
                    sm.userSign = t_displayName.Result;
                    break;
                case "Domain user":
                    Task<string> t_domainUser = Jlib.getUserInfo("Domain user");
                    await t_domainUser;
                    sm.userSign = t_domainUser.Result;
                    break;
                case "RFC 822":
                    Task<string> t_rfc822 = Jlib.getUserInfo("RFC 822");
                    await t_rfc822;
                    sm.userSign = t_rfc822.Result;
                    break;
                default:
                    break;
            }
        }

        private void ResetLocalSettingsBtn_Tapped(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer ls = Windows.Storage.ApplicationData.Current.LocalSettings;
            bool hasContainer = ls.Containers.ContainsKey("xyvContainer");
            if (hasContainer)
            {
                //ls.Containers["xyvContainer"];
                ls.DeleteContainer("xyvContainer");
            }
        }


        private void ClearDescriptionsBtn_Tapped(object sender, RoutedEventArgs e)
        {
            descFiltersC.Clear();
            descFiltersGrid.DataContext = descFiltersC;
        }
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

            // Save settings
            Windows.Storage.ApplicationDataContainer ls = Windows.Storage.ApplicationData.Current.LocalSettings;
            bool hasContainer = ls.Containers.ContainsKey("xyvContainer");
            if (!hasContainer)
            {
                Windows.Storage.ApplicationDataContainer container = ls.CreateContainer("xyvContainer", Windows.Storage.ApplicationDataCreateDisposition.Always);
            }
            ApplicationDataContainer c = ls.Containers["xyvContainer"];

            c.Values["XymondAddr"] = sm.XymondAddr;
            if (sm.XymondAddr != Settings.XymondAddr) Status.changedXymondAddr = true;
            Settings.XymondAddr = sm.XymondAddr;

            c.Values["XymondPort"] = sm.XymondPort;
            if (sm.XymondPort != Settings.XymondPort) Status.changedXymondAddr = true;
            Settings.XymondPort = sm.XymondPort;

            c.Values["userSignType"] = sm.userSignType;            Settings.userSignType = sm.userSignType;

            if (sm.userSignType == "Custom") c.Values["userSign"] = sm.userSign; //Don't overwrite with userSign from AD
            Settings.userSign = sm.userSign;

            c.Values["ackPattern"] = sm.ackPattern;              Settings.ackPattern = sm.ackPattern;
            c.Values["disPattern"] = sm.disPattern;            Settings.disPattern = sm.disPattern;
            c.Values["casePattern"] = sm.casePattern;            Settings.casePattern = sm.casePattern;
            c.Values["showCaseURL"] = sm.showCaseURL;            Settings.showCaseURL = sm.showCaseURL;
            c.Values["createCaseURL"] = sm.createCaseURL;            Settings.createCaseURL = sm.createCaseURL;

            if ((bool)createMethodPOST.IsChecked) sm.createCaseMethod = "POST";
            else if ((bool)createMethodJSON.IsChecked) sm.createCaseMethod = "JSON";
            c.Values["createCaseMethod"] = sm.createCaseMethod;             Settings.createCaseMethod = sm.createCaseMethod;

            c.Values["createCasePostKey"] = sm.createCasePostKey;             Settings.createCasePostKey = sm.createCasePostKey;

            c.Values["caseWebUser"] = sm.caseWebUser;             Settings.createCaseWebUser = sm.caseWebUser;
            c.Values["caseWebPw"] = sm.caseWebPw;             Settings.createCaseWebPw = sm.caseWebPw;
            c.Values["caseResponseSuccess"] = sm.caseResponseSuccess;             Settings.caseResponseSuccess = sm.caseResponseSuccess;
            c.Values["caseResponseExists"] = sm.caseResponseExists;             Settings.caseResponseExists = sm.caseResponseExists;
            c.Values["caseResponseFailed"] = sm.caseResponseFailed;             Settings.caseResponseFailed = sm.caseResponseFailed;

            c.Values["printFriendlyPattern"] = sm.printFriendlyPattern;             Settings.printFriendlyPattern = sm.printFriendlyPattern;
            c.Values["printFriendlyReq"] = sm.printFriendlyReq;             Settings.printFriendlyReq = sm.printFriendlyReq;

            c.Values["ColumnDocURL"] = sm.ColumnDocURL;             Settings.ColumnDocURL = sm.ColumnDocURL;
            c.Values["docsURL"] = sm.docsURL;             Settings.docsURL = sm.docsURL;

            c.Values["onGreenDelay"] = sm.onGreenDelay;             Settings.onGreenDelay = sm.onGreenDelay;
            c.Values["refreshDelay"] = sm.refreshDelay;             Settings.refreshDelay = sm.refreshDelay * 1000;
            c.Values["refreshSpan"] = sm.refreshSpan;             Settings.refreshSpan = sm.refreshSpan;

            c.Values["emptyPaneFill"] = sm.emptyPaneFill;             Settings.emptyPaneFill = sm.emptyPaneFill;
            c.Values["maxLineLength"] = sm.maxLineLength;             Settings.maxLineLength = sm.maxLineLength;
            c.Values["maxLines"] = sm.maxLines;             Settings.maxLines = sm.maxLines;
            c.Values["textSize"] = sm.textSize;             Settings.textSize = sm.textSize;
            c.Values["CommandLabels"] = sm.CommandLabels;             Settings.CommandLabels = sm.CommandLabels;
            c.Values["CommandTextBox"] = sm.CommandTextBox;             Settings.CommandTextBox = sm.CommandTextBox;
            c.Values["ColumnHeaderFlyout"] = sm.ColumnHeaderFlyout;             Settings.ColumnHeaderFlyout = sm.ColumnHeaderFlyout;
            //c.Values["ColumnHeaderInline"] = sm.ColumnHeaderInline;     Settings.ColumnHeaderInline = sm.ColumnHeaderInline;
            c.Values["newBold"] = sm.newBold;             Settings.newBold = sm.newBold;
            c.Values["newSaturate"] = sm.newSaturate;             Settings.newSaturate = sm.newSaturate;
            c.Values["ackBold"] = sm.ackBold;             Settings.ackBold = sm.ackBold;
            c.Values["ackSaturate"] = sm.ackSaturate;             Settings.ackSaturate = sm.ackSaturate;
            c.Values["showManualBlue"] = sm.showManualBlue;             Settings.showManualBlue = sm.showManualBlue;
            c.Values["showConsole"] = sm.showConsole;             Settings.showConsole = sm.showConsole;
            c.Values["disableUntilGreen"] = sm.disableUntilGreen;             Settings.disableUntilGreen = sm.disableUntilGreen;

            c.Values["pane_status"] = sm.pane_status;             Settings.pane_status = sm.pane_status;
            c.Values["pane_history"] = sm.pane_history;           Settings.pane_history = sm.pane_history;
            c.Values["pane_docs"] = sm.pane_docs;                 Settings.pane_docs = sm.pane_docs;
            c.Values["pane_case"] = sm.pane_case;                 Settings.pane_case = sm.pane_case;
            c.Values["pane_test"] = sm.pane_test;                 Settings.pane_test = sm.pane_test;
            c.Values["pane_trends"] = sm.pane_trends;             Settings.pane_trends = sm.pane_trends;
            c.Values["pane_log"] = sm.pane_log;                   Settings.pane_log = sm.pane_log;
            c.Values["pane_info"] = sm.pane_info;                 Settings.pane_info = sm.pane_info;
            c.Values["pane_debug"] = sm.pane_debug;               Settings.pane_debug = sm.pane_debug;

            c.Values["verboseLog"] = sm.verboseLog; Settings.verboseLog = sm.verboseLog;
            c.Values["cacheOnStart"] = sm.cacheOnStart; Settings.cacheOnStart = sm.cacheOnStart;
            c.Values["cacheHours"] = sm.cacheHours; Settings.cacheHours = sm.cacheHours;
            c.Values["maxRetry"] = sm.maxRetry; Settings.maxRetry = sm.maxRetry;
            c.Values["histPattern"] = sm.histPattern; Settings.histPattern = sm.histPattern;
            c.Values["ackHistURL"] = sm.ackHistURL; Settings.ackHistURL = sm.ackHistURL;
            c.Values["ackHistPattern"] = sm.ackHistPattern; Settings.ackHistPattern = sm.ackHistPattern;
            c.Values["histLimit"] = sm.histLimit; Settings.histLimit = sm.histLimit;



            Windows.Storage.ApplicationDataCompositeValue saveCreateCasePairs = new Windows.Storage.ApplicationDataCompositeValue();
            foreach (ValuePair ccp in sm.createCasePairs)
            {
                saveCreateCasePairs[ccp.key] = ccp.val;
            }
            c.Values["createCasePairs"] = saveCreateCasePairs;

            Windows.Storage.ApplicationDataCompositeValue saveDescFilters = new Windows.Storage.ApplicationDataCompositeValue();
            Settings.descFiltersDict = DescFilter.DescFilterC2D(descFiltersC);
            var dfC = DescFilter.DescFilterD2C(Settings.descFiltersDict);
            foreach (DescFilter df in dfC)
            {
                saveDescFilters[df.test] = df.pattern;
            }
            c.Values["descFilters"] = saveDescFilters;


            

        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void newBtn_Tapped(object sender, RoutedEventArgs e)
        {
            string patternForNew;
            if (Settings.descFiltersDict.ContainsKey("DEFAULT"))
            {
                patternForNew = Settings.descFiltersDict["DEFAULT"];
            }
            else
            {
                patternForNew = "(?<=&%COLOR% )(?<c1>.*?)(?=(\\r|\\n))";
            }

            descFiltersC.Add(new DescFilter("new test", patternForNew));
            descFiltersGrid.SelectItem(descFiltersC.Last());
            descFiltersGrid.ScrollItemIntoView(descFiltersC.Last());
            //descFiltersGrid.BeginEdit(descFiltersGrid.SelectedItem);
        }
        private void deleteBtn_Tapped(object sender, RoutedEventArgs e)
        {
            descFiltersC.Remove((DescFilter)descFiltersGrid.SelectedItem);
        }
        private void editBtn_Tapped(object sender, RoutedEventArgs e)
        {
            descFiltersGrid.BeginEdit(descFiltersGrid.SelectedItem);
        }

        private async void ImportBtn_Tapped(object sender, RoutedEventArgs a)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
            picker.FileTypeFilter.Add(".vfx");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                try
                { 
                    Debug.WriteLine("Importing settings from file: " + file.Name);
                    string configImport = await Windows.Storage.FileIO.ReadTextAsync(file);
                    //Debug.WriteLine(configImport);

                    Match m;
                    string importPattern;
                    string patternPre = @"(?<=(?<!#)";
                    string patternPost = @"="").+?(?=""(\r|\n))"; 
                    var settings = sm.GetType().GetProperties();
                    foreach (PropertyInfo setting in settings)
                    {
                        //Debug.WriteLine("sm." + setting.Name + " " + setting.PropertyType);

                        importPattern = patternPre + setting.Name + patternPost;
                        m = Regex.Match(configImport, importPattern);
                        if (m.Success)
                        {
                            if (setting.PropertyType.FullName == "System.String")
                            {
                                sm.GetType().GetProperty(setting.Name).SetValue(sm, m.Value, null);
                                Debug.WriteLine("Import setting: " + setting.Name + "=" + m.Value + "  type: System.String" );
                            }
                            else if (setting.PropertyType.FullName == "System.Int32")
                            {
                                if (Int32.TryParse(m.Value, out int parseVal))
                                {
                                    sm.GetType().GetProperty(setting.Name).SetValue(sm, parseVal, null);
                                    Debug.WriteLine("Import setting: " + setting.Name + "=" + m.Value + "  type: System.Int32");
                                }
                                else 
                                    Debug.WriteLine("Import setting: " + setting.Name + " found: " + m.Value + "  type: System.Int32 - FAILED TO PARSE");
                            }
                            else if (setting.PropertyType.FullName == "System.Boolean")
                            {
                                if (Boolean.TryParse(m.Value, out bool parseVal))
                                {
                                    sm.GetType().GetProperty(setting.Name).SetValue(sm, parseVal, null);
                                    Debug.WriteLine("Import setting: " + setting.Name + "=" + m.Value + "  type: System.Boolean");
                                }
                                else
                                    Debug.WriteLine("Import setting: " + setting.Name + " found: " + m.Value + "  type: System.Boolean - FAILED TO PARSE");
                            }
                            else
                                Debug.WriteLine("Import setting: " + setting.Name + " - unknown type: " + setting.PropertyType.FullName);

                        }
                        else
                        {
                            Debug.WriteLine("Import setting: " + setting.Name + " - not found");
                        }
                    }


                    //m = Regex.Match(configImport, @"(?<=(?<!#)XymondAddr="").+?(?=""(\r|\n))"); if (m.Success) sm.XymondAddr = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)XymondPort="").+?(?=""(\r|\n))"); if (m.Success) if (Int32.TryParse(m.Value, out int parseVal)) { sm.XymondPort = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)onGreenDelay="").+?(?=""(\r|\n))"); if (m.Success) if (Int32.TryParse(m.Value, out int parseVal)) { sm.onGreenDelay = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)refreshDelay="").+?(?=""(\r|\n))"); if (m.Success) if (Int32.TryParse(m.Value, out int parseVal)) { sm.refreshDelay = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)refreshSpan="").+?(?=""(\r|\n))"); if (m.Success) if (Int32.TryParse(m.Value, out int parseVal)) { sm.refreshSpan = parseVal; Debug.WriteLine("Import found: " + m.Value); }

                    //m = Regex.Match(configImport, @"(?<=(?<!#)userSignType="").+?(?=""(\r|\n))"); if (m.Success) sm.userSignType = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)userSign="").+?(?=""(\r|\n))"); if (m.Success) sm.userSign = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)webUser="").+?(?=""(\r|\n))"); if (m.Success) sm.webUser = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)webPw="").+?(?=""(\r|\n))"); if (m.Success) sm.webPw = m.Value; Debug.WriteLine("Import found: " + m.Value);

                    //// Nya
                    //m = Regex.Match(configImport, @"(?<=(?<!#)maxLineLength="").+?(?=""(\r|\n))"); if (m.Success) if (Int32.TryParse(m.Value, out int parseVal)) { sm.maxLineLength = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)maxLines="").+?(?=""(\r|\n))"); if (m.Success) if (Int32.TryParse(m.Value, out int parseVal)) { sm.maxLines = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)textSize="").+?(?=""(\r|\n))"); if (m.Success) if (Int32.TryParse(m.Value, out int parseVal)) { sm.textSize = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)CommandLabels="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.CommandLabels = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)CommandTextBox="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.CommandTextBox = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)newBold="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.newBold = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)ackBold="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.ackBold = parseVal; Debug.WriteLine("Import found: " + m.Value); }

                    //m = Regex.Match(configImport, @"(?<=(?<!#)showManualBlue="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.showManualBlue = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)showConsole="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.showConsole = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)disableUntilGreen="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.disableUntilGreen = parseVal; Debug.WriteLine("Import found: " + m.Value); }

                    //m = Regex.Match(configImport, @"(?<=(?<!#)newSaturate="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.newSaturate = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)ackSaturate="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.ackSaturate = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)emptyPaneFill="").+?(?=""(\r|\n))"); if (m.Success) sm.emptyPaneFill = m.Value; Debug.WriteLine("Import found: " + m.Value);


                    //m = Regex.Match(configImport, @"(?<=(?<!#)pane_status="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.pane_status = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)pane_history="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.pane_history = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)pane_docs="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.pane_docs = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)pane_case="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.pane_case = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)pane_test="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.pane_test = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)pane_trends="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.pane_trends = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)pane_log="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.pane_log = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)pane_info="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.pane_info = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)pane_debug="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.pane_debug = parseVal; Debug.WriteLine("Import found: " + m.Value); }

                    //m = Regex.Match(configImport, @"(?<=(?<!#)cacheOnStart="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.cacheOnStart = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)cacheHours="").+?(?=""(\r|\n))"); if (m.Success) if (Int32.TryParse(m.Value, out int parseVal)) { sm.cacheHours = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)verboseLog="").+?(?=""(\r|\n))"); if (m.Success) if (Boolean.TryParse(m.Value, out bool parseVal)) { sm.verboseLog = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)maxRetry="").+?(?=""(\r|\n))"); if (m.Success) if (Int32.TryParse(m.Value, out int parseVal)) { sm.maxRetry = parseVal; Debug.WriteLine("Import found: " + m.Value); }
                    //m = Regex.Match(configImport, @"(?<=(?<!#)histPattern="").+?(?=""(\r|\n))"); if (m.Success) sm.histPattern = m.Value; Debug.WriteLine("Import found: " + m.Value);

                   

                    ////string patternPre = @"(?<=(?<!#)"; string patternPost = @"="").+?(?=""(\r|\n))";
                    ////string settingName = "ackHistURL";
                    ////string importPattern = patternPre + settingName + patternPost;
                    ////m = Regex.Match(configImport, importPattern);
                    ////if (m.Success)
                    ////{
                    ////    sm.ackHistURL = m.Value;
                    ////    Debug.WriteLine("Import setting: " + settingName + " found: " + m.Value);
                    ////}
                    ////else
                    ////{
                    ////    Debug.WriteLine("Import setting: " + settingName + " - not found");
                    ////}

                    //m = Regex.Match(configImport, @"(?<=(?<!#)ackHistURL="").+?(?=""(\r|\n))"); if (m.Success) sm.ackHistURL = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)ackHistPattern="").+?(?=""(\r|\n))"); if (m.Success) sm.ackHistPattern = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)histLimit="").+?(?=""(\r|\n))"); if (m.Success) if (Int32.TryParse(m.Value, out int parseVal)) { sm.histLimit = parseVal; Debug.WriteLine("Import found: " + m.Value); }

                    //// Slut nya

                    //m = Regex.Match(configImport, @"(?<=(?<!#)casePattern="").+?(?=""(\r|\n))"); if (m.Success) sm.casePattern = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)showCaseURL="").+?(?=""(\r|\n))"); if (m.Success) sm.showCaseURL = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)createCaseURL="").+?(?=""(\r|\n))"); if (m.Success) sm.createCaseURL = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)createCaseMethod="").+?(?=""(\r|\n))"); if (m.Success) sm.createCaseMethod = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)caseResponseSuccess="").+?(?=""(\r|\n))"); if (m.Success) sm.caseResponseSuccess = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)caseResponseExists="").+?(?=""(\r|\n))"); if (m.Success) sm.caseResponseExists = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)caseResponseFailed="").+?(?=""(\r|\n))"); if (m.Success) sm.caseResponseFailed = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)caseWebUser="").+?(?=""(\r|\n))"); if (m.Success) sm.caseWebUser = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)caseWebPw="").+?(?=""(\r|\n))"); if (m.Success) sm.caseWebPw = m.Value; Debug.WriteLine("Import found: " + m.Value);


                    //m = Regex.Match(configImport, @"(?<=(?<!#)printFriendlyPattern="").+?(?=""(\r|\n))"); if (m.Success) sm.printFriendlyPattern = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)printFriendlyReq="").+?(?=""(\r|\n))"); if (m.Success) sm.printFriendlyReq = m.Value; Debug.WriteLine("Import found: " + m.Value);

                    //m = Regex.Match(configImport, @"(?<=(?<!#)docsURL="").+?(?=""(\r|\n))"); if (m.Success) sm.docsURL = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)ColumnDocURL="").+?(?=""(\r|\n))"); if (m.Success) sm.ColumnDocURL = m.Value; Debug.WriteLine("Import found: " + m.Value);



                    //m = Regex.Match(configImport, @"(?<=(?<!#)red_hex="").*?(?="")"); if (m.Success) sm.red_hex = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)strongRed_hex="").*?(?="")"); if (m.Success) sm.strongRed_hex = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)yellow_hex="").*?(?="")"); if (m.Success) sm.yellow_hex = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)strongYellow_hex="").*?(?="")"); if (m.Success) sm.strongYellow_hex = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)purple_hex="").*?(?="")"); if (m.Success) sm.purple_hex = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)strongPurple_hex="").*?(?="")"); if (m.Success) sm.strongPurple_hex = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)clear_hex="").*?(?="")"); if (m.Success) sm.clear_hex = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)blue_hex="").*?(?="")"); if (m.Success) sm.blue_hex = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)green_hex="").*?(?="")"); if (m.Success) sm.green_hex = m.Value; Debug.WriteLine("Import found: " + m.Value);

                    //m = Regex.Match(configImport, @"(?<=(?<!#)StatusURL="").*?(?="")"); if (m.Success) sm.StatusURL = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)HistURL="").*?(?="")"); if (m.Success) sm.HistURL = m.Value; Debug.WriteLine("Import found: " + m.Value);
                    //m = Regex.Match(configImport, @"(?<=(?<!#)TrendsURL="").*?(?="")"); if (m.Success) sm.TrendsURL = m.Value; Debug.WriteLine("Import found: " + m.Value);


                    MatchCollection casePairsColl = Regex.Matches(configImport, @"caseKey=""(?<caseKey>(?<!#).*)""\s*caseVal=""(?<caseVal>.*)""");
                    if (casePairsColl.Count > 0)
                    {
                        sm.createCasePairs.Clear();
                        foreach (Match cp in casePairsColl)
                        {
                            string caseKey = cp.Groups["caseKey"].Value;
                            string caseVal = cp.Groups["caseVal"].Value;
                            ValuePair vp = new ValuePair(caseKey, caseVal);
                            sm.createCasePairs.Add(vp);
                        }
                    }


                    Dictionary<string, string> descImport = DescFilter.DescFilterC2D(descFiltersC);
                    MatchCollection dColl = Regex.Matches(configImport, @"test=""(?<test>(?<!#).*)""\s*pattern=""(?<pattern>.*)""");
                    foreach (Match d in dColl)
                    {
                        string test = d.Groups["test"].Value;
                        string pattern = d.Groups["pattern"].Value;
                        if (descImport.ContainsKey(test))
                            descImport[test] = pattern;
                        else
                            descImport.Add(test, pattern);
                    }
                    descFiltersC = DescFilter.DescFilterD2C(descImport);
                    descFiltersGrid.DataContext = descFiltersC;

                    //string[] defaultNonTests = { "info", "trends", "clientlog", "ztrends", };
                    //string[] defaultIgnoreCols = { "apt", "hdpsr"};
                    //string[] defaultIgnoreTags = { "nobb2", "nonongreen" };

                }
                catch (Exception e)
                {
                    Debug.WriteLine("Importing file:" + e);
                    Status.log("Importing file:" + e);
                    ViewModel.Alert(null, null, "Importing file:" + e);
                }

            }
            else
            {
                //this.textBlock.Text = "Operation cancelled.";
            }

        }
        private async void ExportDescBtn_Tapped(object sender, RoutedEventArgs a)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Xymon Viewer settings", new List<string>() { ".vfx" });
            savePicker.SuggestedFileName = "My descriptions";

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.

                try
                { 
                    Windows.Storage.CachedFileManager.DeferUpdates(file);
                    String str = String.Empty;
                    str += "##### Viewer for Xymon v" + Settings.appVersion + " settings #####" + Environment.NewLine + Environment.NewLine;

                    str += "### Descriptions ###" + Environment.NewLine;
                    foreach (DescFilter d in descFiltersC)
                    {
                        //str += d.test + " ::: " + d.pattern + Environment.NewLine;
                        str += "test=\"" + d.test + "\"" + Environment.NewLine;
                        str += "pattern=\"" + d.pattern + "\"" + Environment.NewLine;
                    }
                    str += "### Descriptions end ###" + Environment.NewLine + Environment.NewLine;

                    await Windows.Storage.FileIO.WriteTextAsync(file, str);
                    Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                    if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    {
                        Debug.WriteLine("File " + file.Name + " was saved.");
                        Status.log("File " + file.Name + " was saved.");
                    }
                    else
                    {
                        Debug.WriteLine("File " + file.Name + " COULD NOT be saved.");
                        Status.log("File " + file.Name + " COULD NOT be saved.");
                        ViewModel.Alert(null, null, "File " + file.Name + " could not be saved.");
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Saving file:" + e);
                    Status.log("Saving file:" + e);
                    ViewModel.Alert(null, null, "Saving file:" + e);
                }

            }
            else
            {
                //this.textBlock.Text = "Operation cancelled.";
            }
        }

        private async void ExportBtn_Tapped(object sender, RoutedEventArgs a)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Xymon Viewer settings", new List<string>() { ".vfx" });
            savePicker.SuggestedFileName = "My settings";

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                try
                { 
                    // Prevent updates to the remote version of the file until
                    // we finish making changes and call CompleteUpdatesAsync.
                    Windows.Storage.CachedFileManager.DeferUpdates(file);

                    string str = String.Empty;
                    str += "##### Viewer for Xymon v" + Settings.appVersion + " settings #####" + Environment.NewLine + Environment.NewLine;

                    str += "### Server settings ###" + Environment.NewLine;
                    str += "XymondAddr=\"" + Settings.XymondAddr + "\"" + Environment.NewLine;
                    str += "XymondPort=\"" + Settings.XymondPort + "\"" + Environment.NewLine;
                    str += "userSignType=\"" + Settings.userSignType + "\"" + Environment.NewLine;
                    str += "userSign=\"" + Settings.userSign + "\"" + Environment.NewLine;
                    str += "refreshDelay=\"" + Settings.refreshDelay / 1000 + "\"" + Environment.NewLine;
                    str += "refreshSpan=\"" + Settings.refreshSpan + "\"" + Environment.NewLine;
                    str += "printFriendlyPattern=\"" + Settings.printFriendlyPattern + "\"" + Environment.NewLine;
                    str += "printFriendlyReq=\"" + Settings.printFriendlyReq + "\"" + Environment.NewLine;
                    str += "histPattern=\"" + Settings.histPattern + "\"" + Environment.NewLine;
                    str += "ackHistURL=\"" + Settings.ackHistURL + "\"" + Environment.NewLine;
                    str += "ackHistPattern=\"" + Settings.ackHistPattern + "\"" + Environment.NewLine;
                    str += Environment.NewLine;

                    str += "### User preferences ###" + Environment.NewLine;
                    str += "onGreenDelay=\"" + Settings.onGreenDelay + "\"" + Environment.NewLine;
                    str += "disableUntilGreen=\"" + Settings.disableUntilGreen + "\"" + Environment.NewLine;
                    str += "maxRetry=\"" + Settings.maxRetry + "\"" + Environment.NewLine;
                    str += "histLimit=\"" + Settings.histLimit + "\"" + Environment.NewLine;
                    str += "verboseLog=\"" + Settings.verboseLog + "\"" + Environment.NewLine;
                    str += "cacheOnStart=\"" + Settings.cacheOnStart + "\"" + Environment.NewLine;
                    str += "cacheHours=\"" + Settings.cacheHours + "\"" + Environment.NewLine;
                    str += Environment.NewLine;

                    str += "### User visual preferences ###" + Environment.NewLine;
                    str += "maxLineLength=\"" + Settings.maxLineLength + "\"" + Environment.NewLine;
                    str += "maxLines=\"" + Settings.maxLines + "\"" + Environment.NewLine;
                    str += "textSize=\"" + Settings.textSize + "\"" + Environment.NewLine;
                    str += "CommandLabels=\"" + Settings.CommandLabels + "\"" + Environment.NewLine;
                    str += "CommandTextBox=\"" + Settings.CommandTextBox + "\"" + Environment.NewLine;
                    str += "newBold=\"" + Settings.newBold + "\"" + Environment.NewLine;
                    str += "newSaturate=\"" + Settings.newSaturate + "\"" + Environment.NewLine;
                    str += "ackBold=\"" + Settings.ackBold + "\"" + Environment.NewLine;
                    str += "ackSaturate=\"" + Settings.ackSaturate + "\"" + Environment.NewLine;
                    str += "showConsole=\"" + Settings.showConsole + "\"" + Environment.NewLine;
                    str += "showManualBlue=\"" + Settings.showManualBlue + "\"" + Environment.NewLine;
                    str += "emptyPaneFill=\"" + Settings.emptyPaneFill + "\"" + Environment.NewLine;
                    str += "pane_status=\"" + Settings.pane_status + "\"" + Environment.NewLine;
                    str += "pane_history=\"" + Settings.pane_history + "\"" + Environment.NewLine;
                    str += "pane_docs=\"" + Settings.pane_docs + "\"" + Environment.NewLine;
                    str += "pane_case=\"" + Settings.pane_case + "\"" + Environment.NewLine;
                    str += "pane_test=\"" + Settings.pane_test + "\"" + Environment.NewLine;
                    str += "pane_trends=\"" + Settings.pane_trends + "\"" + Environment.NewLine;
                    str += "pane_log=\"" + Settings.pane_log + "\"" + Environment.NewLine;
                    str += "pane_info=\"" + Settings.pane_info + "\"" + Environment.NewLine;
                    str += "pane_debug=\"" + Settings.pane_debug + "\"" + Environment.NewLine;
                    str += Environment.NewLine;

                    str += "### Ticketing ###" + Environment.NewLine;
                    str += "casePattern=\"" + Settings.casePattern + "\"" + Environment.NewLine;
                    str += "showCaseURL=\"" + Settings.showCaseURL + "\"" + Environment.NewLine;
                    str += "createCaseURL=\"" + Settings.createCaseURL + "\"" + Environment.NewLine;
                    str += "createCaseMethod=\"" + Settings.createCaseMethod + "\"" + Environment.NewLine;
                    foreach (ValuePair vp in Settings.createCasePairs)
                    {
                        str += "caseKey=\"" + vp.key + "\"" + Environment.NewLine;
                        str += "caseVal=\"" + vp.val + "\"" + Environment.NewLine;
                    }
                    str += "caseResponseSuccess=\"" + Settings.caseResponseSuccess + "\"" + Environment.NewLine;
                    str += "caseResponseExists=\"" + Settings.caseResponseExists + "\"" + Environment.NewLine;
                    str += "caseResponseFailed=\"" + Settings.caseResponseFailed + "\"" + Environment.NewLine;
                    //str += "caseWebUser=\"" + Settings.createCaseWebUser + "\"" + Environment.NewLine;
                    //str += "caseWebPw=\"" + Settings.createCaseWebPw + "\"" + Environment.NewLine;
                    //str += "showCaseWebUser=\"" + Settings.showCaseWebUser + "\"" + Environment.NewLine;
                    //str += "showCaseWebPw=\"" + Settings.showCaseWebPw + "\"" + Environment.NewLine;
                    str += Environment.NewLine;


                    str += "### Descriptions ###" + Environment.NewLine;
                    foreach (DescFilter d in descFiltersC)
                    {
                        //str += d.test + " ::: " + d.pattern + Environment.NewLine;
                        str += "test=\"" + d.test + "\"" + Environment.NewLine;
                        str += "pattern=\"" + d.pattern + "\"" + Environment.NewLine;
                    }
                    str += "### Descriptions end ###" + Environment.NewLine + Environment.NewLine;

                    /*
                    str += Environment.NewLine + "### Normally you do not need to modify anything below this point ###" + Environment.NewLine;
                
                    #ColumnDocURL=""
                    #docsURL=""
                
                    #StatusURL="/svcstatus.sh?HOST=%HOSTNAME%&SERVICE=%TESTNAME%"
                    #HistURL="/history.sh?HISTFILE=%HOSTNAME%.%TESTNAME%&BARSUMS=0"
                    #TrendsURL="/svcstatus.sh?HOST=%HOSTNAME%&SERVICE=%TRENDSCOL%"
                
                    #disPattern="^(Disabled by:? ?)(?<user>\w+(\s\w+)*)( @ )?.*\\nReason: (?<msg>.*)\\n"
                    #ackPattern="^\s*(?<msg>.*)\\nAcked by: (?<user>\w+(\s\w+)*)"
                
                    #bool[] startView = { false, true, true, true, false, false, false };    // ack, red, yellow, purple, clear, blue, green
                
                    #defaultNonTests = { "info", "trends", "clientlog" }
                    #defaultIgnoreCols = {}
                    #defaultIgnoreTags = { "nobb2", "nonongreen" }
                
                    #webUser=""
                    #webPw=""  
                
                     */


                    await Windows.Storage.FileIO.WriteTextAsync(file, str);
                    Windows.Storage.Provider.FileUpdateStatus status =
                        await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                    if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    {
                        Debug.WriteLine("File " + file.Name + " was saved.");
                        Status.log("File " + file.Name + " was saved.");
                    }
                    else
                    {
                        Debug.WriteLine("File " + file.Name + " COULD NOT be saved.");
                        Status.log("File " + file.Name + " COULD NOT be saved.");
                        ViewModel.Alert(null, null, "File " + file.Name + " could not be saved.");
                    }
                }
                 catch (Exception e)
                {
                    Debug.WriteLine("Saving file:" + e);
                    Status.log("Saving file:" + e);
                    ViewModel.Alert(null, null, "Saving file:" + e);
                }
            }
            else
            {
                //this.textBlock.Text = "Operation cancelled.";
            }
        }


        private void copyBtn_Tapped(object sender, RoutedEventArgs e)
        {
            xymonConnect.xymonGetSingleStatus(hostBox.Text, testBox.Text);

        }

        //private void ExportDescBtn_Tapped(object sender, TappedRoutedEventArgs e)
        //{
        //    var dp = new DataPackage();

        //    string str = "### Descriptions ###" + Environment.NewLine;
        //    foreach (DescFilter d in descFiltersC)
        //    {
        //        //str += d.test + " ::: " + d.pattern + Environment.NewLine;
        //        str += "test=\"" + d.test + "\"" + Environment.NewLine;
        //        str += "pattern=\"" + d.pattern + "\"" + Environment.NewLine;
        //    }
        //    str += "### Descriptions end ###" + Environment.NewLine + Environment.NewLine;

        //    dp.SetText(str);
        //    Clipboard.SetContent(dp);
        //}

        private void NewCasePairBtn_Tapped(object sender, RoutedEventArgs e)
        {
            sm.createCasePairs.Add(new ValuePair(String.Empty, String.Empty));
            CreateCaseGrid.SelectItem(sm.createCasePairs.Last());
            CreateCaseGrid.ScrollItemIntoView(sm.createCasePairs.Last());
            //CreateCaseGrid.BeginEdit(sm.createCasePairs.Last());
        }
        private void EditCasePairBtn_Tapped(object sender, RoutedEventArgs e)
        {
            CreateCaseGrid.BeginEdit(CreateCaseGrid.SelectedItem);
        }
        private void DeleteCasePairBtn_Tapped(object sender, RoutedEventArgs e)
        {
            sm.createCasePairs.Remove((ValuePair)CreateCaseGrid.SelectedItem); //  Remove((DescFilter)descFiltersGrid.SelectedItem);
        }

        //private void CacheBtn_Tapped(object sender, RoutedEventArgs e)
        //{
        //    MainPage.GetCache(-1); // Get from beginning of time
        //}

    }
}