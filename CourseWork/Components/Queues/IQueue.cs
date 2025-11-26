namespace CourseWork.Components.Queues;

public interface IQueue<T>
{
    int Count { get; }
    bool TryEnqueue(T item);
    T Dequeue();
}