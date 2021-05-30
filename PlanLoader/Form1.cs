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

namespace PlanLoader
{
    public partial class Form1 : Form
    {
        Bitmap currenImage;
        bool isTheContourClosed = false;
        List<ConsoleApp.Logic.Point> pointsCurrentFigure = new List<ConsoleApp.Logic.Point>();
        List<List<ConsoleApp.Logic.Point>> readyFigures = new List<List<ConsoleApp.Logic.Point>>();
        ConsoleApp.Logic.Point centralPoint = null;

        public Form1()
        {
            InitializeComponent();
            pictureBox1.Paint += PictureBox1_Paint;
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var detail in readyFigures)
                DrawDetail(detail, e, true);
            
            DrawDetail(pointsCurrentFigure, e, false);
            if (isTheContourClosed && centralPoint != null) {
                e.Graphics.DrawEllipse(new Pen(Color.Blue, 3), new Rectangle(centralPoint.X - 3, centralPoint.Y - 3, 3, 3));
            }
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                currenImage = new Bitmap(ofd.FileName);
                pictureBox1.Image = currenImage;
                OnOffSaveMent(true);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            currenImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pictureBox1.Image = currenImage;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (isTheContourClosed)
            {
                if (centralPoint != null)
                {
                    if (((MouseEventArgs)e).Button == MouseButtons.Left)
                        centralPoint = GetCurrentPoint(e);
                    if (((MouseEventArgs)e).Button == MouseButtons.Right)
                        centralPoint = null;
                }
                else
                {
                    if (((MouseEventArgs)e).Button == MouseButtons.Left) {
                        centralPoint = GetCurrentPoint(e);
                        info_label.Text = "Центральная точка установлена, теперь фигуру можно сохранить";
                    }
                    if (((MouseEventArgs)e).Button == MouseButtons.Right)
                    {
                        pointsCurrentFigure.RemoveAt(pointsCurrentFigure.Count - 1);
                        isTheContourClosed = false;
                        info_label.Text = "";
                    }
                }
            }
            else
            {
                if (((MouseEventArgs)e).Button == MouseButtons.Left)
                    pointsCurrentFigure.Add(GetCurrentPoint(e));
                if (((MouseEventArgs)e).Button == MouseButtons.Right)
                    pointsCurrentFigure.RemoveAt(pointsCurrentFigure.Count - 1);
                if (((MouseEventArgs)e).Button == MouseButtons.Middle)
                {
                    pointsCurrentFigure.Add(pointsCurrentFigure.First());
                    isTheContourClosed = true;
                    info_label.Text = "Фигура завершена, установите центральную точку внутри фигуры";
                }
            }
            pictureBox1.Refresh();
        }

        private ConsoleApp.Logic.Point GetCurrentPoint(EventArgs e)
        {
            Point p = ((MouseEventArgs)e).Location;
            return new ConsoleApp.Logic.Point(p.X, p.Y);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFigure();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OnOffSaveMent(false);
        }

        // включить/выключить кнопку сохранения и параметры фигуры
        private void OnOffSaveMent(bool enabled)
        {
            textBox1.Enabled = enabled;
            numericUpDown1.Enabled = enabled;
            button3.Enabled = enabled;
        }

        private void DrawDetail(List<ConsoleApp.Logic.Point> detailPoints, PaintEventArgs e, bool drawAsReady)
        {
            for (int i = 0; i < detailPoints.Count; i++)
            {
                if (!drawAsReady)
                    e.Graphics.DrawEllipse(new Pen(Color.Red, 3), new Rectangle(detailPoints[i].X - 3, detailPoints[i].Y - 3, 3, 3));
                if (i == 2 || i % 2 == 0 && i > 2)
                    e.Graphics.DrawCurve(new Pen(drawAsReady ? Color.Blue : Color.Red, 3), detailPoints.Skip(i - 2).Take(3).Select(p => p.GetDrawing()).ToArray());
            }
        }

        private void SaveFigure()
        {
            var articul = textBox1.Text;
            int number = (int)numericUpDown1.Value;

            try
            {
                Validate(articul, number);
            }
            catch (Exception ex)
            {
                errorLabel.Text = ex.Message;
                return;
            }
            
            errorLabel.Text = "";
            List<DetailModel> detailModels = new List<DetailModel>();

            detailModels.Add(new DetailModel() { Articul = articul, DetailNumber = number, PointNumber = 0, X = 0, Y = 0 });
            for (int i = 0; i < pointsCurrentFigure.Count - 1; i++)
            {
                detailModels.Add(new DetailModel()
                {
                    Articul = articul,
                    DetailNumber = number,
                    PointNumber = i + 1,
                    X = pointsCurrentFigure[i].X - centralPoint.X,
                    Y = pointsCurrentFigure[i].Y - centralPoint.Y,
                });
            }
            try
            {
                ConsoleApp.DBConnector.CreateList<DetailModel>(detailModels);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка при сохранени детали", ex.Message);
                return;
            }
            MessageBox.Show("Деталь успешно сохранена");
            ResetAfterSave();
        }

        private void Validate(string articul, int number)
        {
            if (!isTheContourClosed)
                throw new Exception("Фигура не завершена, чтобы завершить фигуру нажмите на колесико");
            if (centralPoint == null)
                throw new Exception("Не выставлена центральная точка");
            if (String.IsNullOrEmpty(textBox1.Text))
                throw new Exception("Заполните артикул");
            if (numericUpDown1.Value < 1)
                throw new Exception("Заполните номер детали");

            var same_detail = ConsoleApp.DBConnector.GetList<DetailModel>().Where(d => d.Articul == articul && d.DetailNumber == number).FirstOrDefault();
            if (same_detail != null)
                throw new Exception("Деталь с таким артикулом и номером уже существует");
        }

        private void ResetAfterSave()
        {
            info_label.Text = "";
            readyFigures.Add(pointsCurrentFigure);
            pointsCurrentFigure = new List<ConsoleApp.Logic.Point>();
            centralPoint = null;
            isTheContourClosed = false;
            pictureBox1.Refresh();
        }
    }
}
