using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBattery : MonoBehaviour
{
    public GameObject missilePrefab;
    public int missilesStored = 10;

    public bool fire(float x, float y) {
        if (missilesStored <= 0) {
            return false;
        }

        var missile = ObjectPoolManager.Instance.getObjectInstance(missilePrefab).GetComponent<Missile>();
        missile.transform.position = transform.position;
        missile.launch(new Vector2(x, y));
        missile.SetActive(true);
        return true;
    }
}
