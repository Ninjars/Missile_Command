﻿using Shapes;
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

    public void initialise(GameObject subject, TrailSettings trailSettings) {
        gameObject.SetActive(true);

        this.subject = subject;
        this.zPos = trailSettings.zPos;
        this.initialColor = trailSettings.color;
        this.decayTime = trailSettings.fadeDuration;

        isDecaying = false;
        currentColor = initialColor;

        line.Start = getPosition();
        line.End = getPosition();
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
                gameObject.SetActive(false);
                
            } else {
                currentColor.a = Mathf.Lerp(1, 0, decay / decayTime);
                line.Color = currentColor;
            }

        } else if (subject != null && !subject.activeInHierarchy) {
            subject = null;
            isDecaying = true;
            decayStart = Time.time;
        
        } else {
            line.End = getPosition();
        }
    }
}
