namespace LSA_Base
{
    public class IncorrectAdjustmentResultException : Exception
    {
        public IncorrectAdjustmentResultException() { }

        public IncorrectAdjustmentResultException(string message) : base(message) { }

        public IncorrectAdjustmentResultException(string message, Exception inner) : base(message, inner) { }
    }
}
