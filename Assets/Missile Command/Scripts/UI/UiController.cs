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

    public CityUpgradeUI cityUpgradeUIPrefab;
    public BatteryUpgradeUI batteryUpgradeUIPrefab;

    private CityUpgradeUI[] cityUpgradeUIs;
    private BatteryUpgradeUI[] batteryUpgradeUIs;
    private GameState gameState;
    private UiMode currentMode;
    private Colors colors { get { return Colors.Instance; } }

    internal void setGameState(GameState gameState) {
        this.gameState = gameState;
        inGamePanel.GetComponent<InGameUI>().setGameState(gameState);
        clearUpgradeUI();
    }

    private void clearUpgradeUI() {
        cityUpgradeUIs = null;
        batteryUpgradeUIs = null;
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
                    clearUpgradeUI();

                    loseWaves.text = $"{gameState.levelsCompleted}";
                    loseSurvivors.text = $"{gameState.populationEvacuated + gameState.citiesPopulation}";
                    loseDead.text = $"{gameState.populationDead}";
                    break;
                }
            case UiMode.WIN_SCREEN: {
                    hideAllPanels();
                    show(winPanel);
                    clearUpgradeUI();

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
        if (cityUpgradeUIs != null) {
            foreach (var upgrade in cityUpgradeUIs) {
                upgrade.gameObject.SetActive(false);
            }
        }
        if (batteryUpgradeUIs != null) {
            foreach (var upgrade in batteryUpgradeUIs) {
                upgrade.gameObject.SetActive(false);
            }
        }
    }

    private void showUpgradeOptions() {
        Debug.Log("showUpgradeOptions()");
        if (cityUpgradeUIs == null) {
            cityUpgradeUIs = generateCityUpgradeUIs(gameState, cityUpgradeUIPrefab, () => onUpgradePurchased());
        }
        foreach (var upgrade in cityUpgradeUIs) {
            upgrade.gameObject.SetActive(true);
        }
        if (batteryUpgradeUIs == null) {
            batteryUpgradeUIs = generateBatteryUpgradeUIs(gameState, batteryUpgradeUIPrefab, () => onUpgradePurchased());
        }
        foreach (var upgrade in batteryUpgradeUIs) {
            upgrade.gameObject.SetActive(true);
        }
    }

    private static CityUpgradeUI[] generateCityUpgradeUIs(GameState gameState, CityUpgradeUI prefab, Action onUpgradeAction) {
        return gameState.cities.Select(city => {
            CityUpgradeUI ui = GameObject.Instantiate<CityUpgradeUI>(prefab);
            ui.initialise(city, onUpgradeAction);
            ui.gameObject.SetActive(false);
            return ui;
        }).ToArray();
    }

    private static BatteryUpgradeUI[] generateBatteryUpgradeUIs(GameState gameState, BatteryUpgradeUI prefab, Action onUpgradeAction) {
        return gameState.missileBatteries.Select(battery => {
            BatteryUpgradeUI ui = GameObject.Instantiate<BatteryUpgradeUI>(prefab);
            ui.initialise(battery, onUpgradeAction);
            ui.gameObject.SetActive(false);
            return ui;
        }).ToArray();
    }

    private void onUpgradePurchased() {
        Debug.Log("upgrade purchased");
    }

    internal bool isChoosingUpgrade() {
        return false;
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
