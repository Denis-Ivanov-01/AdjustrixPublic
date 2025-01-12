namespace Adjustment
{

    public class IncorrectGeometryAnalysis : Exception
    {
        public IncorrectGeometryAnalysis() { }

        public IncorrectGeometryAnalysis(string message) : base(message) { }

        public IncorrectGeometryAnalysis(string message, Exception inner) : base(message, inner) { }
    }

    public class IncorrectAdjustmentResultException : Exception
    {
        public IncorrectAdjustmentResultException() { }

        public IncorrectAdjustmentResultException(string message) : base(message) { }

        public IncorrectAdjustmentResultException(string message, Exception inner) : base(message, inner) { }
    }
}
