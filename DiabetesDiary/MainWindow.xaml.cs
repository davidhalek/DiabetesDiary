using DiabetesDiary.Database;
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

/// <summary>
/// Main window
/// </summary>
namespace DiabetesDiary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        History today, week, twoWeek, month;
        string[] todayString, weekString, twoWeekString, monthString;
        List<Entry> entryLog;
        string dateString;
        DiaryEntryForm entryWindow;
        CollectionViewSource entryLogViewSource;
        int entryType;

        public MainWindow()
        {
            InitializeComponent();


            // Bind the Entry Log
            entryLog = Db.GetEntryLog();
            entryLogViewSource = (CollectionViewSource)(FindResource("EntryLogViewSource"));
            entryLogViewSource.Source = entryLog;

            // Event handler called when the entry form is updated
            DiaryEntryForm.EntryDateUpdated 
                += new EventHandler(DiaryEntryForm_EntryDateUpdated);

            // Initialize the history listbox arrays
            initBoxes();
        }

        /// <summary>
        /// Entry has been updated, makes sure history control and datagrid are updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DiaryEntryForm_EntryDateUpdated(object sender, EventArgs e)
        {
            entryLog = Db.GetEntryLog();
            entryLogViewSource.View.Refresh();
            initBoxes();
            entryLogViewSource = (CollectionViewSource)(FindResource("EntryLogViewSource"));
            entryLogViewSource.Source = entryLog;

        }

        /// <summary>
        /// Populate the history control
        /// </summary>
        public void initBoxes()
        {
            // Populate history
            today = Db.GetHistory(0);
            week = Db.GetHistory(7);
            twoWeek = Db.GetHistory(14);
            month = Db.GetHistory(30);

            // Populate the history strings
            todayString = today.GetVariances(today, "Today");
            weekString = week.GetVariances(today, "Last Week");
            twoWeekString = twoWeek.GetVariances(today, "Last 14");
            monthString = month.GetVariances(today, "Last 30 Days");

            // Finally, populate the listboxes with the string form
            todayListBox.ItemsSource = todayString;
            weekListBox.ItemsSource = weekString;
            twoWeekListBox.ItemsSource = twoWeekString;
            monthListBox.ItemsSource = monthString;
        }

        /// <summary>
        /// Event handler for the selection being changed in the log datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;

            // This handler is called when the form is loaded as well, this just ensures that
            // nothing is selected the first time around (defaults to first row being selected
            // otherwise)
            if (entryWindow == null)
            {
                dg.UnselectAll();
                return;
            }

            Entry row = (Entry)dg.SelectedItems[0];
            DataGrid temp = dataGrid;
            entryType = row.EntryType;
            dateString = row.DateStr;

            // Make sure the entry form updates its values
            entryWindow.UpdateFromDatagrid(row.DateStr, row.EntryType);
        }

        /// <summary>
        /// Empty handler, but it's checked for
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //initBoxes();
            //todayListBox.ItemsSource = todayString;
            //twoWeekListBox.ItemsSource = twoWeekString;
            //monthListBox.ItemsSource = monthString;

        }

        // ===============================================================
        // Various event handlers for things being loaded
        // ===============================================================

        private void TodayListBox_Loaded(object sender, RoutedEventArgs e)
        {
            todayListBox.ItemsSource = todayString;
        }

        private void WeekListBox_Loaded(object sender, RoutedEventArgs e)
        {
            weekListBox.ItemsSource = weekString;
        }
        private void TwoWeekListBox_Loaded(object sender, RoutedEventArgs e)
        {
            twoWeekListBox.ItemsSource = twoWeekString;
        }
        private void MonthListBox_Loaded(object sender, RoutedEventArgs e)
        {
            monthListBox.ItemsSource = monthString;
        }
        private void DiaryEntryForm_Loaded(object sender, RoutedEventArgs e)
        {
            entryWindow = (DiaryEntryForm)sender;
            return;
        }
    }
}



//MessageBox.Show("WINDOW_LOADED");
// MessageBox.Show("WINDOW_LOADED entryWindow is " + entryWindow.ToString());
//dataGrid = sender;

//Console.WriteLine("DiaryEntryForm_Loaded");
//MessageBox.Show("DiaryEntryForm_Loaded");

