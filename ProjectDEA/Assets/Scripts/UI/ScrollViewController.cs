using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScrollViewController : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect; // ScrollRectの参照
        [SerializeField] private float _scrollSpeed = 0.1f; // スクロールの速さ

        private void Update()
        {
            // キーボードの左右矢印キーで横スクロールを制御
            if (Input.GetKey(KeyCode.A))
            {
                ScrollLeft();
            }
            else if (Input.GetKey(KeyCode.D))
            {
                ScrollRight();
            }
        }

        private void ScrollRight()
        {
            // ScrollRectの水平スクロール位置を変更（右にスクロール）
            _scrollRect.horizontalNormalizedPosition += _scrollSpeed * Time.deltaTime;
            // Mathf.Clampでスクロール範囲を0〜1に制限
            _scrollRect.horizontalNormalizedPosition = Mathf.Clamp(_scrollRect.horizontalNormalizedPosition, 0f, 1f);
        }

        private void ScrollLeft()
        {
            // ScrollRectの水平スクロール位置を変更（左にスクロール）
            _scrollRect.horizontalNormalizedPosition -= _scrollSpeed * Time.deltaTime;
            // Mathf.Clampでスクロール範囲を0〜1に制限
            _scrollRect.horizontalNormalizedPosition = Mathf.Clamp(_scrollRect.horizontalNormalizedPosition, 0f, 1f);
        }
    }
}