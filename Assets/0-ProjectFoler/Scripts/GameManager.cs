using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject player, aiPlayer;
    private Vector3 playerFirstPos, aiFirstPos;

    [SerializeField] private Transform levelsParent;
    private int currentLevel = 0;

    [SerializeField] private Transform finishArea;
    private float levelLength;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < levelsParent.childCount; i++)
            levelsParent.GetChild(i).gameObject.SetActive(false);
        
        currentLevel = PlayerPrefs.GetInt("level", 0);
        levelsParent.GetChild(currentLevel).gameObject.SetActive(true);

        playerFirstPos = player.transform.position;
        aiFirstPos = aiPlayer.transform.position;

        levelLength = Vector3.Distance(playerFirstPos, finishArea.position);
    }

    public void Win()
    {
        print("win");
        Time.timeScale = 0f;

        player.GetComponent<Player>().HingeJoint.useMotor = false;
        aiPlayer.GetComponent<AIPlayer>().HingeJoint.useMotor = false;

        UIController.Instance.OpenWinPanel();
    }

    public void OpenNextLevel()
    {
        levelsParent.GetChild(currentLevel).gameObject.SetActive(false);

        if (currentLevel != levelsParent.childCount - 1)
        {
            currentLevel += 1;
            PlayerPrefs.SetInt("level", currentLevel);
        }
        else
        {
            currentLevel = 0;
            PlayerPrefs.SetInt("level", currentLevel);
        }

        levelsParent.GetChild(currentLevel).gameObject.SetActive(true);

        player.transform.DOMove(playerFirstPos,2f).SetEase(Ease.OutSine).SetUpdate(true);
        aiPlayer.transform.DOMove(aiFirstPos, 2f).SetEase(Ease.OutSine).SetUpdate(true).OnComplete(()=> {

            player.GetComponent<Player>().HingeJoint.transform.localPosition = Vector3.zero;
            aiPlayer.GetComponent<AIPlayer>().HingeJoint.transform.localPosition = Vector3.zero;

            DOVirtual.DelayedCall(2f,() =>
            {
                player.GetComponent<Player>().HingeJoint.useMotor = true;
                aiPlayer.GetComponent<AIPlayer>().HingeJoint.useMotor = true;
            });

        });

        Time.timeScale = 1f;

    }

    public void GameOver()
    {
        print("game over");
        Time.timeScale = 0f;

        player.GetComponent<Player>().HingeJoint.useMotor = false;
        aiPlayer.GetComponent<AIPlayer>().HingeJoint.useMotor = false;

        UIController.Instance.OpenGameOverPanel();
    }

    public void RestartTheLevel()
    {
        player.transform.DOMove(playerFirstPos, 2f).SetEase(Ease.OutSine).SetUpdate(true);
        aiPlayer.transform.DOMove(aiFirstPos, 2f).SetEase(Ease.OutSine).SetUpdate(true).OnComplete(() => {

            player.GetComponent<Player>().HingeJoint.transform.localPosition = Vector3.zero;
            aiPlayer.GetComponent<AIPlayer>().HingeJoint.transform.localPosition = Vector3.zero;

            DOVirtual.DelayedCall(2f, () =>
            {
                player.GetComponent<Player>().HingeJoint.useMotor = true;
                aiPlayer.GetComponent<AIPlayer>().HingeJoint.useMotor = true;
            });

        });

        Time.timeScale = 1f;
    }

    public void LateUpdate()
    {
        var rate = (levelLength - Vector3.Distance(player.transform.position, finishArea.position)) / levelLength;
        UIController.Instance.UpdateLevelBar(rate);
    }

}