using System;
using System.Collections;
using Cinemachine;
using Manager.Cam.PostProcess;
using Manager.Map;
using UnityEngine;

namespace Manager.Cam
{
    public class VCamChanger : MonoBehaviour
    {
        private InRoomChecker _roomChecker;
        private VignetteHandler _vignetteHandler;

        public enum CamKind
        {
            RoomVCam,
            RoadVCam
        }

        [Serializable]
        public struct VCamInfo
        {
            public CamKind _kind;
            public CinemachineVirtualCameraBase _vCam;
            public float _vignetteValue;
        }

        [SerializeField] private VCamInfo[] _vCams;
        [SerializeField] private Transform _player;
        [SerializeField] private float _vignetteDuration;
        
        private const int LowPriority = 5;
        private const int HighPriority = 10;

        private float _vignetteValue;
        private bool _currentIsRoom;
        private Coroutine _vignetteCoroutine;

        private void Start()
        {
            _roomChecker = new InRoomChecker();
            _vignetteHandler = VignetteHandler.Instance;
        }

        private void Update()
        {
            var playerRoom = _roomChecker.CheckStayRoomNum(_player.position);
            SetCameraPriority(playerRoom != InRoomChecker.ErrorRoomNum);
        }

        private void SetCameraPriority(bool isInRoom)
        {
            if (_currentIsRoom == isInRoom) return;

            // カメラの優先度を切り替え
            UpdateCameraPriority(isInRoom ? CamKind.RoomVCam : CamKind.RoadVCam);

            // ビネット効果の切り替えを開始
            SetVignette(isInRoom ? _vCams[(int)CamKind.RoomVCam]._vignetteValue : _vCams[(int)CamKind.RoadVCam]._vignetteValue);

            _currentIsRoom = isInRoom;
        }

        private void UpdateCameraPriority(CamKind activeCamKind)
        {
            foreach (var camInfo in _vCams)
            {
                camInfo._vCam.Priority = camInfo._kind == activeCamKind ? HighPriority : LowPriority;
            }
        }

        private void SetVignette(float targetValue)
        {
            if (_vignetteCoroutine != null) StopCoroutine(_vignetteCoroutine);
            _vignetteCoroutine = StartCoroutine(ChangeVignette(targetValue));
        }

        private IEnumerator ChangeVignette(float targetFactor)
        {
            var startValue = _vignetteValue;
            var elapsedTime = 0f;

            // 指定した時間が経過するまで処理を繰り返す
            while (elapsedTime < _vignetteDuration)
            {
                // 経過時間に応じてvignette値を滑らかに変化させる
                _vignetteValue = Mathf.Lerp(startValue, targetFactor, elapsedTime / _vignetteDuration);
                elapsedTime += Time.deltaTime;
                
                //UnityEngine.Debug.Log(_vignetteValue);
                // VignetteHandlerを通じて値を反映
                _vignetteHandler.SetVignetteIntensity(_vignetteValue);
                yield return null;
            }

            _vignetteValue = targetFactor;
            _vignetteHandler.SetVignetteIntensity(_vignetteValue);
            _vignetteCoroutine = null;
        }
    }
}
