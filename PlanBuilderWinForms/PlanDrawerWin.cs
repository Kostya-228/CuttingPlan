using ConsoleApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlanBuilderWinForms
{
    public partial class PlanDrawerWin : Form
    {

        Graphics g;
        ConsoleApp.Logic.Detail detail; 
        public PlanDrawerWin()
        {
            InitializeComponent();
            g = panel1.CreateGraphics();
            LoadDetails();
        }

        public void LoadDetails()
        {
            //ConsoleApp.Logic.Point[] points = DBConnector.GetDetailPoints().Where(
            //    det => det.DetailNumber == 1
            //    ).Select(det => new ConsoleApp.Logic.Point(det.X, det.Y)).ToArray();

            //detail = new ConsoleApp.Logic.Detail(points.Skip(1).ToArray(), 10, points.First());
            //detail.position.X += 100;
            //detail.position.Y += 100;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
