using Shapes;
using UnityEngine;

[RequireComponent(typeof(Line))]
public class LinearTrail : MonoBehaviour {
    public float zPos;
    private float decayTime;
    private bool isDecaying;
    private float decayStart;
    private Color initialColor;
    private Color currentColor;
    private GameObject subject;

    private Line _line;
    private Line line {
        get {
            if (_line == null) {
                _line = GetComponent<Line>();
            }
            return _line;
        }
    }

    public void initialise(GameObject subject, TrailSettings trailSettings, Color color) {
        gameObject.SetActive(true);

        this.subject = subject;
        this.zPos = trailSettings.zPos;
        this.initialColor = color;
        this.decayTime = trailSettings.fadeDuration;

        isDecaying = false;
        currentColor = initialColor;

        Vector3 position = getPosition();
        line.Start = position;
        line.End = position;
        line.Color = currentColor;
    }

    private Vector3 getPosition() {
        return new Vector3(
            subject.transform.position.x, 
            subject.transform.position.y, 
            zPos
        );
    }

    private void Update() {
        if (isDecaying) {
            float decay = Time.time - decayStart;
            if (decay >= decayTime) {
                subject = null;
                _line = null;
                gameObject.SetActive(false);
                
            } else {
                currentColor.a = Mathf.Lerp(1, 0, decay / decayTime);
                line.Color = currentColor;
            }

        } else if (subject == null || !subject.activeInHierarchy) {
            onSubjectDisabled();

        } else {
            line.End = getPosition();
        }
    }

    internal void onSubjectDisabled() {
            subject = null;
            isDecaying = true;
            decayStart = Time.time;
    }
}
