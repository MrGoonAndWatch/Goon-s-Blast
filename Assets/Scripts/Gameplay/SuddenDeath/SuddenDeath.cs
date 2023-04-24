using UnityEngine;

public abstract class SuddenDeath : MonoBehaviour
{
    protected bool SuddenDeathStarted;

    public void StartSuddenDeath()
    {
        SuddenDeathStarted = true;
    }
}
