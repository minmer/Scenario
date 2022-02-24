using ManagementSystemLibrary;
using ManagementSystemLibrary.AMS;
using ManagementSystemLibrary.ManagementSystem;
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
using System.Windows.Shapes;
using System.Xml;

namespace ScenarioCreator
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly ObservableCollection<AMSDevice> devices = new ();

        public LoginWindow()
        {
            InitializeComponent();
            this.ListView_Devices.ItemsSource = devices;
            ReadDevicePreferences();
        }

        private void Button_AddAccount_Click(object sender, RoutedEventArgs e)
        {
            if (this.CheckBox_LinkAccount.IsChecked == true
                && long.TryParse(this.TextBox_DeviceAccount.Text, out long id))
            {
                _ = this.LinkAccountAsync(id);
            }
            else
            {
                _ = this.CreateAccountAsync();
            }
        }

        private void Button_AddAssociation_Click(object sender, RoutedEventArgs e)
        {
            if (this.CheckBox_LinkAssociation.IsChecked == true
                && long.TryParse(this.TextBox_Association.Text, out long id))
            {
                _ = this.LinkAssociationAsync(id);
            }
            else
            {
                _ = this.CreateAssociationAsync();
            }
        }

        private void Button_AddScenario_Click(object sender, RoutedEventArgs e)
        {
            if (this.CheckBox_LinkScenario.IsChecked == true
                && long.TryParse(this.TextBox_Scenario.Text, out long id))
            {
                _ = this.LinkScenarioAsync(id);
            }
            else
            {
                _ = this.CreateScenarioAsync();
            }
        }

        private void Button_ShowAddAccount_Click(object sender, RoutedEventArgs e)
        {
            DialogHost_Root.IsOpen = true;
            TabControl_Dialog.SelectedIndex = 0;
        }

        private void Button_ShowAddAssociation_Click(object sender, RoutedEventArgs e)
        {
            DialogHost_Root.IsOpen = true;
            TabControl_Dialog.SelectedIndex = 1;
        }

        private void Button_ShowAddScenario_Click(object sender, RoutedEventArgs e)
        {
            DialogHost_Root.IsOpen = true;
            TabControl_Dialog.SelectedIndex = 2;
        }

        private void Button_NewScenario_Click(object sender, RoutedEventArgs e)
        {
        }

        private async Task CreateAccountAsync()
        {
            if (await AMSAccount.CreateAsync(App.Pipeline, this.TextBox_DeviceAccount.Text).ConfigureAwait(true) is AMSAccount account)
            {
                if (await AMSDevice.CreateAsync(account, this.TextBox_DeviceName.Text).ConfigureAwait(true) is AMSDevice device)
                {
                    this.devices.Add(device);
                    this.SaveDevicePreferences();
                }
            }

            this.DialogHost_Root.IsOpen = false;
        }

        private async Task LinkAccountAsync(long id)
        {
            if (await AMSDevice.CreateAsync(App.Pipeline, id, this.TextBox_DeviceName.Text).ConfigureAwait(true) is AMSDevice device)
            {
                this.devices.Add(device);
                this.SaveDevicePreferences();
            }

            this.DialogHost_Root.IsOpen = false;
        }

        private async Task CreateAssociationAsync()
        {
            if (this.TreeView_Associations.SelectedValue is SCAssociation scAssociation
                && scAssociation?.Association is AMSAssociation association)
            {
                if (await AMSAssociation.CreateAsync(association, TextBox_Association.Text).ConfigureAwait(true) is AMSAssociation newAssociation)
                {
                    await AMSAssociate.CreateAsync(newAssociation, association, MSAccessType.Administrator);
                    await association.Account.DepositeNameAsync(newAssociation).ConfigureAwait(true);
                    await association.DepositeNameAsync(newAssociation).ConfigureAwait(true);
                }
            }

            this.DialogHost_Root.IsOpen = false;
        }

        private async Task LinkAssociationAsync(long id)
        {
            if (this.TreeView_Associations.SelectedValue is SCAssociation scAssociation
                && scAssociation?.Association is AMSAssociation association)
            {
                await AMSAssociate.CreateAsync(new AMSAssociation(association, id), association, MSAccessType.Contributor);
                await association.Account.DepositeNameAsync(new AMSAssociation(association, id)).ConfigureAwait(true);
                await association.DepositeNameAsync(new AMSAssociation(association, id)).ConfigureAwait(true);
            }

            this.DialogHost_Root.IsOpen = false;
        }

        private async Task CreateScenarioAsync()
        {
            if (this.TreeView_Associations.SelectedValue is SCAssociation scAssociation
                && scAssociation?.Association is AMSAssociation association)
            {
                if (await SMSScenario.CreateAsync(association, TextBox_Scenario.Text).ConfigureAwait(true) is SMSScenario scenario)
                {
                    await SMSContender.CreateAsync(scenario, association, MSAccessType.Administrator);
                }
            }

            this.DialogHost_Root.IsOpen = false;
        }

        private async Task LinkScenarioAsync(long id)
        {
            if (this.TreeView_Associations.SelectedValue is SCAssociation scAssociation
                && scAssociation?.Association is AMSAssociation association)
            {
                await SMSContender.CreateAsync(new SMSScenario(association, id), association, MSAccessType.Contributor);
            }

            this.DialogHost_Root.IsOpen = false;
        }

        private void ReadDevicePreferences()
        {
            try
            {
                using XmlReader xmlReader = XmlReader.Create("config.xml");
                xmlReader.ReadToFollowing("devices");
                if (int.TryParse(xmlReader.GetAttribute("count"), out int count))
                {
                    for (int index = 0; index < count; index++)
                    {
                        xmlReader.ReadToFollowing("device");
                        if (long.TryParse(xmlReader.GetAttribute("id"), out long id)
                            && Convert.FromBase64String(xmlReader.GetAttribute("key")) is byte[] key
                            && Convert.FromBase64String(xmlReader.GetAttribute("signature")) is byte[] signature)
                        {
                            this.devices.Add(new AMSDevice(App.Pipeline, id, key, signature));
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void SaveDevicePreferences()
        {
            using XmlWriter xmlWriter = XmlWriter.Create("config.xml");

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("devices");
            xmlWriter.WriteAttributeString("count", devices.Count.ToString());
            for (int index = 0; index < this.devices.Count; index++)
            {
                xmlWriter.WriteStartElement("device");
                xmlWriter.WriteAttributeString("id", devices[index].ID.ToString());
                xmlWriter.WriteAttributeString("key", Convert.ToBase64String(devices[index].PrivateKey.ExportRSAPrivateKey()));
                xmlWriter.WriteAttributeString("signature", Convert.ToBase64String(devices[index].PrivateSignature.ExportRSAPrivateKey()));
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
        }

        private void ListView_Devices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _ = this.DeviceSelectedAsync();
        }

        private async Task DeviceSelectedAsync()
        {
            if (this.ListView_Devices.SelectedValue is AMSDevice device
                && await device.VerifyAsync().ConfigureAwait(true) == true)
            {
                if (await device.GetAccountAsync().ConfigureAwait(true) is AMSAccount account)
                {
                    if (await account.GetMainAssociationAsync().ConfigureAwait(true) is AMSAssociation association)
                    {
                        this.TreeView_Associations.ItemsSource = new SCAssociation[] { new SCAssociation { Association = association } };
                    }
                }
            }
        }

        private void TreeView_Associations_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.TreeView_Associations.SelectedValue is SCAssociation association
                && association?.Association is not null)
            {
                _ = this.LoadScenariosAsync(association.Association);
            }
        }

        private async Task LoadScenariosAsync(AMSAssociation association)
        {
            this.ListView_Scenarios.ItemsSource = await (await association.LoadScenariosAsync().ConfigureAwait(true)).LoadChildrenAsync<SMSContender, AMSAssociation, SMSScenario>().ConfigureAwait(true);
        }

        private void ListView_Scenarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ListView_Scenarios.SelectedValue is SMSScenario scenario
                && scenario?.AccessType <= MSAccessType.Contributor)
            {
                new MainWindow(scenario).Show();
                this.Close();
            }
        }
    }
}
