using System.Diagnostics;
using CourseWork.Components.Processors;
using CourseWork.Components.Queues;
using CourseWork.Components.Selectors;
using CourseWork.Components.TimeProviders;
using CourseWork.Core;
using CourseWork.Nodes;

namespace CourseWork.ScalabilityTests
{
    public class SimpleItem { }
    public class TypeAItem : SimpleItem { }
    public class TypeBItem : SimpleItem { }

    public class TypeCheckingSelector<T>(Node<T> nextNode) : INodeSelector<T>
    {
        public Node<T> GetNext(T item)
        {
            if (item is TypeAItem)
            {
                // logic imitation
            }
            else if (item is TypeBItem)
            {
                // logic imitation
            }

            return nextNode;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            SimulationConfig.EnableLogging = false;

            Console.WriteLine("Comparing Simple Network vs Type-Aware Network");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine("| Nodes Count | Simple Time (ms) | Type-Aware Time (ms) |");
            Console.WriteLine("|-------------|------------------|----------------------|");

            int[] nodesCounts = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };

            RunTestScenario(10, false);

            foreach (var count in nodesCounts)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                var simpleTime = RunTestScenario(count, isTypeAware: false);

                GC.Collect();
                GC.WaitForPendingFinalizers();
                var complexTime = RunTestScenario(count, isTypeAware: true);

                Console.WriteLine($"| {count,11} | {simpleTime,16} | {complexTime,20} |");
            }

            Console.WriteLine(new string('-', 60));
            Console.WriteLine("Benchmark finished.");
        }

        static long RunTestScenario(int nodesCount, bool isTypeAware)
        {
            var nodes = new List<Node<SimpleItem>>();

            ProcessNode<SimpleItem>? prevNode = null;
            ProcessNode<SimpleItem>? firstNode = null;

            for (int i = 0; i < nodesCount; i++)
            {
                var processor = new Server<SimpleItem>(1, new ConfigurableTimeProvider<SimpleItem>(_ => 1.0));

                var node = new ProcessNode<SimpleItem>(
                    processor,
                    new FifoQueue<SimpleItem>()
                );

                if (prevNode != null)
                {
                    if (isTypeAware)
                        prevNode.NextNodeSelector = new TypeCheckingSelector<SimpleItem>(node);
                    else
                        prevNode.NextNodeSelector = new DirectSelector<SimpleItem>(node);
                }

                nodes.Add(node);
                prevNode = node;
                if (firstNode == null) firstNode = node;
            }

            var disposeNode = new DisposeNode<SimpleItem>();
            nodes.Add(disposeNode);

            if (isTypeAware)
                prevNode!.NextNodeSelector = new TypeCheckingSelector<SimpleItem>(disposeNode);
            else
                prevNode!.NextNodeSelector = new DirectSelector<SimpleItem>(disposeNode);

            var random = new Random();

            var timeProvider = new ConfigurableTimeProvider<SimpleItem>(_ => 0.1);

            var creator = new CreateNode<SimpleItem>(
                itemFactory: () => isTypeAware
                    ? (random.NextDouble() > 0.5 ? new TypeAItem() : new TypeBItem())
                    : new SimpleItem(),
                intervalProvider: timeProvider
            );

            if (isTypeAware)
                creator.NextNodeSelector = new TypeCheckingSelector<SimpleItem>(firstNode!);
            else
                creator.NextNodeSelector = new DirectSelector<SimpleItem>(firstNode!);

            nodes.Add(creator);

            var model = new SimulationModel<SimpleItem>(nodes);
            var sw = Stopwatch.StartNew();

            model.Run(500);
            sw.Stop();

            return sw.ElapsedMilliseconds;
        }
    }
}
