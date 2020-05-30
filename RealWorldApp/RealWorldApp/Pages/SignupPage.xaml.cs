using FoodApp.Models;
using FoodApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Region = FoodApp.Models.Region;

namespace FoodApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupPage : ContentPage
    {
        public ObservableCollection<Region> RegionsCollection;
        public ObservableCollection<City> CityCollection;


        public SignupPage()
        {
            RegionsCollection = new ObservableCollection<Region>();
            CityCollection = new ObservableCollection<City>();

            InitializeComponent();
            GetRegion();
            GetCity();
            //EntBirthDate.MinimumDate = new DateTime(2018, 1, 1);
            //EntBirthDate.MaximumDate = new DateTime(2018, 12, 31);
            EntBirthDate.Date = DateTime.Now;
        }

        private async void GetRegion()
        {
            try
            {
                var Regions = await ApiService.GetRegions();
                int cnt = Regions.Count();
                foreach (var region in Regions)
                {
                    RegionsCollection.Add(region);
                }
                pickerRegion.ItemsSource = RegionsCollection;
                pickerRegion.ItemDisplayBinding = new Binding("RegionName");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void GetCity()
        {
            try
            {
                var Citis = await ApiService.GetCitys();
                int cnt = Citis.Count();
                foreach (var City in Citis)
                {
                    CityCollection.Add(City);
                }
                pickerCity.ItemsSource = CityCollection;
                pickerCity.ItemDisplayBinding = new Binding("CityName");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void BtnSignUp_Clicked(object sender, EventArgs e)
        {
            if (!EntPassword.Text.Equals(EntConfirmPassword.Text))
            {
                await DisplayAlert("Password mismatch", "Please check your confirm password", "Cancel");
            }
            else
            {
                //var selectedcity = pickerCity.SelectedItem;
                //string Scity = selectedcity.();
                Guid selectedregion = (Guid)pickerRegion.SelectedItem.GetType().GetProperty("RegionID").GetValue(pickerRegion.SelectedItem);
                Guid selectedcity = (Guid)pickerCity.SelectedItem.GetType().GetProperty("CityID").GetValue(pickerCity.SelectedItem);
                string txtGender = "";
                if (Gender.SelectedItem.ToString() == "Male")
                    txtGender = "M";
                else
                    txtGender = "F";

                var today = DateTime.Today;
                // Calculate the age.
                var age = today.Year - EntBirthDate.Date.Year;
                // Go back to the year the person was born in case of a leap year
                if (EntBirthDate.Date > today.AddYears(-age))
                    age--;

                if (age <= 0)
                {
                    await DisplayAlert("wrong Date Of Birth", "Please check your Date Of Birth", "Cancel");
                    return;
                }

                var response = await ApiService.RegisterUser(EntName.Text, EntEmail.Text, EntPassword.Text, EntFname.Text, EntLname.Text, EntPN1.Text, EntPN2.Text, EntAddress.Text, txtGender, EntBirthDate.Date.ToString(), selectedcity, selectedregion, age);
                if (response)
                {
                    await DisplayAlert("Hi", "Your account has been created", "Alright");
                    await Navigation.PushModalAsync(new LoginPage());
                }
                else
                {
                    await DisplayAlert("Oops", "Something went wrong", "Cancel");
                }
            }
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new LoginPage());
        }
    }
}