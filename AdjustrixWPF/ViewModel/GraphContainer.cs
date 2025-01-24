using System;
using Adjustment;
using Adjustment.NetworkAnalysis;

namespace AdjustrixWPF.ViewModel
{
    public class GraphContainer
    {//todo: consider using this
        public event Action<Graph<HeightDelta>> GraphChanged;

        public void ChangeGraph(Graph<HeightDelta> graph)
        {
            GraphChanged?.Invoke(graph);
        }
    }
}
