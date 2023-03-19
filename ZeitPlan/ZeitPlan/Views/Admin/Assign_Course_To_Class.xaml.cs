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
    public partial class Assign_Course_To_Class : ContentPage
    {
        public Assign_Course_To_Class()
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
            var firebaseList = (await App.firebaseDatabase.Child("TBL_CLASS").OnceAsync<TBL_CLASS>()).Select(x => new TBL_CLASS
            {

                CLASS_NAME = x.Object.CLASS_NAME,

            }).ToList();
            var refinedList = firebaseList.Select(x => x.CLASS_NAME).ToList();
            ddlClass.ItemsSource = refinedList;

            //COURSE_FID
            var firebaseList1 = (await App.firebaseDatabase.Child("TBL_COURSE").OnceAsync<TBL_COURSE>()).Select(x => new TBL_COURSE
            {

                COURSE_NAME = x.Object.COURSE_NAME,

            }).ToList();
            var refinedList1 = firebaseList1.Select(x => x.COURSE_NAME).ToList();
            ddlCourse.ItemsSource = refinedList1;
        }

        private async void btnassign_Clicked(object sender, EventArgs e)
        {
            try
            {
                //if (string.IsNullOrEmpty(txtCClass.Text) || string.IsNullOrEmpty(txtCCourse.Text))
                //{
                //    await DisplayAlert("Error", "please fill all the required fields try again", "ok");
                //    return;
                //}
                if (ddlClass.SelectedItem == null)
                {
                    await DisplayAlert("Error", "please select Class and try again", "ok");
                    return;
                }
                if (ddlCourse.SelectedItem == null)
                {
                    await DisplayAlert("Error", "please select Course and try again", "ok");
                    return;
                }
                //var check = (await App.firebaseDatabase.Child("TBL_CLASS").OnceAsync<TBL_CLASS>()).FirstOrDefault();

                //if (check != null)
                //{
                //    await DisplayAlert("Error", check.Object.CLASS_NAME + "This Class is already Picked .", "ok");
                //    return;
                //}
                //var check1 = (await App.firebaseDatabase.Child("TBL_COURSE").OnceAsync<TBL_COURSE>()).FirstOrDefault();

                //if (check1 != null)
                //{
                //    await DisplayAlert("Error", check1.Object.COURSE_NAME + "This COURSE is already Picked .", "ok");
                //    return;
                //}
                LoadingInd.IsRunning = true;
                int LastID, NewID = 1;

                var LastRecord = (await App.firebaseDatabase.Child("TBL_COURSE_ASSIGN").OnceAsync<TBL_COURSE_ASSIGN>()).FirstOrDefault();
                if (LastRecord != null)
                {
                    LastID = (await App.firebaseDatabase.Child("TBL_COURSE_ASSIGN").OnceAsync<TBL_COURSE_ASSIGN>()).Max(a => a.Object.COURSE_ASSIGN_ID);
                    NewID = ++LastID;
                }
                List<TBL_CLASS> cl = (await App.firebaseDatabase.Child("TBL_CLASS").OnceAsync<TBL_CLASS>()).Select(x => new TBL_CLASS
                {
                    CLASS_NAME = x.Object.CLASS_NAME,
                    //SESSION = x.Object.SESSION,
                    //SECTION = x.Object.SECTION,
                    //SHIFT = x.Object.SHIFT,

                }).ToList();
                int selected = cl[ddlClass.SelectedIndex].CLASS_ID;
                //COURSE_fid
                List<TBL_COURSE> cs = (await App.firebaseDatabase.Child("TBL_COURSE").OnceAsync<TBL_COURSE>()).Select(x => new TBL_COURSE
                {
                    //COURSE_ID = x.Object.COURSE_ID,
                    COURSE_NAME = x.Object.COURSE_NAME

                }).ToList();
                int selected1 = cs[ddlCourse.SelectedIndex].COURSE_ID;

                TBL_COURSE_ASSIGN ca = new TBL_COURSE_ASSIGN()
                {
                    COURSE_ASSIGN_ID = NewID,

                    CLASS_FID = selected,
                    COURSE_FID = selected1,
                    

                };
                await App.firebaseDatabase.Child("TBL_COURSE_ASSIGN").PostAsync(ca);
                LoadingInd.IsRunning = false;
                await DisplayAlert("Success", "Course Assign to Class  ", "Ok");

            }
            catch (Exception ex)
            {
                LoadingInd.IsRunning = false;
                await DisplayAlert("Error", "Something went wrong Please try again later.\nError:" + ex.Message, "ok");
                return;
            }

        }
    }
}