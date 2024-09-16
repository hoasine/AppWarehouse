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
    public partial class AddStockTakePage : ContentPage
    {
        AddStockTakeViewModel viewModel;

        public AddStockTakePage()
        {
            InitializeComponent();

            BindingContext = viewModel = new AddStockTakeViewModel(Navigation);
        }

        private async void TextChangedAlternately_Tapped(object sender, EventArgs e)
        {
            var search = sender as SearchBar;

            var viewmodel = BindingContext as AddStockTakeViewModel;

            viewmodel.SearchAlternatelyAsync(search.Text);

        }

        protected override void OnAppearing()
        {
            viewModel.LoadItemsCommand.Execute(null);

            base.OnAppearing();
        }

    }
}