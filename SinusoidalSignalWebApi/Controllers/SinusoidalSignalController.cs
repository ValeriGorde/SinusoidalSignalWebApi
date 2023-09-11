using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System;
using SinusoidalSignalWebApi.Models;
using System.Diagnostics.CodeAnalysis;

namespace SinusoidalSignalWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SinusoidalSignalController : ControllerBase
    {
        private const int ImageWidth = 1200;
        private const int ImageHeight = 900;

        [HttpPost]
        public IActionResult GenerateSignal(SinusoidalSignalParams parametrs)
        {
            double A = parametrs.A;
            double Fd = parametrs.Fd;
            double Fs = parametrs.Fs;
            int N = parametrs.N;

            int totalPoints = (int)(Fd * N / Fs);
            double Td = 1 / Fd;

            Bitmap image = new Bitmap(ImageWidth, ImageHeight);
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(Color.White);

            // Режим сглаживания 
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

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

                // Рисуем линию между двумя точками
                graphics.DrawLine(new Pen(Color.Black, 3f), x1, y1Scaled, x2, y2Scaled);
            }

            // Создаём поток для сохранения изображения
            var imageStream = new MemoryStream();
            image.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
            imageStream.Position = 0;

            // Отправляем изображение клиенту в виде графического файла
            return File(imageStream, "image/png", "sinusoidal_signal.png");
        }
    }
}
