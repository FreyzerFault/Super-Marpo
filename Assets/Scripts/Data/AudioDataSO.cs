using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Data
{
    [CreateAssetMenu(fileName = "Audio Data", menuName = "Audio/Audio Data", order = 1)]
    public class AudioDataSO : ScriptableObject
    {
        [SerializedDictionary("Scene", "Music Clip")]
        public SerializedDictionary<string, AudioClip> sceneMusicSelection;

        public AudioClip GetMusicByScene(Scene scene) => 
            GetMusicBySceneName(scene.name);
        
        public AudioClip GetMusicBySceneIndex(int sceneIndex) => 
            GetMusicBySceneName(SceneManager.GetSceneByBuildIndex(sceneIndex).name);

        public AudioClip GetMusicBySceneName(string sceneName) => 
            sceneMusicSelection.GetValueOrDefault(sceneName);
    }
}
