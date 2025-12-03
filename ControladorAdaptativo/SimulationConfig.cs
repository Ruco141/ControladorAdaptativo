using System;

namespace ControladorAdaptativo
{
    public record SimulationConfig(
        int SemaforoCount,
        TimeSpan Duration,
        int TickMs,
        int DequeueIntervalMs,
        int MinArrivalMs,
        int MaxArrivalMs,
        string[] SemaforoNames
    );
}