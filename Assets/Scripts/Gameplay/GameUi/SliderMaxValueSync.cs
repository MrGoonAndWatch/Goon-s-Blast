using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderMaxValueSync : MonoBehaviour
{
    [SerializeField]
    private Slider _syncFromSlider;
    [SerializeField]
    private TMP_InputField _syncFromInput;
    [SerializeField]
    private Slider _syncToSlider;

    private SliderTextSync _syncToSliderSyncScript;
    
    void Start()
    {
        _syncToSliderSyncScript = _syncToSlider.GetComponent<SliderTextSync>();
        _syncFromSlider.onValueChanged.AddListener(UpdateMaxValue);
        _syncFromInput.onValueChanged.AddListener(UpdateMaxValueFromText);
        UpdateMaxValue(_syncFromSlider.value);
    }

    public void UpdateMaxValueFromText(string newText)
    {
        float newValue;
        if (!float.TryParse(newText, out newValue)) return;
        UpdateMaxValue(newValue);
    }

    public void UpdateMaxValue(float newValue)
    {
        _syncToSlider.maxValue = newValue;
        if (_syncToSliderSyncScript != null)
            _syncToSliderSyncScript.Refresh();
    }
}
