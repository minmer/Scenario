using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScenarioCreator.SCControl
{
    public class SCEntry : INotifyPropertyChanged
    {
        private Point p;
        private SCCondition? parent;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Type? Type { get; set; }

        public string? Description { get; set; }

        public SCCondition? Parent { get
            {
                return this.parent;
            }
            set
            {
                this.parent = value;
            }
        }

        public object? Value { get; set; }

        public Point P
        {
            get
            {
                return p;
            }

            set
            {
                p = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.P)));
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.PInit)));
            }
        }

        public Point PInit
        {
            get
            {
                return new Point(p.X - 50, p.Y);
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            this.PropertyChanged?.Invoke(this, args);
        }
    }
}
