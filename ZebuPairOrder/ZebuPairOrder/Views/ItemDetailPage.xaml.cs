using System.ComponentModel;
using Xamarin.Forms;
using ZebuPairOrder.ViewModels;

namespace ZebuPairOrder.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}