using UnityEngine;

public class BombMesh : MonoBehaviour
{
    [SerializeField] private Bomb _parentBomb;

    private void Start()
    {
        if (_parentBomb.IsMine()) return;

        var rb = GetComponent<Rigidbody>();
        Destroy(rb);
    }

    public Bomb GetBomb()
    {
        return _parentBomb;
    }

    private void OnCollisionEnter(Collision c)
    {
        _parentBomb.StopThrow();
    }

    private void OnTriggerEnter(Collider c)
    {
        // Debug.Log($"BombMesh - ON TRIGGER ENTER w/ {c.name}");
        var explosion = c.GetComponent<Explosion>();
        if (explosion != null)
            _parentBomb.Explode();
    }
}
