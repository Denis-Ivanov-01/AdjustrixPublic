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

    public class NetworkNotConnectedException : Exception
    {
        public NetworkNotConnectedException() { }

        public NetworkNotConnectedException(string message) : base(message) { }

        public NetworkNotConnectedException(string message, Exception inner) : base(message, inner)
        {

        }
    }

    public class HangingPointException : Exception
    {
        public HangingPointException()
        {

        }

        public HangingPointException(string message) : base(message)
        {

        }

        public HangingPointException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
