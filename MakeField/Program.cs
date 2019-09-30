using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace MakeField
{
    class Program
    {
        private static double landfallMaxWindSpeed;
        private static double baseWindSpeed;

        static void Main(string[] args)
        {
            // Setting to approximate Hurricane Hugo
            Program.landfallMaxWindSpeed = 222.0;
            Program.baseWindSpeed = 64.4;
            double valRange = Program.landfallMaxWindSpeed - Program.baseWindSpeed;

            double dotPitch = 1000.0;
            double xWidth = 500;
            double yHeight = 500;
            double cnt = xWidth * yHeight;

            using (var img = new Bitmap((int)xWidth, (int)yHeight))
            {
                for (double row = 0; row < xWidth; row++)
                {
                    for (double col = 0; col < yHeight; col++)
                    {
                        double speed = Program
                            .ComputeMaxWindSpeed(col * dotPitch, (row - yHeight / 2) * dotPitch);
                        double colrVal = (speed - Program.baseWindSpeed);
                        colrVal /= valRange; colrVal *= -255.0; colrVal += 255;
                        var color = Color.FromArgb(255, 255, (int) colrVal);
                        img.SetPixel((int) col, (int) row, color);
                    }
                }

                string outFileName =
                    @"C:\Directory\";
                outFileName = "Wind Field Map.png";

                img.Save(outFileName, System.Drawing.Imaging.ImageFormat.Png);
            }
            
        }


        private static double ComputeMaxWindSpeed(double x, double offset, double a = 360.0, double? maxWindSpeedAt00 = null)
        {
            double PeakSpeed = Program.landfallMaxWindSpeed;
            if (maxWindSpeedAt00 != null)
                PeakSpeed = (double)maxWindSpeedAt00;
            double baseSpeed = Program.baseWindSpeed;
            double Pb = PeakSpeed - baseSpeed;
            a *= 1000.0;
            double b = Pb * a * a;

            double speedAtOffset0 = Program.secondDerivHyperobla(x: x, a: a, b: b) + baseSpeed;

            /* Bookmark: Adjust 'a' for side winds */
            if (offset > 0.0)
                a *= 0.667;
            else
                a *= 0.45;

            Pb = speedAtOffset0 - baseSpeed;
            b = Pb * a * a;

            return Program.secondDerivHyperobla(x: offset, a: a, b: b) + baseSpeed;
        }


        /// <summary>
        /// Equation for the second derivitive of the hyperbola.
        /// </summary>
        /// <returns></returns>
        private static double secondDerivHyperobla(double x, double a, double b)
        {
            double a2 = Math.Pow(a, 2);
            double x2 = Math.Pow(x, 2);
            double y = b /
                ((a2 + x2) * Math.Sqrt(1.0 + (x2 / a2)));

            return y;
        }

    }
}
