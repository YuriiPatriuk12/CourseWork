using CourseWork.Components.Selectors;
using CourseWork.Core;

namespace CourseWork.Components.Selectors;

public class WeightedSelector<T> : INodeSelector<T>
{
    private readonly List<(Node<T> Node, double Weight)> _nodes;
    private readonly Random _random = new();

    public WeightedSelector(List<(Node<T> Node, double Weight)> nodes)
    {
        if (Math.Abs(nodes.Sum(n => n.Weight) - 1.0) > 1e-9)
            throw new ArgumentException("Sum of weights must be 1.");

        _nodes = nodes;
    }

    public Node<T> GetNext(T item)
    {
        double randomValue = _random.NextDouble();
        double cumulativeWeight = 0;

        foreach (var (node, weight) in _nodes)
        {
            cumulativeWeight += weight;
            if (randomValue < cumulativeWeight)
            {
                return node;
            }
        }
        return _nodes.Last().Node;
    }
}