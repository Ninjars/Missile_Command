﻿using UnityEngine;

[CreateAssetMenu(fileName = "Bomber Data", menuName = "Missile Command/Bomber")]
public class BomberData : WeaponData {
    public Bomber weaponPrefab;

    [Tooltip("Weapon launched by the bomber")]
    public ICBMData bombAttackData;

    [Tooltip("Number of bombers per spawn, following same flight path.")]
    public RangeIntData bombersPerWing;

    [Tooltip("Delay between bomb events.")]
    public RangeData chargeTime;

    [Tooltip("Horizontal speed.")]
    public RangeData speed;

    [Tooltip("Ability of bomber to avoid explosions.")]
    public RangeData evasionSpeed;
}
