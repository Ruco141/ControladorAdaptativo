using System;

namespace ControladorAdaptativo.Controls
{
    // Selecciona el índice del Semaforo objetivo
    public interface ISemaforoSelector
    {
        int Select(Semaforo[] semaforos);
    }
}
