using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ScenarioCreator.SCControl
{
    public class SCTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? BondTemplate { get; set; }
        public DataTemplate? ConditionTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement element
                && element?.DataContext is SCCondition)
            {
                return this.ConditionTemplate;
            }

            return this.BondTemplate;
        }
    }
}
