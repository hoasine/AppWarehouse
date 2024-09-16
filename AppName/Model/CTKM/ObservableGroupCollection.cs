using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AppName.ViewModels.BarCode.Model
{
    public class ObservableGroupCollection<S, T> : ObservableCollection<T>
    {
        private readonly S _key;
        public ObservableGroupCollection(System.Linq.IGrouping<S, T> group)
            : base(group)
        {
            _key = group.Key;
        }
        public S Key
        {
            get { return _key; }
        }
    }
}
