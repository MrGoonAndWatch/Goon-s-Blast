using UnityEngine;

public class BombMesh : MonoBehaviour
{
    [SerializeField] private Bomb _parentBomb;
    
    private void OnTriggerEnter(Collider c)
    {
        // Debug.Log($"BombMesh - ON TRIGGER ENTER w/ {c.name}");
        var explosion = c.GetComponent<Explosion>();
        if (explosion != null)
            _parentBomb.Explode();
    }
}
