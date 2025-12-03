using System;
using ControladorAdaptativo.Controls;

namespace ControladorAdaptativo
{
    public class Program
    {
        // 1) Crear semáforos
        // 2) Sembrar una cantidad inicial con ArrivalGenerator
        // 3) Crear selector/política y ejecutar TrafficRunner durante un tiempo
        // 4) Mostrar colas finales
        public static void Main()
        {
            int semaforoCount = 4;
            var semaforos = new Semaforo[semaforoCount];
            for (int i = 0; i < semaforoCount; i++) semaforos[i] = new Semaforo();

            var generator = new ArrivalGenerator();


            // Seed inicial: hasta 10 coches por semáforo
            var initial = generator.SeedInitial(semaforos, 10);

            // Configuración
            var config = new SimulationConfig(
                SemaforoCount: semaforoCount,
                Duration: TimeSpan.FromSeconds(10),
                TickMs: 100,
                DequeueIntervalMs: 700,
                MinArrivalMs: 100,
                MaxArrivalMs: 300,
                SemaforoNames: new[] { "NS", "SN", "EO", "OE" }
            );


            // Mostrar vehículos iniciales por semáforo
            Console.WriteLine("Vehículos iniciales por semáforo:");
            for (int i = 0; i < initial.Length; i++)
            {
                string nm = (i < config.SemaforoNames.Length) ? config.SemaforoNames[i] : $"#{i}";
                Console.WriteLine($"  {nm}: {initial[i]}");
            }

            Console.WriteLine("Iniciando simulación");


            // Selector y política
            var selector = new MaxQueueSelector();
            var policy = new CompareWithMaxPolicy(selector);


            // Orquestador que procesa semáforos según la política
            var runner = new TrafficRunner(semaforos, selector, policy, generator);
            var summary = runner.Run(config);


            // Mostrar resultados finales
            Console.WriteLine();
            Console.WriteLine("Resumen:");
            Console.WriteLine($"  Total llegadas: {summary.TotalArrivals}");
            Console.WriteLine($"  Total salidas: {summary.TotalDepartures}");
            Console.WriteLine("  Por semáforo (llegadas / salidas / cola restante):");
            for (int i = 0; i < semaforoCount; i++)
            {
                string nm = (i < config.SemaforoNames.Length) ? config.SemaforoNames[i] : $"#{i}";
                Console.WriteLine($"    {nm}: {summary.ArrivalsPerSemaforo[i]} / {summary.DeparturesPerSemaforo[i]} / {summary.FinalQueue[i]}");
            }

            Console.WriteLine("Simulación terminada.");
        }
    }
}

