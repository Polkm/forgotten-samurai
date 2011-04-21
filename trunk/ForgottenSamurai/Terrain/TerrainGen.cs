using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForgottenSamurai
{
    enum InterpolateMethod : byte
    {
        Linear = 1,
        Cosine = 2,
        Cubic = 3
    }

    class TerrainGen
    {
        private int iSeed;
        private InterpolateMethod itplMethod;

        public TerrainGen(int seed, InterpolateMethod l)
        {
            iSeed = seed;
            itplMethod = l;
        }

        public double Noise(int x, int y)
        {
            int n = x + y * iSeed;
            n = (n << 13) ^ n;
            return (1.0d - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0d);
        }

        public double SmoothNoise(int x, int y)
        {
            double corners = (Noise(x - 1, y - 1) + Noise(x + 1, y - 1) + Noise(x - 1, y + 1) + Noise(x + 1, y + 1)) / 16;
            double sides = (Noise(x - 1, y) + Noise(x + 1, y) + Noise(x, y - 1) + Noise(x, y + 1)) / 8;
            double center = Noise(x, y) / 4;
            return corners + sides + center;
        }

        public static double CubicInterpolate(double v0, double v1, double v2, double v3, double x)
        {
            double P = (v3 - v2) - (v0 - v1);
            double Q = (v0 - v1) - P;
            double R = v2 - v0;
            return P * x * x * x + Q * x * x + R * x + v1;
        }

        public static double CosineInterpolate(double a, double b, double x)
        {
            double ft = x * Math.PI;
            double f = (1 - Math.Cos(ft)) * 0.5d;
            return a * (1 - f) + b * f;
        }

        public static double LinearInterpolate(double a, double b, double x)
        {
            return a * (1 - x) + b * x;
        }

        private double InterpolatedNoise(double x, double y)
        {
            if (x < 0) x--;
            if (y < 0) y--;

            int iX = (int)x;
            double fX = x - iX;
            if (x < 0) fX++;

            int iY = (int)y;
            double fY = y - iY;
            if (y < 0) fY++;

            if (itplMethod == InterpolateMethod.Cubic)
            {

                double h0 = CubicInterpolate(SmoothNoise(iX - 1, iY - 1), SmoothNoise(iX - 1, iY), SmoothNoise(iX - 1, iY + 1), SmoothNoise(iX - 1, iY + 2), fY);
                double h1 = CubicInterpolate(SmoothNoise(iX, iY - 1), SmoothNoise(iX, iY), SmoothNoise(iX, iY + 1), SmoothNoise(iX, iY + 2), fY);
                double h2 = CubicInterpolate(SmoothNoise(iX + 1, iY - 1), SmoothNoise(iX + 1, iY), SmoothNoise(iX + 1, iY + 1), SmoothNoise(iX + 1, iY + 2), fY);
                double h3 = CubicInterpolate(SmoothNoise(iX + 2, iY - 1), SmoothNoise(iX + 2, iY), SmoothNoise(iX + 2, iY + 1), SmoothNoise(iX + 2, iY + 2), fY);

                return CubicInterpolate(h0, h1, h2, h3, fX);

            }

            double v1 = SmoothNoise(iX, iY);
            double v2 = SmoothNoise(iX + 1, iY);
            double v3 = SmoothNoise(iX, iY + 1);
            double v4 = SmoothNoise(iX + 1, iY + 1);

            if (itplMethod == InterpolateMethod.Cosine)
            {

                double i1 = CosineInterpolate(v1, v2, fX);
                double i2 = CosineInterpolate(v3, v4, fX);

                return CosineInterpolate(i1, i2, fY);

            }

            if (itplMethod == InterpolateMethod.Linear)
            {

                double i1 = LinearInterpolate(v1, v2, fX);
                double i2 = LinearInterpolate(v3, v4, fX);

                return LinearInterpolate(i1, i2, fY);

            }

            return 0;
        }

        public double GenerateNoise(double x, double y, double persistence, int octaves)
        {
            //x += int.MaxValue / 2;
            //y += int.MaxValue / 2;
            double total = 0;
            for (int i = 0; i < octaves; i++)
            {
                double frequency = Math.Pow(2, i);
                double amp = Math.Pow(persistence, i);
                total += InterpolatedNoise(x * frequency, y * frequency) * amp;
            }
            return total;
        }
    }
}
