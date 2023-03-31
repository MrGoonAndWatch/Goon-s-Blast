using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DestructibleSpawnChanceUISync : MonoBehaviour
{
    [SerializeField]
    private Slider _spawnChanceSlider;
    [SerializeField]
    private TMP_InputField _spawnChanceInput;

    public void OnSpawnChanceSliderChange(float newValue)
    {
        _spawnChanceInput.SetTextWithoutNotify((newValue * 100).ToString("F"));
    }

    public void OnSpawnChanceInputChange(string newValue)
    {
        float newValueNum;
        if (!float.TryParse(newValue, out newValueNum)) return;
        if (newValueNum > 100)
            newValueNum = 100;
        else if (newValueNum < 0)
            newValueNum = 0;
        _spawnChanceInput.SetTextWithoutNotify(newValueNum.ToString("F"));
        _spawnChanceSlider.SetValueWithoutNotify(newValueNum / 100);
    }

    public void OnSpawnChanceInputUnfocus(string currentValue)
    {
        float spawnChanceInputNum;
        if (!float.TryParse(_spawnChanceInput.text, out spawnChanceInputNum))
        {
            OnSpawnChanceSliderChange(_spawnChanceSlider.value);
        }
    }
}