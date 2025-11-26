namespace CourseWork.Example
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Starting Manufacturing Plant Simulation ---");

            const double simulationTime = 480.0;
            var (model, recorder) = ManufacturingPlantModelBuilder.CreateModel();

            model.Run(simulationTime);

            recorder.PrintFinalStats();

            Console.WriteLine("--- Simulation Complete ---");
        }
    }
}

