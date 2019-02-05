using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viewer_for_Xymon
{
    public class VFXDebug
    {

        //public async Task<String> ShowDebug(Fount f)
        public String ShowDebug(Fount f)
        {
                      
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

            page += "<body bgcolor=\"#ddd\">";


            page += "<center><h1>" + "Xymon" + "</h1></center>";

            page += "<h2>" + "hostname" + "</h2>";
            page += "<p>" + f.hostname + "</p>";

            page += "<h2>" + "testname" + "</h2>";
            page += "<p>" + f.testname + "</p>";

            page += "<h2>" + "color" + "</h2>";
            page += "<p>" + f.color + "</p>";

            
            page += "<h2>" + "flags" + "</h2>";
            page += "<p>" + f.flags + "</p>";

            page += "<h2>" + "lastchange_epoch" + "</h2>";
            page += "<p>" + f.lastchange_epoch + "</p>";

            page += "<h2>" + "lastchange" + "</h2>";
            page += "<p>" + f.lastchange + "</p>";

            page += "<h2>" + "logtime" + "</h2>";
            page += "<p>" + f.logtime + "</p>";

            page += "<h2>" + "validtime" + "</h2>";
            page += "<p>" + f.validtime + "</p>";

            page += "<h2>" + "acktime" + "</h2>";
            page += "<p>" + f.acktime + "</p>";

            page += "<h2>" + "disabletime" + "</h2>";
            page += "<p>" + f.disabletime + "</p>";

            page += "<h2>" + "sender" + "</h2>";
            page += "<p>" + f.sender + "</p>";

            page += "<h2>" + "cookie" + "</h2>";
            page += "<p>" + f.cookie + "</p>";

			page += "<h2>" + "line1" + "</h2>";
            page += "<p>" + f.line1 + "</p>";

			page += "<h2>" + "XMH_PAGEPATHTITLE" + "</h2>";
            page += "<p>" + f.XMH_PAGEPATHTITLE + "</p>";

			page += "<h2>" + "ackmsg" + "</h2>";
            page += "<p>" + f.ackmsg + "</p>";

			page += "<h2>" + "dismsg" + "</h2>";
            page += "<p>" + f.dismsg + "</p>";

			page += "<h2>" + "client" + "</h2>";
            page += "<p>" + f.client + "</p>";

			page += "<h2>" + "clntstamp" + "</h2>";
            page += "<p>" + f.clntstamp + "</p>";

			page += "<h2>" + "flapinfo" + "</h2>";
            page += "<p>" + f.flapinfo + "</p>";

			page += "<h2>" + "stats" + "</h2>";
            page += "<p>" + f.stats + "</p>";

			page += "<h2>" + "XMH_DGNAME" + "</h2>";
            page += "<p>" + f.XMH_DGNAME + "</p>";

			page += "<h2>" + "XMH_NOPROPYELLOW" + "</h2>";
            page += "<p>" + f.XMH_NOPROPYELLOW + "</p>";

			page += "<h2>" + "XMH_NOPROPRED" + "</h2>";
            page += "<p>" + f.XMH_NOPROPRED + "</p>";

			page += "<h2>" + "XMH_NOPROPPURPLE" + "</h2>";
            page += "<p>" + f.XMH_NOPROPPURPLE + "</p>";

            page += "<h2>" + "XMH_FLAG_NONONGREEN" + "</h2>";
            page += "<p>" + f.XMH_FLAG_NONONGREEN + "</p>";


            

            page += "<h2>" + "XMH_RAW" + "</h2>";
            page += "<p>" + f.XMH_RAW + "</p>";


           


            page += "<center><h1>" + "VfX" + "</h1></center>";

            page += "<h2>" + "Field count" + "</h2>";
            page += "<p>" + f.rawFields + "</p>";

            page += "<h2>" + "description" + "</h2>";
            page += "<p>" + f.description + "</p>";

            page += "<h2>" + "greenDelay" + "</h2>";
            page += "<p>" + f.greenDelay + "</p>";

            page += "<h2>" + "previousColor" + "</h2>";
            page += "<p>" + f.previousColor + "</p>";

            page += "<h2>" + "ackuser" + "</h2>";
            page += "<p>" + f.ackuser + "</p>";

            page += "<h2>" + "disuser" + "</h2>";
            page += "<p>" + f.disuser + "</p>";

            page += "<h2>" + "pageroot" + "</h2>";
            page += "<p>" + f.pageroot + "</p>";

            page += "<h2>" + "pagepath" + "</h2>";
            page += "<p>" + f.pagepath + "</p>";

            page += "<h2>" + "updateTime" + "</h2>";
            page += "<p>" + f.updateTime + "</p>";

            page += "<h2>" + "updateColor" + "</h2>";
            page += "<p>" + f.updateColor + "</p>";


            page += "<center><h1>" + "Raw" + "</h1></center>";

            page += "<center><h1>" + "Xymon msg" + "</h1></center>";
            page += "<h2>" + "msg" + "</h2>";
            page += "<p>" + f.msg + "</p>";


            page += "<h2>" + "Xymonrow" + "</h2>";
            page += "<p>" + f.rawElement + "</p>";


            page += "<br/><br/></body></html>";
            return page;

        }
    }
}
