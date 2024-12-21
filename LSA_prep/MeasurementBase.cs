namespace LSA_Base
{

    public interface IEdge<TNode, out TEdge>
        where TNode : INode
        where TEdge : IEdge<TNode, TEdge>
    {
        public TNode FromPoint { get; set; }

        public TNode ToPoint { get; set; }

        public double Length { get; set; }

        //todo: check how to do this shit
        public TEdge Reverse();
    }

    public interface IOneDimMeasurement
    {
        public double Value { get; set; }
    }

    public interface IDirectedMeasurement
    {
        public bool IsReversed { get; set; }
    }

    public abstract class MeasurementBase<TNode, TEdge> : IEdge<TNode, TEdge>, IDirectedMeasurement
        where TNode : INode
        where TEdge : IEdge<TNode, TEdge>, new()

    {
        public abstract TNode FromPoint { get; set; }

        public abstract TNode ToPoint { get; set; }

        public double Length { get; set; }

        public bool IsReversed { get; set; }

        public MeasurementBase(TNode fromPoint, TNode toPoint, double length, bool negative = false)
        {
            FromPoint = fromPoint;
            ToPoint = toPoint;
            Length = length;
            IsReversed = negative;
        }

        public MeasurementBase() { }

        public abstract TEdge Reverse();
    }
}
