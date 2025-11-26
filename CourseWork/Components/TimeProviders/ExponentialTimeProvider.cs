namespace CourseWork.Components.TimeProviders;

public class ExponentialTimeProvider(double mean) : ITimeProvider
{
    private readonly Random _random = new();
    public double GetTime() => -mean * Math.Log(_random.NextDouble());
}