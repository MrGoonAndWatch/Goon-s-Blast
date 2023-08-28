using Assets.Scripts.Constants;
using Photon.Pun;
using UnityEngine;

public class Destructable : TileWithProperties
{
    [SerializeField]
    private Animator _animationController;

    private DestructibleBlockProperties _properties;

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
        if (explosion != null && explosion.DestroysBlocks())
        {
            if (_initialized && PhotonNetwork.IsMasterClient)
                _powerupSpawner.SpawnPowerup(this);
            Destroy(gameObject);
        }
    }

    public override void SetDefaultProperties()
    {
        _properties = new DestructibleBlockProperties
        {
            Contents = GameConstants.DestructableContents.Nothing,
            SpawnPowerupChance = 0,
        };
        UpdateAnimator();
    }

    public override void LoadProperties(string propertyJson)
    {
        _properties = LoadProperties<DestructibleBlockProperties>(propertyJson);
        UpdateAnimator();
    }

    public GameConstants.DestructableContents GetContents()
    {
        return _properties.Contents;
    }

    public float GetSpawnChance()
    {
        return _properties.SpawnPowerupChance;
    }

    private void UpdateAnimator()
    {
        if (_properties.Contents != GameConstants.DestructableContents.Nothing)
        {
            _animationController.SetBool(GameConstants.AnimationVariables.DestructibleBlockHasItem, true);
        }
    }
}
