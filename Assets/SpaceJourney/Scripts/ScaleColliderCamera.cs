using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScaleColliderCamera : MonoBehaviour
{
    public Camera _cam;
    public BoxCollider2D _collider;

    private void Awake()
    {
        //var pSize = FindObjectOfType<UIRoot>().GetComponent<UIPanel>().GetWindowSize();

        //if ((pSize.y / pSize.x) > (19.2f / 10.8f))
        //{
        //    _cam.orthographicSize = (pSize.y / pSize.x) * 5.4f;
        //}

        if (_collider != null && _cam != null)
        {
            _collider.size = new Vector2(10.8f, 19.2f);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying && _collider != null && _cam != null)
        {
            _collider.size = new Vector2(10.8f, 19.2f);
        }
    }
}
