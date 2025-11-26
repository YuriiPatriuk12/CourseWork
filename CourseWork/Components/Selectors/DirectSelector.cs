using CourseWork.Components.Selectors;
using CourseWork.Core;

namespace CourseWork.Components.Selectors;

public class DirectSelector<T>(Node<T> nextNode) : INodeSelector<T>
{
    public Node<T> GetNext(T item) => nextNode;
}