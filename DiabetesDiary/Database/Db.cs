using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows.Controls;
using System.Data;
using System.Windows;

/*
 * This contains all the database functionality
 */

namespace DiabetesDiary.Database
{
    class Db
    {
        // These are for running from the IDE.  
        // Each one has its own database.  This was pulled out into a root folder
        // that each project was in.

        //private const string DatabaseFile = "../../Database/diabetes-diary.db";
        //private const string LiveDBFile = "e:/Projects/diabetes-diary.db";
        //private const string DatabaseSource = "data source=" + LiveDBFile;

        // These are for running the live version
        // Local database is in same directory as the executables
        private const string LiveDBFile = "diabetes-diary.db";
        private const string DatabaseFile = LiveDBFile;
        private const string DatabaseSource = "data source=" + DatabaseFile;


        // Returns a connection to the database
        public static SQLiteConnection DbConn()
        {
            return new SQLiteConnection(DatabaseSource);
        }

        /// <summary>
        /// binds box with the entry types and their ids
        /// </summary>
        /// <param name="box">combobox to supply the values for</param>
        public static void BindEntryTypeBox(ComboBox box)
        {
            using (var conn = new SQLiteConnection(DatabaseSource))
            {
                String query = "SELECT * from EntryType";
                using (var da = new SQLiteDataAdapter(query, conn))
                {
                    //conn.Open();
                    DataSet ds = new DataSet();
                    da.Fill(ds, "EntryType");
                    box.ItemsSource = ds.Tables[0].DefaultView;
                    box.DisplayMemberPath = ds.Tables[0].Columns["EntryType"].ToString();
                    box.SelectedValuePath = ds.Tables[0].Columns["EntryTypeID"].ToString();

                    //conn.Close();
                    Console.WriteLine("===> BindEntryTypeBox <===");
                }
            }
        } // BindEntryTypeBox

