namespace CourseWork.Example
{
    public class ManufacturingStatRecorder
    {
        public int FinishedDetails { get; private set; }

        public Dictionary<Type, int> DefectiveDetails { get; } = new()
    {
        { typeof(SteelDetail), 0 },
        { typeof(AluminumDetail), 0 }
    };

        public int TotalDefectiveCount => DefectiveDetails.Sum(pair => pair.Value);
        public int TotalDetailsCreated => FinishedDetails + TotalDefectiveCount;

        public void RecordFinishedDetail(IDetail detail)
        {
            FinishedDetails++;
        }

        public void RecordDefectiveDetail(IDetail detail)
        {
            DefectiveDetails[detail.GetType()]++;
        }

        public void PrintFinalStats()
        {
            Console.WriteLine("--- Manufacturing Final Statistics ---");
            Console.WriteLine($"Total finished details: {FinishedDetails}");
            Console.WriteLine($"Total defective details: {TotalDefectiveCount}");

            if (TotalDetailsCreated > 0)
            {
                double defectivePercentage = (double)TotalDefectiveCount / TotalDetailsCreated;
                Console.WriteLine($"Percentage of defective details: {defectivePercentage:P2}");

                if (TotalDefectiveCount > 0)
                {
                    Console.WriteLine("Defective details breakdown:");
                    double defectiveSteel = (double)DefectiveDetails[typeof(SteelDetail)] / TotalDefectiveCount;
                    double defectiveAluminum = (double)DefectiveDetails[typeof(AluminumDetail)] / TotalDefectiveCount;
                    Console.WriteLine($" - Steel: {defectiveSteel:P2}");
                    Console.WriteLine($" - Aluminum: {defectiveAluminum:P2}");
                }
            }
            Console.WriteLine("--------------------------------------\n");
        }
    }
}
