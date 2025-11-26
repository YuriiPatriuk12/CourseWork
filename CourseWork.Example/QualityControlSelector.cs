using CourseWork.Components.Selectors;
using CourseWork.Core;

namespace CourseWork.Example
{
    public class QualityControlSelector(Node<IDetail> finishedNode, Node<IDetail> defectiveNode) : INodeSelector<IDetail>
    {
        private readonly Random _random = new();

        public Node<IDetail> GetNext(IDetail item)
        {
            double defectProbability = item switch
            {
                SteelDetail => 0.05,
                AluminumDetail => 0.02,
                _ => 0
            };

            return _random.NextDouble() < defectProbability ? defectiveNode : finishedNode;
        }
    }
}
