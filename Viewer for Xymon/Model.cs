using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Core.Data;

namespace Viewer_for_Xymon
{
    public static class Model {
        private static IncrementalLoadingCollection<Fount> _incFc = new IncrementalLoadingCollection<Fount>(async count => { return (from f in fc select f); }) { BatchSize = 100 };
        public static IncrementalLoadingCollection<Fount> fc { get => _incFc; set => _incFc = value; }
    }

    public class Fount : BindableBase
    {
        /* Fields */
		// Xymondboard default fields
        private string _hostname;
        private string _testname;
        private string _color = "na";
        private string _flags;
        private string _lastchange;
        private int _lastchange_epoch;
        private string _logtime;
        private int _logtime_epoch;
        private string _validtime;
        private int _validtime_epoch;
        private string _acktime;
        private int _acktime_epoch;
        private string _disabletime;
        private int _disabletime_epoch;
        private string _sender;
        private string _cookie;
        private string _line1;
        // Extras   
        private string _XMH_PAGEPATHTITLE;
        private string _ackmsg;
        private string _dismsg;
        private string _client;  // client conf data available?
        private string _clntstamp;  // latest update from client
        private int _clntstamp_epoch;
        private string _flapinfo;
        private string _stats;  // statchanges since xymond start
        private string _XMH_FLAG_NONONGREEN;
        private string _XMH_DGNAME;

        private string _XMH_NOPROPYELLOW;  // 
        private string _XMH_NOPROPRED;  //
        private string _XMH_NOPROPPURPLE;  //
        private string _XMH_RAW;  // All flags
        private string _msg;  // full status message

        // Processed fields
        private string _ackuser;
        private string _ackmsg_processed;
        private string _disuser;
        private string _dismsg_processed;
        private string _pageroot;
        private string _pagepath;
        private string _description;
        private string _updateTime;
        private string _updateColor;


        // Viewer use
        private int _greenDelay;
        private string _previousColor;

        // Debug
        private string _rawElement;
        private int _rawFields;

        /* Accessors */
        // Xymondboard default fields
        public string hostname { get => _hostname; set => SetProperty(ref _hostname, value); }
        public string testname { get => _testname; set => SetProperty(ref _testname, value); }
        public string color { get => _color; set => SetProperty(ref _color, value); }
        public string flags { get => _flags; set => SetProperty(ref _flags, value); }
        public string lastchange { get => _lastchange; set => SetProperty(ref _lastchange, value); }
        public int lastchange_epoch { get => _lastchange_epoch; set => SetProperty(ref _lastchange_epoch, value); }
        public string logtime { get => _logtime; set => SetProperty(ref _logtime, value); }
        public int logtime_epoch { get => _logtime_epoch; set => SetProperty(ref _logtime_epoch, value); }
        public string validtime { get => _validtime; set => SetProperty(ref _validtime, value); }
        public int validtime_epoch { get => _validtime_epoch; set => SetProperty(ref _validtime_epoch, value); }
        public string acktime { get => _acktime; set => SetProperty(ref _acktime, value); }
        public int acktime_epoch { get => _acktime_epoch; set => SetProperty(ref _acktime_epoch, value); }
        public string disabletime { get => _disabletime; set => SetProperty(ref _disabletime, value); }
        public int disabletime_epoch { get => _disabletime_epoch; set => SetProperty(ref _disabletime_epoch, value); }
        public string sender { get => _sender; set => SetProperty(ref _sender, value); }
        public string cookie { get => _cookie; set => SetProperty(ref _cookie, value); }
        public string line1 { get => _line1; set => SetProperty(ref _line1, value); }
        // Extras   
        public string XMH_PAGEPATHTITLE { get => _XMH_PAGEPATHTITLE; set => SetProperty(ref _XMH_PAGEPATHTITLE, value); }
        public string ackmsg { get => _ackmsg; set => SetProperty(ref _ackmsg, value); }
        public string dismsg { get => _dismsg; set => SetProperty(ref _dismsg, value); }
        public string client { get => _client; set => SetProperty(ref _client, value); }  // client conf data available?
        public string clntstamp { get => _clntstamp; set => SetProperty(ref _clntstamp, value); }  // latest update from client
        public int clntstamp_epoch { get => _clntstamp_epoch; set => SetProperty(ref _clntstamp_epoch, value); }
        public string flapinfo { get => _flapinfo; set => SetProperty(ref _flapinfo, value); }
        public string stats { get => _stats; set => SetProperty(ref _stats, value); }  // statchanges since xymond start
        public string XMH_FLAG_NONONGREEN { get => _XMH_FLAG_NONONGREEN; set => SetProperty(ref _XMH_FLAG_NONONGREEN, value); }
        public string XMH_DGNAME { get => _XMH_DGNAME; set => SetProperty(ref _XMH_DGNAME, value); }
        public string XMH_NOPROPYELLOW { get => _XMH_NOPROPYELLOW; set => SetProperty(ref _XMH_NOPROPYELLOW, value); }  // 
        public string XMH_NOPROPRED { get => _XMH_NOPROPRED; set => SetProperty(ref _XMH_NOPROPRED, value); }  //
        public string XMH_NOPROPPURPLE { get => _XMH_NOPROPPURPLE; set => SetProperty(ref _XMH_NOPROPPURPLE, value); }  //
        public string XMH_RAW { get => _XMH_RAW; set => SetProperty(ref _XMH_RAW, value); }  // All flags
        public string msg { get => _msg; set => SetProperty(ref _msg, value); }  // full status message

