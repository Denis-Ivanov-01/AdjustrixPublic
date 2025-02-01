namespace AdjustrixBase.DataModels;


public interface IEdge<TNode>
    where TNode : INode
    
{
    public TNode FromPoint { get; set; }

    public TNode ToPoint { get; set; }

    public double Length { get; set; }

    public IEdge<TNode> Reverse();
}

public interface IOneDimMeasurement
{
    public double Value { get; set; }
}

public interface IDirectedMeasurement
{
    public bool IsReversed { get; set; }
}

public interface IDeactivatable
{
    public bool IsEnabled { get; set; }
}

public abstract class MeasurementBase<TNode> : IEdge<TNode>, IDirectedMeasurement, IDeactivatable
    where TNode : INode
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

    public abstract IEdge<TNode> Reverse();
}
