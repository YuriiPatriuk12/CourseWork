namespace CourseWork.Components.Processors;

public class MultiChannelProcessor<T> : IProcessor<T>
{
    private readonly List<IProcessor<T>> _channels;

    public MultiChannelProcessor(List<IProcessor<T>> channels)
    {
        if (channels == null || channels.Count == 0)
            throw new ArgumentException("Channels list cannot be empty");

        _channels = channels;
    }

    public bool IsBusy => _channels.All(c => c.IsBusy);

    public bool TryStartProcessing(T item, double currentTime)
    {
        var freeChannel = _channels.FirstOrDefault(c => !c.IsBusy);

        if (freeChannel != null)
        {
            return freeChannel.TryStartProcessing(item, currentTime);
        }

        return false;
    }

    public T CompleteProcessing(double currentTime)
    {
        var finishedChannel = _channels.FirstOrDefault(c => Math.Abs(c.GetCompletionTime() - currentTime) < 1e-9);

        if (finishedChannel == null)
            throw new InvalidOperationException("No channel finished at this time.");

        return finishedChannel.CompleteProcessing(currentTime);
    }

    public double GetCompletionTime()
    {
        var busyChannels = _channels.Where(c => c.IsBusy).ToList();

        if (busyChannels.Count == 0)
            return double.PositiveInfinity;

        return busyChannels.Min(c => c.GetCompletionTime());
    }

    public void AdvanceTime(double deltaTime)
    {
        foreach (var channel in _channels)
        {
            channel.AdvanceTime(deltaTime);
        }
    }

    public double GetLoad(double totalTime)
    {
        if (totalTime <= 0) return 0;
        return _channels.Average(c => c.GetLoad(totalTime));
    }
}