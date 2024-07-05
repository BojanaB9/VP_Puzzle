using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public delegate void DelegateOnClick();

    public DelegateOnClick btnPlay;

    public GameObject topPanel;
    public GameObject bottomPanel;
    public GameObject completePanel;

    public Text timertext;
    public int totalTiles;
    
    IEnumerator UIFadeIn(GameObject panel, float duration = 2.5f)
    {
        Graphic[] graphics = panel.GetComponentsInChildren<Graphic>();
        foreach (Graphic graphic in graphics)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0.0f);

        }
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float normalTime = timer / duration;
            foreach (Graphic graphic in graphics)
            {
                graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, normalTime);
            }
            yield return null;
        }
        foreach (Graphic graphic in graphics)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1.0f);

        }
    }
    public void EnableBottomPanel(bool flag)
    {
        bottomPanel.SetActive(flag);
        if (flag)
        {
            UIFadeIn(bottomPanel);
        }
    }
    public void setTotalTiles(int n)
    {
        totalTiles = n;
    }
    
    public void EnableTopPanel(bool flag)
    {
        topPanel.SetActive(flag);
        if (flag)
        {
            UIFadeIn(topPanel);
        }
    }

    public void EnableCompletionpanel(bool flag)
    {
        completePanel.SetActive(flag);
        if(flag)
        { UIFadeIn(completePanel);}
    }

    public void OnClickPlay()
    {
        btnPlay?.Invoke();
        //string name = EventSystem.current.currentSelectedGameObject.name;
    }

    public void UpdateTime(double n)
    {
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(n);
        string timeString = string.Format("{0:D2} : {1:D2} : {2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

        timertext.text = timeString;
    }

    public void onClickPlayAgain()
    {
        SceneManager.LoadScene("Sceene_jigsaw");

    }
    public void onClickExit()
    {
        Application.Quit();
    }

}
