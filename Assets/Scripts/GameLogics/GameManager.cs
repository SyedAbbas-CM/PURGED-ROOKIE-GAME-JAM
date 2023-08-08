using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public PathManager pathManager;

    private void Start()
    {
        // Simulating the end of the setup phase.
        // In a real scenario, you'd transition between phases based on player input or other triggers.
        EndSetupPhase();
    }

    void EndSetupPhase()
    {
        //pathManager.CalculatePrimaryPath();
        // Start spawning enemies or begin the combat phase here.
    }
}