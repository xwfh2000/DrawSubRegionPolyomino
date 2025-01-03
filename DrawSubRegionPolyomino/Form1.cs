﻿using System;

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
            KeyPreview = true;
            TBGridSize.Text = ConfigurationManager.AppSettings.Get("GridSize");
            TBRoundRatio.Text = ConfigurationManager.AppSettings.Get("RoundRatio");
            TBPenwidth.Text = ConfigurationManager.AppSettings.Get("PenWidth");
            TBFontSize.Text = ConfigurationManager.AppSettings.Get("FontSize");
            colorDialog1.Color = Color.FromArgb(Convert.ToInt32(ConfigurationManager.AppSettings.Get("PenColor")));
            colorDialog2.Color = Color.FromArgb(Convert.ToInt32(ConfigurationManager.AppSettings.Get("BrushColor")));
            colorDialog3.Color = Color.FromArgb(Convert.ToInt32(ConfigurationManager.AppSettings.Get("FontColor")));
            tbXCor.Text = ConfigurationManager.AppSettings.Get("XCor");
            tbYCor.Text = ConfigurationManager.AppSettings.Get("YCor");

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
            return x - 1 < 0 ? ",," : Pieces[x - 1, y].contents;
        }
        private string GetRightGorup(int x, int y, Piece[,] Pieces)
        {
            return x + 1 > Pieces.GetLength(0) - 1 ? ",," : Pieces[x + 1, y].contents;
        }
        private string GetUpGorup(int x, int y, Piece[,] Pieces)
        {
            return y - 1 < 0 ? ",," : Pieces[x, y - 1].contents;
        }
        private string GetDownGorup(int x, int y, Piece[,] Pieces)
        {
            return y + 1 > Pieces.GetLength(1) - 1 ? ",," : Pieces[x, y + 1].contents;
        }
        private string GetULGroup(int x, int y, Piece[,] Pieces)
        {
            return (x - 1 < 0 || y - 1 < 0) ? ",," : Pieces[x - 1, y - 1].contents;
        }
        private string GetURGroup(int x, int y, Piece[,] Pieces)
        {
            return (x + 1 > Pieces.GetLength(0) - 1 || y - 1 < 0) ? ",," : Pieces[x + 1, y - 1].contents;
        }
        private string GetDLroup(int x, int y, Piece[,] Pieces)
        {
            return (x - 1 < 0 || y + 1 > Pieces.GetLength(1) - 1) ? ",," : Pieces[x - 1, y + 1].contents;
        }
        private string GetDRGroup(int x, int y, Piece[,] Pieces)
        {
            return (x + 1 > Pieces.GetLength(0) - 1 || y + 1 > Pieces.GetLength(1) - 1) ? ",," : Pieces[x + 1, y + 1].contents;
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
            if (M[0] == "")
                return false;
            return !JudgeULCircle(x, y, pieces);
        }
        private bool JudgeURCorner(int x, int y, Piece[,] Pieces)
        {
            string[] M = Pieces[x, y].contents.Split(',');
            if (M[0] == "")
                return false;
            return !JudgeURCircle(x, y, pieces);
        }
        private bool JudgeDLCorner(int x, int y, Piece[,] Pieces)
        {
            string[] M = Pieces[x, y].contents.Split(',');
            if (M[0] == "")
                return false;
            return !JudgeDLCircle(x, y, pieces);
        }
        private bool JudgeDRCorner(int x, int y, Piece[,] Pieces)
        {
            string[] M = Pieces[x, y].contents.Split(',');
            if (M[0] == "")
                return false;
            return !JudgeDRCircle(x, y, pieces);
        }
        private bool JudgeCenter(int x, int y, Piece[,] Pieces)
        {
            string[] me = Pieces[x, y].contents.Split(',');
            return me[0] != "";
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
            if (M[0] == "")
                return false;
            return true;
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
            if (M[0] == "")
                return false;
            return true;
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
            if (M[0] == "")
                return false;
            return true;
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
            if (M[0] == "")
                return false;
            return true;
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
            ((U[0] != "" && UL[0] == U[0] && UL[1] == U[1]) ||
             (M[0] != "" && M[0] == L[0] && M[1] == L[1]));
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
            ((U[0] != "" && UR[0] == U[0] && UR[1] == U[1]) ||
             (M[0] != "" && M[0] == R[0] && M[1] == R[1]));
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
            ((UL[0] != "" && UL[0] == L[0] && UL[1] == L[1]) ||
             (M[0] != "" && M[0] == U[0] && M[1] == U[1]));
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
            ((DL[0] != "" && DL[0] == L[0] && DL[1] == L[1]) ||
             (M[0] != "" && M[0] == D[0] && M[1] == D[1]));
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
        }
        private string ValidContent(string content)
        {
            int commacount = 0;
            foreach (char c in content)
            {
                if (c == ',')
                    commacount++;
            }
            if (commacount >= 2)
                return content;
            else
                return ",,";
        }
        private void DrawPoly(Pen pen, Brush brush, Color fontColor, Graphics gp, double gridsize, Piece[,] Pieces, double roundRatio)
        {
            for (int x = 0; x < Pieces.GetLength(0); x++)
                for (int y = 0; y < Pieces.GetLength(1); y++)
                {
                    Pieces[x, y].contents = ValidContent(Pieces[x, y].contents);
                }
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
                    if (RBTNDraw.Checked == true && ",," != Pieces[x, y].contents)
                    {
                        string[] groupNUm = Pieces[x, y].contents.Split(',');
                        DrawStr(gp, fontColor, gridsize, X, Y, groupNUm[2]);
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
                    item.Text = ",,";
                }
            }
            Invalidate();
        }

        private void BTNChangeColor_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            Invalidate();
        }

        private void BTNBrushColor_Click(object sender, EventArgs e)
        {
            colorDialog2.ShowDialog();
            Invalidate();
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
            AddUpdateAppSettings("XCor", tbXCor.Text);
            AddUpdateAppSettings("YCor", tbYCor.Text);
            string pieces = "";
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
                                string content = item.Text;
                                int count = 0;
                                foreach (char c in content)
                                    if (c == ',')
                                        count++;

                                pieces += count <= 0 ? ",," : content;//0组表示为空。,为小分割
                                if (j < 9)
                                    pieces += "|";//|为中分割
                            }

                        }
                    }
                }
                if (i < 9)
                    pieces += ".";// .为大分割
            }
            AddUpdateAppSettings("Pieces", pieces);
        }

        private void BTNFontColor_Click(object sender, EventArgs e)
        {
            colorDialog3.ShowDialog();
            Invalidate();
        }

        string[,] GetMapStrs()
        {
            string[,] Map = new string[10, 10];
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
                        if (item is TextBox && item.Name == "textBox" + n)
                        {
                            Map[i, j] = item.Text;
                        }
                    }
                }
            }
            return Map;
        }
        string[,] MoveMap(string[,] Map, int direction)// 0:上 1:下 2:左 3:右 
        {
            switch (direction)
            {
                case 0:
                    for (int col = 0; col < 10; col++)
                    {
                        string temp = Map[0, col];
                        for (int row = 0; row < 9; row++)
                        {
                            Map[row, col] = Map[row + 1, col];
                        }
                        Map[9, col] = temp;
                    }
                    break;
                case 1:
                    for (int col = 0; col < 10; col++)
                    {
                        string temp = Map[9, col];
                        for (int row = 9; row > 0; row--)
                        {
                            Map[row, col] = Map[row - 1, col];
                        }
                        Map[0, col] = temp;
                    }
                    break;
                case 2:
                    for (int row = 0; row < 10; row++)
                    {
                        string temp = Map[row, 0];
                        for (int col = 0; col < 9; col++)
                        {
                            Map[row, col] = Map[row, col + 1];
                        }
                        Map[row, 9] = temp;
                    }
                    break;
                case 3:
                    for (int row = 0; row < 10; row++)
                    {
                        string temp = Map[row, 9];
                        for (int col = 9; col > 0; col--)
                        {
                            Map[row, col] = Map[row, col - 1];
                        }
                        Map[row, 0] = temp;
                    }
                    break;
            }
            return Map;
        }
        int[] RotatePoint(int A, int B, int x, int y, int wise)// 0:顺  1：逆  2:上下  3:左右
        {
            if (wise == 0)
            {
                int a = (B - y + x) % 10;
                int b = (x - A + y) % 10;
                return new int[] { a >= 0 ? a : a + 10, b >= 0 ? b : b + 10 };
            }
            if (wise == 1)
            {
                int a = (y - B + x) % 10;
                int b = (A - x + y) % 10;
                return new int[] { a >= 0 ? a : a + 10, b >= 0 ? b : b + 10 };
            }
            if (wise == 3)
            {
                int b = (2 * y - B) % 10;
                return new int[] { A, b >= 0 ? b : b + 10 };
            }
            if (wise == 2)
            {
                int a = (2 * x - A) % 10;
                return new int[] { a >= 0 ? a : a + 10, B };
            }
            return new int[] { A, B };

        }
        string[,] RotateMap(string[,] Map, int wise, int x, int y)// 0:顺时针  1：逆时针
        {
            string[,] output = new string[10, 10];
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    var temp = RotatePoint(i, j, x, y, wise);
                    output[temp[0], temp[1]] = Map[i, j];
                }
            return output;
        }
        void RestoreMap(string[,] map)
        {
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
                        if (item is TextBox && item.Name == "textBox" + n)
                        {
                            item.Text = map[i, j];
                        }
                    }
                }
            }
        }
        private void btnUp_Click(object sender, EventArgs e)
        {
            string[,] map = GetMapStrs();
            map = MoveMap(map, 0);
            RestoreMap(map);
            Invalidate();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            string[,] map = GetMapStrs();
            map = MoveMap(map, 1);
            RestoreMap(map);
            Invalidate();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            string[,] map = GetMapStrs();
            map = MoveMap(map, 2);
            RestoreMap(map);
            Invalidate();
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            string[,] map = GetMapStrs();
            map = MoveMap(map, 3);
            RestoreMap(map);
            Invalidate();
        }



        private void BtnClockwise_Click(object sender, EventArgs e)
        {
            string[,] map = GetMapStrs();
            map = RotateMap(map, 0, Convert.ToInt32(tbYCor.Text), Convert.ToInt32(tbXCor.Text));
            RestoreMap(map);
            Invalidate();
        }

        private void BtnAntiClockwise_Click(object sender, EventArgs e)
        {
            string[,] map = GetMapStrs();
            map = RotateMap(map, 1, Convert.ToInt32(tbYCor.Text), Convert.ToInt32(tbXCor.Text));
            RestoreMap(map);
            Invalidate();
        }

        private void BtnUpDown_Click(object sender, EventArgs e)
        {
            string[,] map = GetMapStrs();
            map = RotateMap(map, 2, Convert.ToInt32(tbYCor.Text), Convert.ToInt32(tbXCor.Text));
            RestoreMap(map);
            Invalidate();
        }

        private void BtnLeftRight_Click(object sender, EventArgs e)
        {
            string[,] map = GetMapStrs();
            map = RotateMap(map, 3, Convert.ToInt32(tbYCor.Text), Convert.ToInt32(tbXCor.Text));
            RestoreMap(map);
            Invalidate();
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                MessageBox.Show("v0.22:\r\n" +
                    "将默认空组从\"0\"改为\"\";");
            }
        }
    }
}
