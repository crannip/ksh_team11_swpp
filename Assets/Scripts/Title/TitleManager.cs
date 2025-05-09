using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public List<Button> menuButtons;
    private int currentIndex = 0;

    private void Start()
    {
        HighlightButton(currentIndex);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = (currentIndex + 1) % menuButtons.Count;
            HighlightButton(currentIndex);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = (currentIndex - 1 + menuButtons.Count) % menuButtons.Count;
            HighlightButton(currentIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            menuButtons[currentIndex].onClick.Invoke();
        }
    }

    private void HighlightButton(int index)
    {
        for (int i = 0; i < menuButtons.Count; i++)
        {
            ColorBlock colors = menuButtons[i].colors;
            colors.normalColor = (i == index) ? Color.green : Color.white;
            menuButtons[i].colors = colors;
        }
    }

    // 버튼 클릭 시 실행
    public void StartGame()
    {
        StartCoroutine(FadeManager.Instance.FadeAndLoadScene("GameScene")); // GameScene으로 이동
    }

    public void Option()
    {
        Debug.Log("Option 버튼 눌림");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
