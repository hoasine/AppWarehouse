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
    public partial class EditCycleCountPage : ContentPage
    {
        public EditCycleCountPage(StockCountModel model)
        {
            InitializeComponent();

            BindingContext = new EditCycleCountViewModel(Navigation, model);
        }
    }
}