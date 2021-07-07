using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auTouch
{
    public class DotPorperty : INotifyPropertyChanged
    {
        public DotPorperty()
        {
            _count = -1;
            _min = 0;
            _sec = 1;
            _ms = 0;
        }

        private string _name;
        private int _count;
        private int _min, _sec, _ms;

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
                _count = value < -1 ? -1 : value;
                OnPropertyChanged("Count");
            }
        }

        public int Min
        {
            get
            {
                return _min;
            }
            set
            {
                if (value < 0)
                    _min = 0;
                else
                    _min = value;
                OnPropertyChanged("Min");
            }
        }

        public int Sec
        {
            get
            {
                return _sec;
            }
            set
            {
                if (value < 0)
                    _sec = 0;
                else
                    _sec = value > 59 ? 59 : value;
                OnPropertyChanged("Sec");
            }
        }

        public int Ms
        {
            get
            {
                return _ms;
            }
            set
            {
                if (value < 0)
                    _ms = 0;
                else
                    _ms = value > 999 ? 999 : value;
                OnPropertyChanged("Ms");
            }
        }

        public int Interval
        {
            get
            {
                return (_min * 3600 * 1000) + (_sec * 1000) + _ms;
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
