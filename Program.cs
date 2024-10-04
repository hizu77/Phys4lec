using System;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Console.WriteLine("Enter the x and y dimensions of the shell");
            //Console.WriteLine("Enter the initial x and y velocities for 1 ball, radius and mass through space in one line");
            //Console.WriteLine("Enter the initial x and y velocities for 2 balls, radius and mass through space in one line");

            var arguments = args.Select(float.Parse).ToArray();
            
            var size = new float[2];
            Array.Copy(arguments, 0, size, 0, 2);

            var firstBall = new float[4];
            Array.Copy(arguments, 2, firstBall, 0, 4);
            
            var secondBall = new float[4];
            Array.Copy(arguments, 6, secondBall, 0, 4);
            
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(firstBall, secondBall, size));
        }
    }
}