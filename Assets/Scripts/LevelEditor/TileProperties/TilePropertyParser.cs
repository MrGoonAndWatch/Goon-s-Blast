using System;
using TMPro;
using UnityEngine;

public abstract class TilePropertyParser : MonoBehaviour
{
    public abstract void Initialize(string propertiesJson);
    public abstract string SerializeProperties();
    public abstract bool IsValid();

    protected void PopulatePickerFromEnum(TMP_Dropdown targetDropdown, Type enumType)
    {
        var types = Enum.GetValues(enumType);
        targetDropdown.options.Clear();
        for (var i = 0; i < types.Length; i++)
        {
            var currentTypeStr = types.GetValue(i).ToString();
            targetDropdown.options.Add(new TMP_Dropdown.OptionData(currentTypeStr));
        }
    }
}
