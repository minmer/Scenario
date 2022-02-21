using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScenarioCreator.SCControl
{
    /// <summary>
    /// Interaction logic for ConditionControl.xaml
    /// </summary>
    public partial class ConditionControl : Border
    {
        private Point? conditionPosition;
        public static SCBond? NewBond { get; set; }

        public ConditionControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty FrameParentProperty = DependencyProperty.Register(name: "FrameParent",typeof(ItemsControl),typeof(ConditionControl));

        public ItemsControl? FrameParent
        {
            get => GetValue(FrameParentProperty) as ItemsControl;
            set => SetValue(FrameParentProperty, value);
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (this.DataContext is SCCondition condition
                    && conditionPosition is not null
                    && e.GetPosition(this) is Point point)
                {
                    Vector vector = point - this.conditionPosition.Value;
                    condition.X += vector.X;
                    condition.Y += vector.Y;
                }
                else if (NewBond?.Output is not null
                    && this.FrameParent is not null)
                {
                    NewBond.Output.P = e.GetPosition(this.FrameParent);
                }
                else
                {
                }
            }
        }

        private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.FrameParent is not null)
            {
                this.FrameParent.PreviewMouseMove += Canvas_PreviewMouseMove;
                this.FrameParent.PreviewMouseLeftButtonUp += FrameParent_PreviewMouseLeftButtonUp;
                this.conditionPosition = e.GetPosition(this);
            }
        }

        private void Button_Open_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button
                && this.DataContext is SCCondition condition)
            {
                condition.IsOpen ^= true;
                button.Content = condition.IsOpen ? "⮉" : "⮋";
                Grid.SetRow(this.ItemsControl_Inputs, condition.IsOpen ? 1 : 0);
            }
        }

        private void Ellipse_Input_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is Ellipse ellipse
                && ellipse?.DataContext is SCEntry entry
                && this.FrameParent is not null)
            {
                entry.P = ellipse.TranslatePoint(new Point(4, 4), this.FrameParent);
            }
        }

        private void Ellipse_Output_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is Ellipse ellipse
                && this.FrameParent is not null
                && this.DataContext is SCCondition condition)
            {
                condition.OutputP = ellipse.TranslatePoint(new Point(4, 4), this.FrameParent);
            }
        }

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Ellipse ellipse
                && this.FrameParent?.ItemsSource is ObservableCollection<object> list
                && this.DataContext is SCCondition condition)
            {
                NewBond = new SCBond { Input = condition, Output = new SCEntry { P = condition.OutputP } };
                list.Add(NewBond);
                condition.OutputP = ellipse.TranslatePoint(new Point(4, 4), this.FrameParent);
                this.FrameParent.PreviewMouseMove += Canvas_PreviewMouseMove;
                this.FrameParent.PreviewMouseLeftButtonUp += FrameParent_PreviewMouseLeftButtonUp;
            }
        }

        private void FrameParent_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.FrameParent is not null)
            {
                this.FrameParent.PreviewMouseMove -= Canvas_PreviewMouseMove;
                this.FrameParent.PreviewMouseLeftButtonUp -= FrameParent_PreviewMouseLeftButtonUp;
                this.conditionPosition = null;
                if (NewBond is not null
                    && this.FrameParent?.ItemsSource is ObservableCollection<object> list)
                {
                    if (Mouse.DirectlyOver is FrameworkElement element
                        && element?.DataContext is SCEntry entry)
                    {
                        NewBond.Output = entry;
                        NewBond = null;
                    }
                    else
                    {
                        list.Remove(NewBond);
                    }
                }
            }
        }
    }
}
