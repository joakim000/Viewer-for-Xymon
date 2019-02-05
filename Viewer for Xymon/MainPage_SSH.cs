using Windows.UI.Xaml.Controls;

namespace Viewer_for_Xymon
{
    public sealed partial class MainPage : Page
    {
    
         /*
        private void TestSshCmd(object sender, RoutedEventArgs e)
        {
            string pathXymon = Settings.XymonBin;
            //string pathStaChg = "/home/xymon/data/hist/allevents";
            //string pathHistLog = "/home/xymon/data/histlog";
            string sp = " ";

            string XymondAddr = Settings.XymondAddr;
            string SshAddr = Settings.SshAddr;
            //int SshPort = 22;

            string SshUser = "xymon";
            string SshPw = "sdqfe";

            var connectionInfo = new PasswordConnectionInfo(SshAddr, SshUser, SshPw);

            using (var client = new SshClient(connectionInfo))
            {
                client.Connect();
                // string concat with sp as join?
                // var cmd = client.RunCommand(pathXymon + sp + XymondAddr + sp + "\"ping\"");
                var cmd = client.RunCommand(pathXymon + sp + XymondAddr + sp + "\"xymondboard\"");
                var res = cmd.Result;
                client.Disconnect();
            }

        } */


    }
}
