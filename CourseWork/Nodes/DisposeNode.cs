using CourseWork.Core;

namespace CourseWork.Nodes;

public class DisposeNode<T> : Node<T>
{
    private readonly Action<T, double>? _onItemDisposed;
    public int DisposedCount { get; private set; }

    public DisposeNode(Action<T, double>? onItemDisposed = null)
    {
        _onItemDisposed = onItemDisposed;
    }

    public override void Enter(T item, double entryTime)
    {
        DisposedCount++;
        SimulationConfig.Log($"{CurrentTime:F2}: [{Name}] Item disposed.");

        _onItemDisposed?.Invoke(item, CurrentTime);
    }

    public override double GetNextEventTime() => double.PositiveInfinity;
    public override void ProcessEvent() { } 

    public override void PrintResults()
    {
        SimulationConfig.Log($"--- Results for {Name} ---");
        SimulationConfig.Log($"Total items disposed: {DisposedCount}");
        SimulationConfig.Log("---------------------------------\n");
    }
}