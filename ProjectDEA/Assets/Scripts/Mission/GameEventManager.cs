using UnityEngine;

namespace Mission
{
    // こいつはオブジェクトが検索して特定の状況下でイベントを発火するやつ
    public class GameEventManager : MonoBehaviour
    {
        public event System.Action<int> OnEnemyDefeated;
        public event System.Action<int> OnGimmickCompleted;
        public event System.Action<int> OnItemCollected;

        private void Start()
        {
            CheckSingleton();
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

        public void EnemyDefeated(int enemyID) => OnEnemyDefeated?.Invoke(enemyID);
        public void GimmickCompleted(int gimmickID) => OnGimmickCompleted?.Invoke(gimmickID);
        public void ItemCollected(int itemID) => OnItemCollected?.Invoke(itemID);
    }
}