        /// <summary>
        /// save an entry to the database
        /// </summary>
        /// <param name="ob">entry object to save</param>
        /// <returns>true for success, false for failure</returns>
        public static bool SaveEntry(Entry ob)
        {
            using (var conn = new SQLiteConnection(DatabaseSource))
            {
                using (var comm = new SQLiteCommand( conn ) )
                {
                    String query;
                    //int entryId = ob.EntryId;

                    query =
                        "INSERT OR REPLACE INTO Entry " +
                        "( UserID, Date, Time, EntryTypeID, Glucose, BaseDose, GlucoseAdj, " +
                        " FastActingDose, SlowActingDose, " +
                        " CarbGrams, FoodDesc, Notes )" +
                        " VALUES ( " +
                        " @user, @date, @time, @typeId, @glucose, @baseDose, @glucAdj, " +
                        " @fastDose, @slowDose, " +
                        " @carbs, @food, @notes ) "; 

                    conn.Open();
                    comm.CommandText = query;
                    comm.Parameters.AddWithValue("@user", ob.UserId);
                    comm.Parameters.AddWithValue("@date", ob.Date);
                    comm.Parameters.AddWithValue("@time", ob.Time);
                    comm.Parameters.AddWithValue("@typeId", ob.EntryType);
                    comm.Parameters.AddWithValue("@glucose", ob.Glucose);
                    comm.Parameters.AddWithValue("@baseDose", ob.BaseDose);
                    comm.Parameters.AddWithValue("@glucAdj", ob.GlucoseAdj);
                    comm.Parameters.AddWithValue("@fastDose", ob.FastActingDose);
                    comm.Parameters.AddWithValue("@slowDose", ob.SlowActingDose);
                    comm.Parameters.AddWithValue("@carbs", ob.CarbGrams);
                    comm.Parameters.AddWithValue("@food", ob.FoodDesc);
                    comm.Parameters.AddWithValue("@notes", ob.Notes);
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return true;
        } // SaveEntry()


        /// <summary>
        /// return an existing entry
        /// </summary>
        /// <param name="date">date of entry</param>
        /// <param name="type">entry type id of entry</param>
        /// <returns>an entry object for that entry</returns>        
        public static Entry GetEntry(int date, int type)
        {
            Entry ob = new Entry();
            ob.Date = date;
            ob.EntryType = type;

            using (var conn = new SQLiteConnection(DatabaseSource))
            {
                using (var comm = new SQLiteCommand(conn))
                {
                    var query =
                        "SELECT * FROM Entry "
                        + "WHERE Date = @date AND EntryTypeID = @type";
                    conn.Open();
                    comm.CommandText = query;
                    comm.Parameters.AddWithValue("@date", date);
                    comm.Parameters.AddWithValue("@type", type); 
                    //List<string> results = new List<string>();

                    using (var reader = comm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ob.UserId = Int32.Parse(reader["UserID"].ToString());
                            ob.Time = Int32.Parse(reader["Time"].ToString());
                            ob.Glucose = Int32.Parse(reader["Glucose"].ToString());
                            ob.BaseDose = Int32.Parse(reader["BaseDose"].ToString());
                            ob.GlucoseAdj = Int32.Parse(reader["GlucoseAdj"].ToString());
                            ob.FastActingDose = Int32.Parse(reader["FastActingDose"].ToString());
                            ob.SlowActingDose = Int32.Parse(reader["SlowActingDose"].ToString());
                            ob.CarbGrams = Int32.Parse(reader["CarbGrams"].ToString());
                            ob.FoodDesc = reader["FoodDesc"].ToString();
                            ob.Notes = reader["Notes"].ToString(); ;
                        }
                    }
                }
            }
            return ob;
        } // GetEntry


        /// <summary>
        /// returns a list of all entries
        /// </summary>
        /// <returns>List of entry objects</returns>
        public static List<Entry> GetEntryLog()
        {
            List<Entry> entries = new List<Entry>();

            using (var conn = new SQLiteConnection(DatabaseSource))
            {
                using (var comm = new SQLiteCommand(conn))
                {
                    var query =
                        "SELECT * FROM Entry " +
                        "ORDER BY Date DESC, EntryTypeID ASC; ";
                    conn.Open();
                    comm.CommandText = query;

                    using (var reader = comm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Entry ob = new Entry();
                            ob.Date = Int32.Parse(reader["Date"].ToString());
                            ob.EntryType = Int32.Parse(reader["EntryTypeID"].ToString());
                            ob.UserId = Int32.Parse(reader["UserID"].ToString());
                            ob.Time = Int32.Parse(reader["Time"].ToString());
                            ob.Glucose = Int32.Parse(reader["Glucose"].ToString());
                            ob.BaseDose = Int32.Parse(reader["BaseDose"].ToString());
                            ob.GlucoseAdj = Int32.Parse(reader["GlucoseAdj"].ToString());
                            ob.FastActingDose = Int32.Parse(reader["FastActingDose"].ToString());
                            ob.SlowActingDose = Int32.Parse(reader["SlowActingDose"].ToString());
                            ob.CarbGrams = Int32.Parse(reader["CarbGrams"].ToString());
                            ob.FoodDesc = reader["FoodDesc"].ToString();
                            ob.Notes = reader["Notes"].ToString(); 
                            entries.Add(ob);
                        }
                    }
                }
            }
            return entries;
        } // GetEntryLog()

        public static History GetHistory(int duration)
        {
            using (var conn = new SQLiteConnection(DatabaseSource))
            {
                using (var comm = new SQLiteCommand(conn))
                {
                    conn.Open();

                    int date = Misc.DateAsInt();
                    int lastDate = date - duration;

                    string query = 
                        "SELECT EntryTypeID, ROUND(AVG(Glucose), 0), SUM(Glucose), COUNT(Glucose) " + 
                        "FROM Entry " +
                        "WHERE Date >= @lastDate " +
                        "GROUP BY EntryTypeID ";
                    comm.CommandText = query;
                    comm.Parameters.AddWithValue("@lastDate", lastDate);
                    
                    var reader = comm.ExecuteReader();
                    int index = -1;
                    var ob = new History();
                    int sum = 0; int count = 0;

                    while (reader.Read())
                    { 
                        index = reader.GetInt32(0);
                        int avg = Int32.Parse(reader[1].ToString());
                        int s = Int32.Parse(reader[2].ToString());
                        int c = Int32.Parse(reader[3].ToString());
                        sum += s;
                        count += c;
                        //MessageBox.Show("DB: "+index +
                        //    "\t" + avg + "\t" + sum + "\t" + count);

                        ob.Duration = duration;
                        switch (index)
                        {
                            case 1: ob.Breakfast = avg; break;
                            case 2: ob.Lunch = avg; break;
                            case 3: ob.Dinner = avg; break;
                            case 4: ob.Bedtime = avg; break;
                            default: break;
                        }

                    }

                    conn.Close();
                    ob.Average = (sum > 0 ) ? (int)sum / count : 0;
                    return ob;
                }
            }
        }


    } // Entry class
}

