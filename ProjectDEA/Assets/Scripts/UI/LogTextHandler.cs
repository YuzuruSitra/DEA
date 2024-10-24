using UnityEngine;

namespace UI
{
    public class LogTextHandler : MonoBehaviour
    {
        [SerializeField] private LogTextDisable[] _logTextDisable;
        private const string ErrorMessage = "";
        
        
        public void AddLog(string message, bool isDisable = true)
        {
            var previousMessage = message;

            foreach (var t in _logTextDisable)
            {
                // 現在のオブジェクトがアクティブかどうかチェック
                var currentMessage = t.TMPro.enabled ? t.CurrentText : ErrorMessage;
        
                // メッセージを設定
                t.ReceiveMessage(previousMessage, isDisable);
        
                // エラーメッセージがあれば、以降の処理をスキップ
                if (currentMessage == ErrorMessage) return;
        
                // 次のループのためにメッセージを移動
                previousMessage = currentMessage;
            }
        }

        public void AllOnDisableTMPro()
        {
            foreach (var t in _logTextDisable)
            {
                t.OnDisableTMPro();
            }
        }
        
    }
}
