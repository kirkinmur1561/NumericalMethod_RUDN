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
        private static readonly List<double> XNode = new()
        {
            Math.Round(0.5 * H, Digits),//X0
            Math.Round(1.5 * H, Digits),//X1
            Math.Round(2.5 * H, Digits),//X2
            Math.Round(4.5 * H, Digits),//X3
            Math.Round(6.5 * H, Digits) //X4
        };

        /// <summary>
        /// Исходная функция
        /// </summary>
        /// <param name="xI">значение х в итерации i</param>     
        private static double Func(double xI) =>
            Math.Sqrt(xI) * Math.Sin(xI) + 1;

        /// <summary>
        /// (xj-xi)*...
        /// </summary>
        /// <param name="Xi">значение узла</param>
        /// <param name="count">номер шага</param>
        /// <returns></returns>
        private static double BasicPolynomials(double Xi, int count) =>
            XNode
                .Where(w => w != Xi)
                .Take(count)
                .Aggregate<double, double>(1, (current, v) =>
                    current * (XNode.FirstOrDefault(f => f == Xi) - v));


        /// <summary>
        /// Sum f(xj)/(xj-xi)
        /// </summary>
        /// <param name="Xi">значение узла</param>
        /// <param name="j">номер шага</param>
        private static double Part_1(double Xi, int j) =>
            XNode
                .Where(w => w <= Xi)
                .Sum(s => Func(s) / BasicPolynomials(s, j));

        /// <summary>
        /// (x-x0)*(x-x1)* ... *(x-xn-1)
        /// </summary>
        /// <param name="x">Значение Х</param>
        /// <param name="take">N-1</param>
        private static double Part_2(double x, int take) =>
            XNode
                .Take(take)
                .Aggregate<double, double>(1, (c, v) =>
                    c * (x - v));
        
        
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

            double[] ysI =
                xsI
                    .Select(x =>
                        XNode[0] +
                        Part_1(XNode[1], 1) * Part_2(x, 1) +
                        Part_1(XNode[2], 2) * Part_2(x, 2) +
                        Part_1(XNode[3], 3) * Part_2(x, 3) +
                        Part_1(XNode[4], 4) * Part_2(x, 4))
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