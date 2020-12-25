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
            double gridsize = Convert.ToDouble(TBGridSize.Text);
            double roundratio = Convert.ToDouble(TBRoundRatio.Text);
            float penWidth = Convert.ToSingle(TBPenwidth.Text);
            Color penColor = Color.Black;
            List<Point> pts = new List<Point>();
            Piece[,] pieces = new Piece[10, 10];
            for (int i = 0; i < pieces.GetLength(0); i++)
                for (int j = 0; j < pieces.GetLength(1); j++)
                {
                    pieces[i, j].groupNUm = "";
                }
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

            DrawPoly(new Pen(penColor, penWidth), gp, gridsize, pieces, roundratio);
            gp.Dispose();
        }

        struct Piece
        {
            public string groupNUm;
        }
        /// <summary>
        /// 画十种轮廓线（四个拐角+六种线段）
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="gp"></param>
        /// <param name="gridsize"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="radius"></param>
        #region 
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
        #endregion        

        /// <summary>
        /// 取临近的组
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Pieces"></param>
        /// <returns></returns>
        #region 
        private string GetLeftGorup(int x, int y, Piece[,] Pieces)
        {
            return x - 1 < 0 ? "" : Pieces[x - 1, y].groupNUm;
        }
        private string GetRightGorup(int x, int y, Piece[,] Pieces)
        {
            return x + 1 > Pieces.GetLength(0) - 1 ? "" : Pieces[x + 1, y].groupNUm;
        }
        private string GetUpGorup(int x, int y, Piece[,] Pieces)
        {
            return y - 1 < 0 ? "" : Pieces[x, y - 1].groupNUm;
        }
        private string GetDownGorup(int x, int y, Piece[,] Pieces)
        {
            return y + 1 > Pieces.GetLength(1) - 1 ? "" : Pieces[x, y + 1].groupNUm;
        }
        private string GetULGroup(int x, int y, Piece[,] Pieces)
        {
            return (x - 1 < 0 || y - 1 < 0) ? "" : Pieces[x - 1, y - 1].groupNUm;
        }
        private string GetURGroup(int x, int y, Piece[,] Pieces)
        {
            return (x + 1 > Pieces.GetLength(0) - 1 || y - 1 < 0) ? "" : Pieces[x + 1, y - 1].groupNUm;
        }
        private string GetDLroup(int x, int y, Piece[,] Pieces)
        {
            return (x - 1 < 0 || y + 1 > Pieces.GetLength(1) - 1) ? "" : Pieces[x - 1, y + 1].groupNUm;
        }
        private string GetDRGroup(int x, int y, Piece[,] Pieces)
        {
            return (x + 1 > Pieces.GetLength(0) - 1 || y + 1 > Pieces.GetLength(1) - 1) ? "" : Pieces[x + 1, y + 1].groupNUm;
        }
        #endregion

        /// <summary>
        /// 判断是否画轮廓
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Pieces"></param>
        /// <returns></returns>
        #region
        private bool JudgeULCircle(int x, int y, Piece[,] Pieces)
        {
            return GetLeftGorup(x, y, Pieces) != Pieces[x, y].groupNUm && 
                GetUpGorup(x, y, Pieces) != Pieces[x, y].groupNUm &&
                Pieces[x,y].groupNUm!="";
        }
        private bool JudgeURCircle(int x, int y, Piece[,] Pieces)
        {
            return GetRightGorup(x, y, Pieces) != Pieces[x, y].groupNUm && 
                GetUpGorup(x, y, Pieces) != Pieces[x, y].groupNUm &&
                Pieces[x, y].groupNUm != "";
        }
        private bool JudgeDLCircle(int x, int y, Piece[,] Pieces)
        {
            return GetLeftGorup(x, y, Pieces) != Pieces[x, y].groupNUm && 
                GetDownGorup(x, y, Pieces) != Pieces[x, y].groupNUm &&
                Pieces[x, y].groupNUm != "";
        }
        private bool JudgeDRCircle(int x, int y, Piece[,] Pieces)
        {
            return GetDownGorup(x, y, Pieces) != Pieces[x, y].groupNUm && 
                GetRightGorup(x, y, Pieces) != Pieces[x, y].groupNUm &&
                Pieces[x, y].groupNUm != "";
        }
        private bool JudgeLeftLine(int x, int y, Piece[,] Pieces)
        {
            //  aa       b  
            //   --  或  --
            //  |b     a|a
            string l = GetLeftGorup(x, y, Pieces);
            string ul = GetULGroup(x, y, Pieces);
            string u = GetUpGorup(x, y, Pieces);
            string me = Pieces[x, y].groupNUm;
            return (u != "" && u == ul && u != me) || (l != "" && l == me && u != me);
        }
        private bool JudgeMidLine(int x, int y, Piece[,] Pieces)
        {
            string u = GetUpGorup(x, y, Pieces);
            string me = Pieces[x, y].groupNUm;
            return (u == ""&&me!="")||(me==""&&u!="");
        }
        private bool JudgeRightLine(int x, int y, Piece[,] Pieces)
        {
            // aa    b
            // -     -
            // b|    a|a
            string r = GetRightGorup(x, y, Pieces);
            string ur = GetURGroup(x, y, Pieces);
            string u = GetUpGorup(x, y, Pieces);
            string me = Pieces[x, y].groupNUm;
            return (u != "" && u == ur && u != me) || (r != "" && r == me && u != me);
        }
        private bool JudgeUpLine(int x, int y, Piece[,] Pieces)
        {
            //a       a
            //a|b   b|a
            string l = GetLeftGorup(x, y, Pieces);
            string ul = GetULGroup(x, y, Pieces);
            string u = GetUpGorup(x, y, Pieces);
            string me = Pieces[x, y].groupNUm;
            return (l != "" && l == ul && l != me) || (u != "" && u == me && l != me);
        }
        private bool JudgeLmidLine(int x, int y, Piece[,] Pieces)
        {
            string l = GetLeftGorup(x, y, Pieces);
            string me = Pieces[x, y].groupNUm;
            return (l == "" && me != "") || (me == "" && l != "");
        }
        private bool JudgeDownLine(int x, int y, Piece[,] Pieces)
        {
            string d = GetDownGorup(x, y, Pieces);
            string l = GetLeftGorup(x, y, Pieces);
            string dl = GetDLroup(x, y, Pieces);
            string me = Pieces[x, y].groupNUm;
            return (d != "" && d == me && l != me) || (l != "" && l == dl && l != me);
        }
        #endregion

        private void DrawPoly(Pen pen, Graphics gp, double gridsize, Piece[,] Pieces, double roundRatio)
        {
            for (int x = 0; x < Pieces.GetLength(0); x++)
                for (int y = 0; y < Pieces.GetLength(1); y++)
                {
                    int X = (int)(x * gridsize);
                    int Y = (int)(y * gridsize);
                    int radius = (int)(gridsize / 2 * roundRatio);
                    if (JudgeULCircle(x, y, Pieces))
                        DrawULCircle(pen, gp, gridsize, X, Y, radius);
                    if (JudgeURCircle(x, y, Pieces))
                        DrawURCircle(pen, gp, gridsize, X, Y, radius);
                    if (JudgeDLCircle(x, y, Pieces))
                        DrawDLCircle(pen, gp, gridsize, X, Y, radius);
                    if (JudgeDRCircle(x, y, Pieces))
                        DrawDRCircle(pen, gp, gridsize, X, Y, radius);
                    if (JudgeLeftLine(x, y, Pieces))
                        DrawLeftLine(pen, gp, gridsize, X, Y, radius);
                    if (JudgeMidLine(x, y, Pieces))
                        DrawMidLine(pen, gp, gridsize, X, Y, radius);
                    if (JudgeRightLine(x, y, Pieces))
                        DrawRightLine(pen, gp, gridsize, X, Y, radius);
                    if (JudgeUpLine(x, y, Pieces))
                        DrawUpLine(pen, gp, gridsize, X, Y, radius);
                    if (JudgeLmidLine(x, y, Pieces))
                        DrawLmidLine(pen, gp, gridsize, X, Y, radius);
                    if (JudgeDownLine(x, y, Pieces))
                        DrawDownLine(pen, gp, gridsize, X, Y, radius);
                }

        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void Empty_Click(object sender, EventArgs e)
        {
            foreach (Control item in panel1.Controls)
            {
                //判断是为TextBox框
                if (item is TextBox)
                {
                    item.Text = "";
                }
            }
        }

        private void Set_Click(object sender, EventArgs e)
        {

        }
    }
}
