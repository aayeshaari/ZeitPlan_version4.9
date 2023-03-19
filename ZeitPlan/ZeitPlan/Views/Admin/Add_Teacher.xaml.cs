using Firebase.Database.Query;
using Plugin.Media;
using Plugin.Media.Abstractions;
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
    public partial class Add_Teacher : ContentPage
    {
        private MediaFile _mediaFile;
        public static string PicPath = "image_picker";
        public Add_Teacher()
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
            var firebaseList = (await App.firebaseDatabase.Child("TBL_DEPARTMENT").OnceAsync<TBL_DEPARTMENT>()).Select(x => new TBL_DEPARTMENT
            {

                DEPARTMENT_NAME = x.Object.DEPARTMENT_NAME,

            }).ToList();
            var refinedList = firebaseList.Select(x => x.DEPARTMENT_NAME).ToList();
            ddlDepartment.ItemsSource = refinedList;
            //TEACHER_COURSE_ASSIGN_FID
        }
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            try
            {
                var response = await DisplayActionSheet("Select image source", "Close", "", "From Galary", "From Camera");
                if (response == "From Camera")
                {
                    await CrossMedia.Current.Initialize();
                    if (!CrossMedia.Current.IsTakePhotoSupported)
                    {
                        await DisplayAlert("Error", "Phone is not take photo supported ", "ok");
                        return;
                    }
                    var mediaoptions = new StoreCameraMediaOptions()
                    {
                        PhotoSize = PhotoSize.Medium
                    };
                    var SelectedImg = await CrossMedia.Current.TakePhotoAsync(mediaoptions);
                    if (SelectedImg == null)
                    {
                        await DisplayAlert("Error", "Error taking img.... ", "ok");
                        return;
                    }
                    _mediaFile = SelectedImg;
                    PicPath = SelectedImg.Path;
                    Previewpic.Source = SelectedImg.Path;
                }
                if (response == "From Galary")
                {
                    await CrossMedia.Current.Initialize();
                    if (!CrossMedia.Current.IsPickPhotoSupported)
                    {
                        await DisplayAlert("Error", "Phone is not Pick Photo Supported", "OK");
                        return;
                    }
                    var mediaOptions = new PickMediaOptions()
                    {
                        PhotoSize = PhotoSize.Medium
                    };

                    var SelectedImg = await CrossMedia.Current.PickPhotoAsync(mediaOptions);

                    if (SelectedImg == null)
                    {
                        await DisplayAlert("Error", "Error Picking Image...", "OK");
                        return;
                    }

                    _mediaFile = SelectedImg;
                    PicPath = SelectedImg.Path;
                    Previewpic.Source = SelectedImg.Path;

                }

            }
            catch (Exception ex)
            {

                await DisplayAlert("Message", "Something went wrong\n" + ex.Message, "ok");
            }

        }

        private async void btncat_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtTechName.Text) || string.IsNullOrEmpty(txtTechEmail.Text) || string.IsNullOrEmpty(txtTechPassword.Text) || string.IsNullOrEmpty(txtTechPhone.Text) || string.IsNullOrEmpty(txtTechAdress.Text))
                {
                    await DisplayAlert("Error", "please fill all the required fields try again", "ok");
                    return;
                }
                if (ddlDepartment.SelectedItem == null)
                {
                    await DisplayAlert("Error", "please select Degree and try again", "ok");
                    return;
                }

                //var check = (await App.firebaseDatabase.Child("TBL_TEACHER").OnceAsync<TBL_TEACHER>()).FirstOrDefault(x => x.Object.TEACHER_EMAIL== txtTechEmail.Text);

                //if (check != null)
                //{
                //    await DisplayAlert("Error", check.Object.TEACHER_EMAIL + "This TEACHER is already Added .", "ok");
                //    return;
                //}
                LoadingInd.IsRunning = true;
                int LastID, NewID = 1;

                var LastRecord = (await App.firebaseDatabase.Child("TBL_TEACHER").OnceAsync<TBL_TEACHER>()).FirstOrDefault();
                if (LastRecord != null)
                {
                    LastID = (await App.firebaseDatabase.Child("TBL_TEACHER").OnceAsync<TBL_TEACHER>()).Max(a => a.Object.TEACHER_ID);
                    NewID = ++LastID;
                }
                var Department = (await App.firebaseDatabase.Child("TBL_DEPARTMENT").OnceAsync<TBL_DEPARTMENT>()).FirstOrDefault(x => x.Object.DEPARTMENT_NAME == ddlDepartment.SelectedItem.ToString());
                //List<TBL_DEPARTMENT> degs = (await App.firebaseDatabase.Child("TBL_DEPARTMENT").OnceAsync<TBL_DEPARTMENT>()).Select(x => new TBL_DEPARTMENT
                //{
                //    DEPARTMENT_ID = x.Object.DEPARTMENT_ID,
                //    DEPARTMENT_NAME = x.Object.DEPARTMENT_NAME

                //}).ToList();
                //int selected = degs[ddlDepartment.SelectedIndex].DEPARTMENT_ID;
                //TEACHER_COURSE_ASSIGN
                //List<TBL_TEACHER_COURSE_ASSIGN> css = (await App.firebaseDatabase.Child("TBL_TEACHER_COURSE_ASSIGN").OnceAsync<TBL_TEACHER_COURSE_ASSIGN>()).Select(x => new TBL_TEACHER_COURSE_ASSIGN
                //{
                //    TEACHER_COURSE_ASSIGN_ID = x.Object.TEACHER_COURSE_ASSIGN_ID,


                //}).ToList();
                //int selected1 = css[ddltechassign.SelectedIndex].TEACHER_COURSE_ASSIGN_ID;

                var StoredImageURL = await App.FirebaseStorage
               .Child("TechImages")
               .Child(txtTechName.Text + ".jpg")
               .PutAsync(_mediaFile.GetStream());


                TBL_TEACHER t = new TBL_TEACHER()
                {
                    TEACHER_ID = NewID,
                    TEACHER_NAME = txtTechName.Text,
                    TEACHER_EMAIL = txtTechEmail.Text,
                    TEACHER_ADDRESS=txtTechAdress.Text,
                    TEACHER_PASSWORD= txtTechPassword.Text,
                    TEACHER_PHNO=txtTechPhone.Text,
                    Image = StoredImageURL,
                    DEPARTMENT_FID = Department.Object.DEPARTMENT_ID,
                    Status = "Pending"
                    //TEACHER_COURSE_ASSIGN_FID=selected1,
                };

                await App.firebaseDatabase.Child("TBL_TEACHER").PostAsync(t);
                LoadingInd.IsRunning = false;
                await DisplayAlert("Success", "Teacher Added  ", "Ok");

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
