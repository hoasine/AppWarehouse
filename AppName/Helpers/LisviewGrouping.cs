using Xamarin.Forms;
using AppName.Core;
using Realms;
using AppName.Model;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using Prism.Behaviors;
using System.Collections.ObjectModel;

namespace AppName
{
    public class PartnersGrouping<K, T> : ObservableCollection<T>
    {
        public K Key { get; private set; }

        public PartnersGrouping(K key, IEnumerable<T> items)
        {
            Key = key;
            foreach (var item in items)
            {
                this.Items.Add(item);
            }
        }
    }
}