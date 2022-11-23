// Program 3 (from provided Program 2 solution)
// CIS 200-50
// Fall 2021
// Due: 11/15/2021
// By: 5129153

// File: Prog2Form.cs
// This class creates the main GUI for Program 2. It provides a
// File menu with About and Exit items, an Insert menu with Address and
// Letter items, and a Report menu with List Addresses and List Parcels
// items.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace UPVApp
{
    public partial class Prog2Form : Form
    {
        private UserParcelView upv; // The UserParcelView

        // Precondition:  None
        // Postcondition: The form's GUI is prepared for display. A few test addresses are
        //                added to the list of addresses
        public Prog2Form()
        {
            InitializeComponent();

            upv = new UserParcelView();

            // Test Data - removed and saved to program 3 data file
        }

        // Precondition:  File, About menu item activated
        // Postcondition: Information about author displayed in dialog box
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Grading ID: 5129153 \n" +
               "CIS 200-50 \n" +
               "Fall 2021 \n" +
               "Due: 11/15/2021");
        }

        // Precondition:  File, Exit menu item activated
        // Postcondition: The application is exited
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Precondition:  Insert, Address menu item activated
        // Postcondition: The Address dialog box is displayed. If data entered
        //                are OK, an Address is created and added to the list
        //                of addresses
        private void addressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddressForm addressForm = new AddressForm();    // The address dialog box form
            DialogResult result = addressForm.ShowDialog(); // Show form as dialog and store result
            int zip; // Address zip code

            if (result == DialogResult.OK) // Only add if OK
            {
                if (int.TryParse(addressForm.ZipText, out zip))
                {
                    upv.AddAddress(addressForm.AddressName, addressForm.Address1,
                        addressForm.Address2, addressForm.City, addressForm.State,
                        zip); // Use form's properties to create address
                }
                else // This should never happen if form validation works!
                {
                    MessageBox.Show("Problem with Address Validation!", "Validation Error");
                }
            }

            addressForm.Dispose(); // Best practice for dialog boxes
                                   // Alternatively, use with using clause as in Ch. 17
        }

        // Precondition:  Report, List Addresses menu item activated
        // Postcondition: The list of addresses is displayed in the addressResultsTxt
        //                text box
        private void listAddressesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder(); // Holds text as report being built
                                                        // StringBuilder more efficient than String
            string NL = Environment.NewLine;            // Newline shorthand

            result.Append("Addresses:");
            result.Append(NL); // Remember, \n doesn't always work in GUIs
            result.Append(NL);

            foreach (Address a in upv.AddressList)
            {
                result.Append(a.ToString());
                result.Append(NL);
                result.Append("------------------------------");
                result.Append(NL);
            }

            reportTxt.Text = result.ToString();

            // -- OR --
            // Not using StringBuilder, just use TextBox directly

            //reportTxt.Clear();
            //reportTxt.AppendText("Addresses:");
            //reportTxt.AppendText(NL); // Remember, \n doesn't always work in GUIs
            //reportTxt.AppendText(NL);

            //foreach (Address a in upv.AddressList)
            //{
            //    reportTxt.AppendText(a.ToString());
            //    reportTxt.AppendText(NL);
            //    reportTxt.AppendText("------------------------------");
            //    reportTxt.AppendText(NL);
            //}

            // Put cursor at start of report
            reportTxt.Focus();
            reportTxt.SelectionStart = 0;
            reportTxt.SelectionLength = 0;
        }

        // Precondition:  Insert, Letter menu item activated
        // Postcondition: The Letter dialog box is displayed. If data entered
        //                are OK, a Letter is created and added to the list
        //                of parcels
        private void letterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LetterForm letterForm; // The letter dialog box form
            DialogResult result;   // The result of showing form as dialog
            decimal fixedCost;     // The letter's cost

            if (upv.AddressCount < LetterForm.MIN_ADDRESSES) // Make sure we have enough addresses
            {
                MessageBox.Show("Need " + LetterForm.MIN_ADDRESSES + " addresses to create letter!",
                    "Addresses Error");
                return; // Exit now since can't create valid letter
            }

            letterForm = new LetterForm(upv.AddressList); // Send list of addresses
            result = letterForm.ShowDialog();

            if (result == DialogResult.OK) // Only add if OK
            {
                if (decimal.TryParse(letterForm.FixedCostText, out fixedCost))
                {
                    // For this to work, LetterForm's combo boxes need to be in same
                    // order as upv's AddressList
                    upv.AddLetter(upv.AddressAt(letterForm.OriginAddressIndex),
                        upv.AddressAt(letterForm.DestinationAddressIndex),
                        fixedCost); // Letter to be inserted
                }
               else // This should never happen if form validation works!
                {
                    MessageBox.Show("Problem with Letter Validation!", "Validation Error");
                }
            }

            letterForm.Dispose(); // Best practice for dialog boxes
                                  // Alternatively, use with using clause as in Ch. 17
        }

        // Precondition:  Report, List Parcels menu item activated
        // Postcondition: The list of parcels is displayed in the parcelResultsTxt
        //                text box
        private void listParcelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // This report is generated without using a StringBuilder, just to show an
            // alternative approach more like what most students will have done
            // Method AppendText is equivalent to using .Text +=

            decimal totalCost = 0;                      // Running total of parcel shipping costs
            string NL = Environment.NewLine;            // Newline shorthand

            reportTxt.Clear(); // Clear the textbox
            reportTxt.AppendText("Parcels:");
            reportTxt.AppendText(NL); // Remember, \n doesn't always work in GUIs
            reportTxt.AppendText(NL);

            foreach (Parcel p in upv.ParcelList)
            {
                reportTxt.AppendText(p.ToString());
                reportTxt.AppendText(NL);
                reportTxt.AppendText("------------------------------");
                reportTxt.AppendText(NL);
                totalCost += p.CalcCost();
            }

            reportTxt.AppendText(NL);
            reportTxt.AppendText($"Total Cost: {totalCost:C}");

            // Put cursor at start of report
            reportTxt.Focus();
            reportTxt.SelectionStart = 0;
            reportTxt.SelectionLength = 0;
        }

        // Precondition:  File, Save As menu item activated
        // Postcondition: Data is saved using object serialization with binary formatting
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BinaryFormatter formatter = new BinaryFormatter(); // object to serialize in binary format

            FileStream output;  // stream for writing to file
            DialogResult result; // result for dialog box

            string fileName;  // file name for saving

            using (SaveFileDialog fileChooser = new SaveFileDialog())
            {
                fileChooser.CheckFileExists = false; // allows to create file

                result = fileChooser.ShowDialog(); // get dialog box output
                fileName = fileChooser.FileName; // get file name
            }

            
            if (result == DialogResult.OK) // ensure user clicked OK
            {
                // message box if file name is invalid or empty
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Invalid File name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // use file stream to save a valid file name
                else
                {
                    try
                    {
                        // open file with write access
                        output = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);

                        // serialize upv object
                        formatter.Serialize(output, upv);
                    }
                    catch (IOException)
                    {
                        // message box if file could not be saved
                        MessageBox.Show("Error writing file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            
        }

        // Precondition:  File, Open menu item activated
        // Postcondition: Data is opened using deserialization
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BinaryFormatter reader = new BinaryFormatter(); // object to deserialize in binary format

            FileStream input;  // stream for reading file
            DialogResult result; // result for dialog box

            string fileName;  // file name for saving

            using (OpenFileDialog fileChooser = new OpenFileDialog())
            {
                result = fileChooser.ShowDialog(); // get dialog box output
                fileName = fileChooser.FileName; // get file name
            }

            if (result == DialogResult.OK) // ensure user clicked OK
            {
                // message box if file name is invalid or empty
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Invalid File name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // use file stream for read file
            else
            {
                try
                {
                    // deserialize and store data entered
                    input = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                   UserParcelView upv = (UserParcelView) reader.Deserialize(input);
                }

                catch (IOException)
                {
                    // message box if file could not be opened
                    MessageBox.Show("Error reading file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        // precondition: Edit, Address menu item selected
        // postcondition: Selected address is edited
        private void addressToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EditAddressForm editAddressForm = new EditAddressForm(upv.AddressList); // address edit form
            DialogResult result = editAddressForm.ShowDialog(); // show form dialog
            int addressIndex;

            if (result == DialogResult.OK)
            {
                addressIndex = editAddressForm.AddressSelection; // selected edit index

                if (addressIndex >= 0)
                {
                    Address editAddress = upv.AddressAt(addressIndex); // selected address to edit

                    AddressForm addressForm = new AddressForm(); // open address dialog box

                    // show prev. entered form values
                    addressForm.AddressName = editAddress.Name;
                    addressForm.Address1 = editAddress.Address1;
                    addressForm.Address2 = editAddress.Address2;
                    addressForm.City = editAddress.City;
                    addressForm.State = editAddress.State;
                    addressForm.ZipText = $"{editAddress.Zip}";

                    result = addressForm.ShowDialog(); // show form dialog

                    // edit form values
                    editAddress.Name = addressForm.AddressName;
                    editAddress.Address1 = addressForm.Address1;
                    editAddress.Address2 = addressForm.Address2;
                    editAddress.City = addressForm.City;
                    editAddress.State = addressForm.State;
                    addressForm.ZipText = addressForm.ZipText;

                }
            }
        }
    }
}