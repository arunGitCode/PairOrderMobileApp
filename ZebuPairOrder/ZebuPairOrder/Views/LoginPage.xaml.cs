using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZebuPairOrder.ViewModels;

namespace ZebuPairOrder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            this.BindingContext = new LoginViewModel();
        }
    }
}