namespace CourseWork.Core
{
    public abstract class Node<T>
    {
        private static int _nextId = 0;
        public int Id { get; } = _nextId++;
        public string Name { get; init; } = $"Node_{_nextId}";
        protected double CurrentTime { get; private set; }

        public abstract double GetNextEventTime();
        public abstract void ProcessEvent();

        public virtual void Enter(T item, double entryTime) { }

        public void UpdateTime(double newTime)
        {
            if (newTime > CurrentTime)
            {
                OnTimeAdvanced(newTime - CurrentTime);
                CurrentTime = newTime;
            }
        }

        protected virtual void OnTimeAdvanced(double deltaTime) { }
        public abstract void PrintResults();
    }
}
