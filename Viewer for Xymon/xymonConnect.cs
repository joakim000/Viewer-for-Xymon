using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Viewer_for_Xymon
{
    public static class xymonConnect
    {
		public static async Task<string> connect(string xymonCmd) 
		{
            Status.processingCmd = xymonCmd;
            if (Settings.verboseLog)
                Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + xymonCmd.Replace(Settings.fields, " {default fields}"));

            string xymondAddr = Settings.XymondAddr;
            if (xymondAddr == null)
            {
                return "Error connecting: Xymon name is null." + Environment.NewLine + "Error connecting: Xymon name is null.";
            }
            else if (xymondAddr == "")
            {
                return "Error connecting: Xymon name is empty." + Environment.NewLine + "Error connecting: Xymon name is empty.";
            }
            Int32 xymondPort = Settings.XymondPort;
            StringBuilder xymonRecv = new StringBuilder();
            TcpClient socket = new TcpClient();
            try
            {
                await socket.ConnectAsync(xymondAddr, xymondPort);
				//Debug.WriteLine("Connected to xymond for command: " + xymonCmd);
                NetworkStream stream = socket.GetStream();
                socket.ReceiveBufferSize = 104857600;
                //socket.ReceiveBufferSize = 1048576;
                Byte[] data = Encoding.UTF8.GetBytes(xymonCmd);
                await stream.WriteAsync(data, 0, data.Length);
                socket.Client.Shutdown(SocketShutdown.Send);
				if (stream.CanRead) 
				{
                    byte[] readBuf = new byte[104857600];
                    //byte[] readBuf = new byte[1048576];
                    //byte[] readBuf = new byte[4096];
                    //int bytesRead = 0;
                    //Task<int> bytesRead = stream.ReadAsync(readBuf, 0, readBuf.Length, default(CancellationToken));
                    //Task<int> bytesRead = stream.ReadAsync(readBuf, 0, readBuf.Length);
                    do
					{
                        Task<int> bytesRead = stream.ReadAsync(readBuf, 0, readBuf.Length);
                        await bytesRead;
                        //Debug.WriteLine(bytesRead);
                        if (Settings.verboseLog) Debug.WriteLine(Environment.NewLine + "Task status: " + bytesRead.Status + " " + bytesRead.Result);
                        xymonRecv.AppendFormat("{0}", Encoding.UTF8.GetString(readBuf, 0, bytesRead.Result));
					}
					while (stream.DataAvailable);
					//Debug.WriteLine(xymonRecv.ToString());
				}
				else return ("Error getting data: Could not read stream.");  
            }
            catch (SocketException e)
            {
                string eLine = new StringReader(e.ToString()).ReadLine();
                string error = "Error connecting to " + xymondAddr + " at port " + xymondPort + Environment.NewLine + eLine;
                //Status.refreshActive = false;
                Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + error);
                Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + error);

                return error;
                //OpenErrorDialog(null, null, error);
            }
			catch (IOException e)
            {
                string eLine = new StringReader(e.ToString()).ReadLine();
                string error = "Errpr connecting to " + xymondAddr + " at port " + xymondPort + Environment.NewLine + eLine;
                //Status.refreshActive = false;
                Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + error);
                Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + error);
                return error;
                //OpenErrorDialog(null, null, error);
            }
            catch (Exception e)
            {
                string eLine = new StringReader(e.ToString()).ReadLine();
                string error = "Error connecting to " + xymondAddr + " at port " + xymondPort + Environment.NewLine + eLine;
                //Status.refreshActive = false;
                Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + error);
                Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + error);
                return error;
                //OpenErrorDialog(null, null, error);
            }
			finally 
			{
				socket.Dispose();
			}
			return xymonRecv.ToString();
		}
		
        public static async void xymonDisableAsync(DisableJob job)
        {
            string disableMsg = "Disabled by: " + Settings.userSign + "\n" + "Reason: " + job.message + "\n";
            foreach (DisableTarget target in job.targets)
            {
                string xymonCmd = "disable " + target.hostname + "." + target.testname + " " + job.minutes + " " + disableMsg;
					//Debug.WriteLine(xymonCmd);
				Task<string> t = connect(xymonCmd); 
                await t; // Necessary to wait...? 
            }
        }

        public static async void xymonEnableAsync(DisableJob job)
        {
            //string disableMsg = "Disabled by: " + Settings.userSign + "\n" + "Reason: " + job.message + "\n";
            foreach (DisableTarget target in job.targets)
            {
                string xymonCmd = "enable " + target.hostname + "." + target.testname; // + " " + job.minutes + " " + disableMsg;
                Debug.WriteLine(xymonCmd);
                Task<string> t = connect(xymonCmd);
                await t; // Necessary to wait...? 
            }
        }


        public static async void xymonAckAsync(AckJob aj)
        {
            string ackMsg = aj.message + " \nAcked by: " + Settings.userSign;
            foreach (AckTarget target in aj.targets)
            {
                string xymonCmd = "xymondack " + target.cookie + " " + aj.minutes + " " + ackMsg;
					//Debug.WriteLine(xymonCmd);
				Task<string> t = connect(xymonCmd);
                await t; // Necessary to wait...?
            }
        }

        public static async Task<string> xymonGetConfig()
        {
            string result = "failed: ";

            Task<string> t = xymonConnect.xymonGetFileAsync("xymonserver.cfg");
            await t;
            string serverconf = t.Result;

            string val; string p; Match m;
            try
            {
                p = @"(XYMONSERVERWWWNAME="")(?<cap>.*)""";
                m = Regex.Match(serverconf, p);
                if (m.Success)
                {
                    val = m.Groups["cap"].Value;
                    Settings.XymonWWWName = val.Trim();
                    Debug.WriteLine("XYMONSERVERWWWNAME=" + val);
                }
                else
                {
                    result += "XymonWWWName ";
                }

                p = @"XYMONSERVERWWWURL=""(?<cap>.*)""";
                m = Regex.Match(serverconf, p);
                if (m.Success)
                {
                    val = m.Groups["cap"].Value;
                    Settings.XymonWWWURL = val.Trim();
                    Debug.WriteLine("XYMONSERVERWWWURL=" + val);
                }
                else
                {
                    result += "XymonWWWURL ";
                }

                p = @"XYMONSERVERCGIURL=""(?<cap>.*)""";
                m = Regex.Match(serverconf, p);
                if (m.Success)
                {
                    val = m.Groups["cap"].Value;
                    Settings.XymonCGIURL = val.Trim();
                    Debug.WriteLine("XYMONSERVERCGIURL=" + val);
                }
                else
                {
                    result += "XymonCGIURL ";
                }

                p = @"XYMONSERVERSECURECGIURL=""(?<cap>.*)""";
                m = Regex.Match(serverconf, p);
                if (m.Success)
                {
                    val = m.Groups["cap"].Value;
                    Settings.XymonSecureCGIURL = val.Trim();
                    Debug.WriteLine("XYMONSERVERSECURECGIURL=" + val);
                }
                else
                {
                    result += "XymonSecureCGIURL ";
                }

                p = @"INFOCOLUMN=""(?<cap>.*)""";
                m = Regex.Match(serverconf, p);
                if (m.Success)
                {
                    val = m.Groups["cap"].Value;
                    Settings.InfoCol = val;
                    Settings.ignoreCols.Add(val.Replace(" ","\\s"));
                    Debug.WriteLine("INFOCOLUMN=" + val);
                }
                else
                {
                    result += "InfoCol ";
                }

                p = @"TRENDSCOLUMN=""(?<cap>.*)""";
                m = Regex.Match(serverconf, p);
                if (m.Success)
                {
                    val = m.Groups["cap"].Value;
                    Settings.TrendsCol = val;
                    Settings.ignoreCols.Add(val.Replace(" ", "\\s"));
                    Debug.WriteLine("TRENDSCOLUMN=" + val);
                }
                else
                {
                    result += "TrendsCol ";
                }

                p = @"CLIENTCOLUMN=""(?<cap>.*)""";
                m = Regex.Match(serverconf, p);
                if (m.Success)
                {
                    val = m.Groups["cap"].Value;
                    Settings.ClientCol = val;
                    Settings.ignoreCols.Add(val.Replace(" ", "\\s"));
                    Debug.WriteLine("CLIENTCOLUMN=" + val);
                }
                else
                {
                    result += "ClientCol ";
                }

                p = @"XYMONGENOPTS=""(?<cap>.*)""";
                m = Regex.Match(serverconf, p);
                if (m.Success)
                {
                    val = m.Groups["cap"].Value;
                    Settings.XymonGenOpts = val.Trim();
                    Debug.WriteLine("XYMONGENOPTS=" + val);
                    Match docsMatch = Regex.Match(val, @"--docurl=(?<docurl>.*?)(\s|\r|\n|$)");
                    if (docsMatch.Success)
                    {
                        string docs = docsMatch.Groups["docurl"].Value;
                        docs = docs.Replace("%s", "%HOSTNAME%");
                        Settings.docsURL = docs;
                        Debug.WriteLine("--docurl=" + docs);
                    }
                    else Debug.WriteLine("No docurl found");
                    Match ignoreColsMatch = Regex.Match(val, @"--nongreen-ignorecolumns=(?<ignorecols>.*?)(\s|\r|\n|$)");
                    if (ignoreColsMatch.Success)
                    {
                        string ignoreCols = ignoreColsMatch.Groups["ignorecols"].Value;
                        List<string> icList = ignoreCols.Split(',').ToList();
                        foreach (string ic in icList)
                        {
                            if (!Settings.ignoreCols.Contains(ic)) Settings.ignoreCols.Add(ic.Replace(" ","\\s"));
                        }
                        Debug.WriteLine("--nongreen-ignorecolumns=" + ignoreCols);
                    }
                    else Debug.WriteLine("No ignorecols found");
                }
                else
                {
                    result += "XymonGenOpts ";
                }

                p = @"COLUMNDOCURL=""(?<cap>.*)""";
                m = Regex.Match(serverconf, p);
                if (m.Success)
                {
                    val = m.Groups["cap"].Value;
                    val = val.Replace("%s", "%TESTNAME%");
                    val = val.Replace("$CGIBINURL", Settings.XymonCGIURL);
                    val = val.Replace("$XYMONSERVERCGIURL", Settings.XymonCGIURL);
                    val = val.Replace("$XYMONSERVERSECURECGIURL", Settings.XymonSecureCGIURL);
                    val = val.Replace("$XYMONWEB", Settings.XymonWWWURL);
                    val = val.Replace("$XYMONSERVERWWWNAME", Settings.XymonWWWURL);
                    Settings.ColumnDocURL = val.Trim();
                    Debug.WriteLine("COLUMNDOCURL=" + val);
                }
                else
                {
                    result += "ColumnDocURL ";
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "General exception parsing xymonserver.cfg: " + e);
                Status.log(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "General exception parsing xymonserver.cfg: " + e);
            }

            if (result == "failed: ") // Nothing has been added -> Success
            {
                result = "success";
            }
            return result;
        }

        public static async Task<string> xymonGetFileAsync(string filename)
        {
            string xymonCmd = "config " + filename;
			Task<string> t = connect(xymonCmd);
			await t;
            return t.Result;
        }

        public static async void xymonGetSingleStatus(string hostname, string testname)
        {
            string xymonCmd = "xymondboard host=" + hostname + " test=" + testname + " " + "fields=msg"; 
			
			Task<string> t = connect(xymonCmd);
			await t;
			string xymonRecv = t.Result;
            try
            { 
                var dp = new DataPackage();
                string str = String.Empty;
                if (xymonRecv.ToString().Length > 0) str = xymonRecv.ToString();
                else str = "No data recevied from Xymon on command: " + xymonCmd;
                dp.SetText(str);
                Clipboard.SetContent(dp);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Setting clipboard content to xymonRecv: " + e);
                Status.log("Setting clipboard content to xymonRecv: " + e);
            }
        }

        public static async void OpenErrorDialog(object sender, RoutedEventArgs e, string error)
        {
            Status.refreshActive = false;
            var ed = new ErrorDialog(error);
            
            ContentDialogResult result;
            try
            {
                result = await ed.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    //startUpdates();
                    //setup();
                }
                if (result == ContentDialogResult.Secondary)
                {
                    //MainPage.OpenSettingsDialog(null, null);
                }
            }
            catch (Exception generalException) { Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + generalException); }

        }

        public static string nonTestsPatternBuilder()
        {
            string nonTestsPattern = " test=^(?!((";
            foreach (string nonTest in Settings.ignoreCols)
            {
                nonTestsPattern += nonTest.Trim();
                nonTestsPattern += "|";
            }
            //nonTestsPattern = nonTestsPattern.Remove(nonTestsPattern.Length - 1); // Remove the final |
            nonTestsPattern = nonTestsPattern.Trim('|');
            nonTestsPattern += ")$)).*$ ";
            return nonTestsPattern;
        }

        


    }
}
