namespace CourseWork.Components.TimeProviders;

public class ConstantTimeProvider(double time) : ITimeProvider
{
    public double GetTime() => time;
}