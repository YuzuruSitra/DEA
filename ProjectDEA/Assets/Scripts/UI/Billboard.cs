using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private GameObject _canvasObj;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        //　カメラと同じ向きに設定
        _canvasObj.transform.rotation = _camera.transform.rotation;
    }
}
