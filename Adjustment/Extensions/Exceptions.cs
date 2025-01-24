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
        public string pointNumber = "null";

        public HangingPointException()
        {

        }

        public HangingPointException(string message, string pointNumber) : base(message)
        {
            this.pointNumber = pointNumber;
        }

        public HangingPointException(string message) : base(message)
        {

        }

        public HangingPointException(string message, Exception inner) : base(message, inner)
        {

        }
    }

    public class DuplicateMeasurementException : Exception
    {
        public string fromPointNumber = "null";
        public string toPointNumber = "null";

        public DuplicateMeasurementException() { }

        public DuplicateMeasurementException(string message, string fromPointNumber, string toPointNumber) : base(message)
        {
            this.fromPointNumber = fromPointNumber;
            this.toPointNumber = toPointNumber;
        }

        public DuplicateMeasurementException(string message) : base(message)
        {

        }

        public DuplicateMeasurementException(string message, Exception inner) : base(message, inner)
        {

        }
    }

    public class LoopingMeasurementsException : Exception
    {
        public string fromPointNumber = "null";
        public string toPointNumber = "null";

        public LoopingMeasurementsException() { }

        public LoopingMeasurementsException(string message, string fromPointNumber, string toPointNumber) : base(message)
        {
            this.fromPointNumber = fromPointNumber;
            this.toPointNumber = toPointNumber;
        }

        public LoopingMeasurementsException(string message) : base(message)
        {

        }

        public LoopingMeasurementsException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
