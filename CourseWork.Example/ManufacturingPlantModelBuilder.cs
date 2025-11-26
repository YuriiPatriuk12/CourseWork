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
            var statRecorder = new ManufacturingStatRecorder();
            var finishedDetailsSink = new DisposeNode<IDetail>(
                onItemDisposed: (detail, time) => statRecorder.RecordFinishedDetail(detail))
            {
                Name = "Finished Goods Storage"
            };

            var defectiveDetailsSink = new DisposeNode<IDetail>(
                onItemDisposed: (detail, time) => statRecorder.RecordDefectiveDetail(detail))
            {
                Name = "Defective Parts Bin"
            };

            var qualityControl = new ProcessNode<IDetail>(
                processor: new SingleChannelProcessor<IDetail>(new ExponentialTimeProvider(2.0)),
                queue: new FifoQueue<IDetail>()
            )
            {
                Name = "Quality Control",
                NextNodeSelector = new QualityControlSelector(finishedDetailsSink, defectiveDetailsSink)
            };

            var steelLine = new ProcessNode<IDetail>(
                processor: new SingleChannelProcessor<IDetail>(new ExponentialTimeProvider(7.0)),
                queue: new LimitedQueue<IDetail>(10)
            )
            {
                Name = "Steel Milling Line",
                NextNodeSelector = new DirectSelector<IDetail>(qualityControl)
            };

            var aluminumLine = new ProcessNode<IDetail>(
                processor: new SingleChannelProcessor<IDetail>(new ExponentialTimeProvider(4.0)),
                queue: new LimitedQueue<IDetail>(10)
            )
            {
                Name = "Aluminum Lathing Line",
                NextNodeSelector = new DirectSelector<IDetail>(qualityControl)
            };
            
            var random = new Random();
            var creator = new CreateNode<IDetail>(
                itemFactory: () => random.NextDouble() < 0.6 ? new SteelDetail() : new AluminumDetail(),
                intervalProvider: new ExponentialTimeProvider(5.0)
            )
            {
                Name = "Raw Material Input",
                NextNodeSelector = new DetailTypeSelector(steelLine, aluminumLine)
            };

            var nodes = new List<Node<IDetail>>
            {
                creator, steelLine, aluminumLine, qualityControl, finishedDetailsSink, defectiveDetailsSink
            };

            var model = new SimulationModel<IDetail>(nodes);

            return (model, statRecorder);
        }
    }
}
