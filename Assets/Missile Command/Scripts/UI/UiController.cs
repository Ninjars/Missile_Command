using System;
using System.Collections.Generic;
using System.Linq;
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

    public bool isChoosingUpgrade { get; private set; }
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
        hideUpgradeOptions();
    }

    private void hideNonGamePanels() {
        hide(mainMenuPanel);
        hide(winPanel);
        hide(losePanel);
        hideUpgradeOptions();
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
                    showUpgradeOptions();
                    break;
                }
            case UiMode.LOSE_SCREEN: {
                    hideAllPanels();
                    show(losePanel);
                    hideUpgradeOptions();

                    loseWaves.text = $"{gameState.levelsCompleted}";
                    loseSurvivors.text = $"{gameState.populationEvacuated + gameState.citiesPopulation}";
                    loseDead.text = $"{gameState.populationDead}";
                    break;
                }
            case UiMode.WIN_SCREEN: {
                    hideAllPanels();
                    show(winPanel);
                    hideUpgradeOptions();

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
        if (panel != null) {
            panel.gameObject.SetActive(false);
        }
    }

    private void hideUpgradeOptions() {
        if (gameState == null) return;

        foreach (var city in gameState.cities) {
            city.hideUpgradeOptions();
        }
        foreach (var battery in gameState.missileBatteries) {
            battery.hideUpgradeOptions();
        }
    }

    private void showUpgradeOptions() {
        Debug.Log("showUpgradeOptions()");
        isChoosingUpgrade = true;
        foreach (var city in gameState.cities) {
            city.showUpgradeOptions(() => deselectAllUpgradeUis(), () => onUpgradePurchased());
        }
        foreach (var battery in gameState.missileBatteries) {
            battery.showUpgradeOptions(() => deselectAllUpgradeUis(), () => onUpgradePurchased());
        }
    }

    private void deselectAllUpgradeUis() {
        foreach (var city in gameState.cities) {
            city.deselectUpgradeUi();
        }
        foreach (var battery in gameState.missileBatteries) {
            battery.deselectUpgradeUi();
        }
    }

    private void onUpgradePurchased() {
        Debug.Log("upgrade purchased");
        isChoosingUpgrade = false;
        deselectAllUpgradeUis();
        hideUpgradeOptions();
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
