using UnityEngine;

namespace Debugging
{
    [RequireComponent(typeof(Renderer))]
    public class DebugRenderObject : MonoBehaviour
    {
        private Renderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _origColor = _renderer.sharedMaterial.color;
        }

        private void OnEnable()
        {
            UpdateRenderer();
            
            Debug.onDebugModeChanged += EnableRenderer;
        }

        private void OnDisable() => Debug.onDebugModeChanged -= EnableRenderer;

        private void OnDestroy()
        {
            RepaintOriginalColor();
        }


        #region DEBUG RENDERING

        private Color _origColor;

        protected void RepaintOriginalColor() => _renderer.sharedMaterial.color = _origColor;
        
        protected void PaintRed() => _renderer.sharedMaterial.color = new Color(1, 0, 0, _origColor.a);
        protected void PaintGreen() => _renderer.sharedMaterial.color = new Color(0, 1, 0, _origColor.a);


        protected void UpdateRenderer() => EnableRenderer(Debug.Active);

        private void EnableRenderer(bool render)
        {
            if (render) Show();
            else Hide();
        }

        protected void Show() => _renderer.enabled = true;
        protected void Hide() => _renderer.enabled = false;

        #endregion
    }
}
