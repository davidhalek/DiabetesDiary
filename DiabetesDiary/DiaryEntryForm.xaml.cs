using DiabetesDiary.Database;
using System;
using System.Collections.Generic;
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

namespace DiabetesDiary
{
    /// <summary>
    /// Interaction logic for DiaryEntryForm.xaml
    /// </summary>

    public partial class DiaryEntryForm : UserControl
    {
        public int EntryType = 1;
        public int DateSelected = 0;
        public string DateString = "";
        public static event EventHandler EntryDateUpdated;
        public MainWindow mainWindow;

        // Constructor
        public DiaryEntryForm()
        {
            InitializeComponent();

            // Bind entry type combobox
            Db.BindEntryTypeBox(ComboBoxEntryType);
            //ComboBoxEntryType.SelectedIndex = 0;

            // Event handler from entry form
            this.Loaded += (s, e) =>
            {
                MainWindow parentWindow = Window.GetWindow(this) as MainWindow;
                if (parentWindow != null)
                {
                    mainWindow = parentWindow;
                }
            };
        }

        /// <summary>
        /// Whenever the combo box is closed it updates the value
        /// and populates entry form with it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxEntryType_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                EntryType = Int32.Parse(ComboBoxEntryType.SelectedValue.ToString());
            }
            catch (Exception ex)
            {
                return;
            }

