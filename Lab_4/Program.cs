using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lab_4
{
    class Program
    {
        private const decimal H = 0.01m;
        private const int DIGITS = 5;
       
        
        // y(x) = x + e^(-x) - e^(-1)
        private static decimal BasicFunc(decimal x) =>
            Math.Round(x + (decimal)Math.Pow(Math.E, (double)-x) - (decimal)Math.Pow(Math.E, -1), DIGITS);

        /// <summary>
        ///  Массив Xi от 0 до 100 с шагом H
        /// </summary>
        private static decimal[] Xs =
            Enumerable.Range(0, 101)
                .Select(s => Math.Round(s * H, DIGITS))
                .ToArray();
        
        /// <summary>
        /// Массив Yi задается функ BasicFunc
        /// </summary>
        private static decimal[] Ys =
            Xs
                .Select(BasicFunc)
                .ToArray();

        // private static decimal DerivativeOrder(int order, decimal x) =>
        //     order switch
        //     {
        //         0 => BasicFunc(x),
        //         1 => Math.Round(1m - (decimal)Math.Pow(Math.E, (double)-x), 5),
        //         2 => Math.Round((decimal)Math.Pow(Math.E, (double)-x), 5),
        //         3 => -Math.Round((decimal)Math.Pow(Math.E, (double)-x), 5),
        //         4 => Math.Round((decimal)Math.Pow(Math.E, (double)-x), 5)
        //     };
        //
        // private static decimal F1(bool IsNextI, int index) =>
        //     IsNextI
        //         ? Math.Round(Ys[index] + H * DerivativeOrder(1, Xs[index]) +
        //                      ((H * H) / 2) * DerivativeOrder(2, Xs[index]) +
        //                      ((H * H * H) / 6) * DerivativeOrder(3, Xs[index]) +
        //                      ((H * H * H * H) / 24) * DerivativeOrder(4, Xs[index] + H), 5)
        //         : Math.Round(Ys[index] - H * DerivativeOrder(1, Xs[index]) +
        //                      ((H * H) / 2) * DerivativeOrder(2, Xs[index]) -
        //                      ((H * H * H) / 6) * DerivativeOrder(3, Xs[index]) +
        //                      ((H * H * H * H) / 24) * DerivativeOrder(4, Xs[index] - H), 5);
        
            
        
        
        /// <summary>
        /// Функция записывающие в файлы данные формата json и csv. await Task.Delay(30) предназначена для гаранта закрытия потока на запись.
        /// </summary>
        /// <param name="namePoint">Имя записывающейся функции</param>
        /// <param name="pointDs">Массив координат</param>
        private static async Task WriteFiles(string namePoint, PointD[] pointDs)
        {
            string date = DateTime.Now.ToString("G")
                .Replace(":", "")
                .Replace("/", "");
            
            await File.WriteAllTextAsync($"{date}_{namePoint}_file.json",
                JsonSerializer.Serialize(pointDs), Encoding.UTF8);            
            await Task.Delay(30);

            await File.WriteAllTextAsync($"{date}_{namePoint}_file.csv", 
                string.Join("\n", pointDs.Select(s => $"{s.X};{s.Y}")));
            
            await Task.Delay(30);
        }

        
        static async Task Main(string[] args)
        {
            // await WriteFiles("Origin", Enumerable
            //     .Range(0, Xs.Length)
            //     .Select(index => new PointD(Xs[index], Ys[index]))
            //     .ToArray());

            

           
        }
    }
}