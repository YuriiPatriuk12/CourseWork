namespace CourseWork.Components.TimeProviders;

public interface ITimeProvider<T>
{
    double GetTime(T item);
}