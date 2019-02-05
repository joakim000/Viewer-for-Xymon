using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using System.Net.Http;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

namespace Viewer_for_Xymon
{
    public class XWeb
    {
        public async Task<Uri> buildPage(string path)
        {
            string r = String.Empty;
            bool success = false;
            var ub = new UriBuilder();
            ub.Scheme = Settings.httpScheme;
            ub.Host = Settings.XymonWWWName;
            ub.Path = path;
            ub.Port = -1;
            var pageUri = ub.Uri;

            //string pageString = Settings.httpPrefix + Settings.XymonWWWName + Settings.XymonCGIURL + "/svcstatus.sh?HOST=" + "debian9" + "&SERVICE=" + "disk";
            string pageString = "http://debian9/xymon-cgi/svcstatus.sh?HOST=debian9&SERVICE=bbd";
            pageUri = new Uri(pageString);

            var client = new HttpClient();
            try
            {
                var response = await client.GetAsync(pageUri, HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode == HttpStatusCode.Ok)
                {
                    r = response.Content.ToString();
                    //r = r.Replace(Settings.XymonWWWURL, Settings.XymonWWWName + Settings.XymonWWWURL);
                    r = r.Replace(Settings.XymonCGIURL, Settings.httpScheme + "//:" + Settings.XymonWWWName + Settings.XymonCGIURL);
                    //r = r.Replace("about:", Settings.httpScheme + "//:");

                    var store = IsolatedStorageFile.GetUserStoreForApplication();

                    using (var writeFile = new StreamWriter(new IsolatedStorageFileStream("htmlcontent.html", FileMode.Create, FileAccess.Write, store)))
                    {
                        writeFile.Write(r);
                        writeFile.Dispose();
                    }

                    var uri = new Uri("ms-appx-web://htmlcontent.html", UriKind.Absolute);

                    return uri;
                }
                else
                {
                    //return "<html><body><center>Failed to get " + pageUri.ToString() + "</center><center>HTTP status code: " + response.StatusCode.ToString() + "</center></body></html>";
                    r = "<html><body><center>Failed to get " + pageUri.ToString() + "</center><center>HTTP status code: " + response.StatusCode.ToString() + "</center></body></html>";
                    //success = false;
                    //return null;
                }
            }
            catch (OperationCanceledException e) { Debug.WriteLine("Operation cancelled: " + e); }
            catch (Exception e) { Debug.WriteLine("General exception: " + e); }
            finally { client.Dispose(); }

            if (success)
            {
                //r = 
            }
            return null;
        }

