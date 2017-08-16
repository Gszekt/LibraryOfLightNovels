using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace 轻小说文库 {
	public class BookItem : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		//public ObservableCollection<BookIndex> Indexes { get; set; }

		private string title;
		private string author;
		private string classfication;
		private string state;
		private string coverUri;
		private string lastUpdate;
		private string interLinkage;
		private string summary;
		private string readLinkage;

		public string Title {
			get { return title; }
			set { SetProperty(ref title, value); }
		}

		public string Author {
			get { return author; }
			set { SetProperty(ref author, value); }
		}

		public string Classification {
			get { return classfication; }
			set { SetProperty(ref classfication, value); }
		}

		public string State {
			get { return state; }
			set { SetProperty(ref state, value); }
		}

		public string CoverUri {
			get { return coverUri; }
			set { SetProperty(ref coverUri, value); }
		}

		public string LastUpdate {
			get { return lastUpdate; }
			set { SetProperty(ref lastUpdate, value); }
		}

		public string Interlinkage {
			get { return interLinkage; }
			set { SetProperty(ref interLinkage, value); }
		}

		public string Summary {
			get { return summary; }
			set { SetProperty(ref summary, value); }
		}

		public string ReadLinkage {
			get { return readLinkage; }
			set { SetProperty(ref readLinkage, value); }
		}

		protected bool SetProperty<T>(ref T storage,T value,[CallerMemberName] string propertyName = null) {
			if (Equals(storage,value)) {
				return false;
			}
			storage = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		protected void OnPropertyChanged(string propertyName) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
