using Manager.Map;
using UnityEngine;
using Gimmick;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int _targetFrameRate;

        private void Awake()
        {
            CheckSingleton();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // シーンがロードされたときに実行されるメソッド
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (scene.name)
            {
                case "TitleScene":
                    HandleTitleScene();
                    break;
                case "DungeonStart":
                    HandleDungeonStartScene();
                    break;
                case "DungeonIn":
                    HandleDungeonInScene();
                    break;
                case "ResultScene":
                    HandleResultScene();
                    break;
            }
        }
        
        private void HandleTitleScene()
        {
            if (Application.targetFrameRate != _targetFrameRate) Application.targetFrameRate = _targetFrameRate;
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private static void HandleDungeonStartScene()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        private void HandleDungeonInScene()
        {
            var stageGenerator = GameObject.FindWithTag("StageGenerator").GetComponent<StageGenerator>();
            stageGenerator.MapGenerate();
            var navMeshHandler = GameObject.FindWithTag("NavMeshBaker").GetComponent<NavMeshHandler>();
            navMeshHandler.SetNavMeshParents(stageGenerator.NavMeshParents);
            var gimmickGenerator = GameObject.FindWithTag("GimmickGenerator").GetComponent<RoomGimmickGenerator>();
            gimmickGenerator.InitialGenerateGimmicks(stageGenerator, navMeshHandler);
            var playerSetPos = Vector3.zero;
            playerSetPos.x = stageGenerator.RoomInfo[0, (int) StageGenerator.RoomStatus.CenterX];
            playerSetPos.y = 1.88f;
            playerSetPos.z = stageGenerator.RoomInfo[0, (int) StageGenerator.RoomStatus.CenterZ];
            var player = GameObject.FindWithTag("Player");
            player.transform.position = playerSetPos;
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private static void HandleResultScene()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void CheckSingleton()
        {
            var target = GameObject.FindGameObjectWithTag(gameObject.tag);
            var checkResult = target != null && target != gameObject;
            
            if (checkResult)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
    }
}
