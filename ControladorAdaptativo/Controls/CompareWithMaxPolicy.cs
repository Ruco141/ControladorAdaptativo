using System;

namespace ControladorAdaptativo.Controls
{
    // Política simple: cambiar si la cola actual es menor que la cola máxima.
    public class CompareWithMaxPolicy : ISwitchPolicy
    {
        private readonly ISemaforoSelector selector;

        public CompareWithMaxPolicy(ISemaforoSelector selector)
        {
            this.selector = selector;
        }

        public bool ShouldSwitch(int currentIndex, Semaforo[] semaforos, TimeSpan greenElapsed)
        {
            if (semaforos == null || semaforos.Length == 0) return false;
            int maxIndex = selector.Select(semaforos);
            if (maxIndex < 0 || maxIndex >= semaforos.Length) return false;
            if (maxIndex == currentIndex) return false;
            return semaforos[currentIndex].Count() < semaforos[maxIndex].Count();
        }
    }
}
