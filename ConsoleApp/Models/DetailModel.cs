using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Models
{
    [Table(Name = "Детали для вырезания")]
    public class DetailModel
    {

        [Column(Name = "Артикул изделия", IsPrimaryKey = true)]
        public string Articul{ get; set; }
        
        [Column(Name = "Номер детали", IsPrimaryKey = true)]
        public int DetailNumber { get; set; }

        [Column(Name = "Номер точки", IsPrimaryKey = true)]
        public int PointNumber { get; set; }

        [Column(Name = "Координата по X")]
        public int X { get; set; }
        [Column(Name = "Координата по Y")]
        public int Y { get; set; }


        public void Print()
        {
            Console.WriteLine($"{Articul}  {DetailNumber}  {PointNumber}  {X}  {Y}");
        }
    }
}
