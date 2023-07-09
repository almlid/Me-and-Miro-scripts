using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance;
  public GameState state;
  public static event Action<GameState> OnGameStateChange;

  private void Awake()
  {
    Instance = this;
    UpdateGameState(GameState.Running);
  }

  public void UpdateGameState(GameState newState)
  {
    state = newState;

    switch (newState)
    {
      case GameState.MainMenu:
        break;
      case GameState.LoadGame:
        StartGame();
        break;
      case GameState.Running:
        Time.timeScale = 1;
        break;
      case GameState.Paused:
        Time.timeScale = 0;
        break;
      case GameState.GameOver:
        // StopGame();
        break;
      case GameState.ReloadGame:
        ReloadGame();
        break;
      case GameState.Victory:
        HandleVictory();
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
    }
    OnGameStateChange?.Invoke(newState);
  }


  public enum GameState
  {
    MainMenu,
    LoadGame,
    ReloadGame,
    Running,
    Paused,
    GameOver,
    Victory,
  }

  Player player;
  public Vector3 respawnPosition;
  public GameObject enemyPrefab;
  public Transform[] enemySpawnPoints;
  public float spawnDelay = 2f;
  public int maxEnemies = 10;

  private List<GameObject> enemies = new List<GameObject>();

  private void Start()
  {
    UpdateGameState(GameState.Running);
    GameObject playerObject = GameObject.Find("Player");
    player = playerObject.GetComponent<Player>();
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      UpdateGameState(GameState.Paused);
    }
  }

  public void RemoveDeadEnemies()
  {
    for (int i = 0; i < enemies.Count; i++)
    {
      AgentStateMachine stateMachine = enemies[i].GetComponent<AgentStateMachine>();
      if (enemies[i] == null || stateMachine.state == AgentStateMachine.AgentState.Dead)
      {
        enemies.RemoveAt(i);
        i--;
        continue;
      }
    }
  }

  public void StartGame()
  {
    UpdateGameState(GameState.Running);
  }

  public void ReloadGame()
  {
    enemies.Clear();
    SceneManager.LoadScene(2);
  }

  public void EnemyDied(GameObject enemy)
  {
    StartCoroutine(FadeAway(enemy));
  }

  private IEnumerator FadeAway(GameObject enemy)
  {
    if (enemy != null)
    {
      Renderer renderer = enemy.GetComponentInChildren<Renderer>();
      Material originalMaterial = renderer.material;
      Material fadeMaterial = new Material(originalMaterial);
      Color originalColor = fadeMaterial.color;
      for (float t = 0; t < 5f; t += Time.deltaTime)
      {
        fadeMaterial.color = Color.Lerp(originalColor, new Color(0, 0, 0, 0), t / 5f);
        if (renderer)
        {
          renderer.material = fadeMaterial;
        }
        yield return null;
      }
      enemies.Remove(enemy);
      Destroy(enemy);
    }
  }

  private void HandleVictory()
  {
    Debug.Log("VICTORY");
  }

}




