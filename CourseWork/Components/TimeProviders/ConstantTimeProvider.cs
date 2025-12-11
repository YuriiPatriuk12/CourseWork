namespace CourseWork.Components.TimeProviders;

public class ConstantTimeProvider<T>(double time) : ITimeProvider<T>
{
    public double GetTime(T item) => time;
}