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
    public partial class Mange_Teacher_Assign : ContentPage
    {
        public Mange_Teacher_Assign()
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
            LoadingInd.IsRunning = true;

            DataList.ItemsSource = (await App.firebaseDatabase.Child("TBL_TEACHER_COURSE_ASSIGN").OnceAsync<TBL_TEACHER_COURSE_ASSIGN>()).Select(x => new TBL_TEACHER_COURSE_ASSIGN
            {
                TEACHER_COURSE_ASSIGN_ID = x.Object.TEACHER_COURSE_ASSIGN_ID,
                CLASS_FID = x.Object.CLASS_FID,
                TEACHER_FID = x.Object.TEACHER_FID,
            }).ToList();

            LoadingInd.IsRunning = false;



        }
        private async void DataList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var Selected = e.Item as TBL_TEACHER_COURSE_ASSIGN;

            var item = (await App.firebaseDatabase.Child("TBL_TEACHER_COURSE_ASSIGN").OnceAsync<TBL_TEACHER_COURSE_ASSIGN>()).FirstOrDefault(a => a.Object.TEACHER_COURSE_ASSIGN_ID == Selected.TEACHER_COURSE_ASSIGN_ID);

            var choice = await DisplayActionSheet("Options", "Close", "Delete", "View", "Edit");
            if (choice == "View")
            {

                await Navigation.PushAsync(new TeacherCourseAssign_Detail(Selected));
            }
            if (choice == "Delete")
            {
                var q = DisplayAlert("Confirmation", "Are you sure you want to delete" + item.Object.TEACHER_COURSE_ASSIGN_ID, "Yes", "No");
                if (await q)
                {
                    await App.firebaseDatabase.Child("TBL_TEACHER_COURSE_ASSIGN").Child(item.Key).DeleteAsync();
                    LoadData();
                    await DisplayAlert("Confirmation", item.Object.TEACHER_COURSE_ASSIGN_ID + "Deleted permanently", "ok");
                }
                if (choice == "Edit")
                { }
            }
        }
    }
}