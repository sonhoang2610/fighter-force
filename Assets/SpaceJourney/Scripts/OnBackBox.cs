using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EazyEngine.Space.UI;

public interface IBackBehavior
{
     bool onBack();
     int getLevel();
}
public class OnBackBox : MonoBehaviour, IBackBehavior
{
    public UnityEvent onBackEvent;
    public int level = 1;
	// Use this for initialization
	void Start () {
		
	}

    private void OnEnable()
    {
        HUDLayer.Instance.addListenerBack(this);
    }

    private void OnDisable()
    {
        HUDLayer.Instance.removeLisnterBack(this);
    }

    // Update is called once per frame
    public virtual bool onBack()
    {
        gameObject.SetActive(false);
        onBackEvent.Invoke();
        return true;
    }

    public int getLevel()
    {
        return level;
    }
}
