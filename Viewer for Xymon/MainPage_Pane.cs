using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;

namespace Viewer_for_Xymon
{
    public sealed partial class MainPage : Page
    {

        public void SelectPane()
        {
            string str = "<html><body><center><p><strong>Multiple select</strong></p>";
            foreach (Fount f in DataGrid.SelectedItems)
            {
                str = str + "<p>" + f.hostname + " : " + f.testname + "</p>";
            }
            str = str + "</center></body></html>";
            webView1.NavigateToString(str);
        }

        public void OpenStatus(int mode)
        {
            var selected = DataGrid.SelectedItems;
            if (selected.Count() >= 1)
            {
                Fount f = selected.Last() as Fount;
                Uri navPage;

                if (mode == 2)
                {
                    navPage = new XWeb().buildUri(Settings.XymonCGIURL + "/svcstatus.sh?HOST=" + f.hostname + "&SERVICE=" + Settings.InfoCol);
                }
                else
                {
                    navPage = new XWeb().buildUri(Settings.XymonCGIURL + "/svcstatus.sh?HOST=" + f.hostname + "&SERVICE=" + f.testname);
                }

                if (mode == 0 || mode == 2)
                { 
                HttpRequestMessage request = new XWeb().RequestMessage("GET", navPage, Settings.webUser, Settings.webPw);
                webView1.NavigateWithHttpRequestMessage(request);
                SplitMain.IsPaneOpen = true;
                Status.openPane = "status";
                }
                if (mode == 1 )
                {
                    DefaultLaunch(navPage);
                }
            }
        }
        /*
       string path = Settings.XymonCGIURL + "/svcstatus.sh?HOST=" + f.hostname + "&SERVICE=" + f.testname;
       //Task<string> t = new XWeb().buildPage(path);
       Task<Uri> t = new XWeb().buildPage(path);
       //Task<HttpStringContent> t = new XWeb().buildPage(path);
       await t;
       webView1.Source = new Uri(Settings.httpPrefix + Settings.XymonWWWName);
       //webView1.NavigateToString(t.Result);
       webView1.Navigate(t.Result);
       //Debug.WriteLine(webView1.);
       */

        private async void OpenHistory(int limit)
        {
            var selected = DataGrid.SelectedItems;
            if (selected.Count() >= 1)
            {
                Fount f = selected.Last() as Fount;
                SplitMain.IsPaneOpen = true;

                Debug.WriteLine("Calling buildHistory, limit: " + limit + " histLimit-setting: " + Settings.histLimit);
                Task<string> t = new XWeb().buildHistory(f, limit);
                await t;
                //Debug.WriteLine(t.Result);
                try
                {
                    webView1.NavigateToString(t.Result);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " Error loading history: " + exception);
                    Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " Error loading history: " + exception);
                }

                Status.openPane = "history";
            }
        }

        private async void OpenDebug()
        {
            var selected = DataGrid.SelectedItems;
            if (selected.Count() >= 1)
            {
                Fount f = selected.Last() as Fount;
                SplitMain.IsPaneOpen = true;

                //Task<string> t = new VFXDebug().ShowDebug(f);
                //await t;
                //Debug.WriteLine(t.Result);
                //webView1.NavigateToString(t.Result);

                String pageString = new VFXDebug().ShowDebug(f);
                webView1.NavigateToString(pageString);

                Status.openPane = "debug";
            }
        }



        private void OpenDocs()
        {
            var selected = DataGrid.SelectedItems;
            if (selected.Count() >= 1)
            {
                Fount f = selected.Last() as Fount;
                Uri navPage = new XWeb().buildUri(Settings.docsURL.Replace("%HOSTNAME%", f.hostname));
                Debug.WriteLine("Docs navpage: " + navPage);
                HttpRequestMessage request = new XWeb().RequestMessage("GET", navPage, Settings.webUser, Settings.webPw);
                webView1.NavigateWithHttpRequestMessage(request);
                SplitMain.IsPaneOpen = true;
                Status.openPane = "docs";
            }
        }

        private void OpenTrends()
        {
            var selected = DataGrid.SelectedItems;
            if (selected.Count() >= 1)
            {
                Fount f = selected.Last() as Fount;
                Uri navPage = new XWeb().buildUri(Settings.XymonCGIURL + "/svcstatus.sh?HOST=" + f.hostname + "&SERVICE=" + Settings.TrendsCol);
                HttpRequestMessage request = new XWeb().RequestMessage("GET", navPage, Settings.webUser, Settings.webPw);
                webView1.NavigateWithHttpRequestMessage(request);
                SplitMain.IsPaneOpen = true;
                Status.openPane = "trends";
            }
        }

        private void OpenTest()
        {
            var selected = DataGrid.SelectedItems;
            if (selected.Count() >= 1)
            {
                Fount f = selected.Last() as Fount;
                Uri navPage = new XWeb().buildUri(Settings.ColumnDocURL.Replace("%TESTNAME%", f.testname));
                //Uri navPage = new XWeb().buildUri(Settings.XymonCGIURL + "/columndoc.sh?" + f.testname);
                HttpRequestMessage request = new XWeb().RequestMessage("GET", navPage, Settings.webUser, Settings.webPw);
                webView1.NavigateWithHttpRequestMessage(request);
                SplitMain.IsPaneOpen = true;
                Status.openPane = "test";
            }
        }

        private void OpenCase()
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
                    //Debug.WriteLine(f.ackmsg + " " + Settings.casePattern + " " + m.Value);
                    if (!m.Success)
                    {
                        m = Regex.Match(f.dismsg, Settings.casePattern);
                        matchInAck = false;
                    }
                    if (m.Success && matchInAck) caseRef = f.ackmsg.Substring(m.Index, m.Length);
                    if (m.Success && !matchInAck) caseRef = f.dismsg.Substring(m.Index, m.Length);
                    if (m.Success)
                    {
                        Uri navPage = new Uri(Settings.showCaseURL.Replace("%TICKET%", caseRef));
                        webView1.Navigate(navPage);
                        SplitMain.IsPaneOpen = true;
                        Status.openPane = "case";
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " Error matching casepattern: " + exception);
                    Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " Error matching casepattern: " + exception);
                }
            }
        }

        private void OpenLog()
        {
            var selected = DataGrid.SelectedItems;
            if (selected.Count() >= 1)
            {
                Fount f = selected.Last() as Fount;
                Uri navPage = new XWeb().buildUri(Settings.XymonCGIURL + "/svcstatus.sh?HOST=" + f.hostname + "&SERVICE=" + Settings.ClientCol.Replace(" ","%20"));
                HttpRequestMessage request = new XWeb().RequestMessage("GET", navPage, Settings.webUser, Settings.webPw);
                webView1.NavigateWithHttpRequestMessage(request);
                SplitMain.IsPaneOpen = true;
                Status.openPane = "log";
            }
        }

        private void OpenInfo()
        {
            var selected = DataGrid.SelectedItems;
            if (selected.Count() >= 1)
            {
                Fount f = selected.Last() as Fount;
                Uri navPage = new XWeb().buildUri(Settings.XymonCGIURL + "/svcstatus.sh?HOST=" + f.hostname + "&SERVICE=" + Settings.InfoCol.Replace(" ", "%20"));
                HttpRequestMessage request = new XWeb().RequestMessage("GET", navPage, Settings.webUser, Settings.webPw);
                webView1.NavigateWithHttpRequestMessage(request);
                SplitMain.IsPaneOpen = true;
                Status.openPane = "info";
            }
        }


    }
}
