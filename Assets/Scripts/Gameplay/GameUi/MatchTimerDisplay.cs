using TMPro;
using UnityEngine;

public class MatchTimerDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _timerText;

    public void HideDisplay()
    {
        gameObject.SetActive(false);
    }

    public void SetTimerText(string text)
    {
        _timerText.text = text;
    }
}
