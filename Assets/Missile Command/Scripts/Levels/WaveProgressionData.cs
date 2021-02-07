using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveProgression", menuName = "Missile Command/Wave Progression")]
public class WaveProgressionData : ScriptableObject {
    public ICBMCurves ICBM;
    public ICBMCurves RFG;
    public ICBMCurves heavyMIRV;
    public BomberCurves standardBomber;
    public BomberCurves strategicBomber;
    public HammerCurves hammer;

    public List<StageDefinition> stages;
}

[Serializable]
public class StageDefinition {
    public string title;
    public int waveCount;
    public bool icbm;
    public bool heavyMirv;
    public bool rfg;
    public bool standardBomber;
    public bool strategicBomber;
    public bool hammer;
}
