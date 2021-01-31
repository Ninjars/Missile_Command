using UnityEngine;

[CreateAssetMenu(fileName = "Hammer Data", menuName = "Missile Command/Hammer")]
public class HammerData : WeaponData { 
    public Hammer weaponPrefab;

    [Tooltip("Time before weapon can dodge again.")]
    public RangeData dodgeRecharge;

    [Tooltip("Minimum delay between attack events.")]
    public RangeData rechargeTime;

    [Tooltip("Time taken to launch attack once in position.")]
    public RangeData attackTime;

    [Tooltip("Horizontal speed.")]
    public RangeData speed;
}
