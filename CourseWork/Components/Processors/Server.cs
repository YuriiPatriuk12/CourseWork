using System;
using System.Collections.Generic;
using System.Linq;
using CourseWork.Components.TimeProviders;

namespace CourseWork.Components.Processors;

public class Server<T>(int channelsCount, ITimeProvider<T> timeProvider) : IProcessor<T>
{
    private readonly List<(T Item, double FinishTime)> _activeJobs = new();
    private double _totalBusyTime = 0;
    private double _lastUpdateTime = 0;

    public bool IsBusy => _activeJobs.Count >= channelsCount;

    public bool TryStartProcessing(T item, double currentTime)
    {
        UpdateStats(currentTime);

        if (_activeJobs.Count >= channelsCount)
            return false;

        double duration = timeProvider.GetTime(item);
        double finishTime = currentTime + duration;

        _activeJobs.Add((item, finishTime));
        _activeJobs.Sort((a, b) => a.FinishTime.CompareTo(b.FinishTime));

        return true;
    }

    public T CompleteProcessing(double currentTime)
    {
        UpdateStats(currentTime);

        var finishedJobIndex = _activeJobs.FindIndex(j => Math.Abs(j.FinishTime - currentTime) < 1e-9);

        if (finishedJobIndex == -1)
            throw new InvalidOperationException("No job finished at this time.");

        var item = _activeJobs[finishedJobIndex].Item;
        _activeJobs.RemoveAt(finishedJobIndex);

        return item;
    }

    public double GetCompletionTime()
    {
        if (_activeJobs.Count == 0) return double.PositiveInfinity;
        return _activeJobs[0].FinishTime;
    }

    public void AdvanceTime(double deltaTime)
    {}

    public double GetLoad(double totalTime)
    {
        return totalTime > 0 ? _totalBusyTime / (totalTime * channelsCount) : 0;
    }

    private void UpdateStats(double currentTime)
    {
        double deltaTime = currentTime - _lastUpdateTime;
        if (deltaTime > 0)
        {
            _totalBusyTime += _activeJobs.Count * deltaTime;
        }
        _lastUpdateTime = currentTime;
    }
}