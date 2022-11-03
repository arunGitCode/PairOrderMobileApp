using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZebuPairOrder.Services;

namespace ZebuPairOrder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlaceOrder : ContentPage
    {
        public PlaceOrder()
        {
            InitializeComponent();
        }
        void PlaceOrderToBroker(object sender, EventArgs e)
        {

            Application.Current.Properties["StrikePrice"] = ATMStrike.Text.ToUpper();
            Application.Current.Properties["OrderType"] = OrderType.Text.ToUpper();
            var result = new OrderServiceRestSharp().PlaceOrder(ATMStrike.Text.ToUpper(), OrderType.Text.ToUpper());

            if (result.Item1)
            {
                lblResponseStatus.TextColor = Color.Green;
                lblResponseStatus.Text = result.Item2;
            }
            else
            {
                lblResponseStatus.TextColor = Color.Red;
                lblResponseStatus.Text = result.Item2;
            }

        }
    }
}