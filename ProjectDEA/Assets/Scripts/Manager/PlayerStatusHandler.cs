using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class PlayerStatusHandler : MonoBehaviour
    {
        [SerializeField] private int _initialDamage;
        public int PlayerAttackDamage { get; private set; }
        [SerializeField] private int _initialHp;
        public int MaxHp => _initialHp;
        public int PlayerCurrentHp { get; private set; }
        public Action<int> OnChangeHp;
        private bool _addedListener;

        private void Awake()
        {
            CheckSingleton();
        }

        private void OnDestroy()
        {
            if (!_addedListener) return;
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
            
            SceneManager.sceneLoaded += OnSceneLoaded;
            _addedListener = true;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Reset status if the loaded scene is TitleScene
            if (scene.name != "DungeonStart") return;
            SetPlayerAttackDamage(_initialDamage);
            SetPlayerCurrentHp(_initialHp);
        }

        public void SetPlayerAttackDamage(int damage)
        {
            PlayerAttackDamage = damage;
        }

        public void SetPlayerCurrentHp(int hp)
        {
            PlayerCurrentHp = hp;
            OnChangeHp?.Invoke(hp);
        }
    }
}