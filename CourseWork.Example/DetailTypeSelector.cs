using CourseWork.Components.Selectors;
using CourseWork.Core;

namespace CourseWork.Example
{
    public class DetailTypeSelector(Node<IDetail> steelLine, Node<IDetail> aluminumLine) : INodeSelector<IDetail>
    {
        public Node<IDetail> GetNext(IDetail item)
        {
            return item switch
            {
                SteelDetail => steelLine,
                AluminumDetail => aluminumLine,
                _ => throw new NotSupportedException("Unknown detail type")
            };
        }
    }
}
