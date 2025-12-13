using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseWork.Example
{
    public class ManufacturingStatRecorder
    {
        public int FinishedDetails { get; private set; }

        public Dictionary<Type, int> DefectiveDetails { get; } = new()
        {
            { typeof(SteelDetail), 0 },
            { typeof(AluminumDetail), 0 },
            { typeof(WoodDetail), 0 },
            { typeof(PlasticDetail), 0 },
            { typeof(CompositeDetail), 0 }
        };

        public int TotalDefectiveCount => DefectiveDetails.Sum(pair => pair.Value);
        public int TotalDetailsCreated => FinishedDetails + TotalDefectiveCount;

        public void RecordFinishedDetail(IDetail detail)
        {
            FinishedDetails++;
        }

        public void RecordDefectiveDetail(IDetail detail)
        {
            if (DefectiveDetails.ContainsKey(detail.GetType()))
            {
                DefectiveDetails[detail.GetType()]++;
            }
            else
            {
                DefectiveDetails[detail.GetType()] = 1;
            }
        }

        public void PrintFinalStats()
        {
            Console.WriteLine("--- Manufacturing Final Statistics ---");
            Console.WriteLine($"Total finished details: {FinishedDetails}");
            Console.WriteLine($"Total defective details: {TotalDefectiveCount}");

            if (TotalDetailsCreated > 0)
            {
                double totalDefectivePercentage = (double)TotalDefectiveCount / TotalDetailsCreated;
                Console.WriteLine($"Total Defective Percentage: {totalDefectivePercentage:P2}");

                if (TotalDefectiveCount > 0)
                {
                    Console.WriteLine("Defective details breakdown by type:");

                    foreach (var pair in DefectiveDetails)
                    {
                        double typePercentage = (double)pair.Value / TotalDefectiveCount;
                        Console.WriteLine($" - {pair.Key.Name}: {typePercentage:P2} ({pair.Value} pcs)");
                    }
                }
            }
            Console.WriteLine("--------------------------------------\n");
        }
    }
}