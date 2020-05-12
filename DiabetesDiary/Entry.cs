using DiabetesDiary.Database;
using System;
using System.Globalization;
using System.Windows;

/// <summary>
/// This is the class for the individual entries.  It handles most of the
/// operations pertaining to them.  
/// </summary>
namespace DiabetesDiary
{
    class Entry
    {
        public int EntryId { get; set; }
        public int UserId { get; set; }
        public int Date { get; set; }
        public int Time { get; set; }
        public int EntryType { get; set; }
        public int Glucose { get; set; }

        // Insulin
        public int BaseDose { get; set; }
        public int GlucoseAdj { get; set; }
        public int FastActingDose { get; set; }
        public int SlowActingDose { get; set; }

        // Food and notes
        public int CarbGrams { get; set; }
        public string FoodDesc { get; set; }
        public string Notes { get; set; }

        public string DateStr => Misc.DateToString(Date);
        public string TypeStr
        {
            get
            {
                switch(EntryType)
                {
                    case 1: return "Breakfast";
                    case 2: return "Lunch";
                    case 3: return "Dinner";
                    case 4: return "Bedtime";
                    default: return "None";
                }
            }
        }

        // Empty constructor
        public Entry()
        {
            UserId = 1;
            Time = 0;
            Date = 0;
            EntryType = 0;
            Glucose = 0;
            GlucoseAdj = 0;
            BaseDose = 0;
            FastActingDose = 0;
            SlowActingDose = 0;
            CarbGrams = 0;
            FoodDesc = "";
            Notes = "";
        }

        /// <summary>
        /// This calculates the doses for a given blood glucose
        /// level and entry type.
        /// Currently only used to generate random data, but
        /// I may add this to the entry form just to make it
        /// quicker.
        /// </summary>
        /// <param name="gluc">glucose value</param>
        /// <returns>the entry object (this)</returns>
        public Entry CalcDoses(int gluc)
        {
            SlowActingDose = 0;

            switch (EntryType)
            {
                case 1:             // Breakfast
                    BaseDose = 6;
                    break;
                case 2:
                    BaseDose = 8;   // Lunch has higher base dose
                    break;
                case 3:
                    BaseDose = 6;   // Dinner
                    break;
                case 4:             // Bedtime no base dose, has slow dose
                    BaseDose = 0;
                    SlowActingDose = 20;
                    break;
                default:
                    return this;
            }

            this.GlucoseAdj = (Glucose - 101) / 50;
            FastActingDose = BaseDose + GlucoseAdj;

            return this;
        } // CalcDoses()

        // These aren't currently used.  Originally for testing
        public Entry LoadEntry(int date, int type) { return this; }
        public void SaveEntry() { Db.SaveEntry(this); }

        /// <summary>
        /// Mostly for testing, this shows the values of entry class
        /// in string format
        /// </summary>
        /// <returns>string of entry values</returns>
        public override string ToString()
        {
            return
                "Date: " + Misc.DateToString(Date) +
                "\nType: " + EntryType +
                "\nTime: " + Time +
                "\nGlucose: " + Glucose +
                "\nBaseDose: " + BaseDose +
                "\nGlucoseAdj: " + GlucoseAdj +
                "\nFastActingDose: " + FastActingDose +
                "\nSlowActingDose: " + SlowActingDose +
                "\nCarbGrams: " + CarbGrams +
                "\nFoodDesc: " + FoodDesc +
                "\nNotes: " + Notes;
        }

        /// <summary>
        /// This generates dummy data for the database
        /// </summary>
        public void GenerateDummyData()
        {
            TimeSpan tspan = DateTime.Today - new DateTime(1, 1, 1);
            var date = (int)tspan.TotalDays - 10;
            //var date = (int)tspan.TotalDays + 1 - 33;
            var ran = new Random();
            //MessageBox.Show("Today: " + DateTime.Today + "\nOutput: "
                            //+ date
                            //+ "\nConverter: "+Misc.DateToString(date));

            for (int d = 0; d < 10; d++)      // Date
            {
                for (int t = 1; t < 5; t++)   // Type
                {
                    Date = date + d;
                    Time = 0800 + ((t - 1) * 400);
                    EntryType = t;
                    int val =
                        (t * 5) +
                        (80 + ran.Next(40) + ran.Next(40) + ran.Next(40)) -
                        (ran.Next(d));
                    Glucose = val;
                    CalcDoses(val);
                    CarbGrams = ran.Next(20);
                    FoodDesc = "(" + d + "," + t + ") Val was: " + val;
                    Notes = "";
                    Db.SaveEntry(this);
                }
                //MessageBox.Show("New day: "+d+"\t"+Date);
            }

        } // GenerateDummyData()
    }
}

// ========================================================
// stuff for testing
// ========================================================

//MessageBox.Show("Adjustments for Glucose " + gluc.ToString()
//    + "\nType: " + EntryType
//    + "\nBase: " + BaseDose
//    + "\nAdj: " + GlucoseAdj
//    + "\nTotal: " + FastActingDose
//    + "\nSlow: " + SlowActingDose
//    + "\n\n"
//    ); ;
//   } // Entry class
//}

//public Entry LoadEntry(int entryId) { return this; }
