using DiabetesDiary.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

// History object
namespace DiabetesDiary
{
    public class History
    {
        public int Duration { set; get; }
        public int Breakfast { set; get; }
        public int Lunch { set; get; }
        public int Dinner { get; set; }
        public int Bedtime { get; set; }
        public int Average { get; set; }
        public string Header { get; set; }

        // Constructor
        public History()
        {
            Duration = 0;
            Breakfast = 0;
            Lunch = 0;
            Dinner = 0;
            Bedtime = 0;
            Average = 0;
        }

        /// <summary>
        /// Get the variance information for current entry vs historic
        /// </summary>
        /// <param name="today">history object to compare</param>
        /// <param name="header">header to use</param>
        /// <returns>string array with the variances shown</returns>
        public string[] GetVariances(History today, string header)
        {
            Header = header;
            string[] results = new string[6];
            results[0] = header;
            results[1] = FormatStr(today.Breakfast, this.Breakfast);
            results[2] = FormatStr(today.Lunch, this.Lunch);
            results[3] = FormatStr(today.Dinner, this.Dinner);
            results[4] = FormatStr(today.Bedtime, this.Bedtime);
            results[5] = FormatStr(today.Average, this.Average);
            string t = "";

            foreach (string s in results)
            {
                t += s + "\n";
            }

            return results;
        }

        /// <summary>
        /// Helper method to format a single entry
        /// </summary>
        /// <param name="today"></param>
        /// <param name="otherDay"></param>
        /// <returns></returns>
        public string FormatStr(int today, int otherDay)
        {
            if (otherDay <= 0)
                return "N/A";

            if (Duration == 0)
            {
                if (today > 0)
                    return today + "";
                return "";
            }

            if (today == 0)
            {
                return otherDay + "";
            }

            if (today == otherDay)
                return (today > 0) ? otherDay + "" : "-";

            int diff = today - otherDay;
            return String.Format("{1} ({0:+#;-#;+0})", diff, otherDay);
        }

        /// <summary>
        /// String format for the history object
        /// </summary>
        /// <returns>string with the various properties in string form</returns>
        public override string ToString()
        {
            return String.Format(
                "Header: {5}\n Breakfast: {0}\nLunch: {1}\nDinner: {2}\nBedtime: {3}\nAverage: {4}",
                Breakfast, Lunch, Dinner, Bedtime, Average, Header);
        }
    }


}
