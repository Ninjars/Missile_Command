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
    public TextMeshProUGUI upgradePoints;

    public bool canPickUpgrades { get; private set; }
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
        hideUpgradeOptions();
    }

    private void hideNonGamePanels() {
        hide(mainMenuPanel);
        hide(winPanel);
        hide(losePanel);
        hide(upgradePanel);
        hideUpgradeOptions();
    }

    public void setUiMode(UiMode mode) {
        if (currentMode == mode || gameState == null) return;
        this.currentMode = mode;
        switch (mode) {
            case UiMode.MAIN_MENU: {
                    hideAllPanels();
                    clearAllCityUi();
                    show(mainMenuPanel);
                    break;
                }
            case UiMode.IN_GAME: {
                    hideNonGamePanels();
                    if (gameState.levelsCompleted < 3) {
                        showAllCityUi(false);
                        showAllMissileBatteryLabels();
                    } else {
                        hideAllCityUi();
                        hideAllMissileBatteryUi();
                    }
                    show(inGamePanel);
                    break;
                }
            case UiMode.LEVEL_END: {
                    hideNonGamePanels();
                    showAllCityUi(false);
                    break;
                }
            case UiMode.UPGRADE: {
                    hideNonGamePanels();
                    show(upgradePanel);
                    showAllCityUi(true);
                    showAllMissileBatteryUpgradeIndicator();
                    showUpgradeOptions();
                    updateUpgradesText();
                    break;
                }
            case UiMode.LOSE_SCREEN: {
                    hideAllPanels();
                    show(losePanel);
                    hideUpgradeOptions();
                    hideAllMissileBatteryUi();
                    showAllCityUi(false);

                    loseWaves.text = $"{gameState.levelsCompleted}";
                    loseSurvivors.text = $"{gameState.populationEvacuated + gameState.citiesPopulation}";
                    loseDead.text = $"{gameState.populationDead}";
                    break;
                }
            case UiMode.WIN_SCREEN: {
                    hideAllPanels();
                    show(winPanel);
                    hideUpgradeOptions();
                    hideAllMissileBatteryUi();
                    showAllCityUi(false);

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
        foreach (var city in gameState.cities) {
            if (city.upgradeState.hasAnyAvailableUpgrades) {
                city.showUpgradeOptions(
                    () => {
                        deselectAllUpgradeUis();
                    },
                    () => onUpgradePurchased()
                );
            }
        }
        foreach (var battery in gameState.missileBatteries) {
            if (battery.upgradeState.hasAnyAvailableUpgrades) {
                battery.showUpgradeOptions(() => deselectAllUpgradeUis(), () => onUpgradePurchased());
            }
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
        gameState.onUpgradePointSpent();
        updateUpgradesText();
        // canPickUpgrades = gameState.canUpgradeSomething();
        // if (!canPickUpgrades) {
        //     deselectAllUpgradeUis();
        //     hideUpgradeOptions();
        // }
    }

    private void updateUpgradesText() {
        upgradePoints.text = $"UPGRADES AVAILABLE: {gameState.upgradePoints}";
    }

    private void showAllMissileBatteryLabels() {
        if (gameState.missileBatteries == null) return;
        foreach (var battery in gameState.missileBatteries) {
            battery.showLabel();
        }
    }

    private void showAllMissileBatteryUpgradeIndicator() {
        if (gameState.missileBatteries == null) return;
        foreach (var battery in gameState.missileBatteries) {
            battery.showUpgradeIndicator();
        }
    }

    private void hideAllMissileBatteryUi() {
        if (gameState.missileBatteries == null) return;
        foreach (var battery in gameState.missileBatteries) {
            battery.hideUi();
        }
    }

    private void showAllCityUi(bool isUpgrading) {
        if (gameState.cities == null) return;
        foreach (var city in gameState.cities) {
            city.showUi(isUpgrading);
        }
    }

    private void hideAllCityUi() {
        if (gameState.cities == null) return;
        foreach (var city in gameState.cities) {
            city.fadeOutUi();
        }
    }

    private void clearAllCityUi() {
        if (gameState.cities == null) return;
        foreach (var city in gameState.cities) {
            city.hideUi();
        }
    }
}

public enum UiMode {
    PRE_INIT,
    MAIN_MENU,
    IN_GAME,
    LEVEL_END,
    UPGRADE,
    LOSE_SCREEN,
    WIN_SCREEN,
}
