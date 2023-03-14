using Assets.Scripts.Constants;
using Photon.Pun;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    public GameConstants.DestructableContents Contains;
    [Tooltip("The chance (between 0 and 1) that this container will spawn the specified item when destroyed.")]
    public float SpawnPowerupChance = 1.0f;

    private bool _initialized;
    private PowerupSpawner _powerupSpawner;
    
    public void SetPowerupSpawner(PowerupSpawner spawner)
    {
        _powerupSpawner = spawner;
        _initialized = true;
    }

    private void OnTriggerEnter(Collider c)
    {
        var explosion = c.GetComponent<Explosion>();
        if (explosion != null)
        {
            if (_initialized && PhotonNetwork.IsMasterClient)
                _powerupSpawner.SpawnPowerup(this);
            Destroy(gameObject);
        }
    }
}
