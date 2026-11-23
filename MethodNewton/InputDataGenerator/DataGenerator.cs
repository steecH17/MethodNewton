using System;
using MathNet.Numerics;

namespace MethodNewton
{
    public class DataGenerator
    {
        private Random random = new Random();
        private MathParser _parser;
        private MathExpression _function;
        private double _maxNoise;
        private readonly List<int> sign = new List<int> { 1, -1 };
        public DataGenerator(string expression, double maxNoise)
        {
            _parser = new MathParser();
            _function = _parser.Parse(expression);
            _maxNoise = maxNoise;
        }
        public double MaxNoise
        {
            get { return _maxNoise; }
            set { _maxNoise = value; }
        }

        public (List<List<double>>, List<double>) GenerateData2D(int startPoint, int endPoint)
        {
            List<List<double>> xData = new List<List<double>>();
            List<double> yData = new List<double>();
            int dataInd = 0;

            for (int i = startPoint; i < endPoint + 1; i++)
            {
                double randomNoise = sign[random.Next(2)] * random.NextDouble() * _maxNoise;
                xData.Add(new List<double>());
                xData[dataInd].Add(i);
                yData.Add(FunctionValue(i) + randomNoise);
                dataInd++;
            }

            return (xData, yData);
        }

        private double FunctionValue(double argument)
        {
            Dictionary<string, double> variables = new Dictionary<string, double>
            {
                { "x", argument }
            };

            return _function.Evaluate(variables);
        }
    }
}