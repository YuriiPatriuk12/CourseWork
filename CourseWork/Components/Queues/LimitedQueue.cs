using CourseWork.Components.Queues;

namespace CourseWork.Components.Queues;

public class LimitedQueue<T>(int maxSize) : IQueue<T>
{
    private readonly Queue<T> _queue = new();
    public int Count => _queue.Count;
    public bool TryEnqueue(T item)
    {
        if (_queue.Count >= maxSize)
            return false;

        _queue.Enqueue(item);
        return true;
    }
    public T Dequeue() => _queue.Dequeue();
}