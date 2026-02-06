using System;
using EditorAttributes;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BOTWSlider : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Required] private Slider mainSlider;
    [SerializeField, Required] private Slider backSlider;

    [Header("Settings")]
    [SerializeField] private float maxValue;
    
    [Header("Main Slider Settings")]
    [SerializeField] private Color mainSliderColor;
    [SerializeField] private bool animateMainSlider;
    [SerializeField, Suffix("In Percent Per Seconds")] private float mainSliderSpeed = 1f;
    

    [Header("Back Slider Settings")]
    [SerializeField] private Color backSliderColor;
    [SerializeField] private bool animateBackSlider;
    [SerializeField, Suffix("In Percent Per Seconds")] private float backSliderSpeed = .8f;
    
    [FormerlySerializedAs("debug")]
    [Header("Debug")]
    [SerializeField] private bool useDebug;
    [SerializeField] private float debugValue;
    
    [ShowInInspector] private float currentValue;
    [ShowInInspector] private float targetCurrentValue;
    
    public float CurrentPercent => Mathf.Clamp(currentValue / maxValue, 0f, 1f);

    protected void Start()
    {
        Init();
    }

    protected void Update()
    {
        if (useDebug)
            SetCurrentValue(debugValue);
        
        if (!animateMainSlider)
        {
            currentValue = targetCurrentValue;
        }
        else
        {
            float sign = (currentValue < targetCurrentValue) ? 1f : -1f;
            var min = (currentValue < targetCurrentValue) ? currentValue : targetCurrentValue;
            var max = (currentValue < targetCurrentValue) ? targetCurrentValue : currentValue;
            
            currentValue = Mathf.Clamp(currentValue + sign * maxValue * mainSliderSpeed, min, max);
        }

        float backSliderCurrentValue = backSlider.value * maxValue;
        float backSliderNewValue = backSliderCurrentValue;
        if (animateBackSlider)
        {
            var sign = (backSliderCurrentValue < targetCurrentValue) ? 1f : -1f;
            var min = (backSliderCurrentValue < targetCurrentValue) ? backSliderCurrentValue : targetCurrentValue;
            var max = (backSliderCurrentValue < targetCurrentValue) ? targetCurrentValue : backSliderCurrentValue;
            backSliderNewValue = Mathf.Clamp(backSliderCurrentValue + sign * maxValue * backSliderSpeed, min, max);
        }
        
        mainSlider.value = CurrentPercent;
        backSlider.value = backSliderNewValue / maxValue;
    }

    protected void OnValidate()
    {
        Init();
    }


    private void Init()
    {
        currentValue = maxValue;
        targetCurrentValue = currentValue;
        
        mainSlider.value = 1;
        backSlider.value = 1;

        mainSlider.maxValue = 1f;
        backSlider.maxValue = 1f;
        
        // Change color of sliders
        mainSlider.fillRect.GetComponent<Image>().color = mainSliderColor;
        backSlider.fillRect.GetComponent<Image>().color = backSliderColor;
    }
    
    public void SetCurrentValue(float value)
    {
        targetCurrentValue = value;
    }

    /**
     * Decrements current value by drainValue, if it is not possible returns false instead
     */
    public bool Drain(float drainValue)
    {
        if (targetCurrentValue - drainValue < 0)
        {
            return false;
        }
        
        targetCurrentValue -= drainValue;
        return true;
    }

    /**
     * Sets max value.
     * If keepPercent is true, will stay the same percent, else will reset to full
     */
    public void SetMaxValue(float value, bool keepPercent = false)
    {
        if (!keepPercent)
        {
            maxValue = value;
            Init();    
        }
        
        var lastPercent = CurrentPercent;
        maxValue = value;
        currentValue = lastPercent * maxValue;
    }
}
