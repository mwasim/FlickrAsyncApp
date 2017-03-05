using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SearchInfo : BindableBase
    {
        private ObservableCollection<SearchItemResult> _list;
        public ObservableCollection<SearchItemResult> List => _list;

        public SearchInfo()
        {
            _list = new ObservableCollection<SearchItemResult>();
            _list.CollectionChanged += delegate
            {
                OnPropertyChanged(nameof(List));
            };
        }

        private string _searchTerm;

        public string SearchTerm
        {
            get { return _searchTerm; }
            set { SetProperty(ref _searchTerm, value); }
        }
    }
}
