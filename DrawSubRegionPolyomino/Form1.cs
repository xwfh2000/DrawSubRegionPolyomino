using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DrawSubRegionPolyomino
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics gp = e.Graphics;
            double gridsize = 64;
            double roundratio = 0.5f;
            float penWidth = 1;
            List<Point> pts = new List<Point>();
            Piece[,] pieces = new Piece[10, 10];
            foreach (Control item in panel1.Controls)
            {
                //判断是为TextBox框
                if (item is TextBox && item.Text != "")
                {
                    string name = item.Name;
                    int textnum = Convert.ToInt32(name.Substring(7, name.Length - 7)) - 1;
                    pieces[textnum % 10, textnum / 10].groupNUm = item.Text;
                }
            }

            DrawPoly(new Pen(Color.Black, penWidth), gp, gridsize, pieces, roundratio);
            gp.Dispose();
        }
        private void DrawL(Pen pen, Graphics gp, int X1, int Y1, int X2, int Y2)
        {
            gp.DrawLine(pen, new Point(X1, Y1), new Point(X2, Y2));
        }

        private void DrawLeftLine(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawLine(pen, X, Y, X + radius, Y);
        }
        private void DrawMidLine(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawLine(pen, X + radius, Y, X + (int)gridsize - radius, Y);
        }
        private void DrawRightLine(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawLine(pen, X + (int)gridsize - radius, Y, X + (int)gridsize, Y);
        }
        private void DrawUpLine(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawLine(pen, X, Y, X, Y + radius);
        }
        private void DrawLmidLine(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawLine(pen, X, Y + radius, X, Y + (int)gridsize - radius);
        }
        private void DrawDownLine(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawLine(pen, X, Y + (int)gridsize - radius, X, Y + (int)gridsize);
        }
        private void DrawULCircle(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawArc(pen, X, Y, 2 * radius, 2 * radius, 180, 90);
        }
        private void DrawURCircle(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawArc(pen, X + (int)gridsize - 2 * radius, Y, 2 * radius, 2 * radius, 270, 90);
        }
        private void DrawDLCircle(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawArc(pen, X, Y + (int)gridsize - 2 * radius, 2 * radius, 2 * radius, 90, 90);
        }
        private void DrawDRCircle(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawArc(pen, X + (int)gridsize - 2 * radius, Y + (int)gridsize - 2 * radius, 2 * radius, 2 * radius, 0, 90);
        }
        struct Piece
        {
            public string groupNUm;
        }
        private void DrawPoly(Pen pen, Graphics gp, double gridsize, Piece[,] Pieces, double roundRatio)
        {
            for (int i = 0; i < Pieces.GetLength(0); i++)
                for (int j = 0; j < Pieces.GetLength(1); j++)
                    if (Pieces[i, j].groupNUm != "0")
                    {
                        int X = (int)(i * gridsize);
                        int Y = (int)(j * gridsize);
                        int radius = (int)(gridsize / 2 * roundRatio);
                        DrawULCircle(pen, gp, gridsize, X, Y, radius);
                        DrawURCircle(pen, gp, gridsize, X, Y, radius);
                        DrawDLCircle(pen, gp, gridsize, X, Y, radius);
                        DrawDRCircle(pen, gp, gridsize, X, Y, radius);
                        DrawLeftLine(pen, gp, gridsize, X, Y, radius);
                        DrawMidLine(pen, gp, gridsize, X, Y, radius);
                        DrawRightLine(pen, gp, gridsize, X, Y, radius);
                        DrawUpLine(pen, gp, gridsize, X, Y, radius);
                        DrawLmidLine(pen, gp, gridsize, X, Y, radius);
                        DrawDownLine(pen, gp, gridsize, X, Y, radius);
                    }

        }

        private void refresh_Click(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
