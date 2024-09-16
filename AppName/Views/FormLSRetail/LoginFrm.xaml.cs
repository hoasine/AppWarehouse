using System;
using System.Collections.Generic;

using Xamarin.Forms;
using AppName.Core;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using Rg.Plugins.Popup.Services;
using AppName.CustomRenderer;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using AppName.Model;
using System.Linq;
using Plugin.Connectivity;

namespace AppName
{
    public partial class LoginFrm : ContentPage
    {
        LoginViewModel viewModel;

        public LoginFrm()
        {
            InitializeComponent();

            BindingContext = viewModel = new LoginViewModel(Navigation);

            var cookie = Application.Current.Properties.Count;

            if (cookie > 0)
            {
                //var username = Application.Current.Properties["UserName"].ToString();
                //if (!string.IsNullOrEmpty(username))
                //{
                //    txtUserName.Text = username;
                //}
                //else
                //{
                //    txtPassword.Text = "";
                //    txtUserName.Text = "";
                //}
            }
        }

        protected override void OnAppearing()
        {
            viewModel.LoadItemsCommand.Execute(null);

            Application.Current.Properties["ISCheckVPN"] = true;

            base.OnAppearing();
        }
    }
}
