using System.Collections;
using UnityEngine;

namespace UI
{
    public class PopUpMenu: MonoBehaviour
    {
        private GameObject _menu;

        private void Awake()
        {
            _menu = transform.GetChild(0).gameObject;
            Hide();
        }
        
        public void Toggle() => _menu.SetActive(!gameObject.activeSelf);
        public void Show() => _menu.SetActive(true);
        public void Hide() => _menu.SetActive(false);
        
        public void ShowWithDelay(float delaySeconds = 1f) => 
            StartCoroutine(ShowWithDelayCoroutine(delaySeconds));

        private IEnumerator ShowWithDelayCoroutine(float delaySeconds = 1f)
        {
            yield return new WaitForSeconds(delaySeconds);
            Show();
        }
    }
}
