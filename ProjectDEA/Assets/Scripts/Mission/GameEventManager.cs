using System;
using UnityEngine;

namespace Mission
{
    // こいつはオブジェクトが検索して特定の状況下でイベントを発火するやつ
    public class GameEventManager : MonoBehaviour
    {
        public event Action<int> OnEnemyDefeated;
        public event Action<int> OnGimmickCompleted;
        public event Action<int> OnItemUsed;

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
        public void ItemUsed(int itemID) => OnItemUsed?.Invoke(itemID);
    }
}

