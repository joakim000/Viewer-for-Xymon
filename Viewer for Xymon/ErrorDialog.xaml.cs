using System;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Viewer_for_Xymon
{
    public sealed partial class ErrorDialog : ContentDialog
    {
        public ErrorDialog(string error)
        {
            string myError = error.Replace("Exception:", "Exception:" + Environment.NewLine);
            this.DataContext = myError;

            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
