using System;
using System.Collections;
using System.Collections.Generic;
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


    private GameState gameState;
    private UiMode currentMode;

    internal void setGameState(GameState gameState) {
        this.gameState = gameState;
        inGamePanel.GetComponent<InGameUI>().setGameState(gameState);
    }

    public void setUiMode(UiMode mode) {
        if (currentMode == mode) return;
        this.currentMode = mode;
        switch (mode) {
            case UiMode.MAIN_MENU: {
                show(mainMenuPanel);
                hide(inGamePanel);
                hide(winPanel);
                hide(losePanel);
                break;
            }
            case UiMode.IN_GAME: {
                hide(mainMenuPanel);
                show(inGamePanel);
                hide(winPanel);
                hide(losePanel);
                break;
            }
            case UiMode.LOSE_SCREEN: {
                hide(mainMenuPanel);
                hide(inGamePanel);
                hide(winPanel);
                show(losePanel);

                loseWaves.text = $"{gameState.levelsCompleted}";
                loseSurvivors.text = $"{gameState.populationEvacuated + gameState.citiesPopulation}";
                loseDead.text = $"{gameState.populationDead}";
                break;
            }
            case UiMode.WIN_SCREEN: {
                hide(mainMenuPanel);
                hide(inGamePanel);
                show(winPanel);
                hide(losePanel);

                winWaves.text = $"{gameState.levelsCompleted}";
                winSurvivors.text = $"{gameState.populationEvacuated + gameState.citiesPopulation}";
                winDead.text = $"{gameState.populationDead}";
                break;
            }
        }
    }

    private void show(RectTransform panel) {
        panel.gameObject.SetActive(true);
    }

    private void hide(RectTransform panel) {
        panel.gameObject.SetActive(false);
    }
}

public enum UiMode {
    PRE_INIT,
    MAIN_MENU,
    IN_GAME,
    LOSE_SCREEN,
    WIN_SCREEN,
}
