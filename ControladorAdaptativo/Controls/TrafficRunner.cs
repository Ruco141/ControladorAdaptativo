using System;
using System.Diagnostics;

namespace ControladorAdaptativo.Controls
{
    public record SimulationSummary
    {
        public int TotalArrivals { get; init; }
        public int TotalDepartures { get; init; }
        public int[] ArrivalsPerSemaforo { get; init; } = Array.Empty<int>();
        public int[] DeparturesPerSemaforo { get; init; } = Array.Empty<int>();
        public int[] FinalQueue { get; init; } = Array.Empty<int>();
        public TimeSpan Duration { get; init; }
    }

    // Orquestador mínimo que genera llegadas durante la simulación
    // y procesa Semaforo según la política. Devuelve un resumen al terminar.
    public class TrafficRunner
    {
        private readonly Semaforo[] semaforos;
        private readonly ISemaforoSelector selector;
        private readonly ISwitchPolicy policy;
        private readonly ArrivalGenerator generator;
        private readonly Random rnd = new();

        // Nombres fijos de la intersección (siempre se usan si no se pasa config)
        private static readonly string[] DefaultNames = new[] { "NS", "SN", "EO", "OE" };

        public TrafficRunner(Semaforo[] semaforos, ISemaforoSelector selector, ISwitchPolicy policy, ArrivalGenerator generator)
        {
            this.semaforos = semaforos;
            this.selector = selector;
            this.policy = policy;
            this.generator = generator;
        }

        // Ejecuta la simulación usando SimulationConfig y devuelve un resumen.
        public SimulationSummary Run(SimulationConfig config)
        {
            var stopwatch = Stopwatch.StartNew();
            int current = 0;
            int lastDequeueMs = 0;
            int greenElapsedMs = 0;

            int nextArrivalDueMs = rnd.Next(config.MinArrivalMs, config.MaxArrivalMs + 1);
            int n = semaforos.Length;

            // Contadores para reportes
            int totalArrivals = 0;
            int totalDepartures = 0;
            int[] arrivalsPerSemaforo = new int[n];
            int[] departuresPerSemaforo = new int[n];

            var names = config.SemaforoNames ?? DefaultNames;

            while (stopwatch.Elapsed < config.Duration)
            {
                int remaining = (int)(config.Duration - stopwatch.Elapsed).TotalMilliseconds;
                int sleep = Math.Min(config.TickMs, Math.Max(0, remaining));
                Thread.Sleep(sleep);

                lastDequeueMs += sleep;
                greenElapsedMs += sleep;
                nextArrivalDueMs -= sleep;

                // Generar llegadas globales cuando toque
                while (nextArrivalDueMs <= 0)
                {
                    var (id, idx) = generator.AddRandomArrival(semaforos);
                    totalArrivals++;
                    if (idx >= 0 && idx < n) arrivalsPerSemaforo[idx]++;
                    nextArrivalDueMs += rnd.Next(config.MinArrivalMs, config.MaxArrivalMs + 1);
                }

                // Intentar sacar vehículo durante el verde
                if (lastDequeueMs >= config.DequeueIntervalMs)
                {
                    if (semaforos[current].TryDequeue(out var id))
                    {
                        totalDepartures++;
                        departuresPerSemaforo[current]++;
                    }
                    lastDequeueMs = 0;
                }

                // Evaluar cambio de semáforo según la policy
                if (policy.ShouldSwitch(current, semaforos, TimeSpan.FromMilliseconds(greenElapsedMs)))
                {
                    int next = selector.Select(semaforos);
                    if (next >= 0 && next < semaforos.Length && next != current)
                    {
                        current = next;
                        greenElapsedMs = 0;
                        lastDequeueMs = 0;
                    }
                }
            }

            stopwatch.Stop();

            int[] finalQueue = new int[n];
            for (int i = 0; i < n; i++) finalQueue[i] = semaforos[i].Count();

            return new SimulationSummary
            {
                TotalArrivals = totalArrivals,
                TotalDepartures = totalDepartures,
                ArrivalsPerSemaforo = arrivalsPerSemaforo,
                DeparturesPerSemaforo = departuresPerSemaforo,
                FinalQueue = finalQueue,
                Duration = stopwatch.Elapsed
            };
        }
    }
}
