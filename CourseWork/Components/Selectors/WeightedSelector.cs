using CourseWork.Components.Selectors;
using CourseWork.Core;

namespace CourseWork.Components.Selectors;

public class WeightedSelector<T>(Func<T, List<(Node<T> Node, double Weight)>> weightProvider) : INodeSelector<T>
{
    private readonly Random _random = new();

    public Node<T> GetNext(T item)
    {
        var weightedNodes = weightProvider(item);

        double totalWeight = weightedNodes.Sum(n => n.Weight);
        if (Math.Abs(totalWeight - 1.0) > 1e-9)
            throw new InvalidOperationException($"Sum of weights for item {item} must be 1.0, but was {totalWeight}");

        double randomValue = _random.NextDouble();
        double cumulativeWeight = 0;

        foreach (var (node, weight) in weightedNodes)
        {
            cumulativeWeight += weight;
            if (randomValue < cumulativeWeight)
            {
                return node;
            }
        }

        return weightedNodes.Last().Node;
    }
}