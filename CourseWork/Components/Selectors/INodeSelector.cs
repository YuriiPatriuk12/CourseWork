using CourseWork.Core;

namespace CourseWork.Components.Selectors;

public interface INodeSelector<T>
{
    Node<T> GetNext(T item);
}