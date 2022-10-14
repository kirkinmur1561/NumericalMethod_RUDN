using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Lab_1
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
        private static PointF[] _lagrangePoint;        

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

        /// <summary>
        /// Базисные полиномы
        /// </summary>
        /// <param name="xI">значение х в итерации i</param>
        /// <param name="i">номер полинома i</param>     
        private static double BasicPolynomials(double xI, double i) =>
            i switch
            {
                1 => (xI - XNode[1]) / (XNode[0] - XNode[1]) * (xI - XNode[2]) / (XNode[0] - XNode[2]) *
                    (xI - XNode[3]) / (XNode[0] - XNode[3]) * (xI - XNode[4]) / (XNode[0] - XNode[4]),
                2 => (xI - XNode[0]) / (XNode[1] - XNode[0]) * (xI - XNode[2]) / (XNode[1] - XNode[2]) *
                    (xI - XNode[3]) / (XNode[1] - XNode[3]) * (xI - XNode[4]) / (XNode[1] - XNode[4]),
                3 => (xI - XNode[0]) / (XNode[2] - XNode[0]) * (xI - XNode[1]) / (XNode[2] - XNode[1]) *
                    (xI - XNode[3]) / (XNode[2] - XNode[3]) * (xI - XNode[4]) / (XNode[2] - XNode[4]),
                4 => (xI - XNode[0]) / (XNode[3] - XNode[0]) * (xI - XNode[1]) / (XNode[3] - XNode[1]) *
                    (xI - XNode[2]) / (XNode[3] - XNode[2]) * (xI - XNode[4]) / (XNode[3] - XNode[4]),
                5 => (xI - XNode[0]) / (XNode[4] - XNode[0]) * (xI - XNode[1]) / (XNode[4] - XNode[1]) *
                    (xI - XNode[2]) / (XNode[4] - XNode[2]) * (xI - XNode[3]) / (XNode[4] - XNode[3]),
                _ => 0
            };        

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
        
        
        /// <summary>
        /// Рабочая функция \sqrt {\х }
        /// </summary>      
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
                    .Select(xI =>
                        XNode.Sum(xIj => 
                            Func(XNode[XNode.IndexOf(xIj)]) * BasicPolynomials(xI, XNode.IndexOf(xIj) + 1)))
                    .ToArray();
            /*Технология LINQ.
             Суть такова, что функция образается к каждому значению массива xsI и проводит над ним операции.
             Произведение функции Func с аргументом от массива XNode по индексу от индекса массива XNode и 
             функции BasicPolynomials с арг. массива xsI по индексу перебора, и арг. номера итерации от массива XNode.
             Результат суммируется, и записывается в массив.
             Функция ToArray() нужна для преобрахования из IEnumerable<double> в double[]*/
                    

            _lagrangePoint =
                Enumerable
                    .Range(0, xsI.Length)
                    .Select(s => new PointF((float)xsI[s], (float)ysI[s]))
                    .ToArray();

            await WriteFiles("Lagrange", _lagrangePoint);
            
            Console.CursorVisible = false;
            Console.WriteLine("Program end work!\n\tPress Enter...");
            Console.ReadLine();
        }
    }
}