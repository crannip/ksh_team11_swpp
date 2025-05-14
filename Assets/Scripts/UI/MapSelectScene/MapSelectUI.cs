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
    
    protected override void Start()
    {
        base.Start();
        leaderBoardManager = FindAnyObjectByType<LeaderBoardManager>();
        
        onEnterDown.AddListener(OnEnterDown);
        onHorizontalDown.AddListener(OnHorizontalDown);
        
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

    private void OnEnterDown()
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

    private void OnHorizontalDown(int v)
    {
        Debug.Log("ASDGSAG");
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
}
