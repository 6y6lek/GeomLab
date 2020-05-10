using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeomLab
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
        class MyPoint
        {
            public Point point;
            public int id;
            public object Clone()
            {
                return new MyPoint { point = this.point, id = this.id };
            }
        }
        public double area(Point t1, Point t2, Point t3)
        {
            return Math.Abs((t1.X * t2.Y + t2.X * t3.Y + t3.X * t1.Y) - (t1.Y * t2.X + t2.Y * t3.X + t3.Y * t1.X)) / 2.0;
        }
        Graphics g;
        int size = 0;  // количество точек исходного множества
        Point[] Aa; // множество точек для которых строится оболочка
        int iter = 0;
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (size != 0)
            {
                g = CreateGraphics();
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                
                Aa[iter] = new Point(PointToClient(Control.MousePosition).X, PointToClient(Control.MousePosition).Y);
                g.FillEllipse(Brushes.DarkBlue, Aa[iter].X - 3, Aa[iter].Y - 3, 6, 6);
                iter++;
                if (iter >= size && iter >= 3)
                {
                    int j = 1, i = 0;
                    List<Point> S = new List<Point>();
                    S = ConvexHull(Aa.ToList());
                    
                    while (j < S.Count() && (S[j].X != 0 || S[j].Y != 0)) j++;
                    Point[] polygon = new Point[j];
                    for (i = 0; i < j; i++) polygon[i] = S[i];
                    g.FillPolygon(Brushes.Violet, polygon);
                    for (i = 0; i < size; i++)
                        g.FillEllipse(Brushes.DarkBlue, Aa[i].X - 3, Aa[i].Y - 3, 6, 6);
                    iter = 0;
                    List<MyPoint> points;
                    points = new List<MyPoint>();
                    for (i = 0; i < S.Count(); i++)
                    {
                        MyPoint m = new MyPoint();
                        m.id = i;
                        m.point = S[i];
                        points.Add(m);
                    }
                    MyPoint A = points[0], B = points[1], C = points[2];
                    MyPoint bA =(MyPoint)A.Clone(), bB = (MyPoint)B.Clone(), bC = (MyPoint)C.Clone();
                    while (true)
                    {
                        while (true)
                        {
                            while (true)
                            {
                                if (C.id != points.Count() - 1)
                                {
                                    MyPoint nextC = points[C.id + 1];
                                    if (area(A.point, B.point, C.point) < area(A.point, B.point, nextC.point))
                                        C = nextC;
                                    else break;
                                }
                                else
                                {
                                    MyPoint nextC = points[0];
                                    if (area(A.point, B.point, C.point) < area(A.point, B.point, nextC.point))
                                         C = nextC;
                                    else break;
                                }
                            }
                            if (B.id != points.Count() - 1)
                            {
                                MyPoint nextB = points[B.id + 1];
                                if (area(A.point, B.point, C.point) < area(A.point, nextB.point, C.point))
                                    B = nextB;
                                else break;
                            }
                            else
                            {
                                MyPoint nextB = points[0];
                                if (area(A.point, B.point, C.point) < area(A.point, nextB.point, C.point))
                                    B = nextB;
                                else break;
                            }
                        }
                        if (area(A.point, B.point, C.point) > area(bA.point, bB.point, bC.point))
                        {
                             bA = A; bB = B; bC = C; 
                        }

                        if (A.id != points.Count() - 1)
                            A = points[A.id + 1];
                        else A = points[0];

                        if (A.point == B.point)
                        {
                            if (B.id != points.Count() - 1)
                                B = points[B.id + 1];
                            else B = points[0];
                        }

                        if (B.point == C.point)
                        {
                            if (C.id != points.Count() - 1)
                                C = points[C.id + 1];
                            else C = points[0];
                        }
                        if (A.id == 0) break;
                    }
                    g.FillEllipse(Brushes.Yellow, bA.point.X - 3, bA.point.Y - 3, 6, 6);
                    g.FillEllipse(Brushes.Yellow, bB.point.X - 3, bB.point.Y - 3, 6, 6);
                    g.FillEllipse(Brushes.Yellow, bC.point.X - 3, bC.point.Y - 3, 6, 6);
                    Pen pen = new Pen(Color.Green, 1);
                    g.DrawLine(pen,bA.point,bB.point);
                    g.DrawLine(pen, bB.point, bC.point);
                    g.DrawLine(pen, bC.point, bA.point);
                }
              
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            size = Convert.ToInt32(numericUpDown1.Value);
            Aa = new Point[size];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
        }
        public static List<Point> ConvexHull(List<Point> points)
        {

            List<Point> hull = new List<Point>();
            Point vPointOnHull = points.Where(p => p.X == points.Min(min => min.X)).First();
            //
            Point vEndpoint;
            do
            {
                hull.Add(vPointOnHull);
                vEndpoint = points[0];

                for (int i = 1; i < points.Count; i++)
                {
                    if ((vPointOnHull == vEndpoint)
                     || (Orientation(vPointOnHull, vEndpoint, points[i]) == -1))
                    {
                        vEndpoint = points[i];
                    }
                }
                points.Remove(vEndpoint);
                vPointOnHull = vEndpoint;

            }
            while (vEndpoint != hull[0]);

            return hull;
        }

        private static int Orientation(Point p1, Point p2, Point p)
        {
            int Orin = (p2.X - p1.X) * (p.Y - p1.Y) - (p.X - p1.X) * (p2.Y - p1.Y);

            if (Orin > 0)
                return -1;
            if (Orin < 0)
                return 1; 

            return 0; 
        }
    }
}
