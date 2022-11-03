using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZebuPairOrder.Models;

namespace ZebuPairOrder.ViewModels
{
    public class ExpiryConfigViewModel : BaseViewModel
    {

        public ExpiryConfigViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(ExpiryDate);
        }

        public string UserId
        {
            get => Preferences.Get(nameof(UserId), "");
            set
            {
                if (!string.IsNullOrEmpty(UserId) && ExpiryDate == value)
                    return;

                Preferences.Set(nameof(UserId), value);
                OnPropertyChanged(nameof(UserId));
            }

        }
        public string ApiKey
        {
            get => Preferences.Get(nameof(ApiKey), "");
            set
            {
                if (!string.IsNullOrEmpty(nameof(ApiKey)) && ApiKey == value)
                    return;

                Preferences.Set(nameof(ApiKey), value);
                OnPropertyChanged(nameof(ApiKey));
            }
        }
        public string ExpiryDate
        {
            get => Preferences.Get(nameof(ExpiryDate), "");
            set
            {
                if (!string.IsNullOrEmpty(ExpiryDate) && ExpiryDate == value)
                    return;

                Preferences.Set(nameof(ExpiryDate), value);
                OnPropertyChanged(nameof(ExpiryDate));
            }

        }
        public string ExpiryMonth
        {
            get => Preferences.Get(nameof(ExpiryMonth), "");
            set
            {
                if (!string.IsNullOrEmpty(ExpiryMonth) && ExpiryMonth == value)
                    return;

                Preferences.Set(nameof(ExpiryMonth), value);
                OnPropertyChanged(nameof(ExpiryMonth));
            }

        }
        public string Year
        {
            get => Preferences.Get(nameof(Year), "");
            set
            {
                if (!string.IsNullOrEmpty(nameof(Year)) && Year == value)
                    return;

                Preferences.Set(nameof(Year), value);
                OnPropertyChanged(nameof(Year));
            }
        }

        public string Quantity
        {
            get => Preferences.Get(nameof(Quantity), "");
            set
            {
                if (!string.IsNullOrEmpty(nameof(Quantity)) && Quantity == value)
                    return;

                Preferences.Set(nameof(Quantity), value);
                OnPropertyChanged(nameof(Quantity));
            }
        }
        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        private async void OnCancel()
        {
            await Shell.Current.GoToAsync("//PlaceOrder");
        }
        private async void OnSave()
        {
            ////  Application.Current.Properties["ExpiryDate"] = ExpiryDate;
            ////  Application.Current.Properties["APIKEY"] = APIKEY;
            Preferences.Set(nameof(ExpiryDate), ExpiryDate);
            Preferences.Set(nameof(ApiKey), ApiKey);
            Preferences.Set(nameof(ExpiryMonth), ExpiryMonth);
            Preferences.Set(nameof(Year), Year);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("//PlaceOrder");
        }
    }
}
