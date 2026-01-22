using System;
using MathNet.Numerics.LinearAlgebra;

namespace MethodNewton
{
    public class StatisticModels
    {
        List<FunctionModel> _functionModels;

        public StatisticModels(List<FunctionModel> functionModels)
        {
            _functionModels = functionModels;
        }

        public List<FunctionModel> GetAllModels => _functionModels;

        public FunctionModel? GetMaxDetermModel()
        {
            var maxDeterm = _functionModels.Max(p => p.DeterminationCoeff);
            var maxModels = _functionModels.Where(p => p.DeterminationCoeff == maxDeterm).ToList(); 
            
            return maxModels.MinBy(p => p.CountComponents);
        }

        public FunctionModel? GetMostCompactEquation(double minCoeffDeterm)
        {
            var models = _functionModels.Where(p => p.DeterminationCoeff >= minCoeffDeterm).ToList();
            return models.MinBy(p => p.CountComponents);
        }

        public List<FunctionModel> GetModelsByCoeffDeterm(double minCoeffDeterm)
        {
            return _functionModels.Where(p => p.DeterminationCoeff >= minCoeffDeterm).ToList();
        }
    }
}