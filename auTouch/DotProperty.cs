using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auTouch
{
    class DotPorperty : INotifyPropertyChanged
    {
        public DotPorperty(string name, int count, int interval)
        {
            _name = name;
            _count = count;
            _interval = interval;
        }

        public DotPorperty()
        {
            _count = 0;
            _interval = 0;
        }

        private string _name;
        private int _count;
        private int _interval;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
                OnPropertyChanged("Count");
            }
        }

        public int Interval
        {
            get
            {
                return _interval;
            }
            set
            {
                _interval = value;
                OnPropertyChanged("Interval");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
