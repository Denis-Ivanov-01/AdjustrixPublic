namespace Adjustment
{

    public interface IEdge<TNode, out TEdge>
        where TNode : INode
        where TEdge : IEdge<TNode, TEdge>
    {
        public TNode FromPoint { get; set; }

        public TNode ToPoint { get; set; }

        public double Length { get; set; }

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

        public bool IsEnabled { get; set; } = true;

        public MeasurementBase(TNode FromPoint, TNode ToPoint, double Length, bool IsReversed = false)
        {
            this.FromPoint = FromPoint;
            this.ToPoint = ToPoint;
            this.Length = Length;
            this.IsReversed = IsReversed;
        }

        public MeasurementBase() { }

        public abstract TEdge Reverse();
    }
}
