using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColourManager : MonoBehaviour {

    private Colors colors { get { return Colors.Instance; } }
    private TextMeshProUGUI normalText;
    private Image normalBackground;
    private TextMeshProUGUI highlightText;
    private Image highlightBackground;

    private void Awake() {
        var normalStateObj = transform.Find("Normal").gameObject;
        normalText = normalStateObj.GetComponentInChildren<TextMeshProUGUI>();
        normalBackground = normalStateObj.GetComponentInChildren<Image>();

        var highlightStateObj = transform.Find("Highlighted").gameObject;
        highlightText = highlightStateObj.GetComponentInChildren<TextMeshProUGUI>();
        highlightBackground = highlightStateObj.GetComponentInChildren<Image>();
    }

    private void Start() {
        updateColors();
    }

    public void updateColors() {
        normalText.color = colors.labelTextColor;
        normalBackground.color = colors.labelBackgroundColor;
        highlightText.color = colors.labelTextColor;
        highlightBackground.color = new Color(
            colors.labelBackgroundColor.r,
            colors.labelBackgroundColor.g,
            colors.labelBackgroundColor.b,
            0.4f
        );
    }
}
