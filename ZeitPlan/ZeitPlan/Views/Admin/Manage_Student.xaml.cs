using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZeitPlan.View_Model;

namespace ZeitPlan.Views.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Manage_Student : ContentPage
    {
        public Manage_Student()
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
            List<View_Student> StudentwithClassList = new List<View_Student>();
            var RawStudent = (await App.firebaseDatabase.Child("TBL_STUDENT").OnceAsync<TBL_STUDENT>()).ToList();
            foreach (var item in RawStudent)
            {
                var Class = (await App.firebaseDatabase.Child("TBL_CLASS").OnceAsync<TBL_CLASS>()).FirstOrDefault(x => x.Object.CLASS_ID == item.Object.CLASS_FID);

                if (Class == null)
                {
                    continue;
                }

                StudentwithClassList.Add(
                    new View_Student
                    {
                        CLASS_NAME = Class.Object.CLASS_NAME,
                        STUDENT_NAME = item.Object.STUDENT_NAME,
                        STUDENT_EMAIL = item.Object.STUDENT_EMAIL,
                        STUDENT_PASSWORD = item.Object.STUDENT_PASSWORD,
                        Status = item.Object.Status,
                    });
            }



            DataList.ItemsSource = StudentwithClassList;
            LoadingInd.IsRunning = false;

        }



        private async void DataList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var Selected = e.Item as View_Student;

            var item = (await App.firebaseDatabase.Child("TBL_STUDENT").OnceAsync<TBL_STUDENT>()).FirstOrDefault(a => a.Object.STUDENT_ID == Selected.STUDENT_ID);

            var choice = await DisplayActionSheet("Options", "Close", "Delete","Edit", "View", "Approve", "Block", "Pending");
            if (choice == "View")
            {

                await Navigation.PushAsync(new Student_Detail(item.Object));
               
            }
            if (choice == "Delete")
            {
                var q = DisplayAlert("Confirmation", "Are you sure you want to delete" + item.Object.STUDENT_ID, "Yes", "No");
                if (await q)
                {
                    await App.firebaseDatabase.Child("TBL_STUDENT").Child(item.Key).DeleteAsync();
                    LoadData();
                    await DisplayAlert("Confirmation", item.Object.STUDENT_ID + "Deleted permanently", "ok");
                }
                
            }

            if (choice == "Approve")
            {
                item.Object.Status = "Approved";
                await App.firebaseDatabase.Child("TBL_STUDENT").Child(item.Key).PutAsync(item.Object);
                LoadData(); 
                await DisplayAlert("Confirmation", item.Object.STUDENT_NAME + " is Approved", "ok");
            }
            if (choice == "Block")
            {
                item.Object.Status = "Blocked";
                await App.firebaseDatabase.Child("TBL_STUDENT").Child(item.Key).PutAsync(item.Object);
                LoadData(); 
                await DisplayAlert("Confirmation", item.Object.STUDENT_NAME + " is Blocked", "ok");
            }
            if (choice == "Pending")
            {
                item.Object.Status = "Pending";
                await App.firebaseDatabase.Child("TBL_STUDENT").Child(item.Key).PutAsync(item.Object);
                LoadData(); 
                await DisplayAlert("Confirmation", item.Object.STUDENT_NAME + " is Pending", "ok");
            }
            if (choice == "Edit")
            {
                await Navigation.PushAsync(new Edit_Student(item.Object));
            }

        }
    }
}