using CourseWork.Components.Processors;
using CourseWork.Components.Queues;
using CourseWork.Components.Selectors;
using CourseWork.Components.TimeProviders;
using CourseWork.Core;
using CourseWork.Nodes;

namespace CourseWork.Example
{
    public static class ManufacturingPlantModelBuilder
    {
        public static (SimulationModel<IDetail> Model, ManufacturingStatRecorder Recorder) CreateModel()
        {
            var recorder = new ManufacturingStatRecorder();

            var warehouse = new DisposeNode<IDetail>((d, t) => recorder.RecordFinishedDetail(d))
            { Name = "Warehouse (Finished Goods)" };

            var scrapYard = new DisposeNode<IDetail>((d, t) => recorder.RecordDefectiveDetail(d))
            { Name = "Scrap Yard (Defective)" };

            var line1Processor = new MultiChannelProcessor<IDetail>(new List<IProcessor<IDetail>>
            {
                new SingleChannelProcessor<IDetail>(new ExponentialTimeProvider(7.0)),
                new SingleChannelProcessor<IDetail>(new ExponentialTimeProvider(7.0)),
                new SingleChannelProcessor<IDetail>(new ExponentialTimeProvider(7.0))
            });

            var line1 = new ProcessNode<IDetail>(line1Processor, new FifoQueue<IDetail>())
            {
                Name = "Line 1 (Steel/Rework - 3 Channels)"
            };

            var line2 = new ProcessNode<IDetail>(
                new SingleChannelProcessor<IDetail>(new ExponentialTimeProvider(4.0)),
                new LimitedQueue<IDetail>(3)
            )
            {
                Name = "Line 2 (Aluminum - Limited)"
            };

            line2.OverflowNode = line1;

            var qualityControl = new ProcessNode<IDetail>(
                new SingleChannelProcessor<IDetail>(new ExponentialTimeProvider(2.0)),
                new FifoQueue<IDetail>()
            )
            {
                Name = "Quality Control"
            };

            var random = new Random();
            var creator = new CreateNode<IDetail>(
                () => random.NextDouble() < 0.6 ? new SteelDetail() : new AluminumDetail(),
                new ExponentialTimeProvider(5.0)
            )
            {
                Name = "Source (Raw Material)"
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
}
