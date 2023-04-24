using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderTextSync : MonoBehaviour
{
    [SerializeField]
    private Slider _sliderUi;
    [SerializeField]
    private TMP_InputField _numInputUi;
    [SerializeField]
    [Tooltip("Set the number display to 100x the slider value (i.e. in 'percentage' format)")]
    private bool _showAsPercentage;
    [SerializeField]
    private bool _useWholeNumbers;

    // Start is called before the first frame update
    void Start()
    {
        _numInputUi.text = FormatValue(GetTextValueFromSlider());
        _sliderUi.onValueChanged.AddListener(OnSliderChange);
        _numInputUi.onValueChanged.AddListener(OnTextInputChange);
        _numInputUi.onDeselect.AddListener(OnTextInputUnfocus);
    }

    public void OnSliderChange(float newValue)
    {
        var textValue = GetTextValueFromSlider();
        _numInputUi.SetTextWithoutNotify(FormatValue(textValue));
    }

    public void OnTextInputChange(string newValue)
    {
        float newValueNum;
        if (!float.TryParse(newValue, out newValueNum)) return;
        if (newValueNum > _sliderUi.maxValue)
            newValueNum = _sliderUi.maxValue;
        else if (newValueNum < _sliderUi.minValue)
            newValueNum = _sliderUi.minValue;
        _numInputUi.SetTextWithoutNotify(FormatValue(newValueNum));
        _sliderUi.SetValueWithoutNotify(GetSliderValueFromInput());
    }

    public void OnTextInputUnfocus(string currentValue)
    {
        float spawnChanceInputNum;
        if (!float.TryParse(_numInputUi.text, out spawnChanceInputNum))
        {
            OnSliderChange(_sliderUi.value);
        }
    }

    private float GetTextValueFromSlider()
    {
        return _showAsPercentage ? _sliderUi.value * 100 : _sliderUi.value;
    }

    private float GetSliderValueFromInput()
    {
        var currentInputVal = float.Parse(_numInputUi.text);
        return _showAsPercentage ? currentInputVal / 100 : currentInputVal;
    }

    private string FormatValue(float value)
    {
        return _useWholeNumbers ? value.ToString("F0") : value.ToString("F");
    }
}
