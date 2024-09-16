using Xamarin.Forms;
using AppName.Core;
using System;

namespace AppName
{
    public partial class FunctionPage : ContentView
    {
        public FunctionPage()
        {
            InitializeComponent();

            BindingContext = new MenuKiemKeViewModel(Navigation);
        }
    }
}
