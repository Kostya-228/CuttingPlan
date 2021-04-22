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
using ConsoleApp.Models;

namespace WindowsForms
{
    public partial class MainForm : Form
    {
        Graphics g;
        ConsoleApp.Logic.Detail current;
        List<ConsoleApp.Logic.Detail> other = new List<ConsoleApp.Logic.Detail>();

        int step = 15;

        public MainForm()
        {
            InitializeComponent();
            g = panel1.CreateGraphics();
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            OnOffMoving(false);
            button8.Enabled = false;
            comboBox1.Items.AddRange(DBConnector.GetList<DetailModel>().GroupBy(d => d.Articul).Select(g => g.Key).ToArray());
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Enabled = true;
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(DBConnector.GetList<DetailModel>().Where(
                 d => d.Articul == (string)comboBox1.SelectedItem
                 ).GroupBy(d => d.DetailNumber).Select(g => g.Key.ToString()).ToArray());
        }

        public void LoadDetail()
        {
            ConsoleApp.Logic.Point[] points = DBConnector.GetList<DetailModel>().Where(
                det => det.DetailNumber == int.Parse((string)comboBox2.SelectedItem) && det.Articul == (string)comboBox1.SelectedItem
                ).Select(det => new ConsoleApp.Logic.Point(det.X, det.Y)).ToArray();

            current = new ConsoleApp.Logic.Detail(points.Skip(1).ToArray(), (int)numericUpDown1.Value, points.First());
            current.position.X += 100;
            current.position.Y += 100;
        }

        public void DrawDetails()
        {
            g.Clear(Color.WhiteSmoke);
            bool is_cross = false;
            foreach (var item in other)
            {
                foreach (var curve1 in item.GetCurves())
                {
                    g.DrawCurve(Pens.Green, curve1.GetDrawing());
                }

                if (!is_cross)
                    if (item.IsCrossing(current))
                        is_cross = true;
            }
            foreach (var curve in current.GetCurves())
            {
                g.DrawCurve(is_cross ? Pens.Red: Pens.Green, curve.GetDrawing());
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                errorLabal.Text = "Выберите Артикул";
                return;
            }
            if (comboBox2.SelectedIndex == -1)
            {
                errorLabal.Text = "Выберите номер детали";
                return;
            }
            LoadDetail();
            DrawDetails();
            OnOffMoving(true);
            button8.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            current.position.X += step;
            DrawDetails();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            current.position.Y += step;
            DrawDetails();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            current.position.X -= step;
            DrawDetails();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            current.position.Y -= step;
            DrawDetails();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            current.Angle += 15;
            DrawDetails();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            current.Angle -= 15;
            DrawDetails();
        }

        private void OnOffMoving(bool enabled)
        {
            button2.Enabled = enabled;
            button3.Enabled = enabled;
            button4.Enabled = enabled;
            button5.Enabled = enabled;
            button6.Enabled = enabled;
            button7.Enabled = enabled;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            other.Add(current);
            current = null;
            OnOffMoving(false);
            button8.Enabled = false;
        }
    }
}