        // Processed fields
        public string ackuser { get => _ackuser; set => SetProperty(ref _ackuser, value); }
        public string ackmsg_processed { get => _ackmsg_processed; set => SetProperty(ref _ackmsg_processed, value); }
        public string disuser { get => _disuser; set => SetProperty(ref _disuser, value); }
        public string dismsg_processed { get => _dismsg_processed; set => SetProperty(ref _dismsg_processed, value); }
        public string pageroot { get => _pageroot; set => SetProperty(ref _pageroot, value); }
        public string pagepath { get => _pagepath; set => SetProperty(ref _pagepath, value); }
        public string description { get => _description; set => SetProperty(ref _description, value); }
        public string updateTime { get => _updateTime; set => SetProperty(ref _updateTime, value); }
        public string updateColor { get => _updateColor; set => SetProperty(ref _updateColor, value); }

        // Viewer use
        public int greenDelay { get => _greenDelay; set => SetProperty(ref _greenDelay, value); }
        public string previousColor { get => _previousColor; set => SetProperty(ref _previousColor, value); }

        // Debug
        public string rawElement { get => _rawElement; set => SetProperty(ref _rawElement, value); }
        public int rawFields { get => _rawFields; set => SetProperty(ref _rawFields, value); }

        public Fount(string hostname, string testname)
        {
            this.hostname = hostname;
            this.testname = testname;
        }

        //public void OnPropertyChanged()
        //{
        //    MainPage.Fc.Remove(this);
        //    MainPage.Fc.Add(this);
        //    Debug.WriteLine("Readded self: " + this.hostname + " : " + this.testname);
        //}

        //public Fount xymon2fount(out Fount f, List<string> xymonRow)
        //{
        //    return f;
        //}
    }

    public class DisableTarget
    {
        public string hostname { get; set; }
        public string testname { get; set; }

        public DisableTarget(string hostname, string testname)
        {
            this.hostname = hostname;
            this.testname = testname;
        }
    }

    public class DisableJob : BindableBase
    {
        private string _user;
        private List<DisableTarget> _targets;
        private string _minutes;
        private string _message;

        public string user { get => _user; set => SetProperty(ref _user, value); }
        public List<DisableTarget> targets { get => _targets; set => SetProperty(ref _targets, value); }
        public string minutes { get => _minutes; set => SetProperty(ref _minutes, value); }
        public string message { get => _message; set => SetProperty(ref _message, value); }

        public Task<string> t;

        public DisableJob(ObservableCollection<object> selected)
        {
            this.minutes = Settings.defaultDisDuration;
            this.user = Settings.userSign;
            this.targets = new List<DisableTarget>();
            foreach (Fount f in selected)
            {
                //Debug.WriteLine(f.hostname + f.testname);
                if (f.ackmsg_processed != null && f.ackmsg_processed != "")
                {
                    if (message == null) message += " " + f.ackmsg_processed;
                    else if (message.IndexOf(f.ackmsg_processed) < 0) message += " " + f.ackmsg_processed;
                }
                if (f.dismsg_processed != null && f.dismsg_processed != "")
                {
                    if (message == null) message += " " + f.dismsg_processed;
                    else if (message.IndexOf(f.dismsg_processed) < 0) message += " " + f.dismsg_processed;
                }

                if (message == null)
                {
                    // TODO
                    //t = new XWeb().checkDisableHistory(f);
                    

                    //message = await t.Result;
                    //await t;

                    //message = new XWeb().checkDisableHistory(f).Result;

                }


                if (message != null) message = message.Trim();
                DisableTarget target = new DisableTarget(f.hostname, f.testname);
                targets.Add(target);
            }
        }
    }

    public class AckTarget
    {
        public string hostname { get; set; }
        public string testname { get; set; }
        public string cookie { get; set; }
        public AckTarget(string hostname, string testname, string cookie)
        {
            this.hostname = hostname;
            this.testname = testname;
            this.cookie = cookie;
        }
    }

    public class AckJob : BindableBase
    {
        private string _user;
        private List<AckTarget> _targets;
        private string _minutes;
        private string _message;

        public string user { get => _user; set => SetProperty(ref _user, value); }
        public List<AckTarget> targets { get => _targets; set => SetProperty(ref _targets, value); }
        public string minutes { get => _minutes; set => SetProperty(ref _minutes, value); }
        public string message { get => _message; set => SetProperty(ref _message, value); }

        public AckJob(ObservableCollection<object> selected)
        {
            this.minutes = Settings.defaultAckDuration;
            this.user = Settings.userSign;
            this.targets = new List<AckTarget>();
            foreach (Fount f in selected)
            {
                //Debug.WriteLine(f.hostname + f.testname + f.cookie);
                //if (f.ackmsg != null && f.ackmsg != "" && (message != null || message.IndexOf(f.ackmsg) < 0)) message += " " + f.ackmsg;

                if (f.ackmsg_processed != null && f.ackmsg_processed != "")
                {
                    if (message == null) message += " " + f.ackmsg_processed;
                    else if (message.IndexOf(f.ackmsg_processed) < 0) message += " " + f.ackmsg_processed;
                }
                if (f.dismsg_processed != null && f.dismsg_processed != "")
                {
                    if (message == null) message += " " + f.dismsg_processed;
                    else if (message.IndexOf(f.dismsg_processed) < 0) message += " " + f.dismsg_processed;
                }

                if (message != null) message = message.Trim();

                AckTarget target = new AckTarget(f.hostname, f.testname, f.cookie);
                targets.Add(target);
            }
        }
    }

    public class ValuePair
    {
        private string _key;
        private string _val;
        public string key { get => _key; set => _key = value; }
        public string val { get => _val; set => _val = value; }

        public ValuePair(string key, string val) 
        {
            _key = key;
            _val = val;
        }

    }

}
