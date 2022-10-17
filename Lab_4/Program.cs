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
        
        private static decimal BasicFunc(decimal x) =>
            Math.Round(x + (decimal)Math.Pow(Math.E, (double)-x) - (decimal)Math.Pow(Math.E, -1), 5);

        private static decimal[] Xs =
            Enumerable.Range(0, 101)
                .Select(s => Math.Round(s * H, 2))
                .ToArray();
        
        private static decimal[] Ys =
            Xs
                .Select(BasicFunc)
                .ToArray();
        
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
        }
    }
}