        public async Task<string> testWeb()
        {
            bool success = false;
            bool cfgError = false;
            // Build URI for test
            if (Settings.XymonWWWName == String.Empty) { Debug.WriteLine("Failed to get WWWName"); cfgError = true; }
            if (Settings.XymonWWWURL == String.Empty) { Debug.WriteLine("Warning: WWWURL from xymonserver.cfg is empty. OK if your www is rootpath on server.");  }
            if (Settings.XymonCGIURL == String.Empty) { Debug.WriteLine("Failed to get CGIURL"); cfgError = true; }
            if (Settings.XymonSecureCGIURL == String.Empty) { Debug.WriteLine("Failed to get WWWSecureCGIURL"); cfgError = true; }

            Uri testUri;
            HttpClient client;

            var ub = new UriBuilder();
            ub.Scheme = "https";
            ub.Host = Settings.XymonWWWName;
            ub.Path = Settings.XymonWWWURL;
            ub.Port = -1;                      // Default for scheme
            //ub.Query =                       // ?getQuery name=value
            //ub.Fragment =                    // #place

            if (!cfgError)
            {
                testUri = ub.Uri;
                client = new HttpClient();
                try
                {
                    Debug.WriteLine("Testing uri: " + testUri);
                    client.DefaultRequestHeaders.Authorization = RequestMessage("GET", testUri, Settings.webUser, Settings.webPw).Headers.Authorization;
                    var httpResponseMessage = await client.GetAsync(testUri);
                    if (httpResponseMessage.StatusCode == HttpStatusCode.Ok)
                    {
                        //Debug.WriteLine("Webtest using this ok: " + ub.Scheme);
                        Settings.httpScheme = ub.Scheme;
                        success = true;
                        return ub.Scheme.ToString();
                    }
                    else
                    {
                        Debug.WriteLine("testWeb result - Http status code: " + httpResponseMessage.StatusCode);
                    }
                    //Debug.WriteLine("HttpStatusCode: " + httpResponseMessage.StatusCode);
                    //Debug.WriteLine(httpResponseMessage.Headers);
                    //Debug.WriteLine(httpResponseMessage.RequestMessage);
                    //Debug.WriteLine(httpResponseMessage.ReasonPhrase);
                    //Debug.WriteLine(httpResponseMessage.Source);
                    //Debug.WriteLine(httpResponseMessage.Content);
                }
                //catch (OperationCanceledException e) { Debug.WriteLine(e); }
                catch (Exception exception)
                {
                    var e = exception.ToString();
                    string refusedError = "connection with the server could not be established";
                    if (ub.Scheme == "https" && e.IndexOf(refusedError) != -1)
                    {
                        ub.Scheme = "http";
                        ub.Port = -1;
                    }
                    else
                    {
                        Debug.WriteLine(e);
                    }
                }
                finally { client.Dispose(); }

                if (!success) { 
                    testUri = ub.Uri; // Now with http
                    client = new HttpClient();
                    try
                    {
                        Debug.WriteLine("Testing uri: " + testUri);
                        client.DefaultRequestHeaders.Authorization = RequestMessage("GET", testUri, Settings.webUser, Settings.webPw).Headers.Authorization;
                        var httpResponseMessage = await client.GetAsync(testUri);
                        if (httpResponseMessage.StatusCode == HttpStatusCode.Ok)
                        {
                            //Debug.WriteLine("Webtest using this ok: " + ub.Scheme);
                            Settings.httpScheme = ub.Scheme;
                            success = true;
                            return ub.Scheme.ToString();
                        }
                        else
                        {
                            Debug.WriteLine("testWeb result - Http status code: " + httpResponseMessage.StatusCode);
                        }
                    }
                    catch (Exception exception)
                    {
                        var e = exception.ToString();
                        string refusedError = "connection with the server could not be established";
                        if (ub.Scheme == "https" && e.IndexOf(refusedError) != -1)
                        {

                        }
                        else
                        {
                            Debug.WriteLine(e);
                        }
                    }
                    finally { client.Dispose(); }
                }
                if (!success)
                {
                    Debug.WriteLine("Failed webtest with both http & https. Web-functions will not work.");
                    return "failed";
                }
            }
            return "failed";
        }

