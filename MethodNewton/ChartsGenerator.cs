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
    public static class ChartsGenerator
    {
        public static void PlotAndSave(
            List<double> xData,
            List<double> yData,
            Vector<double> coefficients,
            string fileName = "approximation.png")
        {

            var plotModel = new PlotModel
            {
                Title = $"Аппроксимация данных\nf(x) = x^{coefficients[0]:0.#####} + {coefficients[1]:0.#####}·sin(x) + {coefficients[2]:0.#####}",
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
                pointsSeries.Points.Add(new DataPoint(xData[i], yData[i]));
            }
            plotModel.Series.Add(pointsSeries);

            var functionSeries = new LineSeries
            {
                Title = "Аппроксимация",
                Color = OxyColors.Red,
                StrokeThickness = 2
            };

            double xMin = xData.Min();
            double xMax = xData.Max();
            int pointsCount = 200;

            for (int i = 0; i <= pointsCount; i++)
            {
                double x = xMin + (xMax - xMin) * i / pointsCount;
                double y = Function(x, coefficients[0], coefficients[1], coefficients[2]);
                functionSeries.Points.Add(new DataPoint(x, y));
            }
            plotModel.Series.Add(functionSeries);

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "X",
                TitleFontSize = 12
            });

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Title = "Y",
                TitleFontSize = 12
            });

            SavePlot(fileName, plotModel);
        }

        private static double Function(double x, double a, double b, double c)
        {
            return Math.Pow(x, a) + b * Math.Sin(x) + c;
        }

        private static void SavePlot(string fileName, PlotModel plotModel)
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