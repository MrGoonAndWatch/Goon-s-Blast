using UnityEngine;

public class HideTileInGame : MonoBehaviour
{
    private void Start()
    {
        var levelEditor = FindObjectOfType<LevelEditorController>();
        if(levelEditor == null)
            gameObject.SetActive(false);
    }
}
