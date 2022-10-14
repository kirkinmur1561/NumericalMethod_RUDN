using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lab_2
{
    class Program
    {
        /// <summary>
        /// Константа округления
        /// </summary>
        private const int Digits = 5;
        
        /// <summary>
        /// Константа шага
        /// </summary>
        private const double H = Math.PI / 7;
        
        /// <summary>
        /// Массив координат исходной функции
        /// </summary>
        private static PointF[] _originalPoint;
        
        /// <summary>
        /// Массив координат множество Лагранжа
        /// </summary>
        private static PointF[] _newtonPoint;        

        /// <summary>
        /// Массив узлов
        /// </summary>
        private static readonly List<double> XNode = new List<double>()
        {
            Math.Round(0.5 * H, Digits),
            Math.Round(1.5 * H, Digits),
            Math.Round(2.5 * H, Digits),
            Math.Round(4.5 * H, Digits),
            Math.Round(6.5 * H, Digits)
        };

        /// <summary>
        /// Исходная функция
        /// </summary>
        /// <param name="xI">значение х в итерации i</param>     
        private static double Func(double xI) =>
            Math.Sqrt(xI) * Math.Sin(xI) + 1;

        private static double BasicPolynomials(double i) =>
            i switch
            {
                0 => (XNode[0] - XNode[1]) * (XNode[0] - XNode[2]) * (XNode[0] - XNode[3]) * (XNode[0] - XNode[4]),
                1 => (XNode[1] - XNode[0]) * (XNode[1] - XNode[2]) * (XNode[1] - XNode[3]) * (XNode[1] - XNode[4]),
                2 => (XNode[2] - XNode[0]) * (XNode[2] - XNode[1]) * (XNode[2] - XNode[3]) * (XNode[0] - XNode[4]),
                3 => (XNode[3] - XNode[0]) * (XNode[3] - XNode[1]) * (XNode[3] - XNode[2]) * (XNode[0] - XNode[4]),
                4 => (XNode[4] - XNode[0]) * (XNode[4] - XNode[1]) * (XNode[4] - XNode[2]) * (XNode[4] - XNode[3]),
                _ => 0
            };

        private static double DividedDifference(params double[] args) =>
            args.Sum(s => Func(s) / BasicPolynomials(args.Length - 1));
        
            
        
        
        /// <summary>
        /// Функция записывающие в файлы данные формата json и csv. await Task.Delay(30) предназначена для гаранта закрытия потока на запись.
        /// </summary>
        /// <param name="namePoint">Имя записывающейся функции</param>
        /// <param name="pointFs">Массив координат</param>
        private static async Task WriteFiles(string namePoint, PointF[] pointFs)
        {
            int num = new Random().Next(0, 1001);
            await File.WriteAllTextAsync($"{num}_{namePoint}_file.json",
                JsonSerializer.Serialize(pointFs), Encoding.UTF8);            
            await Task.Delay(30);

            await File.WriteAllTextAsync($"{num}_{namePoint}_file.csv", string.Join("\n", pointFs.Select(s => $"{s.X};{s.Y}")));
            
            await Task.Delay(30);
        }
        
        static async Task Main()
        {
            double[] xsI = Enumerable
                .Range(1, 100)
                .Select(s => Math.Round(2 * Math.PI / 100 * s, Digits))
                .ToArray();/*Получение набора координат х. 2π/100 * i. */
            
            _originalPoint = xsI
                .Select(s => new PointF((float) s, MathF.Round((float) Func(s), Digits)))
                .ToArray();/*Преобразования исходной функции к массиву координат*/

            await WriteFiles("Original", _originalPoint);

            double[] ysI = xsI
                .Select(xI =>
                    XNode.Sum(xIJ =>
                        DividedDifference(XNode[0]) +
                        DividedDifference(XNode[0], XNode[1]) * (xI - XNode[0]) +
                        DividedDifference(XNode[0], XNode[1], XNode[2]) * (xI - XNode[0]) * (xI - XNode[1]) +
                        DividedDifference(XNode[0], XNode[1], XNode[2], XNode[3]) * (xI - XNode[0]) * (xI - XNode[1]) *
                        (xI - XNode[2]) +
                        DividedDifference(XNode[0], XNode[1], XNode[2], XNode[3], XNode[4]) * (xI - XNode[0]) *
                        (xI - XNode[1]) * (xI - XNode[2]) * (xI - XNode[3])
                    ))
                .ToArray();
            
            _newtonPoint =
                Enumerable
                    .Range(0, xsI.Length)
                    .Select(s => new PointF((float)xsI[s], (float)ysI[s]))
                    .ToArray();

            await WriteFiles("Newton", _newtonPoint);
            
            Console.CursorVisible = false;
            Console.WriteLine("Program end work!\n\tPress Enter...");
            Console.ReadLine();
        }
    }
}