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
    public partial class Assign_Course_To_Teacher : ContentPage
    {
        public Assign_Course_To_Teacher()
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
            //teacher_fid
            var firebaseList2 = (await App.firebaseDatabase.Child("TBL_TEACHER").OnceAsync<TBL_TEACHER>()).Select(x => new TBL_TEACHER
            {

                TEACHER_NAME = x.Object.TEACHER_NAME,

            }).ToList();
            var refinedList2 = firebaseList2.Select(x => x.TEACHER_NAME).ToList();
            ddlTeacher.ItemsSource = refinedList2;



        }

            private async void btnassign_Clicked(object sender, EventArgs e)
        {
            try
            {
                //if (string.IsNullOrEmpty(txtCClass.Text)  || string.IsNullOrEmpty(txtCTeacher.Text) )
                //{
                //    await DisplayAlert("Error", "please fill all the required fields try again", "ok");
                //    return;
                //}
                if (ddlClass.SelectedItem == null)
                {
                    await DisplayAlert("Error", "please select Class and try again", "ok");
                    return;
                }
                if (ddlTeacher.SelectedItem == null)
                {
                    await DisplayAlert("Error", "please select Teacher and try again", "ok");
                    return;
                }
                //var check = (await App.firebaseDatabase.Child("TBL_CLASS").OnceAsync<TBL_CLASS>()).FirstOrDefault();

                //if (check != null)
                //{
                //    await DisplayAlert("Error", check.Object.CLASS_NAME + "This Class is already Picked .", "ok");
                //    return;
                //}
                //var check2 = (await App.firebaseDatabase.Child("TBL_TEACHER").OnceAsync<TBL_TEACHER>()).FirstOrDefault();

                //if (check2 != null)
                //{
                //    await DisplayAlert("Error", check2.Object.TEACHER_EMAIL + "This Teacher is already Picked .", "ok");
                //    return;
                //}
                LoadingInd.IsRunning = true;
                int LastID, NewID = 1;

                var LastRecord = (await App.firebaseDatabase.Child("TBL_TEACHER_COURSE_ASSIGN").OnceAsync<TBL_TEACHER_COURSE_ASSIGN>()).FirstOrDefault();
                if (LastRecord != null)
                {
                    LastID = (await App.firebaseDatabase.Child("TBL_TEACHER_COURSE_ASSIGN").OnceAsync<TBL_TEACHER_COURSE_ASSIGN>()).Max(a => a.Object.TEACHER_COURSE_ASSIGN_ID);
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
                //TEACHER_FID
                List<TBL_TEACHER> te = (await App.firebaseDatabase.Child("TBL_TEACHER").OnceAsync<TBL_TEACHER>()).Select(x => new TBL_TEACHER
                {
                    //TEACHER_ID = x.Object.TEACHER_ID,
                    TEACHER_NAME = x.Object.TEACHER_NAME,

                }).ToList();
                int selected2 = te[ddlTeacher.SelectedIndex].TEACHER_ID;
                TBL_TEACHER_COURSE_ASSIGN tt = new TBL_TEACHER_COURSE_ASSIGN()
                {
                   TEACHER_COURSE_ASSIGN_ID = NewID,

                    CLASS_FID = selected,
                    TEACHER_FID = selected2,

                };
                await App.firebaseDatabase.Child("TBL_TEACHER_COURSE_ASSIGN").PostAsync(tt);
                LoadingInd.IsRunning = false;
                await DisplayAlert("Success", "Course Asign To Teacher", "Ok");

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