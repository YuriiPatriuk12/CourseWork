namespace CourseWork.Components.TimeProviders;

public class ConfigurableTimeProvider<T>(Func<T, double> meanCalculator) : ITimeProvider<T>
{
    private readonly Random _random = new();
    public double GetTime(T item)
    {
        double mean = meanCalculator(item);
        
        return -mean * Math.Log(_random.NextDouble());
    }
}