using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScenarioCreator.SCControl
{
    public class SCBond : INotifyPropertyChanged
    {
        private SCCondition? input;
        private SCEntry? output;

        public int ZIndex { get; } = 0;

        public event PropertyChangedEventHandler? PropertyChanged;

        public SCCondition? Input
        {
            get
            {
                return input;
            }

            set
            {
                input = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Input)));
            }
        }

        public SCEntry? Output
        {
            get
            {
                return output;
            }

            set
            {
                output = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Output)));
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            this.PropertyChanged?.Invoke(this, args);
        }
    }
}
