using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsoleApp;

namespace WindowsForms
{
    public partial class Form1 : Form
    {
        Graphics g;
        ConsoleApp.Logic.Detail detail;
        ConsoleApp.Logic.Detail detail2;

        public Form1()
        {
            InitializeComponent();
            g = panel1.CreateGraphics();
            LoadDetails();

        }

        public void LoadDetails()
        {
            ConsoleApp.Logic.Point[] points = DBConnector.GetDetailPoints().Where(
                det => det.DetailNumber == 1).Where(det => det.PointNumber != 0
                ).Select(det => new ConsoleApp.Logic.Point(det.X, det.Y)).ToArray();
            detail = new ConsoleApp.Logic.Detail(points);
            detail.Size = 10;
            detail2 = (ConsoleApp.Logic.Detail)detail.Clone();
        }

        public void DrawDetails()
        {
            g.Clear(Color.WhiteSmoke);
            foreach (var curve in detail.GetCurves())
            {
                g.DrawCurve(Pens.Green, curve.GetDrawing());
            }


            bool cross = detail.IsCrossing(detail2);
            foreach (var curve in detail2.GetCurves())
            {
                g.DrawCurve(cross ? Pens.Red: Pens.Green, curve.GetDrawing());
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            DrawDetails();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            detail2.position.X += 3;
            DrawDetails();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            detail2.position.Y += 3;
            DrawDetails();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            detail2.position.X -= 3;
            DrawDetails();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            detail2.position.Y -= 3;
            DrawDetails();
        }
    }
}
