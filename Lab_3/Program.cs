using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lab_3
{
    class Program
    {
        private const decimal h = 0.01m;
        private static decimal BasicFunc(decimal x) =>
            Math.Round(-(x * x * x) - 2, 4);

        private static decimal DifferentialEquation(decimal x, decimal y) =>
            (3 * (x * x)) / ((x * x * x) + y + 1);

        private static decimal[] Xs =
            Enumerable.Range(0, 101)
                .Select(s => Math.Round(s * h, 2))
                .ToArray();

        private static decimal[] Ys =
            Xs
                .Select(BasicFunc)
                .ToArray();

        private static decimal RK4Order(int indexOrder, decimal x, decimal y) => 
            indexOrder switch
        {
            1 => DifferentialEquation(x, y),
            2 => DifferentialEquation(x + h / 2, y + h * RK4Order(1, x, y) / 2),
            3 => DifferentialEquation(x + h / 2, y + h * RK4Order(2, x, y) / 2),
            4 => DifferentialEquation(x + h, y + h * RK4Order(3, x, y)),
            _ => throw new ArgumentOutOfRangeException(nameof(indexOrder), indexOrder, null)
        };
        
        /// <summary>
        /// Функция записывающие в файлы данные формата json и csv. await Task.Delay(30) предназначена для гаранта закрытия потока на запись.
        /// </summary>
        /// <param name="namePoint">Имя записывающейся функции</param>
        /// <param name="pointDs">Массив координат</param>
        private static async Task WriteFiles(string namePoint, PointD[] pointDs)
        {
            int num = new Random().Next(0, 1001);
            await File.WriteAllTextAsync($"{num}_{namePoint}_file.json",
                JsonSerializer.Serialize(pointDs), Encoding.UTF8);            
            await Task.Delay(30);

            await File.WriteAllTextAsync($"{num}_{namePoint}_file.csv", 
                string.Join("\n", pointDs.Select(s => $"{s.X};{s.Y}")));
            
            await Task.Delay(30);
        }
        
        static async Task Main(string[] args)
        {
            await WriteFiles("Origin", Enumerable
                .Range(0, Xs.Length)
                .Select(index => new PointD(Xs[index], Ys[index]))
                .ToArray());
            
            #region Метод ломаных Эйлера

            decimal[] YsEuler = new decimal[Xs.Length];
            YsEuler[0] = -2m;
            
            decimal[] func = new decimal[Xs.Length];
            func[0] = DifferentialEquation(Xs[0], YsEuler[0]);
            
            decimal[] h_fucn = new decimal[Xs.Length];
            h_fucn[0] = h * func[0];            

            for (int index = 1; index < Xs.Length; index++)
            {
                YsEuler[index] = Math.Round(YsEuler[index - 1] + h_fucn[index - 1], 5);
                func[index] = Math.Round(DifferentialEquation(Xs[index], YsEuler[index]), 5);
                h_fucn[index] = Math.Round(h * func[index], 5);
            }

            await WriteFiles("Euler",
                Enumerable
                    .Range(0, Xs.Length)
                    .Select(index => new PointD(Xs[index], YsEuler[index])).ToArray());

            #endregion

            #region Метод предиктор-корректор

            decimal[] YsFandC = new decimal[Xs.Length];
            YsFandC[0] = -2m;
            

            for (int index = 1; index < Xs.Length; index++)
            {
                decimal val_func = DifferentialEquation(Xs[index - 1], Ys[index - 1]);
                decimal _y = YsFandC[index - 1] + h * val_func;
                YsFandC[index] =
                    Math.Round(YsFandC[index - 1] + h * ((val_func + DifferentialEquation(Xs[index], _y)) / 2), 5);
            }


            await WriteFiles("ForecastAndCorrection",
                Enumerable.Range(0, Xs.Length)
                    .Select(index => new PointD(Xs[index], YsFandC[index]))
                    .ToArray());
            

            #endregion

            #region Метод Рунге-Кутты

            decimal[] YsRK = new decimal[Xs.Length];
            YsRK[0] = -2m;

            for (int index = 1; index < Xs.Length; index++)
            {
                YsRK[index] = Math.Round(YsRK[index - 1] + (h / 6) * (RK4Order(1, Xs[index - 1], YsRK[index - 1]) +
                                                                      2 * RK4Order(2, Xs[index - 1], YsRK[index - 1]) +
                                                                      2 * RK4Order(3, Xs[index - 1], YsRK[index - 1]) +
                                                                      RK4Order(4, Xs[index - 1], YsRK[index - 1])), 5);
            }
            
            await WriteFiles("Runge_Kutta",
                Enumerable
                    .Range(0, Xs.Length)
                    .Select(index => new PointD(Xs[index], YsRK[index]))
            .ToArray());
            
            Console.CursorVisible = false;
            Console.WriteLine("Program end work!\n\tPress Enter...");
            Console.ReadLine();

            #endregion
        }
    }
}