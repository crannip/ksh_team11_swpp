using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectUI : UIWindow
{
    public List<Sprite> mapImages;
    public GameObject mapImageObj;
    public GameObject gameStartUI;

    public TextMeshProUGUI mapTitle;
    public TextMeshProUGUI leaderBoardName;
    public TextMeshProUGUI leaderBoardTime;

    public TMP_InputField nameInput;
    public TMP_InputField timeInput;

    private int currentSelectedStage = 1;
    private bool isWaiting = false;
    private LeaderBoardManager leaderBoardManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leaderBoardManager = FindAnyObjectByType<LeaderBoardManager>();
        ApplyUIUpdate();
    }

    private void ApplyUIUpdate()
    {
        mapImageObj.GetComponent<Image>().sprite = mapImages[currentSelectedStage - 1];
        leaderBoardName.SetText(leaderBoardManager.GetNameStr(currentSelectedStage));
        leaderBoardTime.SetText(leaderBoardManager.GetTimeStr(currentSelectedStage));
    }

    public void UpdateSelectedStage(int direction)
    {
        currentSelectedStage = (currentSelectedStage + direction);
        if (currentSelectedStage < 1) currentSelectedStage += mapImages.Count;
        if (currentSelectedStage > mapImages.Count) currentSelectedStage -= mapImages.Count;
        isWaiting = false;
        gameStartUI.SetActive(false);
        ApplyUIUpdate();
    }

    public void OnMapImageClick()
    {
        if (!isWaiting)
        {
            isWaiting = true;
            mapTitle.SetText("Stage " + currentSelectedStage);
            gameStartUI.SetActive(true);
        }
    }

    public void StartGame()
    {
        Debug.Log("Stage " + currentSelectedStage + " start");
        //TODO - start game
        //SceneManager.LoadScene(...)
    }

    void OnEnterDown()
    {
        if (!isWaiting)
        {
            OnMapImageClick();
        }
        else
        {
            StartGame();
        }
    }

    void OnHorizontalDown(int v)
    {
        UpdateSelectedStage(v);
    }

    public void OnLeaderBoardSubmit()
    {
        string name = nameInput.text;
        float time = float.Parse(timeInput.text);
        leaderBoardManager.AddRecord(currentSelectedStage, name, time);
        ApplyUIUpdate();
    }

    public void OnLeaderBoardClear()
    {
        leaderBoardManager.ClearRecords();
        ApplyUIUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnEnterDown();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnHorizontalDown(-1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnHorizontalDown(1);
        }
    }
}