        public async Task<String> CreateCaseCall(Fount f)
        {
            string r = String.Empty;
            var client = new HttpClient();
            var caseUri = new Uri(Settings.createCaseURL);
            string httpMethod = "";
            string webUser = Settings.createCaseWebUser;
            string webPw = Settings.createCaseWebPw;
            IHttpContent query; query = new HttpStringContent("foo");

            var postData = new Dictionary<string, string>();
            foreach (ValuePair vp in Settings.createCasePairs)
            {
                string val;
                switch (vp.val)
                {
                    case "%HOSTNAME%": val = f.hostname; break;
                    case "%TESTNAME%": val = f.testname; break;
                    case "%COLOR%": val = f.color; break;
                    case "%DATETIME%": val = f.lastchange; break;
                    case "%DESCRIPTION%": val = f.line1; break;
                    case "%STATUS%": val = f.msg; break;
                    case "%PAGEPATH%": val = f.XMH_PAGEPATHTITLE; break;
                    case "%ACKUSER%": val = f.ackuser; break;
                    case "%ACKMSG%": val = f.ackmsg; break;
                    case "%STAKEHOLDER%": val = f.pageroot; break;
                    //f.lastchange_epoch
                    default: val = vp.val; break;
                }
                postData.Add(vp.key, val);
            }
                        
            if (Settings.createCaseMethod == "POST")
            {
                httpMethod = "POST";
                query = new HttpFormUrlEncodedContent(postData);
            }
            else if (Settings.createCaseMethod == "JSON")
            {
                httpMethod = "POST";
                var itemAsJson = JsonConvert.SerializeObject(postData);
                query = new HttpStringContent(itemAsJson) as HttpStringContent;
                query.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                if (Settings.webUser == null || webUser == "")
                    webUser = "xymon-webservice"; // Service Now
                if (Settings.webUser == null || webUser == "")
                    webPw = "kWDRw5NcAP-M4&$d"; // Service Now
            }

            try
            {
                client.DefaultRequestHeaders.Authorization = RequestMessage(httpMethod, caseUri, webUser, webPw).Headers.Authorization;
                var response = await client.PostAsync(caseUri, query);
                if (response.StatusCode == HttpStatusCode.Ok)
                {
                    r = response.Content.ToString();
                    Debug.WriteLine("CreateCaseCall success: " + response.Content.ToString());
                }
                else
                {
                    r = "CreateCaseCall failed. Statuscode: " + response.StatusCode;
                    Debug.WriteLine("CreateCaseCall failed. Statuscode: " + response.StatusCode);
                    Debug.WriteLine(response.RequestMessage);
                    Debug.WriteLine(response.Content);
                }
            }
            catch (OperationCanceledException e) { Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "OperationCanceledException: " + e); }
            catch (Exception e) { Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "General exception: " + e); }
            finally
            {
                client.Dispose();
            }
            return r;
        }

        public Uri buildUri(string path)
        {
            //var ub = new UriBuilder();
            //ub.Scheme = Settings.httpScheme;
            //ub.Host = Settings.XymonWWWName;
            //ub.Path = Uri.EscapeDataString(path);
            //ub.Port = -1;
            //var uri = ub.Uri;

            Uri uri;
            if (path.IndexOf("://") == -1)
            {
                uri = new Uri(Settings.httpScheme + "://" + Settings.XymonWWWName + path);
            }
            else
            {
                uri = new Uri(path);
            }
            return uri;
        }

        public async Task<String> CreateServiceNowCall(Fount f)
        {
            var postData = new Dictionary<string, string>();
            postData.Add("caller_id", "Driftkontakt Axians AB");
            postData.Add("short_description", "Short description test");
            postData.Add("description", "Long description test");
            postData.Add("impact", "3");
            postData.Add("priority", "3");
            postData.Add("category", "Noc");
            postData.Add("subcategory", "Other");
            postData.Add("contact_type", "Webservice");
            postData.Add("configuration_item", "kn9920");


            var itemAsJson = JsonConvert.SerializeObject(postData);
            var query = new HttpStringContent(itemAsJson);

            var ServiceNowUri = new Uri("https://axprod.service-now.com/api/now/table/incident");
            string ServiceNowUser = "xymon-webservice";
            string ServiceNowPw = "kWDRw5NcAP-M4&$d";

            string r = String.Empty;
            var client = new HttpClient();
            try
            {
                var request = RequestMessage("POST", ServiceNowUri, ServiceNowUser, ServiceNowPw);
                query.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Authorization = request.Headers.Authorization;
               
                var response = await client.PostAsync(ServiceNowUri, query);
                if (response.StatusCode == HttpStatusCode.Ok)
                {
                    r = response.Content.ToString();
                    Debug.WriteLine("ServiceNowCall success: " + response.Content.ToString());
                }
                else
                {
                    r = "ServiceNow call failed. Statuscode: " + response.StatusCode;
                    Debug.WriteLine("ServiceNow call failed. Statuscode: " + response.StatusCode);
                    Debug.WriteLine(response.RequestMessage);
                    Debug.WriteLine(response.Content);
                }
            }
            catch (OperationCanceledException e) { Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "OperationCanceledException: " + e); }
            catch (Exception e) { Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "General exception: " + e); }
            finally
            {
                client.Dispose();
            }
            return r;
        }

        public HttpRequestMessage RequestMessage(string method, Uri uri, string user, string pw)
        {
            HttpMethod m = new HttpMethod(method);
            var request = new HttpRequestMessage(m, uri);

            // Set the header with a strong type.
            if (user != null && pw != null) { 
                string username = user;
                string password = pw;
                var buffer = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary(username + ":" + password, Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
                string base64token = Windows.Security.Cryptography.CryptographicBuffer.EncodeToBase64String(buffer);
                request.Headers.Authorization = new HttpCredentialsHeaderValue("Basic", base64token);
            }
            
            // Get the strong type out
            //System.Diagnostics.Debug.WriteLine("One of the Authorization values: {0}={1}",
            //    request.Headers.Authorization.Scheme,
            //    request.Headers.Authorization.Token);

            //// The ToString() is useful for diagnostics, too.
            //System.Diagnostics.Debug.WriteLine("The Authorization ToString() results: {0}", request.Headers.Authorization.ToString());

            return request;
        }

        public async Task<String> checkDisableHistory(Fount f)
        {
            String prevDisableMsg = null;

            var RootFilter = new HttpBaseProtocolFilter();
            RootFilter.CacheControl.ReadBehavior = Windows.Web.Http.Filters.HttpCacheReadBehavior.MostRecent;
            RootFilter.CacheControl.WriteBehavior = Windows.Web.Http.Filters.HttpCacheWriteBehavior.NoCache;
            var client = new HttpClient(RootFilter);

            string r = String.Empty;
            Uri pageUri;
            pageUri = buildUri(Settings.XymonCGIURL + "/history.sh?HISTFILE=" + f.hostname + "." + f.testname + "&BARSUMS=0&ENTRIES=100");

            try
            {
                var response = await client.GetAsync(pageUri, HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode == HttpStatusCode.Ok)
                {
                    var buffer = await response.Content.ReadAsBufferAsync();
                    var byteArray = buffer.ToArray();
                    r = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                    r = r.Replace("&amp;", "&");

                    string histPattern = Settings.histPattern;
                    MatchCollection mc = Regex.Matches(r, histPattern);
                    Debug.WriteLine("Check disable history: History entries found: " + mc.Count.ToString());
                    Status.log("Check disable history: History entries found: " + mc.Count.ToString());

                    foreach (Match m in mc)
                    {
                        string historyMessage = String.Empty;
                        if (m.Groups["color"].Value == "blue")
                        {
                            var histUri = new Uri(Settings.httpScheme + "://" + Settings.XymonWWWName + Settings.XymonCGIURL + "/" + m.Groups["link"].Value);
                            var histResponse = await client.GetAsync(histUri, HttpCompletionOption.ResponseContentRead);
                            if (histResponse.StatusCode == HttpStatusCode.Ok)
                            {
                                var Hbuffer = await histResponse.Content.ReadAsBufferAsync();
                                var HbyteArray = Hbuffer.ToArray();
                                string histStr = Encoding.UTF8.GetString(HbyteArray, 0, HbyteArray.Length);

                                string disableHistoryPattern = @"<PRE>(\r?\n)+(?<message>(.*\r?\n)+?)\r?\nStatus message when disabled follows";
                                Match disableHistoryMatch = Regex.Match(histStr, disableHistoryPattern);
                                if (disableHistoryMatch.Success)
                                {
                                    historyMessage = disableHistoryMatch.Groups["message"].Value;
                                    Debug.WriteLine("Disable history found: " + historyMessage);
                                    Status.log("Disable history found: " + historyMessage);
                                    prevDisableMsg = historyMessage.Trim();
                                    return prevDisableMsg; // Stop after first blue
                                    //break; 
                                }
                            }
                            else Debug.WriteLine(histResponse.StatusCode + "   " + histUri.ToString());

                            
                        }
                        
                    }

                    return "Error checking disable history";
                }
                else
                {
                    return "HTTP status not OK when checking disable history";
                }


            }
            catch (OperationCanceledException e) { Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Operation cancelled: " + e); }
            catch (Exception e) { Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "General exception: " + e); }
            finally { client.Dispose(); }
            return null; // Should never happen
        }

        public async Task<String> buildHistory(Fount f, int limit)
        {
            //var request = RequestMessage
            //client.DefaultRequestHeaders.Authorization = RequestMessage("GET", testUri, Settings.webUser, Settings.webPw).Headers.Authorization;

            var RootFilter = new HttpBaseProtocolFilter();
            RootFilter.CacheControl.ReadBehavior = Windows.Web.Http.Filters.HttpCacheReadBehavior.MostRecent;
            RootFilter.CacheControl.WriteBehavior = Windows.Web.Http.Filters.HttpCacheWriteBehavior.NoCache;

            var client = new HttpClient(RootFilter);
            var cy = DateTime.Now.Year;
            string page = "<html>";
            page += "<head><style>";
            // CSS
            page += "h1 { color: black; font-size: 1.5em; font-weight: bold;  }";
            page += "table { border-collapse: collapse; font-family: \"Trebuchet MS\", Arial, Helvetica, sans-serif; width: 100%; table-layout: fixed; }";
            page += "caption { caption-side: top; color: black; font-size: 1.1em; font-weight: normal; padding:5px; margin-top: 8px; }";
            page += "tr:nth-child(even) { background-color: #bbbbbb; }";
            page += "tr:nth-child(odd) { background-color: #aaaaaa; }";
            page += "tr:hover { background-color: #ddd; border: 3px; border-color: lightblue; }";
            //page += "table, th, td { border-bottom: 1px solid white; }";
            page += "th, td { text-align: left; padding: 3px; margin-left: 5px; vertical-align=bottom}";
            page += ".leftalign { text-align: left;  }";
            page += ".small { width: 25%; }";
            page += ".middle { width: 30%; }";
            page += ".big { width: 45%; }";
            //page += ".mono { font-family: \"Lucida Console\", Monaco, monospace; font-size: 0.9em }";
            //page += ".mono { font-family: \"Courier New\", Courier, monospace; font-size: 0.85em }";
            page += "a:link { color: black; text-decoration: none; } a:hover { color: blue; text-decoration: underline; } a:active { color: hotpink; text-decoration: underline; }";
            // end CSS
            page += "</style></head>";
            page += "<body bgcolor=\"#ddd\"><center>";
            page += "<h1>" + f.hostname + " - " + f.testname + "</h1>";


            string ackHistURL = Settings.ackHistURL;
            if (ackHistURL != null && ackHistURL != String.Empty)
            {
                Uri ackhistUri = new Uri(ackHistURL);
                try {
                    var ackHistResponse = await client.GetAsync(ackhistUri, HttpCompletionOption.ResponseContentRead);
                    if (ackHistResponse.StatusCode == HttpStatusCode.Ok)
                    {
                        var buffer = await ackHistResponse.Content.ReadAsBufferAsync();
                        var byteArray = buffer.ToArray();
                        var responseStr = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                        String ackHistPattern = Settings.ackHistPattern;
                        //String ackHistPattern = @"<TR BGCOLOR=#000000>\r?\n.*<FONT COLOR=white>(?<date>\w{3} \w{3} \d{1,2} .{8}).*\r?\n<\/FONT><\/TD>\r?\n<TD ALIGN=CENTER BGCOLOR=(?<color>.*)><FONT COLOR=black>(%HOSTNAME%)<\/FONT><\/TD>\r?\n.*<FONT COLOR=white>\s*(%TESTNAME%)\s*<\/FONT><\/TD>\r?\n.*\r?\n.*BGCOLOR=#000033>\s*(?<ackuser>.*)\s*\r?\n<\/TD>\r?\n<TD ALIGN=LEFT>\s*(?<message>(<a href=.*?>)?.*?)\s*<\/";
                        ackHistPattern = ackHistPattern.Replace("%HOSTNAME%", f.hostname);
                        ackHistPattern = ackHistPattern.Replace("%TESTNAME%", f.testname);
                        //Debug.WriteLine(ackHistPattern);

                        page += "<table>";
                        page += "<caption>" + "Acknowledge history" + "</caption>";

                        MatchCollection mc = Regex.Matches(responseStr, ackHistPattern);
                        foreach (Match m in mc)
                        {
                            Int32.TryParse(m.Groups["year"].Value, out int year);
                            String c;
                            switch (m.Groups["color"].Value)
                            {
                                case "red":
                                    c = Settings.red_hex; break;
                                case "yellow":
                                    c = Settings.yellow_hex; break;
                                case "purple":
                                    c = Settings.purple_hex; break;
                                case "clear":
                                    c = Settings.clear_hex; break;
                                case "blue":
                                    c = Settings.blue_hex; break;
                                case "green":
                                    c = Settings.green_hex; break;
                                default:
                                    c = "hotpink"; break;
                            }
                            //Debug.WriteLine(m.Groups["date"] + " " + m.Groups["color"] + " " + m.Groups["ackuser"] + " " + m.Groups["message"]);
                            //Debug.WriteLine(cy + " : " + year);

                            page += "<tr style=\"background-color: " + c + ";\">";
                            if (year != cy) page += "<td class=\"middle mono\">" + m.Groups["date"] + " " + m.Groups["year"] + "</td>";
                            else page += "<td class=\"middle mono\">" + m.Groups["date"] + "</td>";
                            page += "<td class=\"small\">" + m.Groups["ackuser"] + "</td>";
                            page += "<td class=\"big\">" + m.Groups["message"] + "</td>";
                            page += "</tr>";
                        }


                        page += "</table>";
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "General exception: " + e);
                }
            }


            string r = String.Empty;
            //var pageUri = buildUri(Settings.XymonCGIURL + "/history.sh?HISTFILE=" + f.hostname + "." + f.testname + "&BARSUMS=0");
            // &ENTRIES=all

            Uri pageUri;
            //if (limit > 0)
            //{
            //    pageUri = buildUri(Settings.XymonCGIURL + "/history.sh?HISTFILE=" + f.hostname + "." + f.testname + "&BARSUMS=0&ENTRIES=" + limit.ToString());
            //}
            //else
            //{
                pageUri = buildUri(Settings.XymonCGIURL + "/history.sh?HISTFILE=" + f.hostname + "." + f.testname + "&BARSUMS=0&ENTRIES=all");
            //}

            try
            {
                var response = await client.GetAsync(pageUri, HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode == HttpStatusCode.Ok)
                {
                    var buffer = await response.Content.ReadAsBufferAsync();
                    var byteArray = buffer.ToArray();
                    r = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                    r = r.Replace("&amp;", "&");
                    
                    string histPattern = Settings.histPattern;
                    MatchCollection mc = Regex.Matches(r, histPattern);
                    if (Settings.verboseLog) Debug.WriteLine("History entries found: " + mc.Count.ToString() + " Limit: " + limit);
                    if (Settings.verboseLog) Status.log("History entries found: " + mc.Count.ToString() + " Limit: " + limit);

                    page += "<table>";
                    page += "<caption>" + "Status history" + "</caption>";
                    int i = 0;
                    foreach (Match m in mc)
                    {
                        Int32.TryParse(m.Groups["year"].Value, out int year);
                        String c;
                        switch (m.Groups["color"].Value)
                        {
                            case "red":
                                c = Settings.red_hex; break;
                            case "yellow":
                                c = Settings.yellow_hex; break;
                            case "purple":
                                c = Settings.purple_hex; break;
                            case "clear":
                                c = Settings.clear_hex; break;
                            case "blue":
                                c = Settings.blue_hex; break;
                            case "green":
                                c = Settings.green_hex; break;
                            default:
                                c = "hotpink"; break;
                        }
                        
                        string historyMessage = String.Empty;
                        if (m.Groups["color"].Value == "blue")
                        {
                            var histUri = new Uri(Settings.httpScheme + "://" + Settings.XymonWWWName + Settings.XymonCGIURL + "/" + m.Groups["link"].Value );
                            var histResponse = await client.GetAsync(histUri, HttpCompletionOption.ResponseContentRead);
                            if (histResponse.StatusCode == HttpStatusCode.Ok)
                            {
                                var Hbuffer = await histResponse.Content.ReadAsBufferAsync();
                                var HbyteArray = Hbuffer.ToArray();
                                string histStr = Encoding.UTF8.GetString(HbyteArray, 0, HbyteArray.Length);

                                string disableHistoryPattern = @"<PRE>(\r?\n)+(?<message>(.*\r?\n)+?)\r?\nStatus message when disabled follows";
                                Match disableHistoryMatch = Regex.Match(histStr, disableHistoryPattern);
                                if (disableHistoryMatch.Success)
                                {
                                    historyMessage = disableHistoryMatch.Groups["message"].Value;
                                    //Debug.WriteLine(historyMessage);
                                }
                            }
                            else Debug.WriteLine(histResponse.StatusCode + "   " + histUri.ToString());
                        }
                        page += "<tr style=\"background-color: " + c + ";\">";
                        if (year != cy) page += "<td class=\"middle mono\"><a href=\"" + Settings.httpScheme + "://" + Settings.XymonWWWName + Settings.XymonCGIURL + "/" + m.Groups["link"] + "\">" + m.Groups["date"] + " " + m.Groups["year"] + "</a></td>";
                        else page += "<td class=\"middle mono\"><a href=\"" + Settings.httpScheme + "://" + Settings.XymonWWWName + Settings.XymonCGIURL + "/" + m.Groups["link"] + "\">" + m.Groups["date"] + "</a></td>";
                        page += "<td class=\"small\">" + m.Groups["duration"] + "</td>";
                        page += "<td class=\"big\">" + historyMessage + "</td>";
                        page += "</tr>";

                        i++;
                        if (i == limit)
                        {
                            break;
                        }
                    }
                    page += "</table>";

                    if (mc.Count > limit && limit > 0)
                    {
                        page += "<h3>" + i.ToString() + " out of " + mc.Count.ToString() + " history entries listed - double click History to view all";
                    }

                    page += "</center></body></html>";
                    return page;
                }
                else
                {
                    r = "<html><body><center>Failed to get " + pageUri.ToString() + "</center><center>HTTP status code: " + response.StatusCode.ToString() + "</center></body></html>";
                }
            }
            catch (OperationCanceledException e) { Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "Operation cancelled: " + e); }
            catch (Exception e) { Debug.WriteLine(DateTime.Now.ToString("yyMMdd HH:mm:ss") + " " + "General exception: " + e); }
            finally { client.Dispose(); }

            return null; // Should never happen
        }

    }
}
