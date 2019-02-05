using Telerik.Data.Core;

namespace Viewer_for_Xymon
{
    public class ColorFilters
    {
        public TextFilterDescriptor showAck;
        public TextFilterDescriptor showRed;
        public TextFilterDescriptor showYellow;
        public TextFilterDescriptor showPurple;
        public TextFilterDescriptor showClear;
        public TextFilterDescriptor showBlue;
        public TextFilterDescriptor showGreen;
        //public TextFilterDescriptor showNone;
        //public NumericalFilterDescriptor greenDelay;

        public TextFilterDescriptor manualBlueColor;
        public TextFilterDescriptor manualBlueDisUser;
        public CompositeFilterDescriptor showManualBlue;

        public CompositeFilterDescriptor delay;
        public NumericalFilterDescriptor delayTime;
        public TextFilterDescriptor delayGreen;
        public CompositeFilterDescriptor delayFromColor;
        public TextFilterDescriptor delayFromRed;
        public TextFilterDescriptor delayFromYellow;
        public TextFilterDescriptor delayFromPurple;
        public TextFilterDescriptor delayFromClear;
        public TextFilterDescriptor delayFromBlue;



        public ColorFilters()
        {
            showAck = new TextFilterDescriptor();
            showRed = new TextFilterDescriptor();
            showYellow = new TextFilterDescriptor();
            showPurple = new TextFilterDescriptor();
            showClear = new TextFilterDescriptor();
            showBlue = new TextFilterDescriptor();
            showGreen = new TextFilterDescriptor();
            //showNone = new TextFilterDescriptor();
            

            manualBlueColor = new TextFilterDescriptor();
            manualBlueDisUser = new TextFilterDescriptor();
            showManualBlue = new CompositeFilterDescriptor();
            manualBlueColor.PropertyName = "color";
            manualBlueColor.Operator = TextOperator.EqualsTo;
            manualBlueColor.Value = "blue";
            manualBlueDisUser.PropertyName = "disuser";
            manualBlueDisUser.Operator = TextOperator.DoesNotEqualTo;
            manualBlueDisUser.Value = "";
            showManualBlue.Operator = LogicalOperator.And;
            showManualBlue.Descriptors.Add(manualBlueDisUser);
            showManualBlue.Descriptors.Add(manualBlueColor);

            showAck.PropertyName = "acktime";
            showAck.Operator = TextOperator.EqualsTo;
            showAck.Value = "0";

            showRed.PropertyName = "color";
            showRed.Operator = TextOperator.EqualsTo;
            showRed.Value = "red";

            showYellow.PropertyName = "color";
            showYellow.Operator = TextOperator.EqualsTo;
            showYellow.Value = "yellow";

            showPurple.PropertyName = "color";
            showPurple.Operator = TextOperator.EqualsTo;
            showPurple.Value = "purple";

            showClear.PropertyName = "color";
            showClear.Operator = TextOperator.EqualsTo;
            showClear.Value = "clear";

            showBlue.PropertyName = "color";
            showBlue.Operator = TextOperator.EqualsTo;
            showBlue.Value = "blue";

            showGreen.PropertyName = "color";
            showGreen.Operator = TextOperator.EqualsTo;
            showGreen.Value = "green";


            /* Delay on change to green */
            delay = new CompositeFilterDescriptor();
            delay.Operator = LogicalOperator.And;

            delayGreen = new TextFilterDescriptor();
            delayGreen.PropertyName = "color";
            delayGreen.Operator = TextOperator.EqualsTo;
            delayGreen.Value = "green";

            delayTime = new NumericalFilterDescriptor();
            delayTime.PropertyName = "lastchange_epoch";
            delayTime.Operator = NumericalOperator.IsGreaterThan;
            delayTime.Value = 1519958000; // TODO: Comment reason for this value

           

            delayFromRed = new TextFilterDescriptor();
            delayFromRed.PropertyName = "previousColor";
            delayFromRed.Operator = TextOperator.EqualsTo;
            delayFromRed.Value = "red";

            delayFromYellow = new TextFilterDescriptor();
            delayFromYellow.PropertyName = "previousColor";
            delayFromYellow.Operator = TextOperator.EqualsTo;
            delayFromYellow.Value = "yellow";

            delayFromPurple = new TextFilterDescriptor();
            delayFromPurple.PropertyName = "previousColor";
            delayFromPurple.Operator = TextOperator.EqualsTo;
            delayFromPurple.Value = "purple";

            delayFromClear = new TextFilterDescriptor();
            delayFromClear.PropertyName = "previousColor";
            delayFromClear.Operator = TextOperator.EqualsTo;
            delayFromClear.Value = "clear";

            delayFromBlue = new TextFilterDescriptor();
            delayFromBlue.PropertyName = "previousColor";
            delayFromBlue.Operator = TextOperator.EqualsTo;
            delayFromBlue.Value = "blue";

            delayFromColor = new CompositeFilterDescriptor();
            delayFromColor.Operator = LogicalOperator.Or;
            delayFromColor.Descriptors.Add(delayFromRed);
            delayFromColor.Descriptors.Add(delayFromYellow);
            delayFromColor.Descriptors.Add(delayFromPurple);
            //delayFromColor.Descriptors.Add(delayFromClear);
            //delayFromColor.Descriptors.Add(delayFromBlue);

            delay.Descriptors.Add(delayGreen);
            delay.Descriptors.Add(delayTime);
            delay.Descriptors.Add(delayFromColor);
            


        }
    }


    public class PredefFilter
    {
        public TextFilterDescriptor text1;
        public TextFilterDescriptor text2;
        public TextFilterDescriptor text3;

        public CompositeFilterDescriptor filter1;
        public CompositeFilterDescriptor filter2;
        public CompositeFilterDescriptor filter3;

        public PredefFilter()
        {
            text1 = new TextFilterDescriptor();
            text2 = new TextFilterDescriptor();
            text3 = new TextFilterDescriptor();
            
            filter1 = new CompositeFilterDescriptor();
            filter2 = new CompositeFilterDescriptor();
            filter3 = new CompositeFilterDescriptor();


            


        }
    }

}
