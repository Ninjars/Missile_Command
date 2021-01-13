using System.Collections.Generic;
using Shapes;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorLines : MonoBehaviour {
    private Line[] lines;
    private List<MissileBattery> batteries;
    private Colors colors { get { return Colors.Instance; } }

    private void Awake() {
        lines = GetComponentsInChildren<Line>();
    }

    private void Start() {
        foreach (var line in lines) {
            line.Color = colors.targetMarkerColor;
        }
    }

    public void setBatteries(List<MissileBattery> batteries) {
        this.batteries = batteries;
    }

    void Update() {
        updateLines();
    }

    private void OnDisable() {
        foreach (var line in lines) {
            line.gameObject.SetActive(false);
        }
    }

    private void updateLines() {
        if (batteries == null)  return;
        var endPos = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        for (int i = 0; i < batteries.Count; i++) {
            var battery = batteries[i];
            var line = lines[i];
            if (battery.isDestroyed) {
                line.gameObject.SetActive(false);
            } else {
                line.gameObject.SetActive(true);
                line.Start = battery.transform.position + Vector3.up * battery.missileLaunchOffset;
                line.End = new Vector3(endPos.x, endPos.y, line.Start.z);
            }
        }
    }
}
