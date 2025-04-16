using UnityEngine;

namespace Managers
{
    public class QualityManager: MonoBehaviour
    {
        private void Start() => SetQualitySettings();

        private void OnEnable() => SettingsManager.Instance.onLoad += SetQualitySettings;
        private void OnDisable() => SettingsManager.Instance.onLoad -= SetQualitySettings;

        private void SetQualitySettings()
        {
            QualitySettings.vSyncCount = 
                SettingsManager.Instance.GetSetting<bool>("VSync") 
                    ? QualitySettings.vSyncCount = 1
                    : QualitySettings.vSyncCount = 0;
        }
    }
}
