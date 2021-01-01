using TMPro;
using UnityEngine;

public class UiController : MonoBehaviour {
    public RectTransform mainMenuPanel;
    public RectTransform inGamePanel;

    public RectTransform winPanel;
    public TextMeshProUGUI winWaves;
    public TextMeshProUGUI winSurvivors;
    public TextMeshProUGUI winDead;
    
    public RectTransform losePanel;
    public TextMeshProUGUI loseWaves;
    public TextMeshProUGUI loseSurvivors;
    public TextMeshProUGUI loseDead;

    public RectTransform upgradePanel;
    public UpgradeButton upgradeButton1;
    public UpgradeButton upgradeButton2;
    public UpgradeButton upgradeButton3;

    private GameState gameState;
    private UiMode currentMode;
    private Colors colors { get { return Colors.Instance; } }

    internal void setGameState(GameState gameState) {
        this.gameState = gameState;
        inGamePanel.GetComponent<InGameUI>().setGameState(gameState);
    }

    private void hideAllPanels() {
        hide(mainMenuPanel);
        hide(inGamePanel);
        hide(winPanel);
        hide(losePanel);
        hide(upgradePanel);
    }

    private void hideNonGamePanels() {
        hide(mainMenuPanel);
        hide(winPanel);
        hide(losePanel);
        hide(upgradePanel);
    }

    public void setUiMode(UiMode mode) {
        if (currentMode == mode) return;
        this.currentMode = mode;
        switch (mode) {
            case UiMode.MAIN_MENU: {
                hideAllPanels();
                show(mainMenuPanel);
                break;
            }
            case UiMode.IN_GAME: {
                hideNonGamePanels();
                show(inGamePanel);
                break;
            }
            case UiMode.LEVEL_END: {
                hideNonGamePanels();
                show(upgradePanel);

                // TODO: show upgrades for the buttons
                break;
            }
            case UiMode.LOSE_SCREEN: {
                hideAllPanels();
                show(losePanel);

                loseWaves.text = $"{gameState.levelsCompleted}";
                loseSurvivors.text = $"{gameState.populationEvacuated + gameState.citiesPopulation}";
                loseDead.text = $"{gameState.populationDead}";
                break;
            }
            case UiMode.WIN_SCREEN: {
                hideAllPanels();
                show(winPanel);

                winWaves.text = $"{gameState.levelsCompleted}";
                winSurvivors.text = $"{gameState.populationEvacuated + gameState.citiesPopulation}";
                winDead.text = $"{gameState.populationDead}";
                break;
            }
        }
    }

    private void show(RectTransform panel) {
        panel.gameObject.SetActive(true);

        foreach (var textElement in panel.GetComponentsInChildren<TextMeshProUGUI>()) {
            textElement.color = colors.textColor;
        }
        foreach (var button in panel.GetComponentsInChildren<ButtonColourManager>()) {
            button.updateColors();
        }
    }

    private void hide(RectTransform panel) {
        panel.gameObject.SetActive(false);
    }
}

public enum UiMode {
    PRE_INIT,
    MAIN_MENU,
    IN_GAME,
    LEVEL_END,
    LOSE_SCREEN,
    WIN_SCREEN,
}
