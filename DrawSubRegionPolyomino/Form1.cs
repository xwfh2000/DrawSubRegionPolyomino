using System;

using System.Configuration;
using System.Collections.Specialized;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DrawSubRegionPolyomino
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            TBGridSize.Text = ConfigurationManager.AppSettings.Get("GridSize");
            TBRoundRatio.Text = ConfigurationManager.AppSettings.Get("RoundRatio");
            TBPenwidth.Text = ConfigurationManager.AppSettings.Get("PenWidth");
            TBFontSize.Text = ConfigurationManager.AppSettings.Get("FontSize");
            colorDialog1.Color = Color.FromArgb(Convert.ToInt32(ConfigurationManager.AppSettings.Get("PenColor")));
            colorDialog2.Color = Color.FromArgb(Convert.ToInt32(ConfigurationManager.AppSettings.Get("BrushColor")));
            colorDialog3.Color = Color.FromArgb(Convert.ToInt32(ConfigurationManager.AppSettings.Get("FontColor")));
            string pies = ConfigurationManager.AppSettings.Get("Pieces");

            for (int i = 0; i < pieces.GetLength(0); i++)
                for (int j = 0; j < pieces.GetLength(1); j++)
                {
                    pieces[i, j].contents = "";
                }
            string[] px = pies.Replace(" ", "").Replace("\r\n", "").Split('.');
            for (int x = 0; x < px.Length; x++)
            {
                string row = px[x];
                string[] con = row.Split('|');
                for (int y = 0; y < con.Length; y++)
                {                   
                    string n = (x * 10 + y + 1).ToString();
                    if (n.Length < 2)
                        n = "0" + n;
                    //判断是为TextBox框
                    foreach (Control item in panel1.Controls)
                    {
                       if (item is TextBox && (item.Name == "textBox" + n))
                        {
                            item.Text = con[y];
                        }
                    }
                }
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics gp = e.Graphics;
            double gridsize = Convert.ToDouble(TBGridSize.Text);
            double roundratio = Convert.ToDouble(TBRoundRatio.Text);
            float penWidth = Convert.ToSingle(TBPenwidth.Text);
            Color penColor = colorDialog1.Color;
            Color brushColor = colorDialog2.Color;
            Color fontColor = colorDialog3.Color;
            List<Point> pts = new List<Point>();
            foreach (Control item in panel1.Controls)
            {
                //判断是为TextBox框
                if (item is TextBox /*&& item.Text != ""*/)
                {
                    string name = item.Name;
                    int textnum = Convert.ToInt32(name.Substring(7, name.Length - 7)) - 1;
                    pieces[textnum % 10, textnum / 10].contents = item.Text;
                }
            }
            DrawPoly(new Pen(penColor, penWidth), new SolidBrush(brushColor), fontColor, gp, gridsize, pieces, roundratio);
            gp.Dispose();
        }

        struct Piece
        {
            public string contents;
        }
        Piece[,] pieces = new Piece[10, 10];
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
            return x - 1 < 0 ? "0,0,0" : Pieces[x - 1, y].contents;
        }
        private string GetRightGorup(int x, int y, Piece[,] Pieces)
        {
            return x + 1 > Pieces.GetLength(0) - 1 ? "0,0,0" : Pieces[x + 1, y].contents;
        }
        private string GetUpGorup(int x, int y, Piece[,] Pieces)
        {
            return y - 1 < 0 ? "0,0,0" : Pieces[x, y - 1].contents;
        }
        private string GetDownGorup(int x, int y, Piece[,] Pieces)
        {
            return y + 1 > Pieces.GetLength(1) - 1 ? "0,0,0" : Pieces[x, y + 1].contents;
        }
        private string GetULGroup(int x, int y, Piece[,] Pieces)
        {
            return (x - 1 < 0 || y - 1 < 0) ? "0,0,0" : Pieces[x - 1, y - 1].contents;
        }
        private string GetURGroup(int x, int y, Piece[,] Pieces)
        {
            return (x + 1 > Pieces.GetLength(0) - 1 || y - 1 < 0) ? "0,0,0" : Pieces[x + 1, y - 1].contents;
        }
        private string GetDLroup(int x, int y, Piece[,] Pieces)
        {
            return (x - 1 < 0 || y + 1 > Pieces.GetLength(1) - 1) ? "0,0,0" : Pieces[x - 1, y + 1].contents;
        }
        private string GetDRGroup(int x, int y, Piece[,] Pieces)
        {
            return (x + 1 > Pieces.GetLength(0) - 1 || y + 1 > Pieces.GetLength(1) - 1) ? "0,0,0" : Pieces[x + 1, y + 1].contents;
        }
        #endregion

        /// <summary>
        /// 判断是否填充四个角及中心。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Pieces"></param>
        /// <returns></returns>
        #region
        private bool JudgeULCorner(int x, int y, Piece[,] Pieces)
        {
            string[] M = Pieces[x, y].contents.Split(',');
            if (M[0] == "0")
                return false;
            return !JudgeULCircle(x,y,pieces);
            //string[] l = GetLeftGorup(x, y, Pieces).Split(',');
            //string[] u = GetUpGorup(x, y, Pieces).Split(',');
            //string[] me = Pieces[x, y].contents.Split(',');
            //return me[0] != "" && (( l[0]== me[0]&& l[0] == me[0]) || (u[0] == me[0]&& u[1] == me[1]));
        }
        private bool JudgeURCorner(int x, int y, Piece[,] Pieces)
        {
            string[] M = Pieces[x, y].contents.Split(',');
            if (M[0] == "0")
                return false;
            return !JudgeURCircle(x,y,pieces);
            //string[] r = GetRightGorup(x, y, Pieces).Split(',');
            //string[] u = GetUpGorup(x, y, Pieces).Split(',');
            //string[] me = Pieces[x, y].contents.Split(',');
            //return me[0] != "" && ((r[0] == me[0]&& r[0] == me[0]) || (u[0] == me[0]&& u[1] == me[1]));
        }
        private bool JudgeDLCorner(int x, int y, Piece[,] Pieces)
        {
            string[] M = Pieces[x, y].contents.Split(',');
            if (M[0] == "0")
                return false;
            return !JudgeDLCircle(x, y, pieces);
            //string[] l = GetLeftGorup(x, y, Pieces).Split(',');
            //string[] d = GetDownGorup(x, y, Pieces).Split(',');
            //string[] me = Pieces[x, y].contents.Split(',');
            //return me[0] != "" && ((l[0] == me[0]&& l[1] == me[1]) || (d[0] == me[0]&& d[1] == me[1]));
        }
        private bool JudgeDRCorner(int x, int y, Piece[,] Pieces)
        {
            string[] M = Pieces[x, y].contents.Split(',');
            if (M[0] == "0")
                return false;
            return !JudgeDRCircle(x, y, pieces);
            //string[] r = GetRightGorup(x, y, Pieces).Split(',');
            //string[] d = GetDownGorup(x, y, Pieces).Split(',');
            //string[] me = Pieces[x, y].contents.Split(',');
            //return me[0] != "" && ((r[0] == me[0]&& r[1] == me[1]) || (d[0] == me[0]& d[1] == me[1]));
        }
        private bool JudgeCenter(int x, int y, Piece[,] Pieces)
        {
            string[] me = Pieces[x, y].contents.Split(',');
            return me[0] != "0";
        }
        #endregion

        /// <summary>
        /// 填充四个角及中心
        /// </summary>
        /// <param name="brush"></param>
        /// <param name="g"></param>
        /// <param name="gridsize"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="radius"></param>
        #region
        private void FillULCorner(Brush brush, Graphics g, double gridsize, int X, int Y, int radius)
        { //画直角月牙
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(X + radius, Y, X, Y);
            gp.AddLine(X, Y, X, Y - radius);
            gp.AddArc(X, Y, 2 * radius, 2 * radius, 180, 90);
            Region r = new Region(gp);
            g.FillRegion(brush, r);
            gp.Dispose();
            r.Dispose();
        }
        private void FillURCorner(Brush brush, Graphics g, double gridsize, int X, int Y, int radius)
        {  //画直角月牙
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(X + (Single)gridsize - 2 * radius, Y, 2 * radius, 2 * radius, 270, 90);
            gp.AddLine(X + (Single)gridsize, Y + radius, X + (Single)gridsize, Y);
            gp.AddLine(X + (Single)gridsize, Y, X + (Single)gridsize - radius, Y);
            Region r = new Region(gp);
            g.FillRegion(brush, r);
            gp.Dispose();
            r.Dispose();
        }
        private void FillDLCorner(Brush brush, Graphics g, double gridsize, int X, int Y, int radius)
        {  //画直角月牙
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(X, Y + (Single)gridsize - 2 * radius, 2 * radius, 2 * radius, 90, 90);
            gp.AddLine(X, Y + (Single)gridsize - radius, X, Y + (Single)gridsize);
            gp.AddLine(X, Y + (Single)gridsize, X + radius, Y + (Single)gridsize);
            Region r = new Region(gp);
            g.FillRegion(brush, r);
            gp.Dispose();
            r.Dispose();
        }
        private void FillDRCorner(Brush brush, Graphics g, double gridsize, int X, int Y, int radius)
        {   //画直角月牙
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(X + (Single)gridsize - 2 * radius, Y + (Single)gridsize - 2 * radius, 2 * radius, 2 * radius, 0, 90);
            gp.AddLine(X + (Single)gridsize - radius, Y + (Single)gridsize, X + (Single)gridsize, Y + (Single)gridsize);
            gp.AddLine(X + (Single)gridsize, Y + (Single)gridsize, X + (Single)gridsize, Y + (Single)gridsize - radius);
            Region r = new Region(gp);
            g.FillRegion(brush, r);
            gp.Dispose();
            r.Dispose();
        }
        private void FillCenter(Brush brush, Graphics g, double gridsize, int X, int Y, int radius)
        {  //画圆角矩形
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(X + (Single)gridsize - 2 * radius, Y + (Single)gridsize - 2 * radius, 2 * radius, 2 * radius, 0, 90);
            gp.AddLine(X + (Single)gridsize - radius, Y + (Single)gridsize, X + radius, Y + (Single)gridsize);
            gp.AddArc(X, Y + (Single)gridsize - 2 * radius, 2 * radius, 2 * radius, 90, 90);
            gp.AddLine(X, Y + (Single)gridsize - radius, X, Y - (Single)gridsize);
            gp.AddArc(X, Y, 2 * radius, 2 * radius, 180, 90);
            gp.AddLine(X + (Single)gridsize, Y, X + (Single)gridsize - radius, Y);
            gp.AddArc(X + (Single)gridsize - 2 * radius, Y, 2 * radius, 2 * radius, 270, 90);
            gp.CloseFigure();
            Region r = new Region(gp);
            g.FillRegion(brush, r);
            gp.Dispose();
            r.Dispose();
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
            string[] L = GetLeftGorup(x, y, Pieces).Split(',');
            string[] U = GetUpGorup(x, y, Pieces).Split(',');
            string[] M = Pieces[x, y].contents.Split(',');

            if (L[0] == M[0] && L[1] == M[1])
                return false;
            if (U[0] == M[0] && U[1] == M[1])
                return false;
            if (M[0] == "0")
                return false;
            return true;

            //return GetLeftGorup(x, y, Pieces) != Pieces[x, y].contents &&
            //    GetUpGorup(x, y, Pieces) != Pieces[x, y].contents &&
            //    Pieces[x, y].contents != "";
        }
        private bool JudgeURCircle(int x, int y, Piece[,] Pieces)
        {
            string[] R = GetRightGorup(x, y, Pieces).Split(',');
            string[] U = GetUpGorup(x, y, Pieces).Split(',');
            string[] M = Pieces[x, y].contents.Split(',');

            if (R[0] == M[0] && R[1] == M[1])
                return false;
            if (U[0] == M[0] && U[1] == M[1])
                return false;
            if (M[0] == "0")
                return false;
            return true;
            //return GetRightGorup(x, y, Pieces) != Pieces[x, y].contents &&
            //    GetUpGorup(x, y, Pieces) != Pieces[x, y].contents &&
            //    Pieces[x, y].contents != "";
        }
        private bool JudgeDLCircle(int x, int y, Piece[,] Pieces)
        {
            string[] L = GetLeftGorup(x, y, Pieces).Split(',');
            string[] D = GetDownGorup(x, y, Pieces).Split(',');
            string[] M = Pieces[x, y].contents.Split(',');

            if (L[0] == M[0] && L[1] == M[1])
                return false;
            if (D[0] == M[0] && D[1] == M[1])
                return false;
            if (M[0] == "0")
                return false;
            return true;
            //return GetLeftGorup(x, y, Pieces) != Pieces[x, y].contents &&
            //    GetDownGorup(x, y, Pieces) != Pieces[x, y].contents &&
            //    Pieces[x, y].contents != "";
        }
        private bool JudgeDRCircle(int x, int y, Piece[,] Pieces)
        {
            string[] R = GetRightGorup(x, y, Pieces).Split(',');
            string[] D = GetDownGorup(x, y, Pieces).Split(',');
            string[] M = Pieces[x, y].contents.Split(',');

            if (R[0] == M[0] && R[1] == M[1])
                return false;
            if (D[0] == M[0] && D[1] == M[1])
                return false;
            if (M[0] == "0")
                return false;
            return true;
            //return GetDownGorup(x, y, Pieces) != Pieces[x, y].contents &&
            //    GetRightGorup(x, y, Pieces) != Pieces[x, y].contents &&
            //    Pieces[x, y].contents != "";
        }
        private bool JudgeLeftLine(int x, int y, Piece[,] Pieces)
        {
            string[] UL = GetULGroup(x, y, Pieces).Split(',');
            string[] U = GetUpGorup(x, y, Pieces).Split(',');
            string[] L = GetLeftGorup(x, y, Pieces).Split(',');
            string[] M = Pieces[x, y].contents.Split(',');
            //          UL不为空，且UL与U大小组相等，且U与M ！(大小组相等)
            //对称情况：M不为空，且M与L大小组相等，且U与M ！(大小组相等)
            //总结： U与M ！(大小组相等) 且 （U不为空且UL与U大小组相等||M不为空且M与L大小组相等）
            return !(U[0] == M[0] && U[1] == M[1]) &&
            ((U[0]!="0"&&UL[0] == U[0] && UL[1] == U[1])|| 
             (M[0]!="0"&&M[0] == L[0] && M[1] == L[1]));                
        }
        private bool JudgeMidLine(int x, int y, Piece[,] Pieces)
        {
            string[] U = GetUpGorup(x, y, Pieces).Split(',');
            string[] M = Pieces[x, y].contents.Split(',');
            //大组不同
            return U[0] != M[0];
        }
        private bool JudgeRightLine(int x, int y, Piece[,] Pieces)
        {
            string[] UR = GetURGroup(x, y, Pieces).Split(',');
            string[] U = GetUpGorup(x, y, Pieces).Split(',');
            string[] R = GetRightGorup(x, y, Pieces).Split(',');
            string[] M = Pieces[x, y].contents.Split(',');
            //          UR不为空，且UR与U大小组相等，且U与M ！(大小组相等)
            //对称情况：M不为空，且M与R大小组相等，且U与M ！(大小组相等)
            //总结： U与M ！(大小组相等) 且 （U不为空且UR与U大小组相等||M不为空且M与R大小组相等）
            return !(U[0] == M[0] && U[1] == M[1]) &&
            ((U[0] != "0" && UR[0] == U[0] && UR[1] == U[1]) ||
             (M[0] != "0" && M[0] == R[0] && M[1] ==R[1]));
        }
        private bool JudgeUpLine(int x, int y, Piece[,] Pieces)
        {
            string[] UL = GetULGroup(x, y, Pieces).Split(',');
            string[] L = GetLeftGorup(x, y, Pieces).Split(',');
            string[] U = GetUpGorup(x, y, Pieces).Split(',');
            string[] M = Pieces[x, y].contents.Split(',');
            //          UL不为空，且UL与L大小组相等，且L与M ！(大小组相等)
            //对称情况：M不为空，且M与U大小组相等，且L与M ！(大小组相等)
            //总结： L与M ！(大小组相等) 且 （UL不为空且UL与L大小组相等||M不为空且M与U大小组相等）
            return !(L[0] == M[0] && L[1] == M[1]) &&
            ((UL[0] != "0" && UL[0] == L[0] && UL[1] == L[1]) ||
             (M[0] != "0" && M[0] == U[0] && M[1] == U[1]));
        }
        private bool JudgeLmidLine(int x, int y, Piece[,] Pieces)
        {
            string[] L = GetLeftGorup(x, y, Pieces).Split(',');
            string[] M = Pieces[x, y].contents.Split(',');
            //L和M不同组
            return L[0] != M[0];
        }
        private bool JudgeDownLine(int x, int y, Piece[,] Pieces)
        {
            string[] DL = GetDLroup(x, y, Pieces).Split(',');
            string[] D = GetDownGorup(x, y, Pieces).Split(',');
            string[] L = GetLeftGorup(x, y, Pieces).Split(',');            
            string[] M = Pieces[x, y].contents.Split(',');
            //          DL不为空，且DL与L大小组相等，且L与M ！(大小组相等)
            //对称情况：M不为空，且M与D大小组相等，且L与M ！(大小组相等)
            //总结： L与M ！(大小组相等) 且 （DL不为空且DL与L大小组相等||M不为空且M与D大小组相等）
            return !(L[0] == M[0] && L[1] == M[1]) &&
            ((DL[0] != "0" && DL[0] == L[0] && DL[1] == L[1]) ||
             (M[0] != "0" && M[0] == D[0] && M[1] == D[1]));
        }
        #endregion

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
            gp.DrawLine(pen, X + radius, Y, X + (Single)gridsize - radius, Y);
        }
        private void DrawRightLine(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawLine(pen, X + (Single)gridsize - radius, Y, X + (Single)gridsize, Y);
        }
        private void DrawUpLine(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawLine(pen, X, Y, X, Y + radius);
        }
        private void DrawLmidLine(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawLine(pen, X, Y + radius, X, Y + (Single)gridsize - radius);
        }
        private void DrawDownLine(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawLine(pen, X, Y + (Single)gridsize - radius, X, Y + (Single)gridsize);
        }
        private void DrawULCircle(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawArc(pen, X, Y, 2 * radius, 2 * radius, 180, 90);
        }
        private void DrawURCircle(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawArc(pen, X + (Single)gridsize - 2 * radius, Y, 2 * radius, 2 * radius, 270, 90);
        }
        private void DrawDLCircle(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawArc(pen, X, Y + (Single)gridsize - 2 * radius, 2 * radius, 2 * radius, 90, 90);
        }
        private void DrawDRCircle(Pen pen, Graphics gp, double gridsize, int X, int Y, int radius)
        {
            gp.DrawArc(pen, X + (Single)gridsize - 2 * radius, Y + (Single)gridsize - 2 * radius, 2 * radius, 2 * radius, 0, 90);
        }
        #endregion

        private void DrawStr(Graphics gp, Color fontColor, double gridsize, int X, int Y, string s)
        {
            int g = (int)gridsize;
            TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter;
            TextRenderer.DrawText(gp, s, new Font("宋体", Convert.ToInt32(TBFontSize.Text)),
                new Rectangle(X, Y, g, g), fontColor, flags);

            //Size proposedSize = new Size(int.MaxValue, int.MaxValue);
            //Font font = new Font("宋体", Convert.ToInt32(TBFontSize.Text));
            //Size size=TextRenderer.MeasureText(s,font, proposedSize, flags);
            //gp.DrawString(s, font, new SolidBrush( colorDialog3.Color),
            //    new PointF(X+(Single)(gridsize/2-size.Width/2), Y + (Single)(gridsize / 2 - size.Height / 2)));
        }

        private void DrawPoly(Pen pen, Brush brush, Color fontColor, Graphics gp, double gridsize, Piece[,] Pieces, double roundRatio)
        {
            for (int x = 0; x < Pieces.GetLength(0); x++)
                for (int y = 0; y < Pieces.GetLength(1); y++)
                {
                    int X = (int)(x * gridsize);
                    int Y = (int)(y * gridsize);
                    int radius = (int)(gridsize / 2 * roundRatio);
                    //用刷子填充区域
                    if (JudgeULCorner(x, y, Pieces))
                        FillULCorner(brush, gp, gridsize, X, Y, radius);
                    if (JudgeURCorner(x, y, Pieces))
                        FillURCorner(brush, gp, gridsize, X, Y, radius);
                    if (JudgeDLCorner(x, y, Pieces))
                        FillDLCorner(brush, gp, gridsize, X, Y, radius);
                    if (JudgeDRCorner(x, y, Pieces))
                        FillDRCorner(brush, gp, gridsize, X, Y, radius);
                    if (JudgeCenter(x, y, Pieces))
                        FillCenter(brush, gp, gridsize, X, Y, radius);
                    //用线勾勒轮廓
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
                    //画字体
                    if (RBTNDraw.Checked == true)
                    {
                        string[] groupNUm = Pieces[x, y].contents.Split(',');
                        DrawStr(gp, fontColor, gridsize, X, Y,groupNUm[2]);
                    }
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

        private void BTNChangeColor_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();

        }

        private void BTNBrushColor_Click(object sender, EventArgs e)
        {
            colorDialog2.ShowDialog();
        }

        static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        static void ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? "Not Found";
                Console.WriteLine(result);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            AddUpdateAppSettings("GridSize", TBGridSize.Text);
            AddUpdateAppSettings("RoundRatio", TBRoundRatio.Text);
            AddUpdateAppSettings("PenWidth", TBPenwidth.Text);
            AddUpdateAppSettings("FontSize", TBFontSize.Text);
            AddUpdateAppSettings("PenColor", colorDialog1.Color.ToArgb().ToString());
            AddUpdateAppSettings("BrushColor", colorDialog2.Color.ToArgb().ToString());
            AddUpdateAppSettings("FontColor", colorDialog3.Color.ToArgb().ToString());
            string pies = "";
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    string n = (i * 10 + j + 1).ToString();
                    if (n.Length < 2)
                        n = "0" + n;
                    foreach (Control item in panel1.Controls)
                    {                       
                        //判断是为TextBox框
                        if (item is TextBox)
                        {
                            if (item.Name == "textBox" + n)
                            {
                                pies += item.Text!=""? item.Text:"0，0，0";//0组表示为空。,为小分割
                                if (j < 9)
                                    pies += "|";//|为中分割
                            }
                            
                        }
                    }
                }
                if (i < 9)
                    pies += ".";// .为大分割
            }
            AddUpdateAppSettings("Pieces", pies);
        }

        private void BTNFontColor_Click(object sender, EventArgs e)
        {
            colorDialog3.ShowDialog();
        }
    }
}
