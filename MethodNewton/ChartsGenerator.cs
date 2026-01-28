// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using MathNet.Numerics.LinearAlgebra;
// using OxyPlot;
// using OxyPlot.Axes;
// using OxyPlot.Series;
// using OxyPlot.Legends;
// using OxyPlot.SkiaSharp;

// namespace MethodNewton
// {
//     public class ChartsGenerator
//     {
//         private List<IFunction> _function;
//         public ChartsGenerator(List<IFunction> function)
//         {
//             _function = function;
//         }

//         public void PlotAndSave(
//             List<double> xData,
//             List<double> yData,
//             Vector<double> coefficients,
//             string? fileName = null)
//         {
//             if(fileName == null)
//                 fileName = "chart_" + $"{DateTime.Now:HH-mm-ss}"+".png";
//             string title = BuildStringTitle(coefficients);

//             var plotModel = new PlotModel
//             {
//                 Title = $"Аппроксимация данных\n" + title,
//                 TitleFontSize = 14,
//                 DefaultFont = "Arial",
//                 Background = OxyColors.WhiteSmoke,
//                 PlotAreaBackground = OxyColors.White,
//                 PlotAreaBorderColor = OxyColors.Black,

//             };

//             plotModel.Legends.Add(new Legend
//             {
//                 LegendPosition = LegendPosition.LeftTop,
//                 LegendBackground = OxyColor.FromArgb(200, 255, 255, 255),
//                 LegendBorder = OxyColors.Black,
//                 LegendBorderThickness = 1,
//                 LegendFontSize = 12,
//                 LegendPlacement = LegendPlacement.Inside
//             });

//             var pointsSeries = new LineSeries
//             {
//                 Title = "Исходные данные",
//                 MarkerType = MarkerType.Circle,
//                 MarkerSize = 4,
//                 MarkerFill = OxyColors.Blue,
//                 LineStyle = LineStyle.None,
//                 Color = OxyColors.Transparent
//             };

//             for (int i = 0; i < xData.Count; i++)
//             {
//                 pointsSeries.Points.Add(new DataPoint(xData[i], yData[i]));
//             }
//             plotModel.Series.Add(pointsSeries);

//             var functionSeries = new LineSeries
//             {
//                 Title = "Аппроксимация",
//                 Color = OxyColors.Red,
//                 StrokeThickness = 2
//             };

//             double xMin = xData.Min();
//             double xMax = xData.Max();
//             int pointsCount = 200;

//             for (int i = 0; i <= pointsCount; i++)
//             {
//                 double x = xMin + (xMax - xMin) * i / pointsCount;
//                 double y = Function(x, coefficients);
//                 functionSeries.Points.Add(new DataPoint(x, y));
//             }
//             plotModel.Series.Add(functionSeries);

//             plotModel.Axes.Add(new LinearAxis
//             {
//                 Position = AxisPosition.Bottom,
//                 Title = "X",
//                 TitleFontSize = 12
//             });

//             plotModel.Axes.Add(new LinearAxis
//             {
//                 Position = AxisPosition.Left,
//                 Minimum = 0,
//                 Title = "Y",
//                 TitleFontSize = 12
//             });

//             SavePlot(fileName, plotModel);
//         }

//         private double Function(double[] x, Vector<double> coeff)
//         {
//             double functionValue = 0;

//             for (int i = 0; i < _function.Count; i++)
//             {
//                 functionValue += _function[i].Calculate(x, coeff[i]);
//             }

//             return functionValue;
//         }

//         private string BuildStringTitle(Vector<double> coeff)
//         {
//             string title = "";

//             for (int i = 0; i < _function.Count; i++)
//             {
//                 string sign = coeff[i] >= 0 ? "+" : "-";
//                 if (i != 0)
//                     title += $"{sign}";

//                 title += $"{Math.Abs(Math.Round(coeff[i], 4))}*{_function[i]}";
//             }

//             return title;
//         }

//         private void SavePlot(string fileName, PlotModel plotModel)
//         {
//             string directory = Path.Combine(Directory.GetCurrentDirectory(), "result_charts");
//             Directory.CreateDirectory(directory);

