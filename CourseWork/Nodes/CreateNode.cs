using CourseWork.Components.Selectors;
using CourseWork.Components.TimeProviders;
using CourseWork.Core;

namespace CourseWork.Nodes;

public class CreateNode<T>(Func<T> itemFactory, ITimeProvider intervalProvider) : Node<T>
{
    public INodeSelector<T>? NextNodeSelector { get; set; }

    private double _nextCreationTime = 0;
    public int CreatedCount { get; private set; }

    public override double GetNextEventTime() => _nextCreationTime;

    public override void ProcessEvent()
    {
        if (NextNodeSelector is null)
        {
            throw new InvalidOperationException($"CreateNode '{Name}' has no NextNodeSelector configured.");
        }

        var newItem = itemFactory();
        CreatedCount++;
        SimulationConfig.Log($"{CurrentTime:F2}: [{Name}] Created item {newItem}");

        var nextNode = NextNodeSelector.GetNext(newItem);
        nextNode.Enter(newItem, CurrentTime);

        _nextCreationTime = CurrentTime + intervalProvider.GetTime();
    }

    public override void PrintResults()
    {
        SimulationConfig.Log($"--- Results for {Name} ---");
        SimulationConfig.Log($"Total items created: {CreatedCount}");
        SimulationConfig.Log("---------------------------------\n");
    }
}