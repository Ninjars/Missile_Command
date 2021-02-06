using TMPro;
using UnityEngine;

public class FadingTitle : MonoBehaviour {
    public TextMeshProUGUI stageTitle;
    public AnimationCurve fadeAnimationCurve;
    public float animationDuration;

    private Colors colors { get { return Colors.Instance; } }
    private float displayStart;

    private void OnEnable() {
        stageTitle.enabled = false;
    }

    void Update() {
        if (!stageTitle.enabled) return;

        float animFraction = (Time.time - displayStart) / animationDuration;
        if (animFraction > 1) {
            stageTitle.alpha = 0;
            stageTitle.enabled = false;
        } else {
            stageTitle.alpha = fadeAnimationCurve.Evaluate(animFraction);
        }
    }

    internal void display(string title) {
        stageTitle.color = colors.textColor;
        stageTitle.text = title;
        stageTitle.alpha = 0;
        displayStart = Time.time;
        stageTitle.enabled = true;
    }
}