//             string filePath = Path.Combine(directory, fileName);
//             using var stream = File.Create(filePath);
//             var exporter = new PngExporter { Width = 800, Height = 600 };
//             exporter.Export(plotModel, stream);

//             Console.WriteLine($"График сохранен: {filePath}");
//         }
//     }
// }

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Legends;
using OxyPlot.SkiaSharp;

namespace MethodNewton
{
    public class ChartsGenerator
    {
        private List<IFunction> _function;
        public ChartsGenerator(List<IFunction> function)
        {
            _function = function;
        }

        // Основной метод теперь принимает список массивов аргументов xData
        public void PlotAndSave(
            List<List<double>> xDataList,
            List<double> yData,
            Vector<double> coefficients,
            string? fileName = null)
        {
            if (xDataList == null || xDataList.Count == 0) return;

            List<double[]> xData = xDataList.Select(list => list.ToArray()).ToList();

            // Определяем размерность пространства (количество переменных x)
            int dimension = xData[0].Length;

            if (fileName == null)
                fileName = "chart_" + $"{DateTime.Now:HH-mm-ss}" + ".png";

            if (dimension == 1)
            {
                Plot2D(xData, yData, coefficients, fileName);
            }
            else if (dimension == 2)
            {
                Plot3D(xData, yData, coefficients, fileName);
            }
            else
            {
                Console.WriteLine($"Размерность {dimension}D не поддерживается для визуализации (максимум 2 аргумента).");
            }
        }

        // Логика для y = f(x)
        private void Plot2D(List<double[]> xData, List<double> yData, Vector<double> coefficients, string fileName)
        {
            string title = BuildStringTitle(coefficients);

            var plotModel = new PlotModel
            {
                Title = $"Аппроксимация (2D)\n" + title,
                TitleFontSize = 14,
                DefaultFont = "Arial",
                Background = OxyColors.WhiteSmoke,
                PlotAreaBackground = OxyColors.White,
                PlotAreaBorderColor = OxyColors.Black,
            };

            plotModel.Legends.Add(new Legend
            {
                LegendPosition = LegendPosition.LeftTop,
                LegendBackground = OxyColor.FromArgb(200, 255, 255, 255),
                LegendBorder = OxyColors.Black,
                LegendBorderThickness = 1,
                LegendFontSize = 12,
                LegendPlacement = LegendPlacement.Inside
            });

            // 1. Рисуем точки исходных данных
            var pointsSeries = new LineSeries
            {
                Title = "Исходные данные",
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerFill = OxyColors.Blue,
                LineStyle = LineStyle.None,
                Color = OxyColors.Transparent
            };

            for (int i = 0; i < xData.Count; i++)
            {
                // Берем нулевой элемент массива, т.к. размерность 1
                pointsSeries.Points.Add(new DataPoint(xData[i][0], yData[i]));
            }
            plotModel.Series.Add(pointsSeries);

            // 2. Рисуем аппроксимирующую линию
            var functionSeries = new LineSeries
            {
                Title = "Аппроксимация",
                Color = OxyColors.Red,
                StrokeThickness = 2
            };

            // Находим мин/макс по оси X (нулевой индекс)
            double xMin = xData.Min(v => v[0]);
            double xMax = xData.Max(v => v[0]);
            int pointsCount = 200;

            for (int i = 0; i <= pointsCount; i++)
            {
                double xVal = xMin + (xMax - xMin) * i / pointsCount;
                // Формируем массив аргументов для функции
                double yVal = Function(new double[] { xVal }, coefficients);
                functionSeries.Points.Add(new DataPoint(xVal, yVal));
            }
            plotModel.Series.Add(functionSeries);

            // Оси
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X", TitleFontSize = 12 });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y", Minimum = 0, TitleFontSize = 12 });

