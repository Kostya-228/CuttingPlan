using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;
using System.Data.Linq;
using ConsoleApp.Models;

namespace ConsoleApp
{
    public class DBConnector
    {

        private static string conn_str = СuttingPlan.Properties.Settings.Default.ConnectionString;


        public static List<DetailModel> GetDetailPoints()
        {
            using (OleDbConnection connection = new OleDbConnection(conn_str))
            {
                DataContext db = new DataContext(connection);
                Table<DetailModel> details = db.GetTable<DetailModel>();
                return details.ToList();
            }
        }
    }
}
