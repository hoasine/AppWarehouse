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
    public partial class AddCycleCountPage : ContentPage
    {
        AddCycleCountViewModel viewModel;

        public AddCycleCountPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new AddCycleCountViewModel(Navigation);
        }

        private async void TextChangedAlternately_Tapped(object sender, EventArgs e)
        {
            var search = sender as SearchBar;

            var viewmodel = BindingContext as AddCycleCountViewModel;

            viewmodel.SearchAlternatelyAsync(search.Text);

        }

        protected override void OnAppearing()
        {
            viewModel.LoadItemsCommand.Execute(null);

            base.OnAppearing();
        }
    }
}