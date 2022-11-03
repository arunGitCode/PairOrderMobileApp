using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZebuPairOrder.ViewModels;

namespace ZebuPairOrder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExpiryConfig : ContentPage
    {
        public ExpiryConfig()
        {
            InitializeComponent();
            BindingContext = new ExpiryConfigViewModel();
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}