using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ZeitPlan.Views.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Manage_Course_Assign : ContentPage
    {
        public Manage_Course_Assign()
        {
            InitializeComponent();
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                LoadingInd.IsRunning = true;
                LoadData();
                LoadingInd.IsRunning = false;
            }
            catch (Exception ex)
            {
                LoadingInd.IsRunning = false;
                await DisplayAlert("Error", "Something went wrong, please  try again later" + ex.Message, "ok");
                return;
            }
        }
        async void LoadData()
        {
            DataList.ItemsSource = (await App.firebaseDatabase.Child("TBL_COURSE_ASSIGN").OnceAsync<TBL_COURSE_ASSIGN>()).Select(x => new TBL_COURSE_ASSIGN
            {
              COURSE_ASSIGN_ID=x.Object.COURSE_ASSIGN_ID,
                COURSE_FID = x.Object.COURSE_FID,
                CLASS_FID = x.Object.CLASS_FID,
               
            }).ToList();
        }
        private async void DataList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var Selected = e.Item as TBL_COURSE_ASSIGN;

            var item = (await App.firebaseDatabase.Child("TBL_COURSE_ASSIGN").OnceAsync<TBL_COURSE_ASSIGN>()).FirstOrDefault(a => a.Object.COURSE_ASSIGN_ID == Selected.COURSE_ASSIGN_ID);

            var choice = await DisplayActionSheet("Options", "Close", "Delete", "View");
            if (choice == "View")
            {

                await Navigation.PushAsync(new CourseAssign_detail(Selected));
            }
            if (choice == "Delete")
            {
                var q = DisplayAlert("Confirmation", "Are you sure you want to delete" + item.Object.COURSE_ASSIGN_ID, "Yes", "No");
                if (await q)
                {
                    await App.firebaseDatabase.Child("TBL_COURSE_ASSIGN").Child(item.Key).DeleteAsync();
                    LoadData();
                    await DisplayAlert("Confirmation", item.Object.COURSE_ASSIGN_ID + "Deleted permanently", "ok");
                }
                if (choice == "Edit")
                { }
            }
        }
        }
}