using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Page
{
    public Sprite image;
    public string describe;
}

public class Tutorial : MonoBehaviour
{
    [SerializeField] Page[] pages;
    [SerializeField] int curPage;
    [SerializeField] GameObject left;
    [SerializeField] GameObject right;
    [SerializeField] Image image;
    [SerializeField] Text describe_text;
    [SerializeField] Text describe_remains;

    private void Start()
    {
        curPage = 0;
        left.SetActive(false);
        if (pages.Length < 2) right.SetActive(false);
        if (pages.Length != 0) UpdatePage();
    }
    public void NextPage()
    {
        curPage++;
        if (curPage == pages.Length - 1) right.SetActive(false);
        if (!left.activeSelf) left.SetActive(true);
        UpdatePage();
    }
    public void PrviousPage()
    {
        curPage--;
        if (curPage == 0) left.SetActive(false);
        if (!right.activeSelf) right.SetActive(true);
        UpdatePage();
    }
    void UpdatePage()
    {
        image.sprite = pages[curPage].image;
        describe_text.text = pages[curPage].describe;
        describe_remains.text = $"{curPage + 1} / {pages.Length}";
    }
}
