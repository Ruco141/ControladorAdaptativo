using System;

namespace ControladorAdaptativo.Controls
{
    // Selector que devuelve el índice del Semaforo con mayor cola.
    public class MaxQueueSelector : ISemaforoSelector
    {
        public int Select(Semaforo[] semaforos)
        {
            if (semaforos == null || semaforos.Length == 0) return -1;
            int maxIndex = 0;
            int maxValue = semaforos[0].Count();
            for (int i = 1; i < semaforos.Length; i++)
            {
                int v = semaforos[i].Count();
                if (v > maxValue)
                {
                    maxValue = v;
                    maxIndex = i;
                }
            }
            return maxIndex;
        }
    }
}
