using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace RPG.Managers
{
    public class StartGame : MonoBehaviour
    {
        [SerializeField] private bool _showLoadScreen = default;
        private bool _hasSaveData;
        private SaveSystem _tempSaveSystem;
        [SerializeField] private GameSceneSO _characterScreen;
        [SerializeField] private GameSceneSO _newGame;

        private void Start()
        {
            //_hasSaveData = SaveManager.instance.CheckSaveData();
        }
        private void OnEnable()
        {
            //_tempSaveSystem = SaveManager.instance.GetSaveSystem();
        }
        private void OnDestroy()
        {

        }
        public void StartNewGameClient()
        {
          /*  _hasSaveData = false;

            _tempSaveSystem.WriteEmptySaveFile();
            _tempSaveSystem.SetNewGameData();*/

            SceneLoader.instance.SceneRequest(_characterScreen, true);
            Debug.Log("Started New Game");
        }
        public void LaunchGame()
        {
            AutoConnect.instance.JoinLocal();
            SceneLoader.instance.SceneRequest(_newGame, false);
        }
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
