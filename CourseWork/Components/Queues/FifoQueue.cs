namespace CourseWork.Components.Queues;

public class FifoQueue<T> : IQueue<T>
{
    private readonly Queue<T> _queue = new();
    public int Count => _queue.Count;
    public bool TryEnqueue(T item)
    {
        _queue.Enqueue(item);
        return true;
    }
    public T Dequeue() => _queue.Dequeue();
}