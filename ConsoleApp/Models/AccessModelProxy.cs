using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Data.OleDb;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp.Models
{
    public abstract class AccessModelProxy
    {


        public void Create(OleDbConnection connection)
        {
            string sql = GenerateInsertSql();
            OleDbCommand command = new OleDbCommand(sql);
            command.Connection = connection;
            int c = command.ExecuteNonQuery();
            //Console.WriteLine(c);
        }

        public void Update(OleDbConnection connection)
        {
            string sql = GenerateUpdateSql();
            OleDbCommand command = new OleDbCommand(sql);
            command.Connection = connection;
            int c = command.ExecuteNonQuery();
            //Console.WriteLine(c);
        }

        protected string GenerateInsertSql()
        {
            Type type = GetType();
            var attr = type.GetCustomAttributes(true)[0];
            var t_name = ((TableAttribute)attr).Name;

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            var field_names = fields.Select(field =>
                $"[{((ColumnAttribute)field.GetCustomAttributes(typeof(ColumnAttribute), true).First()).Name}]"
                ).ToList();

            var field_values = fields.Select(field =>
                field.FieldType.Name == "String" ? $"'{field.GetValue(this)}'" : field.GetValue(this)
                ).ToList();

            string sql = $"INSERT INTO [{t_name}] ({String.Join(",", field_names)}) VALUES ({String.Join(",", field_values)})";

            return sql;
        }

        public string GenerateUpdateSql()
        {
            Type type = GetType();
            var attr = type.GetCustomAttributes(true)[0];
            var t_name = ((TableAttribute)attr).Name;

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);


            Dictionary<string, Tuple<FieldInfo, string>> primary_keys = new Dictionary<string, Tuple<FieldInfo, string>>();

            //List<Tuple<FieldInfo, string>> primary_keys = new List<Tuple<FieldInfo, string>>();
            foreach (var field in fields)
            {
                var atribute = (ColumnAttribute)field.GetCustomAttributes(typeof(ColumnAttribute), true).First();
                if (atribute.IsPrimaryKey)
                    primary_keys.Add(field.Name, new Tuple<FieldInfo, string>(field, atribute.Name));
            }

            var field_values = fields.Where(field => !primary_keys.ContainsKey(field.Name)).Select(field =>
                    $"[{((ColumnAttribute)field.GetCustomAttributes(typeof(ColumnAttribute), true).First()).Name}] = " +
                    ((field.FieldType.Name == "String") ? $"'{field.GetValue(this)}'" : field.GetValue(this))
                ).ToList();

            var new_values = String.Join(" , ", field_values);



            var condition = String.Join(" AND ", primary_keys.Select(field => $"[{field.Value.Item2}] = " +
            (field.Value.Item1.FieldType.Name == "String" ? $"'{field.Value.Item1.GetValue(this)}'" : field.Value.Item1.GetValue(this))));

            string sql = $"UPDATE [{t_name}] SET {new_values} WHERE {condition}";

            return sql;
        }
    }
}
