using ManagementSystemLibrary.SMS;
using ScenarioCreator.SCControl;
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

namespace ScenarioCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SCCondition condition = new SCCondition(SMSConditionType.LogicAnd);
            SCBond  bond = new SCBond { Input = new SCCondition(SMSConditionType.Start), Output = condition.Inputs[0] };
            this.ItemsControl_Scenario.ItemsSource = new ObservableCollection<object>(new object[]
            {
                bond,
                bond.Input,
                condition,

            });
        }

        private void Path_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Path path
                && ConditionControl.NewBond is null is bool newBond)
            {
                path.Stroke = newBond ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                path.StrokeThickness = newBond ? 4 : 2;
            }
        }

        private void Path_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Path path)
            {
                path.Stroke = new SolidColorBrush(Colors.Black);
                path.StrokeThickness = 2;
            }
        }

        private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.ItemsControl_Scenario.ItemsSource is ObservableCollection<object> list
                && (sender as FrameworkElement)?.DataContext is SCBond bond)
            {
                list.Remove(bond);
            }
        }
    }
}
