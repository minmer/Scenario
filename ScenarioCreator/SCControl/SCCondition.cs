using ManagementSystemLibrary.SMS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScenarioCreator.SCControl
{
    public class SCCondition : INotifyPropertyChanged
    {
        private static readonly Dictionary<SMSConditionType, string> typeNames = new()
        {
            { SMSConditionType.LogicAnd, Resources.LogicAnd },
        };
        private static readonly Dictionary<SMSConditionType, SCEntry[]> typeInputs = new()
        {
            {
                SMSConditionType.LogicAnd,
                new SCEntry[]
                {
                    new SCEntry
                    {
                        Description = Resources.LogicAnd,
                        Type = typeof(string),
                    },

                    new SCEntry
                    {
                        Description = Resources.LogicAnd,
                        Type = typeof(string),
                    },

                    new SCEntry
                    {
                        Description = Resources.LogicAnd,
                        Type = typeof(string),
                    },

                    new SCEntry
                    {
                        Description = Resources.LogicAnd,
                        Type = typeof(string),
                    },
                }
            },
        };

        private double x;
        private double y;
        private double size = 8;
        private Point outputP;
        private bool isOpen = true;
        private SCEntry[] inputs;

        public SCCondition(SMSConditionType type)
        {
            this.Type = type;
            typeInputs.TryGetValue(this.Type, out SCEntry[]? inputs);
            this.inputs = inputs?.Select(input => new SCEntry { Parent = this, Description = input.Description, Type = input.Type }).ToArray() ?? Array.Empty<SCEntry>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsOpen
        {
            get
            {
                return isOpen;
            }

            set
            {
                isOpen = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.IsOpen)));
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Size)));
            }
        }

        public string Name
        {
            get
            {
                typeNames.TryGetValue(this.Type, out string? name);
                return name ?? this.Type.ToString();
            }
        }

        public int ZIndex { get; } = 1;

        public SCEntry[] Inputs
        {
            get
            {
                return this.inputs;
            }
        }

        public SMSConditionType Type { get; set; }

        public double X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.X)));
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.P)));
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Size)));
            }
        }

        public double Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Y)));
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.P)));
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Size)));
            }
        }

        public Point P
        {
            get
            {
                return new Point(this.x, this.y);
            }
        }

        public Point OutputP
        {
            get
            {
                return outputP;
            }

            set
            {
                outputP = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.OutputP)));
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.OutputPInit)));
            }
        }

        public Point OutputPInit
        {
            get
            {
                return new Point(outputP.X +50, outputP.Y);
            }
        }

        public double Size
        {
            get
            {
                return size + Random.Shared.NextDouble() / 1000;
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            this.PropertyChanged?.Invoke(this, args);
        }
    }
}
