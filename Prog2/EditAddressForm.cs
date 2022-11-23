// Program 3 (from provided Program 2 solution)
// CIS 200-50
// Fall 2021
// Due: 11/15/2021
// By: 5129153

// Allows users to edit previously submitted addresses

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UPVApp
{
    public partial class EditAddressForm : Form
    {
        private List<Address> addressList; // list of addresses

        // Precondition: none
        // Postcondition: display GUI
        public EditAddressForm(List<Address> addresses)
        {
            InitializeComponent();

            addressList = addresses;

            // loop to add addresses to combo box
            foreach (Address a in addressList)
            {
                addressListComboBox.Items.Add(a.Name);
            }
        }

        public int AddressSelection
        {
            // precondition: none
            // postcondition: address index returned
            get
            {
                return addressListComboBox.SelectedIndex;
            }
            // precondition: value must be valid and not greater than #
            // of saved addresses, otherwise show error
            // postcondition: index value set
            set
            {
                if (value >= 0 && value < addressList.Count)
                {
                    addressListComboBox.SelectedIndex = value;
                }
                else
                {
                    errorProvider1.SetError(addressListComboBox, "Index must be valid");
                }

            }
        }

        // precondition: changing focus from address list combo box
        // postcondition: shows error unless address selected from list
        private void addressListComboBox_Validating(object sender, CancelEventArgs e)
        {
            if (addressListComboBox.SelectedIndex == -1)
            {
                errorProvider1.SetError(addressListComboBox, "Select an address");
            }
            else
            {
                errorProvider1.SetError(addressListComboBox, ""); // clear error if valid
            }
        }

        // precondition: user clicks OK button
        // postcondition: accepts form after validation
        private void okButton_Click(object sender, EventArgs e)
        {
            if (this.ValidateChildren())
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        // precondition: user presses Cancel
        // postcondition: cancels edit address form
        private void cancelButton_Click(object sender, EventArgs e)
        {
                this.DialogResult = DialogResult.Cancel;
        }
    }
}
