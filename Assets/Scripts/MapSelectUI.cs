using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class MapSelectUI : UIWindow
{
    public List<Sprite> mapImages;
    public GameObject mapImageObj;
    public GameObject gameStartUI;

    public TextMeshProUGUI mapTitle;

    private int currentSelectedStage = 1;
    private bool isWaiting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void UpdateSelectedStage(int direction)
    {
        currentSelectedStage = (currentSelectedStage + direction);
        if (currentSelectedStage < 1) currentSelectedStage += mapImages.Count;
        if (currentSelectedStage > mapImages.Count) currentSelectedStage -= mapImages.Count;
        mapImageObj.GetComponent<Image>().sprite = mapImages[currentSelectedStage - 1];
        isWaiting = false;
        gameStartUI.SetActive(false);
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Enter");
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
