using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class ScrollViewController : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect; // ScrollRectの参照
        [SerializeField] private float _scrollSpeed = 0.1f; // スクロールの速さ
        private InputActions _inputActions;
        private bool _isScrollingRight; // 右方向のスクロール状態
        private bool _isScrollingLeft;  // 左方向のスクロール状態

        private void Start()
        {
            // InputActionsを初期化し、InventryViewScrollアクションのイベントリスナーを登録
            _inputActions = new InputActions();
            _inputActions.Player.InventryViewScroll.performed += StartScrolling;
            _inputActions.Player.InventryViewScroll.canceled += StopScrolling; // キーを離したときに停止
            _inputActions.Enable();
        }

        private void OnDestroy()
        {
            // InventryViewScrollアクションのイベントリスナーを解除
            _inputActions.Player.InventryViewScroll.performed -= StartScrolling;
            _inputActions.Player.InventryViewScroll.canceled -= StopScrolling;
            _inputActions.Disable();
        }

        private void Update()
        {
            if (_isScrollingRight)
            {
                ScrollRight();
            }
            else if (_isScrollingLeft)
            {
                ScrollLeft();
            }
        }

        private void StartScrolling(InputAction.CallbackContext context)
        {
            // 入力値を取得（-1:左, 1:右）
            var scrollDirection = context.ReadValue<float>();

            if (scrollDirection > 0)
            {
                _isScrollingRight = true;
            }
            else if (scrollDirection < 0)
            {
                _isScrollingLeft = true;
            }
        }

        private void StopScrolling(InputAction.CallbackContext context)
        {
            // スクロールの停止
            _isScrollingRight = false;
            _isScrollingLeft = false;
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
