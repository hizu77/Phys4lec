using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private readonly Timer _timer;
        private readonly Ball _ball1;
        private readonly Ball _ball2;
        public Form1(float[] firstBall ,float[] secondBall, float[] box)
        {
            InitializeComponent();
            
            Text = "Elastic Collision Simulation";
            ClientSize = new Size(Convert.ToInt32(box[0]), Convert.ToInt32(box[1]));
            DoubleBuffered = true;

            _ball1 = new Ball(50, 50, firstBall[2], firstBall[0], firstBall[1], firstBall[3]);
            _ball2 = new Ball(400, 200, secondBall[2], secondBall[0], secondBall[1], secondBall[3]);

            _timer = new Timer();
            _timer.Interval = 20;
            _timer.Tick += UpdateSimulation;
            _timer.Start();
        }
        
        private void UpdateSimulation(object sender, EventArgs e)
        {
            _ball1.Move();
            _ball2.Move();

            _ball1.CheckWallCollision(ClientSize);
            _ball2.CheckWallCollision(ClientSize);

            if (_ball1.CheckCollision(_ball2))
            {
                Ball.ResolveCollision(_ball1, _ball2);
            }

            Invalidate();
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            _ball1.Draw(g);
            _ball2.Draw(g);
        }
    }


    public class Ball
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Radius { get;}
        public float VX { get; private set; }
        public float VY { get; private set; }
        public float Mass { get;}

        public Ball(float x, float y, float radius, float vx, float vy, float mass)
        {
            X = x;
            Y = y;
            Radius = radius;
            VX = vx;
            VY = vy;
            Mass = mass;
        }

        public void Move()
        {
            X += VX;
            Y += VY;
        }

        public void CheckWallCollision(Size clientSize)
        {
            if (X - Radius < 0 || X + Radius > clientSize.Width)
            {
                VX = -VX;
            }

            if (Y - Radius < 0 || Y + Radius > clientSize.Height)
            {
                VY = -VY;
            }
        }

        public bool CheckCollision(Ball other)
        {
            float dx = other.X - X;
            float dy = other.Y - Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            return distance < (Radius + other.Radius);
        }

        public static void ResolveCollision(Ball b1, Ball b2)
        {
            float dx = b2.X - b1.X;
            float dy = b2.Y - b1.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance == 0) return;

            float nx = dx / distance;
            float ny = dy / distance;
            
            float v1n = b1.VX * nx + b1.VY * ny;
            float v2n = b2.VX * nx + b2.VY * ny;
            
            float v1t = - b1.VX * ny + b1.VY * nx;
            float v2t = - b2.VX * ny + b2.VY * nx;
            
            
            float rV1n = ((b1.Mass - b2.Mass) * v1n + 2 * b2.Mass * v2n) / (b1.Mass + b2.Mass);
            float rV2n = ((b2.Mass - b1.Mass) * v2n + 2 * b1.Mass * v1n) / (b1.Mass + b2.Mass);
            
            float v1x = rV1n * nx - v1t * ny;
            float v1y = rV1n * ny + v1t * nx;
            
            
            float v2x = rV2n * nx - v2t * ny;
            float v2y = rV2n * ny + v2t * nx;

            b1.VX = v1x;
            b2.VX = v2x;
            
            b1.VY = v1y;
            b2.VY = v2y;
        }

        public void Draw(Graphics g)
        {
            g.FillEllipse(Brushes.Blue, X - Radius, Y - Radius, 2 * Radius, 2 * Radius);
        }
    }
}