using System;
using System.Collections.Generic;
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
        { Name = "Warehouse (Finished)" };

        var scrapYard = new DisposeNode<IDetail>((d, t) => recorder.RecordDefectiveDetail(d))
        { Name = "Scrap Yard (Defective)" };

        var line1TimeLogic = new ConfigurableTimeProvider<IDetail>(item =>
        {
            if (item is SteelDetail) return 7.0;
            if (item is CompositeDetail) return 12.0;
            return 8.0;
        });

        var line1 = new ProcessNode<IDetail>(
            new Server<IDetail>(3, line1TimeLogic), 
            new FifoQueue<IDetail>()
        )
        { Name = "Line 1 (Heavy Duty)" };

        var line2TimeLogic = new ConfigurableTimeProvider<IDetail>(item =>
        {
            if (item is AluminumDetail) return 4.0;
            if (item is PlasticDetail) return 2.0;
            return 5.0;
        });

        var line2 = new ProcessNode<IDetail>(
            new Server<IDetail>(2, line2TimeLogic), 
            new LimitedQueue<IDetail>(5)
        )
        { Name = "Line 2 (Light Duty)" };

        line2.OverflowNode = line1;

        var line3TimeLogic = new ConfigurableTimeProvider<IDetail>(item =>
        {
            if (item is WoodDetail) return 15.0; 
            return 3.0;
        });

        var line3 = new ProcessNode<IDetail>(
            new Server<IDetail>(1, line3TimeLogic),
            new FifoQueue<IDetail>()
        )
        { Name = "Line 3 (Special/Paint)" };

        var qualityControlSelector = new WeightedSelector<IDetail>(item =>
        {
            if (item is PlasticDetail)
            {
                return newList((warehouse, 0.80), (scrapYard, 0.15), (line2, 0.05));
            }
            if (item is SteelDetail)
            {
                return newList((warehouse, 0.95), (scrapYard, 0.02), (line1, 0.03));
            }
            return newList((warehouse, 0.90), (scrapYard, 0.05), (line3, 0.05));
        });

        var qualityNode = new ProcessNode<IDetail>(
             new Server<IDetail>(1, new ConfigurableTimeProvider<IDetail>(_ => 2.0)),
             new FifoQueue<IDetail>()
        )
        { Name = "Quality Control Node" };

        qualityNode.NextNodeSelector = qualityControlSelector;


        var random = new Random();
        var creationTimeLogic = new ConfigurableTimeProvider<IDetail>(_ => 3.0);

        var creator = new CreateNode<IDetail>(
            itemFactory: () =>
            {
                double r = random.NextDouble();
                if (r < 0.2) return new SteelDetail();
                if (r < 0.4) return new AluminumDetail();
                if (r < 0.6) return new WoodDetail();
                if (r < 0.8) return new PlasticDetail();
                return new CompositeDetail();
            },
            intervalProvider: creationTimeLogic
        )
        { Name = "Source (Raw Materials)" };


        creator.NextNodeSelector = new WeightedSelector<IDetail>(item =>
        {
            if (item is SteelDetail || item is CompositeDetail)
                return newList((line1, 1.0)); 

            if (item is AluminumDetail || item is PlasticDetail)
                return newList((line2, 1.0)); 

            return newList((line3, 1.0));
        });

        line1.NextNodeSelector = new DirectSelector<IDetail>(qualityNode);
        line2.NextNodeSelector = new DirectSelector<IDetail>(qualityNode);
        line3.NextNodeSelector = new DirectSelector<IDetail>(qualityNode);

        var nodes = new List<Node<IDetail>> {
            creator, line1, line2, line3, qualityNode, warehouse, scrapYard
        };

        return (new SimulationModel<IDetail>(nodes), recorder);
    }

    private static List<(Node<IDetail> Node, double Weight)> newList(params (Node<IDetail>, double)[] items)
    {
        return [.. items];
    }
}