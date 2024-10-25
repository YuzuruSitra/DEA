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
        //@ƒJƒƒ‰‚Æ“¯‚¶Œü‚«‚Éİ’è
        _canvasObj.transform.rotation = _camera.transform.rotation;
    }
}
