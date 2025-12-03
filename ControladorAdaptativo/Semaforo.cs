using System;
using System.Collections;
using System.Text;

namespace ControladorAdaptativo
{
    public class Semaforo
    {
        private Queue<int> queue = new();
        public bool State { get; private set; }        // false = rojo, true = verde

        public Semaforo(bool initialState = false)
        {
            State = initialState;
        }


        // Agrega un vehiculo (elemento) a la cola
        public void AddElement(int element)
        {
            queue.Enqueue(element);
        }


        // Extrae un elemento si hay alguno
        public bool TryDequeue(out int element)
        {
            if (queue.Count > 0)
            {
                element = queue.Dequeue();
                return true;
            }

            element = -1;
            return false;
        }


        // Devuelve el número de elementos en la cola
        public int Count()
        {
            return queue.Count;
        }


        // Vacía la cola por completo
        public void Clear()
        {
            queue.Clear();
        }


        // Cambia el estado del semáforo
        public void SetState(bool newState)
        {
            State = newState;
        }
    }
}
