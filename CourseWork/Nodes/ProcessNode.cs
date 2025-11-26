using CourseWork.Components.Processors;
using CourseWork.Components.Queues;
using CourseWork.Components.Selectors;
using CourseWork.Core;

namespace CourseWork.Nodes;

public class ProcessNode<T>(IProcessor<T> processor, IQueue<T> queue) : Node<T>
{
    public INodeSelector<T>? NextNodeSelector { get; set; }
    public int ProcessedCount { get; private set; }
    public int FailuresCount { get; private set; }
    private double _queueLengthIntegral = 0;

    public override void Enter(T item, double entryTime)
    {
        if (processor.TryStartProcessing(item, CurrentTime))
        {
            return;
        }

        if (queue.TryEnqueue(item))
        {
            return;
        }

        FailuresCount++;
        SimulationConfig.Log($"{CurrentTime:F2}: [{Name}] Failure. Item rejected.");
    }

    public override double GetNextEventTime() => processor.GetCompletionTime();

    public override void ProcessEvent()
    {
        var finishedItem = processor.CompleteProcessing(CurrentTime);
        ProcessedCount++;
        SimulationConfig.Log($"{CurrentTime:F2}: [{Name}] Finished processing item.");

        if (NextNodeSelector != null)
        {
            var nextNode = NextNodeSelector.GetNext(finishedItem);
            nextNode.Enter(finishedItem, CurrentTime);
        }

        if (queue.Count > 0)
        {
            var itemFromQueue = queue.Dequeue();
            processor.TryStartProcessing(itemFromQueue, CurrentTime);
        }
    }

    protected override void OnTimeAdvanced(double deltaTime)
    {
        _queueLengthIntegral += queue.Count * deltaTime;
        processor.AdvanceTime(deltaTime);
    }

    public override void PrintResults()
    {
        double totalEntered = ProcessedCount + FailuresCount + queue.Count + (processor.IsBusy ? 1 : 0);
        SimulationConfig.Log($"--- Results for {Name} ---");
        SimulationConfig.Log($"Total items processed: {ProcessedCount}");
        SimulationConfig.Log($"Total failures (rejected): {FailuresCount}");
        if (totalEntered > 0)
        {
            SimulationConfig.Log($"Failure probability: {FailuresCount / totalEntered:P2}");
        }
        SimulationConfig.Log($"Average queue length: {_queueLengthIntegral / CurrentTime:F2}");
        SimulationConfig.Log($"Processor load: {processor.GetLoad(CurrentTime):P2}");
        SimulationConfig.Log("---------------------------------\n");
    }
}