using AppName.Model.Pickup;
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
    public partial class EditPickUPPage : ContentPage
    {
        public EditPickUPPage(PickUpProductMaster model)
        {
            InitializeComponent();

            BindingContext = new EditPickupViewModel(Navigation, model);
        }

      
    }
}