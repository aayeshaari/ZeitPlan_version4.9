﻿using ZeitPlan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZeitPlan.Views.Admin;

using ZeitPlan.Views.Teacher;
using ZeitPlan.Views.Student;
using System.Xml.Linq;
using ZeitPlan.Helpers;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace ZeitPlan.LoginSystem
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public string type;
        public Login(string t)
        {

            type = t;
            InitializeComponent();



        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (type == "Teacher")
            {
                Title = "Teacher Login";
             
                App.db.CreateTable<TBL_TEACHER>();
                var teacher = App.db.Table<TBL_TEACHER>().FirstOrDefault();
                if (teacher != null)
                {
                    App.LoggedInUser = teacher;
                    App.Current.MainPage = new Views.Teacher.TeacherSideBar();
                    //Navigation.PushAsync(new TeacherSideBar());
                }
            }
            else if (type == "Student")
            {
                Title = "Student Login";
                App.db.CreateTable<TBL_STUDENT>();
                var student = App.db.Table<TBL_STUDENT>().FirstOrDefault();
                if (student != null)
                {
                    App.LoggedInStudent = student;
                    App.Current.MainPage = new Views.Student.StudentSideBar();
                }
            }
        }




        private async void Button_Clicked(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                await DisplayAlert("Error", "please fill all the required fields try again", "ok");
                return;
            }

            try
            {
                LoadingInd.IsRunning = true;

                //App.db.CreateTable<users>();
                //var check = App.db.Table<users>().FirstOrDefault(x => x.Email == txtEmail.Text && x.Password == txtPassword.Text);


                if (type == "Admin")
                {
                    var check = (await App.firebaseDatabase.Child("users").OnceAsync<users>()).FirstOrDefault(x => x.Object.Email == txtEmail.Text && x.Object.Password == txtPassword.Text);
                    if (check == null)
                    {
                        LoadingInd.IsRunning = false;
                        await DisplayAlert("Error", "Email or password incorrect", "ok");
                        return;
                    }
                    if(check.Object.Status== null)
                    {
                        await DisplayAlert("Error", "Your Request is pending Please wait until it get approved or contact admin.", "ok");
                        return;
                    }

                    if (check.Object.Status == "Pending")
                    {
                        await DisplayAlert("Error", "Your Request is pending Please wait until it get approved or contact admin.", "ok");
                        return;
                    }

                    if (check.Object.Status == "Blocked")
                    {
                        await DisplayAlert("Error", "Your acount is blocked Please wait until it get approved or contact admin.", "ok");
                        return;
                    }

                    App.LoggedInAdmin = check.Object;

                    App.Current.MainPage = new AdminSideBar();
                }
                if (type == "Teacher")
                {
                    var check = (await App.firebaseDatabase.Child("TBL_TEACHER").OnceAsync<TBL_TEACHER>()).FirstOrDefault(x => x.Object.TEACHER_EMAIL == txtEmail.Text && x.Object.TEACHER_PASSWORD == txtPassword.Text);
                    if (check == null)
                    {
                        LoadingInd.IsRunning = false;
                        await DisplayAlert("Error", "Email or password incorrect", "ok");
                        return;
                    }
                    if (check.Object.Status == null)
                    {
                        await DisplayAlert("Error", "Your Request is pending Please wait until it get approved or contact admin.", "ok");
                        return;
                    }


                    if (check.Object.Status == "Pending")
                    {
                        await DisplayAlert("Error", "Your Request is pending Please wait until it get approved or contact admin.", "ok");
                        return;
                    }

                    if (check.Object.Status == "Blocked")
                    {
                        await DisplayAlert("Error", "Your acount is blocked Please wait until it get approved or contact admin.", "ok");
                        return;
                    }

                    //Save Remember Info 

                    if (RememberMeCheck.IsChecked == true)
                    {
                        App.db.CreateTable<TBL_TEACHER>();
                        App.db.Insert(check.Object);
                    }

                    App.LoggedInUser = check.Object;


                    App.Current.MainPage = new TeacherSideBar();
                }
                if (type == "Student")
                {
                    var check = (await App.firebaseDatabase.Child("TBL_STUDENT").OnceAsync<TBL_STUDENT>()).FirstOrDefault(x => x.Object.STUDENT_EMAIL == txtEmail.Text && x.Object.STUDENT_PASSWORD == txtPassword.Text);
                    if (check == null)
                    {
                        LoadingInd.IsRunning = false;
                        await DisplayAlert("Error", "Email or password incorrect", "ok");
                        return;
                    }
                    if (check.Object.Status == null)
                    {
                        await DisplayAlert("Error", "Your Request is pending Please wait until it get approved or contact admin.", "ok");
                        return;
                    }


                    if (check.Object.Status == "Pending")
                    {
                        await DisplayAlert("Error", "Your Request is pending Please wait until it get approved or contact admin.", "ok");
                        return;
                    }

                    if (check.Object.Status == "Blocked")
                    {
                        await DisplayAlert("Error", "Your acount is blocked Please wait until it get approved or contact admin.", "ok");
                        return;
                    }

                    //Save Remember Info 
                    if (RememberMeCheck.IsChecked == true)
                    {
                        App.db.CreateTable<TBL_STUDENT>();
                        App.db.Insert(check.Object);
                    }

                    App.LoggedInStudent = check.Object;

                    App.Current.MainPage = new StudentSideBar();
                }

            }
            catch (Exception ex)
            {
                LoadingInd.IsRunning = false;
                await DisplayAlert("Error", "Something went wrong Please try again later.\nError:" + ex.Message, "ok");
                return;
            }


        }
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (type == "Teacher")
            {
                await Navigation.PushAsync(new Views.Teacher.Register());
            }
            if (type == "Student")
            {
                await Navigation.PushAsync(new Views.Student.Register());
            }
            if(type=="Admin")
            {
                await Navigation.PushAsync(new Views.Admin.Add_Admin());
            }

        }

        private void btnShowPassword_Clicked(object sender, EventArgs e)
        {
            if (txtPassword.IsPassword == true)
            {
                txtPassword.IsPassword = false;
                btnShowPassword.Source = "eyeshow.jpg";
            }
            else
            {
                txtPassword.IsPassword = true;
                btnShowPassword.Source = "eyenot.png";
            }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new MainPage();
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            btnLogin.IsVisible = false;

            frmPass.IsVisible = false;

            btnGetPass.IsVisible = true;
            btnreset.IsVisible = true;


        }

        private async void btnGetPass_Clicked(object sender, EventArgs e)
        {
            if(type=="Teacher")
            {
                var check = (await App.firebaseDatabase.Child("TBL_TEACHER").OnceAsync<TBL_TEACHER>()).FirstOrDefault(x => x.Object.TEACHER_EMAIL == txtEmail.Text);
                if (check == null)
                {
                    LoadingInd.IsRunning = false;
                    await DisplayAlert("Error", "Email not found", "ok");
                    return;
                }

                EmailHelper emailHelper = new EmailHelper();
                emailHelper.EmailSender(check.Object.TEACHER_EMAIL, "Forgot Password", "Your Password = " + check.Object.TEACHER_PASSWORD);
            }
            if(type=="Student")
            {
                var check = (await App.firebaseDatabase.Child("TBL_STUDENT").OnceAsync<TBL_STUDENT>()).FirstOrDefault(x => x.Object.STUDENT_EMAIL == txtEmail.Text);
                if(check==null)
                {
                    LoadingInd.IsRunning = false;
                    await DisplayAlert("Error", "Email not found", "ok");
                    return;
                }
                EmailHelper emailHelper = new EmailHelper();
                emailHelper.EmailSender(check.Object.STUDENT_EMAIL, "Forgot Password", "Your Password = " + check.Object.STUDENT_PASSWORD);
            }
            if(type=="Admin")
            {
                var check = (await App.firebaseDatabase.Child("users").OnceAsync<users>()).FirstOrDefault(x => x.Object.Email == txtEmail.Text);
                if(check==null)
                {
                    LoadingInd.IsRunning = false;
                    await DisplayAlert("Error", "Email not found", "ok");
                    return;
                }
                EmailHelper emailHelper = new EmailHelper();
                emailHelper.EmailSender(check.Object.Email, "Forgot Password", "Your Password=" + check.Object.Password);
            }
           
        }



        private void btnreset_Clicked(object sender, EventArgs e)
        {
            frmPass.IsVisible = true;
            btnLogin.IsVisible = true;
            btnGetPass.IsVisible = false;
            btnreset.IsVisible = false;
        }
    }
}