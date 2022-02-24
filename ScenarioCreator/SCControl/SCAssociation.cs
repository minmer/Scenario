using ManagementSystemLibrary;
using ManagementSystemLibrary.AMS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioCreator.SCControl
{
    public class SCAssociation : INotifyPropertyChanged
    {
        private AMSAssociation? association;
        private SCAssociation[]? children;

        public AMSAssociation? Association
        {
            get
            {
                return association;
            }

            set
            {
                association = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Association)));
            }
        }

        public SCAssociation[]? Children
        {
            get
            {
                _ = LoadChildrenAsync();
                return children;
            }
        }

        private async Task<SCAssociation[]?> LoadChildrenAsync()
        {
            if (this.association is not null
                && this.children is null)
            {
                this.children = (await (await this.association.LoadAssociationsAsync().ConfigureAwait(true)).LoadChildrenAsync<AMSAssociate, AMSAssociation, AMSAssociation>().ConfigureAwait(true)).Select(association => new SCAssociation { Association = association }).ToArray();
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Children)));
            }

            return this.children;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            this.PropertyChanged?.Invoke(this, args);
        }
    }
}