            SavePlot(fileName, plotModel);
        }

        // Логика для y = f(x1, x2) -> Визуализация через HeatMap (Тепловую карту)
        private void Plot3D(List<double[]> xData, List<double> yData, Vector<double> coefficients, string fileName)
        {
            string title = BuildStringTitle(coefficients);

            var plotModel = new PlotModel
            {
                Title = $"Аппроксимация (3D HeatMap)\n" + title,
                TitleFontSize = 14,
                DefaultFont = "Arial",
                Background = OxyColors.WhiteSmoke,
                PlotAreaBackground = OxyColors.White,
                PlotAreaBorderColor = OxyColors.Black,
            };

            // Оси координат (X1 и X2)
            double x1Min = xData.Min(v => v[0]);
            double x1Max = xData.Max(v => v[0]);
            double x2Min = xData.Min(v => v[1]);
            double x2Max = xData.Max(v => v[1]);

            // Добавляем цветовую ось (значение Y)
            plotModel.Axes.Add(new LinearColorAxis
            {
                Position = AxisPosition.Right,
                Palette = OxyPalettes.Jet(100),
                Title = "F(x1, x2)"
            });

            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X1" });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "X2" });

            // 1. Генерируем тепловую карту аппроксимации
            var heatMapSeries = new HeatMapSeries
            {
                X0 = x1Min,
                X1 = x1Max,
                Y0 = x2Min,
                Y1 = x2Max,
                Interpolate = true,
                RenderMethod = HeatMapRenderMethod.Bitmap,
            };

            int gridRes = 100; // Разрешение сетки
            double[,] data = new double[gridRes, gridRes];

            for (int i = 0; i < gridRes; i++)
            {
                double x1 = x1Min + (x1Max - x1Min) * i / (gridRes - 1);
                for (int j = 0; j < gridRes; j++)
                {
                    double x2 = x2Min + (x2Max - x2Min) * j / (gridRes - 1);
                    // Вычисляем значение Z (или Y в нашей нотации)
                    data[i, j] = Function(new double[] { x1, x2 }, coefficients);
                }
            }

            heatMapSeries.Data = data;
            plotModel.Series.Add(heatMapSeries);

            // 2. Добавляем исходные точки поверх карты (чтобы видеть, где были измерения)
            var scatterSeries = new ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 3,
                MarkerFill = OxyColors.Black, // Точки данных черные
                Title = "Точки данных"
            };

            for (int i = 0; i < xData.Count; i++)
            {
                // Рисуем точку в координатах (x1, x2). 
                // Цвет самой точки не меняем, значение Y видно по фону карты, 
                // но наличие точки показывает ГДЕ было измерение.
                scatterSeries.Points.Add(new ScatterPoint(xData[i][0], xData[i][1]));
            }

            plotModel.Series.Add(scatterSeries);

            SavePlot(fileName, plotModel);
        }

        // Обновленный метод вычисления функции для произвольного числа аргументов
        private double Function(double[] x, Vector<double> coeff)
        {
            double functionValue = 0;

            for (int i = 0; i < _function.Count; i++)
            {
                // Теперь передаем массив x целиком
                functionValue += _function[i].Calculate(x, coeff[i]);
            }

            return functionValue;
        }

        private string BuildStringTitle(Vector<double> coeff)
        {
            string title = "";
            // Ограничим длину заголовка, чтобы он не улетал за края при сложных формулах
            int limit = 4; 
            int count = 0;

            for (int i = 0; i < _function.Count; i++)
            {
                if (Math.Abs(coeff[i]) < 1e-4) continue; // Не пишем слишком малые коэффициенты в заголовок
                if (count >= limit) { title += "..."; break; }

                string sign = coeff[i] >= 0 ? "+" : "-";
                if (title.Length > 0 || sign == "-") // Исправление логики первого знака
                     title += $" {sign} ";

                title += $"{Math.Abs(Math.Round(coeff[i], 3))}*{_function[i]}";
                count++;
            }

            if (string.IsNullOrEmpty(title)) title = "Constant / Zero";

            return title;
        }

        private void SavePlot(string fileName, PlotModel plotModel)
        {
            string directory = Path.Combine(Directory.GetCurrentDirectory(), "result_charts");
            Directory.CreateDirectory(directory);

            string filePath = Path.Combine(directory, fileName);
            using var stream = File.Create(filePath);
            var exporter = new PngExporter { Width = 800, Height = 600 };
            exporter.Export(plotModel, stream);

            Console.WriteLine($"График сохранен: {filePath}");
        }
    }
}