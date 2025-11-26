namespace CourseWork.Components.Processors;

public interface IProcessor<T>
{
    bool IsBusy { get; }
    bool TryStartProcessing(T item, double currentTime);
    T CompleteProcessing(double currentTime);
    double GetCompletionTime();
    void AdvanceTime(double deltaTime);
    double GetLoad(double totalTime);
}