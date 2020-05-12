using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


/// <summary>
/// Various helper functions to turn dates into int format, vice-versa
/// and etc
/// </summary>
namespace DiabetesDiary
{
    class Misc
    {
        /// <summary>
        ///  Converts date int to string format
        /// </summary>
        /// <param name="date">is number of days since 1/1/1 </param>
        /// <returns>date in string format (mm/dd/yyyy)</returns>
        public static string DateToString(int date)
        {
            var result = new DateTime(1, 1, 1).AddDays(date);
            return result.ToShortDateString();
        }

        /// <summary>
        /// Returns today's date in int format (days from 1/1/1)
        /// </summary>
        /// <returns>number of days since 1/1/1</returns>
        public static int DateAsInt()
        {
            TimeSpan days = DateTime.Today - new DateTime(1, 1, 1);
            return (int)days.TotalDays;
            //return new DateTime(1, 1, 1).AddDays(DateTime.Today);
        }

        /// <summary>
        /// Makes sure the date is valid, returns it in int form
        /// date comes from entry form date field
        /// </summary>
        /// <param name="dateString">the contents of date field</param>
        /// <returns>number of days since 1/1/1 </returns>
        public static int ValidDate(string dateString)
        {
            int date = DateToInt(dateString);
            return (date <= DateAsInt()) ? date : 0;
        }

        // 03/11, 3/11, 03/11/2020 to int format
        /// <summary>
        /// Parse a string date from various formats and 
        /// return number of days since 1/1/1 for that date
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <returns>number of days since 1/1/1</returns>
        public static int DateToInt(string str)
        {
            int m, d, y;

            var parts = str.Split('/');

            // A shorthand for the form, can just enter 'today'
            if (str == "today")
            {
                m = DateTime.Now.Month;
                y = DateTime.Now.Year;
                d = DateTime.Now.Day;
            }
            else
            {
                // Get year
                if (parts.Length < 2)
                {
                    parts = str.Split('-');
                }

                if (parts.Length < 2 || parts.Length > 3)
                {
                    MessageBox.Show("Date must be format mm/dd/yy or mm-dd-yy.");
                    return 0;
                }

                // If it's mm/dd year is 2020, else it's current year
                if (parts.Length == 3)
                {
                    y = GetYear(parts[2]);
                }
                else
                {
                    y = DateTime.Now.Year;
                }

                // Get month

                try
                {
                    m = Int32.Parse(parts[0]);
                    d = Int32.Parse(parts[1]);
                }
                catch( Exception e)
                {
                    MessageBox.Show("Month and day must be numeric.");
                    return 0;
                }

                if (m < 1 || m > 12)
                {
                    MessageBox.Show("Month needs to be between 1 and 12");
                    return 0;
                }

                if (d < 0 || d > 31)
                {
                    MessageBox.Show("Invalid day, format: mm/dd/yy or mm-dd-yy");
                    return 0;
                }
            }

            try
            {
                TimeSpan days = new DateTime(y, m, d) - new DateTime(1, 1, 1);
                return (int)days.TotalDays;

            }
            catch (Exception e)
            {
                return 0;
            }
            //MessageBox.Show("Date selected = " + m+"-"+d+"-"+y
            //    + "\nInt value: " + days.TotalDays) ;
        }


        /// <summary>
        /// Helper function to get year from two or four digit format
        /// </summary>
        /// <param name="str">Year as a string</param>
        /// <returns>year as an int</returns>
        public static int GetYear(string str)
        {
            string year = str;
            int y;

            if (str.Length == 2)
            {
                year = String.Format("20{0}", str);
            }

            if (year.Length == 4)
            {
                try
                {
                    y = Int32.Parse(year);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Invalid Year: " + year);
                    return 0;
                }

                return y;
            }

            MessageBox.Show("Invalid year, must be two or four digits.");
            return 0;
        }
    }
}

//MessageBox.Show("DATETOSTRING: " + date + " = " + result);


    // 3/32, 3-32, 3-32-13, 
