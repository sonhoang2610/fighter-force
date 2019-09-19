using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageController : MonoBehaviour
{
    [SerializeField]
    float distance = 0;
    [SerializeField]
    UIButton _btnNext = null, _btnPrevious = null, _btnFirst = null, _btnLast = null;
    [SerializeField]
    PageElement[] pages = null;
    [SerializeField]
    int offsetNext = 1;
    [SerializeField]
    EventDelegate _eventOnChoosePage = null;
    float totalSize = 0;
    int currentIndex = 0;
    int totalPage = 0;
    int startPage = 0;

    [ContextMenu("Resort")]
    public void resort()
    {
        totalSize = distance * Mathf.Min(totalPage - startPage, pages.Length);
        float pStart = -totalSize / 2;
        for (int i = 0; i < pages.Length; ++i)
        {
            pages[i].Index = i + startPage;
            int pageIndex = i + startPage;
            if (startPage + i < totalPage)
            {
                pages[i].gameObject.SetActive(true);
            }
            else
            {
                pages[i].gameObject.SetActive(false);
            }
            if (pages[i].gameObject.activeSelf)
            {
                if (pageIndex > 0 && i == 0)
                {
                    pages[i].turnMore();
                }
                if (pageIndex < totalPage - 1 && i == pages.Length - 1)
                {
                    pages[i].turnMore();
                }
                EventDelegate onChoose = new EventDelegate(this, "chooseIndexPage");
                onChoose.parameters[0].value = i;
                if (pages[i].GetComponent<UIButton>().onClick.Count == 0)
                {
                    pages[i].GetComponent<UIButton>().onClick.Add(onChoose);
                }
                pages[i].transform.setLocaPosX(pStart + ((float)i + 0.5f) * distance);
            }
        }
        if (_btnPrevious != null)
        {
            pStart -= distance;
            _btnPrevious.transform.setLocaPosX(pStart);
        }
        if (_btnFirst != null)
        {
            pStart -= distance;
            _btnFirst.transform.setLocaPosX(pStart);
        }
        float pEnd = totalSize / 2;
        if (_btnNext != null)
        {
            pEnd += distance;
            _btnNext.transform.setLocaPosX(pEnd);
        }
        if (_btnLast != null)
        {
            pEnd += distance;
            _btnLast.transform.setLocaPosX(pEnd);
        }
    }

    public void chooseIndexSkin(int index)
    {
        currentIndex = index;
        for (int i = 0; i < pages.Length; ++i)
        {
            pages[i].unChoosed();
            if (i == index)
            {
                pages[i].choosed();
            }
        }
    }

    public int findRealIndex(int pPages)
    {
        for(int i = 0; i < pages.Length; ++i)
        {
            if(pages[i].Index == pPages)
            {
                return i;
            }
        }
        return -1;
    }

    public void chooseIndexPage(int index)
    {
        int pRealPage = pages[index].Index;
        chooseIndexSkin(index);
        if (_eventOnChoosePage != null)
        {
            _eventOnChoosePage.parameters[0].value = pRealPage;
            _eventOnChoosePage.Execute();
        }

        // if ( pages.Length - currentIndex <= offsetNext)
        //  {
        int last = Mathf.Min(pRealPage + offsetNext + 1, totalPage);
        startPage = last - pages.Length;
        if (startPage < 0)
        {
            startPage = 0;
        }    
        // }
        resort();
        index = findRealIndex(pRealPage);
        chooseIndexSkin(index);
        if (startPage + pages.Length >= totalPage)
        {
            _btnLast.gameObject.SetActive(false);
        }
        else
        {
            _btnLast.gameObject.SetActive(true);
        }
        if (startPage == 0)
        {
            _btnFirst.gameObject.SetActive(false);
        }
        else
        {
            _btnFirst.gameObject.SetActive(true);
        }
    }


    public void nextPage()
    {
        currentIndex++;
        chooseIndexPage(currentIndex);
    }

    public void previousPage()
    {
        currentIndex--;
        chooseIndexPage(0);
    }
    public void lastPage()
    {
        startPage = totalPage - pages.Length;
        resort();
        chooseIndexPage(pages.Length-1);

    }

    public void firstPage()
    {
        startPage = 0;
        resort();
        chooseIndexPage(0);
    }

    public void setTotalPage(int page)
    {
        totalPage = page;
        resort();
        if (startPage + pages.Length >= totalPage)
        {
            _btnLast.gameObject.SetActive(false);
        }
        else
        {
            _btnLast.gameObject.SetActive(true);
        }
        if (startPage == 0)
        {
            _btnFirst.gameObject.SetActive(false);
        }
        else
        {
            _btnFirst.gameObject.SetActive(true);
        }
    }
    // Use this for initialization
    void Start()
    {
        setTotalPage(1);
        chooseIndexPage(0);
        if (_btnLast != null)
        {
            _btnLast.onClick.Add(new EventDelegate(this, "lastPage"));
        }
        if (_btnFirst != null)
        {
            _btnFirst.onClick.Add(new EventDelegate(this, "firstPage"));
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
