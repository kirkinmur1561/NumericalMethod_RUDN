using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.VisualBasic;

namespace Lab_1
{
    class Program
    {
        /// <summary>
        /// Константа округления
        /// </summary>
        private const int digits = 5;
        
        /// <summary>
        /// Константа шага
        /// </summary>
        private const double H = Math.PI / 7;
        
        /// <summary>
        /// Массив координат исходной функции
        /// </summary>
        private static PointF[] OriginalPoint;
        
        /// <summary>
        /// Массив координат множество Лагранжа
        /// </summary>
        private static PointF[] LagrangePoint;

        /// <summary>
        /// Массив узлов
        /// </summary>
        private static List<double> XNode = new List<double>()
        {
            Math.Round(0.5 * H, digits),
            Math.Round(1.5 * H, digits),
            Math.Round(2.5 * H, digits),
            Math.Round(4.5 * H, digits),
            Math.Round(6.5 * H, digits)
        };

        /// <summary>
        /// Исходная функция
        /// </summary>
        /// <param name="x_i">значение х в итерации i</param>     
        private static double Func(double x_i) =>
            Math.Sqrt(x_i) * Math.Sin(x_i) + 1;

        /// <summary>
        /// Базисные полиномы
        /// </summary>
        /// <param name="x_i">значение х в итерации i</param>
        /// <param name="i">номер полинома i</param>     
        private static double L_i(double x_i, double i) =>
            i switch
            {
                1 => (x_i - XNode[1]) / (XNode[0] - XNode[1]) * (x_i - XNode[2]) / (XNode[0] - XNode[2]) *
                    (x_i - XNode[3]) / (XNode[0] - XNode[3]) * (x_i - XNode[4]) / (XNode[0] - XNode[4]),
                2 => (x_i - XNode[0]) / (XNode[1] - XNode[0]) * (x_i - XNode[2]) / (XNode[1] - XNode[2]) *
                    (x_i - XNode[3]) / (XNode[1] - XNode[3]) * (x_i - XNode[4]) / (XNode[1] - XNode[4]),
                3 => (x_i - XNode[0]) / (XNode[2] - XNode[0]) * (x_i - XNode[1]) / (XNode[2] - XNode[1]) *
                    (x_i - XNode[3]) / (XNode[2] - XNode[3]) * (x_i - XNode[4]) / (XNode[2] - XNode[4]),
                4 => (x_i - XNode[0]) / (XNode[3] - XNode[0]) * (x_i - XNode[1]) / (XNode[3] - XNode[1]) *
                    (x_i - XNode[2]) / (XNode[3] - XNode[2]) * (x_i - XNode[4]) / (XNode[3] - XNode[4]),
                5 => (x_i - XNode[0]) / (XNode[4] - XNode[0]) * (x_i - XNode[1]) / (XNode[4] - XNode[1]) *
                    (x_i - XNode[2]) / (XNode[4] - XNode[2]) * (x_i - XNode[3]) / (XNode[4] - XNode[3]),
                _ => 0
            };

        /// <summary>
        /// Функция записывающие в файлы данные формата json и csv. await Task.Delay(30) предназначена для гаранта закрытия потока на запись.
        /// </summary>
        /// <param name="name_point">Имя записывающейся функции</param>
        /// <param name="pointFs">Массив координат</param>
        private static async Task WriteFiles(string name_point, PointF[] pointFs)
        {
            int num = new Random().Next(0, 1001);
            await File.WriteAllTextAsync($"{num}_{name_point}_file.json",
                JsonSerializer.Serialize(pointFs), Encoding.UTF8);            
            await Task.Delay(30);

            await File.WriteAllTextAsync($"{num}_{name_point}_file.csv", string.Join("\n", pointFs.Select(s => $"{s.X};{s.Y}")));
            
            await Task.Delay(30);
        }
        
        
        /// <summary>
        /// Рабочая функция
        /// </summary>      
        static async Task Main(string[] args)
        {
            double[] xs_i = Enumerable
                .Range(1, 100)
                .Select(s => Math.Round(2 * Math.PI / 100 * s, digits))
                .ToArray();/*Получение набора координат х*/
            
            OriginalPoint = xs_i
                .Select(s => new PointF((float) s, MathF.Round((float) Func(s), digits)))
                .ToArray();/*Преобразования исходной функции к массиву координат*/

            await WriteFiles("Original", OriginalPoint);

            List<double> ys_i =
                xs_i
                    .Select(x => Math.Round(XNode.Sum(x_ij => Func(x) * L_i(x, XNode.IndexOf(x_ij) + 1)), digits))
                    .ToList();/*Нахождение приближеного значения по средством технологии LINQ*/      
            
            LagrangePoint = Enumerable
                .Range(0, xs_i.Length)
                .Select(s => new PointF((float) xs_i[s], (float) ys_i[s]))
                .ToArray();/*Перобрахование приближенных значений в массив координат*/

            await WriteFiles("Lagrange", LagrangePoint);

            Console.CursorVisible = false;
            Console.WriteLine("Program end work!\n\tPress Enter...");
            Console.ReadLine();
        }
    }
}