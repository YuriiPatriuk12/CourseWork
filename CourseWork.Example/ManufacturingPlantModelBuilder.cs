using CourseWork.Components.Processors;
using CourseWork.Components.Queues;
using CourseWork.Components.Selectors;
using CourseWork.Components.TimeProviders;
using CourseWork.Core;
using CourseWork.Nodes;

namespace CourseWork.Example;

public static class ManufacturingPlantModelBuilder
{
    public static (SimulationModel<IDetail> Model, ManufacturingStatRecorder Recorder) CreateModel()
    {
        var recorder = new ManufacturingStatRecorder();

        var warehouse = new DisposeNode<IDetail>((d, t) => recorder.RecordFinishedDetail(d))
        { Name = "Warehouse (Finished Goods)" };

        var scrapYard = new DisposeNode<IDetail>((d, t) => recorder.RecordDefectiveDetail(d))
        { Name = "Scrap Yard (Defective)" };

        var line1TimeLogic = new ConfigurableTimeProvider<IDetail>(item =>
        {
            if (item is SteelDetail) return 7.0;
            if (item is AluminumDetail) return 10.0;
            return 5.0;
        });

        var line1Processor = new Server<IDetail>(3, line1TimeLogic);

        var line1 = new ProcessNode<IDetail>(line1Processor, new FifoQueue<IDetail>())
        {
            Name = "Line 1"
        };

        var line2TimeLogic = new ConfigurableTimeProvider<IDetail>(item =>
        {
            if (item is AluminumDetail) return 4.0;
            if (item is SteelDetail) return 20.0;
            return 4.0;
        });

        var line2Processor = new Server<IDetail>(1, line2TimeLogic);

        var line2 = new ProcessNode<IDetail>(
            line2Processor,
            new LimitedQueue<IDetail>(3)
        )
        {
            Name = "Line 2"
        };

        line2.OverflowNode = line1;

        var qualityTimeLogic = new ConfigurableTimeProvider<IDetail>(_ => 2.0);

        var qualityControl = new ProcessNode<IDetail>(
            new Server<IDetail>(1, qualityTimeLogic),
            new FifoQueue<IDetail>()
        )
        {
            Name = "Quality Control"
        };

        var random = new Random();
        var creationTimeLogic = new ConfigurableTimeProvider<IDetail>(_ => 5.0);

        var creator = new CreateNode<IDetail>(
            itemFactory: () => random.NextDouble() < 0.6 ? new SteelDetail() : new AluminumDetail(),
            intervalProvider: creationTimeLogic
        )
        {
            Name = "Source"
        };

        creator.NextNodeSelector = new DetailTypeSelector(line1, line2);

        line1.NextNodeSelector = new DirectSelector<IDetail>(qualityControl);
        line2.NextNodeSelector = new DirectSelector<IDetail>(qualityControl);

        qualityControl.NextNodeSelector = new QualityControlFeedbackSelector(
            success: warehouse,
            scrap: scrapYard,
            rework: line1
        );

        var nodes = new List<Node<IDetail>> { creator, line1, line2, qualityControl, warehouse, scrapYard };

        return (new SimulationModel<IDetail>(nodes), recorder);
    }

    private class QualityControlFeedbackSelector(Node<IDetail> success, Node<IDetail> scrap, Node<IDetail> rework)
        : INodeSelector<IDetail>
    {
        private readonly Random _rnd = new();
        public Node<IDetail> GetNext(IDetail item)
        {
            double val = _rnd.NextDouble();
            if (val < 0.90) return success;
            if (val < 0.95) return scrap;
            return rework;
        }
    }
}