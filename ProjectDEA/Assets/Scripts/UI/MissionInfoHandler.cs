using Gimmick;
using Mission;
using TMPro;
using UnityEngine;

namespace UI
{
    public class MissionInfoHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _missionInfoPanel;
        private MissionStateHandler _missionStateHandler;
        [SerializeField] private TextMeshProUGUI _missionText;
        [SerializeField] private string _padding;
        
        private void Start()
        {
            _missionStateHandler = GameObject.FindWithTag("ExitObelisk").GetComponent<ExitObelisk>().MissionStateHandler;
            _missionStateHandler.OnMissionStarted += OpenPanel;
            _missionStateHandler.OnMissionFinished += ClosePanel;
        }

        private void OnDestroy()
        {
            _missionStateHandler.OnMissionStarted -= OpenPanel;
            _missionStateHandler.OnMissionFinished -= ClosePanel;
        }

        private void OpenPanel()
        {
            _missionInfoPanel.SetActive(true);
        }

        private void ClosePanel()
        {
            _missionInfoPanel.SetActive(false);
        }

        private void Update()
        {
            if (!_missionStateHandler.DoingMission) return;
            var mission = _missionStateHandler.CurrentMission;
            _missionText.text = mission.MissionName + "\n" + _padding + mission.CurrentCount + "/" + mission.MaxCount;
        }
    }
}
