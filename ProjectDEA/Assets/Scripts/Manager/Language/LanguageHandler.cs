using System;
using UnityEngine;

namespace Manager.Language
{
    public class LanguageHandler : MonoBehaviour
    {
        public enum Language
        {
            Japanese,
            English
        }
        public Language CurrentLanguage { get; private set; }
        public event Action<Language> OnLanguageChanged;
        
        private void Start()
        {
            CheckSingleton();
        }

        public void SetLanguage(Language language)
        {
            if (CurrentLanguage == language) return;
            CurrentLanguage = language;
            OnLanguageChanged?.Invoke(CurrentLanguage);
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