            if (DateSelected > 0)
            {
                UpdateForm(sender, e);
            }
        }

        /// <summary>
        /// Event handler for date losing focus.
        /// If it's empty it populates it with today's date
        /// It also populates it with today's date if it's 'today'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDate_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtDate.Text))
            {
                //MessageBox.Show("Date box is empty");
                txtDate.Text = "today";
            }

            DateSelected = Misc.DateToInt(txtDate.Text);
            txtDate.Text = Misc.DateToString(DateSelected);
            UpdateForm(sender, e);
        }

        /// <summary>
        /// Clears the entry form when 'clear' button hit
        /// </summary>
        private void ClearFields()
        {
            if (String.IsNullOrEmpty(txtDate.Text))
            {
                //MessageBox.Show("Date box is empty");
                txtDate.Text = "today";
            }

            ComboBoxEntryType.SelectedIndex = -1;
            EntryType = -1;

            DateSelected = Misc.DateToInt(txtDate.Text);
            txtDate.Text = Misc.DateToString(DateSelected);

            txtTime.Clear();
            txtGlucose.Clear();
            txtBaseDosage.Clear();
            txtGlucoseAdjustment.Clear();
            txtTotalDose.Clear();
            txtSlowDose.Clear();
            txtCarbs.Clear();
            txtFoodEaten.Document.Blocks.Clear();
            txtNotes.Document.Blocks.Clear();
        }

        /// <summary>
        /// Called after entry type or date changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateForm(object sender, EventArgs e)
        {
            UpdateForm();
            if (EntryDateUpdated != null)
            {
                EntryDateUpdated(this, e);
            }

        }

        /// <summary>
        /// Validate form - entry type and date 
        /// </summary>
        private void UpdateForm()
        {
            if (DateSelected == 0)
            {
                MessageBox.Show("Please select a date.");
                return;
            }
            else if (EntryType == 0)
            {
                MessageBox.Show("Please select an entry type.");
                return;
            }

            //else
            //{
            //    MessageBox.Show("UpdateRecord should update the fields");
            //}
        }

        /// <summary>
        /// When an entry is selected via the datagrid populate the form
        /// </summary>
        /// <param name="date"></param>
        /// <param name="type"></param>
        private void UpdateFromEntry(string date, int type)
        {
            ClearFields();
            ComboBoxEntryType.SelectedIndex = type - 1;
            DateSelected = Misc.DateToInt(date);
            EntryType = type;
            //MessageBox.Show("UpdateFromEntry called: "
            //    +"\nDateSelected = "+DateSelected + "\nEntryType: "+EntryType);
            Entry entry = Db.GetEntry(DateSelected, type);
            txtDate.Text = Misc.DateToString(entry.Date);
            txtTime.Text = entry.Time.ToString();
            txtGlucose.Text = entry.Glucose.ToString();
            txtBaseDosage.Text = entry.BaseDose.ToString();
            txtGlucoseAdjustment.Text = entry.GlucoseAdj.ToString();
            txtTotalDose.Text = entry.FastActingDose.ToString();
            txtSlowDose.Text = entry.SlowActingDose.ToString();
            txtCarbs.Text = entry.CarbGrams.ToString();
            //txtFoodEaten.Text = entry.Time.ToString();
            //txtNotes.Document.Blocks.Clear();

        }

        /// <summary>
        /// Helper method to make sure number supplied by form is valid
        /// </summary>
        /// <param name="str">the number in string form</param>
        /// <returns>number in int form</returns>
        private int ValidNumber(string str)
        {
            try
            {
                return Int32.Parse(str);    
            }
            catch(Exception e)
            {
                return -1;
            }
        }

        /// <summary>
        /// Get contents of a rich text box control
        /// </summary>
        /// <param name="rtb">rich text box to use</param>
        /// <returns>contents in string form</returns>
        private string GetRichText(RichTextBox rtb)
        {
            TextRange tr = new TextRange(
                rtb.Document.ContentStart, rtb.Document.ContentEnd);
            return tr.Text;
        }

        /// <summary>
        /// Save entry button
        /// Validate and save entry, force update on history
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveEntry(object sender, RoutedEventArgs e) 
        {
            Entry ob = new Entry();
            string msg = "";

            ob.Date = Misc.ValidDate(txtDate.Text);
            ob.EntryType = EntryType;
            ob.Time = ValidNumber(txtTime.Text);

            // Validate data
            if (ob.Date <= 0)
            {
                msg = "Invalid date.";
            }
            else if (EntryType < 1 || EntryType > 4)
            {
                msg = "Select an entry type.";
            }
            else if (ob.Time < 0 || ob.Time > 2400)
            {
                msg = "Time must be between 0 and 2400 (military time).";
            }

            if (msg.Length > 0)
            {
                MessageBox.Show(msg);
                return;
            }

            // Set the rest of it

            ob.Glucose = ValidNumber(txtGlucose.Text);
            ob.BaseDose = ValidNumber(txtBaseDosage.Text);
            ob.GlucoseAdj = ValidNumber(txtGlucoseAdjustment.Text);
            ob.FastActingDose = ValidNumber(txtTotalDose.Text);
            ob.SlowActingDose = ValidNumber(txtSlowDose.Text);
            ob.CarbGrams = ValidNumber(txtCarbs.Text);
            ob.FoodDesc = GetRichText(txtFoodEaten);
            ob.Notes = GetRichText(txtNotes);
            Db.SaveEntry(ob);
            EntryDateUpdated(sender, e);

            // Show a popup if the entry is for current day, to show
            // value against week/two-week/and month averages
            if (ob.Date == Misc.DateAsInt())
            {
                var week = Db.GetHistory(7);
                var month = Db.GetHistory(30);
                var two = Db.GetHistory(14);

                int weekDiff = ob.Glucose - getHist(EntryType, week);
                int twoDiff = ob.Glucose - getHist(EntryType, two);
                int monthDiff = ob.Glucose - getHist(EntryType, month);

                MessageBox.Show("Today's Value: " + ob.Glucose +
                    "\nVs week: " + weekDiff + " (" + getHist(EntryType, week) +
                    ")\nVs two-week: " + twoDiff + " (" + getHist(EntryType, two) +
                    ")\nVs month: " + monthDiff + " (" + getHist(EntryType, month) +
                    ")\n");
            }

        }

        /// <summary>
        /// Helper method to get the glucose value from a history
        /// object
        /// </summary>
        /// <param name="type">entry type id</param>
        /// <param name="ob">the history object to use</param>
        /// <returns>the glucose for that type and object</returns>
        public int getHist(int type, History ob)
        {
            switch (type)
            {
                case 1: return ob.Breakfast;
                case 2: return ob.Lunch;
                case 3: return ob.Dinner;
                case 4: return ob.Bedtime;
            }

            return 0;
        }

        /// <summary>
        /// Update the entry form from the datagrid
        /// </summary>
        /// <param name="date"></param>
        /// <param name="type"></param>
        public void UpdateFromDatagrid(string date, int type)
        {
            EntryType = type;
            DateSelected = Misc.DateToInt(date);
            UpdateFromEntry(date, type);
        }

        /// <summary>
        /// event handler for cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to clear all data?", "Clear Confirmation", System.Windows.MessageBoxButton.YesNo);
            //if (messageBoxResult == MessageBoxResult.Yes)
            DateSelected = Misc.DateToInt("today");
            ClearFields();
        }

        /// <summary>
        /// Unused event handler 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            SaveEntry(sender, e);
        }




    }

}


//MessageBox.Show(Misc.DateToString(737510) + "\t" + Misc.DateToString(737539) );
//Misc.DateToString(2020087/1000);
//Entry x = new Entry();
//x.GenerateDummyData();


//MessageBox.Show("Selected ID: "
//    + EntryType.ToString()
//    + ": "
//    + ComboBoxEntryType.Text);
