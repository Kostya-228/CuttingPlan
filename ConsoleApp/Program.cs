using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Models;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var item in DBConnector.GetList<DetailModel>()) {
                item.Print();
            }
            Console.ReadKey();
        }
    }
}
