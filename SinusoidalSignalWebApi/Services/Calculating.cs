using SinusoidalSignalWebApi.Models;
using System.Drawing;
using System;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace SinusoidalSignalWebApi.Services
{
    public static class Calculating
    {
        private const int ImageWidth = 1200;
        private const int ImageHeight = 900;
        public static MemoryStream GenerateSignal(SinusoidalSignalParams parametrs)
        {
            double A = parametrs.A;
            double Fd = parametrs.Fd;
            double Fs = parametrs.Fs;
            int N = parametrs.N;
            // Количество выборок
            int totalPoints = (int)(Fd * N / Fs);
            double Td = 1 / Fd;

            using (var bitmap = new SKBitmap(ImageWidth, ImageHeight))
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.White);

                // Массив для хранения точек
                double[] points = new double[totalPoints];
                for (int i = 0; i < totalPoints; i++)
                {
                    double t = i * Td;
                    points[i] = A * Math.Sin(2 * Math.PI * Fs * t);
                }

                // Находим наибольшее значение точки
                double maxPoint = points.Max();

                // Задаём масштаб изображения
                double scale = (ImageHeight - 1) / (2 * maxPoint);

                for (int i = 0; i < totalPoints - 1; i++)
                {
                    // Вычисляем x координату первой и второй точки
                    int x1 = i * (ImageWidth - 1) / (totalPoints - 1);
                    int x2 = (i + 1) * (ImageWidth - 1) / (totalPoints - 1);

                    // Вычисляем y координату первой и второй точки с учётом масштаба
                    int y1Scaled = (int)((ImageHeight - 1) / 2 - (points[i] * scale));
                    int y2Scaled = (int)((ImageHeight - 1) / 2 - (points[i + 1] * scale));

                    canvas.DrawLine(x1, y1Scaled, x2, y2Scaled, new SKPaint { Color = SKColors.Black });
                }

                var imageStream = new MemoryStream();
                bitmap.Encode(imageStream, SKEncodedImageFormat.Png, 100);
                imageStream.Position = 0;

                return imageStream;
            }
        }
    }
}
