using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppName
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaoPhieuNhapPage : ContentPage
    {
        public TaoPhieuNhapPage()
        {
            InitializeComponent();

            BindingContext = new TaoPhieuNhapViewModel(Navigation, this);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}