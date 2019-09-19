using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[RequireComponent(typeof(UIRect))]
public class AutoRelativeChild : MonoBehaviour
{
    [SerializeField]
    bool forceRefreshRelative = false;

    [ContextMenu("anchor")]
    public void setRelativeAllchild()
    {
        relativeAllChildOf(transform);
#if UNITY_EDITOR
        EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }

    [ContextMenu("un_anchor")]
    public void clearRelativeAllchild()
    {
        UIRect[] pRects = transform.GetComponentsInChildren<UIRect>();
        for (int i = 0; i < pRects.Length; ++i)
        {
            UIRect pRect = pRects[i];
            if (pRect)
            {
                pRect.SetAnchor((Transform)null);
                pRect.ResetAndUpdateAnchors();
            }
        }
#if UNITY_EDITOR
        EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }

    public void relativeAllChildOf(Transform pTransForm)
    {
        List<UIRect> childList = new List<UIRect>();
        UIRect[] pRects = pTransForm.GetComponentsInChildren<UIRect>(true);
        UIRect myRect = pTransForm.GetComponent<UIRect>();
        for (int i = 0; i < pRects.Length; ++i)
        {
            if (!pRects[i].parent) continue;
            var rectParent = pRects[i].parent.GetComponentInParent<UIRect>();
            if (pRects[i] != myRect && rectParent && rectParent.gameObject == pTransForm.gameObject)
            {
                childList.Add(pRects[i]);
            }
        }
        UIRect _rect = pTransForm.gameObject.GetComponent<UIRect>();
        float[] targetSides = getSides(_rect.worldCorners);
        UIRect[] childs = childList.ToArray();
        for (int i = 0; i < childs.Length; ++i)
        {
            if (childs[i].gameObject != gameObject)
            {
                if (!childs[i].isAnchored || forceRefreshRelative)
                {
                    float[] mySides = getSides(childs[i].worldCorners);
                    float[] relativeSide = new float[4];
                    for (int j = 0; j < 4; j++)
                    {
                        if (j % 2 == 0)
                        {
                            relativeSide[j] = MathExtends.InverseLerpWithOutClamp(targetSides[0], targetSides[2], mySides[j]);
                        }
                        else
                        {
                            relativeSide[j] = MathExtends.InverseLerpWithOutClamp(targetSides[3], targetSides[1], mySides[j]);
                        }
                        var anchor = childs[i].GetAnchorPoint(j);
                        anchor.target = pTransForm;
                        anchor.Set(relativeSide[j], 0);
                    }
                    childs[i].updateAnchors = UIRect.AnchorUpdate.OnEnable;
                    childs[i].ResetAndUpdateAnchors();
                    relativeAllChildOf(childs[i].transform);
                }
            }
        }
    }

    float[] getSides(Vector3[] corners)
    {
        float[] sides = new float[4];
        for(int i = 0; i < 4; ++i)
        {
            int step = i + 1;
            if(step == 4)
            {
                step = 0;
            }
            Vector3 posSide = Vector3.Lerp(corners[i], corners[step], 0.5f);
            if(i %2 == 0)
            {
                sides[i] = posSide.x;
            }
            else
            {
                sides[i] = posSide.y;
            }
        }
        return sides;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("out")]
    public void testOut()
    {
        gameObject.GetComponent<MoveToTarget>().runActionName("out");
    }
}
