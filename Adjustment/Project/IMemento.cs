namespace Adjustment.Project
{
    internal interface IMemento<TObject> where TObject : class
    {
        public TObject GetState();
    }
}
