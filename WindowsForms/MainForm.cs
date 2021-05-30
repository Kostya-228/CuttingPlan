using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsoleApp;
using ConsoleApp.Models;

namespace WindowsForms
{
    public partial class MainForm : Form
    {
        int borderX { get { return (int)numericBorder.Value; } }
        int step { get { return (int)numericUpDown2.Value; } }
        float size { get {
                return (float)numericUpDown1.Value;
            } }

        Graphics g;
        ConsoleApp.Logic.Detail current;
        List<ConsoleApp.Logic.Detail> other = new List<ConsoleApp.Logic.Detail>();

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

            current = new ConsoleApp.Logic.Detail(points.Skip(1).ToArray(), size, points.First());
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

                if (!is_cross && current != null)
                    if (item.IsCrossing(current))
                        is_cross = true;
            }
            if (current != null)
            {
                if (current.IsCrossBorder(borderX))
                    is_cross = true;
                foreach (var curve in current.GetCurves())
                {
                    g.DrawCurve(is_cross ? Pens.Red : Pens.Green, curve.GetDrawing());
                }
            }

            if (other.Count > 0)
            {
                int new_threashold = other.Select(det => det.GetTranslatedPoints().Max(p => p.Y)).Max(x => x);
                g.DrawLine(Pens.Red,
                    new Point(0, new_threashold),
                    new Point(1000, new_threashold));

                g.DrawString(new_threashold.ToString(), Font, Brushes.Red,
                    new Point(10, new_threashold  +1));
            }
            

        }
        private void button1_Click(object sender, EventArgs e)
        {
            try { CheckDetailSelectors(); }
            catch (Exception ex) { 
                errorLabal.Text = ex.Message;
                return;
            }
            LoadDetail();
            current.position.X += 100;
            current.position.Y += 100;
            panel1.Invalidate();
            OnOffMoving(true);
            button8.Enabled = true;
        }

        private void CheckDetailSelectors()
        {
            if (comboBox1.SelectedIndex == -1)
                throw new Exception("Выберите Артикул");
            if (comboBox2.SelectedIndex == -1)
                throw new Exception("Выберите номер детали");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            current.position.X += step;
            panel1.Invalidate();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            current.position.Y += step;
            panel1.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            current.position.X -= step;
            panel1.Invalidate();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            current.position.Y -= step;
            panel1.Invalidate();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            current.Angle += 15;
            panel1.Invalidate();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            current.Angle -= 15;
            panel1.Invalidate();
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

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.W: current.position.Y -= step; break;
                case Keys.A: current.position.X -= step; break;
                case Keys.S: current.position.Y += step; break;
                case Keys.D: current.position.X += step; break;
            }
            panel1.Invalidate();
        }

        private void numericBorder_ValueChanged(object sender, EventArgs e)
        {
            panel1.Invalidate();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            DrawDetails();
            e.Graphics.DrawLine(Pens.Red,
                new Point(borderX, 0),
                new Point(borderX, 1000));

            g.DrawString(borderX.ToString(), Font, Brushes.Red,
                    new Point(borderX + 1, 10));
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try { CheckDetailSelectors(); }
            catch (Exception ex)
            {
                errorLabal.Text = ex.Message;
                return;
            }
            LoadDetail();
            bool isSuccess = TryInsertFigure(
                new Range() { min = 0, max = 1000, step = step },
                new Range() { min = 0, max = borderX, step = step },
                new Range() { min = 0, max = 360, step = 90 }
                );
            if (!isSuccess) {
                current = null;
                MessageBox.Show("не получилось вставить");
                return;
            }
            TryInsertFigure(
                new Range() { min = current.position.Y - step, max = current.position.Y+1, step = 1 },
                new Range() { min = current.position.X - step, max = current.position.X+1, step = 1 },
                new Range() { min = 0, max = 360, step = 90 }
                );
            panel1.Invalidate();
            OnOffMoving(true);
            button8.Enabled = true;
        }

        struct Range
        {
            public int min;
            public int max;
            public int step;
        }

        private bool TryInsertFigure(Range y_range, Range x_range, Range angle_range)
        {
            for (int y = y_range.min; y < y_range.max; y += y_range.step)
            {
                for (int x = x_range.min; x < x_range.max; x += x_range.step)
                {
                    for (int angle = angle_range.min; angle < angle_range.max; angle += angle_range.step)
                    {
                        current.position.X = x;
                        current.position.Y = y;
                        current.Angle = angle;
                        if (current.IsCrossBorder(borderX))
                            continue;
                        if (current.IsCrossing(other.ToArray()))
                            continue;
                        return true;
                    }
                }
            }
            return false;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            List<ConsoleApp.Logic.Detail> details = DBConnector.GetList<DetailModel>().Where(
                det => det.Articul == (string)comboBox1.SelectedItem).GroupBy(det => det.DetailNumber).Select(art =>
                    art.Select(det => new ConsoleApp.Logic.Point(det.X, det.Y)).ToArray()).Select(
                points => new ConsoleApp.Logic.Detail(points.Skip(1).ToArray(), size, points.First())).ToList();

            

            new Thread(() => { Kek(details); }).Start();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void Kek(List<ConsoleApp.Logic.Detail> details)
        {
            var allcombi = allcombinations(Enumerable.Range(0, details.Count()).ToList(), new List<int>()).ToList();
            List<ConsoleApp.Logic.Detail> details_on_plan = new List<ConsoleApp.Logic.Detail>();
            List<ConsoleApp.Logic.Detail> best = new List<ConsoleApp.Logic.Detail>();
            int optimal_treashold = 1000;

            for (int i = 0; i < allcombi.Count(); i++)
            {
                for(int j=0; j < allcombi[i].Count; j++)
                {
                    int index = allcombi[i][j];
                    other = details_on_plan;
                    current = details[index];

                    TryInsertFigure(
                    new Range() { min = 0, max = 1000, step = step },
                    new Range() { min = 0, max = borderX, step = step },
                    new Range() { min = 0, max = 360, step = 90 }
                    );

                    details_on_plan.Add(details[index]);
                }
                current = null;
                
                int new_threashold = details_on_plan.Select(det => det.GetTranslatedPoints().Max(p => p.Y)).Max(x => x);

                if (optimal_treashold > new_threashold)
                {
                    //best = details_on_plan.Select(det => (ConsoleApp.Logic.Detail)det.Clone()).ToList();
                    optimal_treashold = new_threashold;
                    panel1.Invalidate();
                    Thread.Sleep(1000);
                }
                    
                details_on_plan = new List<ConsoleApp.Logic.Detail>();
            }

            MessageBox.Show("подбор окончен");
        }

        private static IEnumerable<List<T>> allcombinations<T>(List<T> arg, List<T> awithout)
        {
            if (arg.Count == 1)
            {
                var result = new List<List<T>>();
                result.Add(new List<T>());
                result[0].Add(arg[0]);
                return result;
            }
            else
            {
                var result = new List<List<T>>();

                foreach (var first in arg)
                {
                    var others0 = new List<T>(arg.Except(new T[1] { first }));
                    awithout.Add(first);
                    var others = new List<T>(others0.Except(awithout));

                    var combinations = allcombinations(others, awithout);
                    awithout.Remove(first);

                    foreach (var tail in combinations)
                    {
                        tail.Insert(0, first);
                        result.Add(tail);
                    }
                }
                return result;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            other = new List<ConsoleApp.Logic.Detail>();
            current = null;
            panel1.Invalidate();
        }
    }
}