// ======================================================================
// Stuff for testing
// ======================================================================


//conn.Open();
//conn.Close();


//if (entryId > -1)
//{
//    query =
//        "UPDATE Entry " +
//        "SET UserID=@user, Date=@date, Time=@time, " +
//        "EntryTypeID=@typeId, Glucose=@glucose, " +
//        "BaseDose=@baseDose, GlucoseAdj=@glucAdj, " +
//        "FastActingDose=@fastDose, SlowActingDose=@slowDose, " +
//        "CarbGrams=@carbs, FoodDesc=@food, Notes=@notes " +
//        "WHERE EntryID = " + entryId;
//}
//else
//{
//    query = 
//        "INSERT INTO Entry " +
//        "( UserID, Date, Time, EntryTypeID, Glucose, BaseDose, GlucoseAdj, " +
//        " FastActingDose, SlowActingDose, " +
//        " CarbGrams, FoodDesc, Notes )" +
//        " VALUES ( " +
//        " @user, @date, @time, @typeId, @glucose, @baseDose, @glucAdj, " +
//        " @fastDose, @slowDose, " +
//        " @carbs, @food, @notes )";
//}

//MessageBox.Show("entryId is: " + entryId + "\nQuery is: "+query);



//public static void Test()
//{
//    using (var conn = new SQLiteConnection(DatabaseSource))
//    {
//        using (var command = new SQLiteCommand(conn))
//        {
//            conn.Open();

//            command.CommandText = "SELECT * from User";
//            List<string> users = new List<string>();

//            using (var reader = command.ExecuteReader())
//            {
//                while (reader.Read())
//                {
//                    users.Add(reader["UserName"].ToString());
//                }
//            }

//            conn.Close();
//            Debug.Assert(users != null);

//            foreach (var user in users)
//            {
//                Console.WriteLine(user.ToString());
//            }

//        }
//    }
//}

//public static bool HasExistingEntry(int date, int type)
//{
//    using (var conn = new SQLiteConnection(DatabaseSource))
//    {
//        using (var comm = new SQLiteCommand(conn))
//        {
//            conn.Open();
//            string query = "SELECT EntryID FROM Entry WHERE " +
//                "Date = @date AND EntryTypeID = @type";
//            comm.CommandText = query;
//            comm.Parameters.AddWithValue("@date", date);
//            comm.Parameters.AddWithValue("@type", type);
//            var reader = comm.ExecuteReader();
//            bool answer = reader.HasRows;
//            conn.Close();
//            return answer;
//        }
//    }
//} // GetExistingEntry()


//public static int GetExistingEntryID(int date, int type)
//{
//    using (var conn = new SQLiteConnection(DatabaseSource))
//    {
//        using (var comm = new SQLiteCommand(conn))
//        {
//            conn.Open();
//            string query = "SELECT EntryID FROM Entry WHERE " +
//                "Date = @date AND EntryTypeID = @type";
//            comm.CommandText = query;
//            comm.Parameters.AddWithValue("@date", date);
//            comm.Parameters.AddWithValue("@type", type);
//            var reader = comm.ExecuteReader();
//            int index = -1;

//            if (reader.Read())
//            {
//                index = Int32.Parse( reader["EntryID"].ToString() );
//            }

//            conn.Close();
//            return index;
//        }
//    }

//}