using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject gameOverPanel;

    [SerializeField] private GameObject drawArea;

    [SerializeField] private Slider levelBar;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void OpenWinPanel()
    {
        drawArea.SetActive(false);
        winPanel.SetActive(true);
    }

    public void OpenGameOverPanel()
    {
        drawArea.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    public void NextLevelButton()
    {
        drawArea.SetActive(true);
        winPanel.SetActive(false);

        GameManager.Instance.OpenNextLevel();
    }
    public void RestartTheLevelButton()
    {
        drawArea.SetActive(true);
        gameOverPanel.SetActive(false);

        GameManager.Instance.RestartTheLevel();
    }

    public void UpdateLevelBar(float value)
    {
        levelBar.value = value;
    }
}