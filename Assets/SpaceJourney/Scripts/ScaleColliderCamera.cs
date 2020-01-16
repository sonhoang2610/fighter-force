using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScaleColliderCamera : MonoBehaviour
{
    public Camera _cam;
    public BoxCollider2D _collider;
    void Start()
    {
        if (_collider != null && _cam != null)
        {
            _collider.size = new Vector2(_cam.orthographicSize * 2 * 1080.0f/1920.0f, _cam.orthographicSize * 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying && _collider != null && _cam != null)
        {
            _collider.size = new Vector2(_cam.orthographicSize * 2 * Screen.width / Screen.height, _cam.orthographicSize * 2);
        }
    }
}
