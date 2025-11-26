namespace CourseWork.Core
{
    public static class SimulationConfig
    {
        public static bool EnableLogging { get; set; } = true;

        public static void Log(string message)
        {
            if (EnableLogging)
            {
                Console.WriteLine(message);
            }
        }
    }
}
