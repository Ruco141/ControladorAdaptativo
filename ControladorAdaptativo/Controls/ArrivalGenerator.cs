using System;

namespace ControladorAdaptativo.Controls
{
    // Crea id para cada elemento y encola las llegadas en la instancia de semáforo.
    public class ArrivalGenerator
    {
        private readonly Random random = new();
        private int nextId = 1;

        // Inserta entre 0 y maxInitial elementos aleatorios por semáforo.
        public int[] SeedInitial(Semaforo[] semaforos, int maxInitial)
        {
            int[] initial = new int[semaforos.Length];
            for (int i = 0; i < semaforos.Length; i++)
            {
                int quantity = random.Next(0, maxInitial + 1);
                for (int c = 0; c < quantity; c++)
                {
                    semaforos[i].AddElement(nextId++);
                }
                initial[i] = quantity;
            }
            return initial;
        }

        // Encola una llegada aleatoria y retorna (id, index).
        public (int id, int index) AddRandomArrival(Semaforo[] semaforos)
        {
            int index = random.Next(semaforos.Length);
            int id = nextId++;
            semaforos[index].AddElement(id);
            return (id, index);
        }
    }
}