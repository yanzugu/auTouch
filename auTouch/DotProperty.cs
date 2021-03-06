using System.ComponentModel;

namespace auTouch
{
    public class DotPorperty : INotifyPropertyChanged
    {
        private string _name;
        private int _count;
        private int _min, _sec, _ms;
        private ClickEventType _eventType;

        public DotPorperty()
        {
            _count = 0;
            _min = 0;
            _sec = 1;
            _ms = 0;
            _eventType = ClickEventType.Left;
        }

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
                _count = value < 0 ? 0 : value;
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
                int total = (_min * 3600 * 1000) + (_sec * 1000) + _ms;
                if (total < 10)
                {
                    total = 10;
                    Ms = 10;
                }
                return total;
            }
        }

        public int a = 0;

        public ClickEventType EventType
        {
            get
            {
                return _eventType;
            }
            set
            {
                _eventType = value;
                OnPropertyChanged("EventType");
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
