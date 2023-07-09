using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
  public GameObject gameRunningPanel, gamePausedPanel, gameOverPanel, victoryPanel;
  public Button resumeButton, restartButton, mainMenuButton, quitGameButton;

  public PlayerHealth playerHealth;

  void Awake()
  {
    GameManager.OnGameStateChange += ChangeUIPanel;
    playerHealth.OnHealthChange += UpdatePlayerHealthBar;
  }

  void OnDestroy()
  {
    GameManager.OnGameStateChange -= ChangeUIPanel;
    playerHealth.OnHealthChange -= UpdatePlayerHealthBar;
  }

  private void Start()
  {
    resumeButton.onClick.AddListener(ResumeGame);
    restartButton.onClick.AddListener(RestartGame);
    mainMenuButton.onClick.AddListener(BackToMainMenu);
    quitGameButton.onClick.AddListener(QuitGame);

  }

  private void ChangeUIPanel(GameManager.GameState state)
  {
    gameRunningPanel.SetActive(state == GameManager.GameState.Running);
    gamePausedPanel.SetActive(state == GameManager.GameState.Paused);
    gameOverPanel.SetActive(state == GameManager.GameState.GameOver);
    victoryPanel.SetActive(state == GameManager.GameState.Victory);

    if (state == GameManager.GameState.GameOver)
    {
      Time.timeScale = 0.2f;
    }
  }

  public void UpdatePlayerHealthBar(float playerHealth) { }

  public void ResumeGame()
  {
    GameManager.Instance.UpdateGameState(GameManager.GameState.Running);
  }

  public void RestartGame()
  {
    GameManager.Instance.UpdateGameState(GameManager.GameState.ReloadGame);
  }

  public void BackToMainMenu()
  {
    SceneManager.LoadScene(1);
  }

  public void QuitGame()
  {
    Application.Quit();
  }
}
