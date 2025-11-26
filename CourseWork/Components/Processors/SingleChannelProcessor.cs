
using CourseWork.Components.TimeProviders;

namespace CourseWork.Components.Processors;

public class SingleChannelProcessor<T>(ITimeProvider serviceTimeProvider) : IProcessor<T>
{
    private T? _currentItem;
    private double _completionTime = double.PositiveInfinity;
    private double _totalWorkTime = 0;

    public bool IsBusy => _currentItem != null;

    public bool TryStartProcessing(T item, double currentTime)
    {
        if (IsBusy) return false;

        _currentItem = item;
        double serviceTime = serviceTimeProvider.GetTime();
        _completionTime = currentTime + serviceTime;
        _totalWorkTime += serviceTime;
        return true;
    }

    public T CompleteProcessing(double currentTime)
    {
        if (!IsBusy) throw new InvalidOperationException("Processor is not busy.");

        var processedItem = _currentItem!;
        _currentItem = default;
        _completionTime = double.PositiveInfinity;
        return processedItem;
    }

    public double GetCompletionTime() => _completionTime;
    public void AdvanceTime(double deltaTime) { }
    public double GetLoad(double totalTime) => totalTime > 0 ? _totalWorkTime / totalTime : 0;
}