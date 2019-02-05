using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Viewer_for_Xymon
{
    public sealed partial class MainPage : Page
    {
        private void InitColorFilters()
        {
            DataGrid.FilterDescriptors.Clear();
            DataGrid.FilterDescriptors.Add(dummyFilter);

            colorFilters.Descriptors.Clear();
            colorFilters.Operator = LogicalOperator.Or;
            if (toggleRed.IsChecked == true) colorFilters.Descriptors.Add(cf.showRed);
            if (toggleYellow.IsChecked == true) colorFilters.Descriptors.Add(cf.showYellow);
            if (togglePurple.IsChecked == true) colorFilters.Descriptors.Add(cf.showPurple);
            if (toggleClear.IsChecked == true) colorFilters.Descriptors.Add(cf.showClear);
            if (toggleBlue.IsChecked == true) colorFilters.Descriptors.Add(cf.showBlue);
            if (toggleManualBlue.IsChecked == true) colorFilters.Descriptors.Add(cf.showManualBlue);
            if (toggleGreen.IsChecked == true) colorFilters.Descriptors.Add(cf.showGreen);


            colorFilters.Descriptors.Add(cf.delay);
            DataGrid.FilterDescriptors.Add(colorFilters);

            

        }

        private void RefreshColorFilters()
        {
            DataGrid.FilterDescriptors.Remove(colorFilters);
            DataGrid.FilterDescriptors.Add(colorFilters);
        }

        private void RefreshGridView()
        {
            DataGrid.FilterDescriptors.Remove(dummyFilter);
            DataGrid.FilterDescriptors.Add(dummyFilter);
        }
    }


    public class CustomBackgroundSelector : ObjectSelector<Brush>
    {
        protected override Brush SelectObjectCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            var f = item as Fount;

            if (f != null)
            { 
                if (f.color == "green")
                {
                    return Settings.green_brush;
                }
                else if (f.color == "yellow" && (!Settings.newSaturate || f.acktime != "0")  && (!Settings.ackSaturate || f.ackuser != Settings.userSign ))
                {
                    return Settings.yellow_brush;
                }
                else if (f.color == "yellow")
                {
                    return Settings.strongYellow_brush;
                }
                //else if (f.color == "yellow")
                //{
                //    return Settings.yellow_brush;
                //}
                else if (f.color == "red" && (!Settings.newSaturate || f.acktime != "0") && (!Settings.ackSaturate || f.ackuser != Settings.userSign))
                {
                    return Settings.red_brush;
                }
                else if (f.color == "red")
                {
                    return Settings.strongRed_brush;
                }
                //else if (f.color == "red")
                //{
                //    return Settings.red_brush;
                //}
                else if (f.color == "purple" && (!Settings.newSaturate || f.acktime != "0") && (!Settings.ackSaturate || f.ackuser != Settings.userSign))
                {
                    return Settings.purple_brush;
                }
                else if (f.color == "purple")
                {
                    return Settings.strongPurple_brush;
                }
                //else if (f.color == "purple")
                //{
                //    return Settings.purple_brush;
                //}
                else if (f.color == "blue")
                {
                    return Settings.blue_brush;
                }
                else if (f.color == "clear")
                {
                    return Settings.clear_brush;
                }
                return base.SelectObjectCore(item, container);
            }
            return new SolidColorBrush(Color.FromArgb(255, 156, 195, 229));
        }
    }



    public class CellStyleSelector : StyleSelector
    {
        public Style NormalFont { get; set; }
        public Style BoldFont { get; set; }
        public Style ItalicFont { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            var cell = (item as DataGridCellInfo);
            var f = cell.Item as Fount;
                        
            //if (!Settings.newBold && !Settings.ackBold) return this.NormalFont;
            if (Settings.newBold && (f.acktime == "" && (f.color == "red" || f.color == "yellow" || f.color == "purple"))  )
            {
                return this.BoldFont;
            }
            if (Settings.ackBold && f.ackuser == Settings.userSign)
            {
                return this.BoldFont;
            }
            if (f.ackuser == Settings.userSign)
            {
                return this.ItalicFont;
            }

            return this.NormalFont;
        }
    }

    public class CellColorSelector : StyleSelector
    {
        public Style GreenFont { get; set; }
        public Style RedFont { get; set; }
        public Style YellowFont { get; set; }
        public Style BlueFont { get; set; }
        public Style BlackFont { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            var cell = (item as DataGridCellInfo);
            var f = cell.Item as Fount;

            if (f.previousColor == "green")
            {
                return this.GreenFont;
            }
            if (f.previousColor == "red")
            {
                return this.RedFont;
            }
            if (f.previousColor == "yellow")
            {
                return this.YellowFont;
            }
            if (f.previousColor == "blue")
            {
                return this.BlueFont;
            }
            return this.BlackFont;
        }
    }


    public class CellBackgroundSelector : StyleSelector
    {
        public Style BgRed { get; set; }
        public Style BgYellow { get; set; }
        public Style BgPurple { get; set; }
        public Style BgClear { get; set; }
        public Style BgBlue { get; set; }
        public Style BgGreen { get; set; }
        public Style BgGrey { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            var cell = (item as DataGridCellInfo);
            var f = cell.Item as Fount;

            if (f.previousColor == "red")
            {
                return this.BgRed;
            }
            if (f.previousColor == "yellow")
            {
                return this.BgYellow;
            }
            if (f.previousColor == "purple")
            {
                return this.BgPurple;
            }
            if (f.previousColor == "clear")
            {
                return this.BgClear;
            }
            if (f.previousColor == "blue")
            {
                return this.BgBlue;
            }
            if (f.previousColor == "green")
            {
                return this.BgGreen;
            }
            return this.BgGrey;
        }
    }

    public class UpdateColorSelector : StyleSelector
    {
        public Style BgRed { get; set; }
        public Style BgYellow { get; set; }
        public Style BgPurple { get; set; }
        public Style BgClear { get; set; }
        public Style BgBlue { get; set; }
        public Style BgGreen { get; set; }
        public Style BgGrey { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            var cell = (item as DataGridCellInfo);
            var f = cell.Item as Fount;

            if (f.updateColor == "red")
            {
                return this.BgRed;
            }
            if (f.updateColor == "yellow")
            {
                return this.BgYellow;
            }
            if (f.updateColor == "purple")
            {
                return this.BgPurple;
            }
            if (f.updateColor == "clear")
            {
                return this.BgClear;
            }
            if (f.updateColor == "blue")
            {
                return this.BgBlue;
            }
            if (f.updateColor == "green")
            {
                return this.BgGreen;
            }
            return this.BgGrey;
        }
    }


}
