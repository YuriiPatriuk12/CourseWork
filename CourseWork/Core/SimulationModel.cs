namespace CourseWork.Core
{
    public class SimulationModel<T>
    {
        private readonly List<Node<T>> _nodes;
        private double _currentTime = 0;

        public SimulationModel(List<Node<T>> nodes)
        {
            _nodes = nodes;
        }

        public void Run(double simulationTime)
        {
            SimulationConfig.Log("--- Simulation Started ---");
            while (_currentTime < simulationTime)
            {
                var nextEvent = _nodes
                    .Select(n => new { Node = n, Time = n.GetNextEventTime() })
                    .OrderBy(x => x.Time)
                    .FirstOrDefault();

                if (nextEvent == null || double.IsPositiveInfinity(nextEvent.Time))
                {
                    SimulationConfig.Log("No more events. Simulation stopped.");
                    break;
                }

                _currentTime = nextEvent.Time;

                foreach (var node in _nodes)
                {
                    node.UpdateTime(_currentTime);
                }

                nextEvent.Node.ProcessEvent();
            }
            SimulationConfig.Log($"\n--- Simulation Finished at time: {_currentTime:F2} ---\n");

            foreach (var node in _nodes)
            {
                node.PrintResults();
            }
        }
    }
}
