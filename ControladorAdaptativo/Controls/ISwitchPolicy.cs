using System;

namespace ControladorAdaptativo.Controls
{
    // Policy that decides if the current Semaforo should switch.
    public interface ISwitchPolicy
    {
        bool ShouldSwitch(int currentIndex, Semaforo[] semaforos, TimeSpan greenElapsed);
    }
}
