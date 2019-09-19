using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
public class EventGameObject : UnityEvent<GameObject>
{

}
public class CustomCenterChild : MonoBehaviour {
    public int intialPage = 0;
    public bool loadSave = false;
    [ShowIf("loadSave")]
    public string saveString = "";
    public EventGameObject onCenterEvent;
    public UnityEvent onLimitDown, onLimitUp, onNormal;

    UICenterOnChild _Center;
    UIScrollView mScrollView;
    GameObject mCenteredObject;
    int currentPage;
    private void Awake()
    {
        _Center = GetComponent<UICenterOnChild>();
        mScrollView = NGUITools.FindInParents<UIScrollView>(gameObject);
        CurrentPage = intialPage;
        if(loadSave && !string.IsNullOrEmpty(saveString))
        {
            CurrentPage = PlayerPrefs.GetInt(saveString, intialPage);
        }
        onNormal.Invoke();
        if (CurrentPage == transform.childCount - 1) onLimitUp.Invoke();
        if (CurrentPage == 0) onLimitDown.Invoke();
        updatePage();
    }

    public UICenterOnChild CenterOnChild
    {
        get { return _Center; }
    }

    public int CurrentPage
    {
        get
        {
            return currentPage;
        }

        set
        {
            currentPage = value;
        }
    }

    public void setCurrentPage(int page)
    {
        CurrentPage = page;
        onNormal.Invoke();
        if (CurrentPage == transform.childCount - 1) onLimitUp.Invoke();
        if (CurrentPage == 0) onLimitDown.Invoke();
        updatePage();
    }

    public virtual void onCenterMethod(GameObject pObject)
    {
        if(CenterOnChild)
            onCenterEvent.Invoke(pObject);
    }

    private void OnEnable()
    {
        //if (CenterOnChild)
        //    CenterOnChild.onCenter += onCenterMethod;
    }

    private void OnDisable()
    {
        //if (CenterOnChild)
        //    CenterOnChild.onCenter -= onCenterMethod;
    }

    public void nextPage()
    {
        if (CurrentPage == transform.childCount - 1) return;
        CurrentPage++;
        onNormal.Invoke();
        if (CurrentPage == transform.childCount - 1) onLimitUp.Invoke();
         if (CurrentPage == 0) onLimitDown.Invoke();
        updatePage();
    }

    public void previousPage()
    {
        if (CurrentPage == 0) return;
        CurrentPage--;
        onNormal.Invoke();
        if (CurrentPage == transform.childCount - 1) onLimitUp.Invoke();
        if (CurrentPage == 0) onLimitDown.Invoke();
        updatePage();
    }
    
    public void updatePage()
    {
        if (transform.childCount == 0)
        {
            onLimitDown.Invoke();
            onLimitUp.Invoke();
            return;
        }
        PlayerPrefs.SetInt(saveString, CurrentPage);
        CenterOn(transform.GetChild(CurrentPage));
    }

    void CenterOn(Transform target)
    {
        CenterOnChild.CenterOn(target);
        mCenteredObject = target.gameObject;
        //        if (target != null && mScrollView != null && mScrollView.panel != null)
        //        {
        //            Transform panelTrans = mScrollView.panel.cachedTransform;
        //            mCenteredObject = target.gameObject;

        //            // Figure out the difference between the chosen child and the panel's center in local coordinates
        //            Vector3 cp = panelTrans.InverseTransformPoint(target.position);
        //            Vector3 cc = panelTrans.InverseTransformPoint(panelCenter);
        //            Vector3 localOffset = cp - cc;

        //            // Offset shouldn't occur if blocked
        //            if (!mScrollView.canMoveHorizontally) localOffset.x = 0f;
        //            if (!mScrollView.canMoveVertically) localOffset.y = 0f;
        //            localOffset.z = 0f;

        //            // Spring the panel to this calculated position
        //#if UNITY_EDITOR
        //            if (!Application.isPlaying)
        //            {
        //                panelTrans.localPosition = panelTrans.localPosition - localOffset;

        //                Vector4 co = mScrollView.panel.clipOffset;
        //                co.x += localOffset.x;
        //                co.y += localOffset.y;
        //                mScrollView.panel.clipOffset = co;
        //            }
        //            else
        //#endif
        //            {
        //                SpringPanel.Begin(mScrollView.panel.cachedGameObject,
        //                    panelTrans.localPosition - localOffset, 8);
        //            }
        //        }
        //        else mCenteredObject = null;

        // Notify the listener
        onCenterEvent.Invoke(mCenteredObject);
        onCenterMethod(mCenteredObject);
    }
}
