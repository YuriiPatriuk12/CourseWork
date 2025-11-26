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
                var nextEventTime = _nodes.Min(n => n.GetNextEventTime());

                if (double.IsPositiveInfinity(nextEventTime))
                {
                    SimulationConfig.Log("No more events. Simulation stopped.");
                    break;
                }

                _currentTime = nextEventTime;

                foreach (var node in _nodes)
                {
                    node.UpdateTime(_currentTime);
                }

                foreach (var node in _nodes)
                {
                    if (Math.Abs(node.GetNextEventTime() - _currentTime) < 1e-9)
                    {
                        node.ProcessEvent();
                    }
                }
            }
            SimulationConfig.Log($"\n--- Simulation Finished at time: {_currentTime:F2} ---\n");

            foreach (var node in _nodes)
            {
                node.PrintResults();
            }
        }
    }
}
