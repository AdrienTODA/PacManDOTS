using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public GameObject titleUI;
    public GameObject gameUI;
    public GameObject winUI;
    public GameObject loseUI;

    public PlayerAnim playerAnim;

    public TMP_Text pelletsTextUI;
    public TMP_Text scoreTextUI;
    public int score, level, lives;

    protected override void Awake()
    {
        base.Awake();

        Reset();
    }

    public void Reset()
    {
        SwitchUI(titleUI);
        score = 0;
        lives = 3;
        LoadLevel(0);
        AudioManager.Instance.PlayMusicRequest("title");
    }

    public void InGame()
    {
        SwitchUI(gameUI);
        AudioManager.Instance.PlayMusicRequest("game");
    }

    public void Win()
    {
        playerAnim.Win();
        SwitchUI(winUI);
        AudioManager.Instance.PlayMusicRequest("win");
    }

    public void GameOver()
    {
        SwitchUI(loseUI);
        AudioManager.Instance.PlayMusicRequest("lose");
    }

    public void LoseLife()
    {
        lives--;
        playerAnim.Lose();
        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void SwitchUI(GameObject newUI)
    {
        titleUI.SetActive(false);
        gameUI.SetActive(false);
        winUI.SetActive(false);
        loseUI.SetActive(false);

        newUI.SetActive(true);
    }

    public void AddPoints(int points)
    {
        score += points;
        scoreTextUI.text = $"Score : {score}";
    }

    public void UpdatePelletsCount(int pelletsAmount)
    {
        pelletsTextUI.text = $"Pellets : {pelletsAmount}";
    }

    public void NextLevel()
    {
        InGame();
        LoadLevel(level + 1);
    }

    public void LoadLevel(int newLevel)
    {
        if (newLevel > 1)
        {
            Reset();
            return;
        }

        UnloadLevel();
        level = newLevel;
        SceneManager.LoadScene($"Level{level}", LoadSceneMode.Additive);
    }

    public void UnloadLevel()
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;

        foreach (var e in em.GetAllEntities())
        {
            em.DestroyEntity(e);
        }

        if (SceneManager.GetSceneByName($"Level{level}").isLoaded)
            SceneManager.UnloadSceneAsync($"Level{level}");
    }
}
