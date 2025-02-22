using System;
using UnityEngine;

namespace Item.CreateScriptableObject
{
    [CreateAssetMenu(fileName = "MemoirsHolder", menuName = "Memoirs/MemoirsHolder")]
    public class MemoirsHolder : ScriptableObject
    {
        [Serializable]
        public struct MemoirsData
        {
            public int _number;
            public string _content;
            public bool _active;
        } 
        public MemoirsData[] _memoirsData;

        
    }
}