using System;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Debug = Debugging.Debug;

namespace Managers
{
	/// <summary>
	///		Game Manager manage States, Scenes and global persistent data between ALL the Game
	///		SingletonPersistent: Singleton + DDOL (Don't Destroy On scene Load)
	///
	///		Change the State with GameManager.Instance.State = GameState
	///		Suscribe to onGameStateChanged in each component that depends on it
	/// </summary>
	public class GameManager : SingletonPersistent<GameManager>
	{
		public enum GameState { MainMenu, GameOver, Playing, Paused, GoalReached }
	
		[SerializeField] private GameState initialState = GameState.MainMenu;
		private GameState _state;
	
		public GameState State
		{
			get => _state;
			set
			{
				_state = value;
				OnStateChange(value);
			}
		}
	
		public bool IsPlaying => _state == GameState.Playing;
		public bool IsPaused => _state == GameState.Paused;
	
		public Action onGameStateChanged; // Suscribe to this to update by the GameState
	
	
		protected override void Awake()
		{
			base.Awake();
			SuscribeSceneLoading();
			State = initialState;
		
			LoadSettings();
		}

		private void Update()
		{
			if ( _state == GameState.Playing)
				UpdateTime();
		}

		private void OnStateChange(GameState newState)
		{
			switch (newState)
			{
				case GameState.MainMenu:
					break;
				case GameState.GameOver:
					break;
				case GameState.Playing:
					break;
				case GameState.Paused:
					break;
				case GameState.GoalReached:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
			}

			onGameStateChanged?.Invoke();
		}


		#region GAME TIME

		private void UpdateTime() => DataManager.Instance.GameTime += Time.deltaTime;

		#endregion


		#region SETTINGS
	
		private void LoadSettings() => Debug.Active = SettingsManager.Instance.GetSetting<bool>("debugMode");

		#endregion
	
	
		#region PAUSE

		public void Pause() => State = GameState.Paused;
		public void Resume() => State = GameState.Playing;
		public void TogglePause() => State = State == GameState.Paused ? GameState.Playing : GameState.Paused;

		#endregion
	

		#region SCENE MANAGEMENT

		private void SuscribeSceneLoading()
		{
			SceneManager.sceneLoaded += (scene, _) =>
			{
				switch (scene.name)
				{
					case "MainMenu":
						State = GameState.MainMenu;
						break;
					case "Game":
						State = GameState.Playing;
						break;
					case "GameOver":
						State = GameState.GameOver;
						break;
				}
			};
		}

		public static void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);
		public static void ReloadScene() => LoadScene(SceneManager.GetActiveScene().name);
		public static void QuitGame()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
		}

		#endregion


		#region PLAYER DATA

		private int _winCount;
		private int _winStreak;
		private bool _win;
	
		public bool Win => _win;
		public int WinCount => _winCount;
		public int WinStreak => _winStreak;

		public void RegisterWin()
		{
			_win = true;
			_winCount++;
			_winStreak++;
		}
	
		public void RegisterLose()
		{
			_win = false;
			_winStreak = 0;
		}

		#endregion
	}
}
