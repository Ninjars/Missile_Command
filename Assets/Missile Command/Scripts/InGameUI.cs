using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour {

    public TextMeshProUGUI waveCount;
    public TextMeshProUGUI safeTextView;

    private GameState gameState;

    internal void setGameState(GameState gameState) {
        this.gameState = gameState;
    }

    void Update() {
        if (gameState != null) {
            safeTextView.text = $"SAFE {gameState.populationEvacuated}";
        }
    }

    private void OnEnable() {
        waveCount.text = $"{gameState.levelsCompleted + 1}";
    }
}
