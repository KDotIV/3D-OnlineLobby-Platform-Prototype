using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine.SceneManagement;

namespace RPG.Managers
{
    /// <summary>
    /// This class is used in startup to manage loading and unloading of scenes
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private GameSceneSO _gameplayScene;
        [SerializeField] private GameSceneSO _menuScene;
        [SerializeField] private GameSceneSO _curtain;

        private GameSceneSO _currentlyLoadedScene;
        private GameSceneSO _previousScene;
        private bool _isLoading = false;
        private bool _locationLoaded = false;
        private bool _gameplayLoaded = false;
        private bool _menuLoaded = false;

        public static SceneLoader instance;

        #region BROADCASTS
        public event Action<GameSceneSO, bool> onSceneRequest;
        public event Action onNewGameRequest;
        public event Action onColdStartUp;
        public event Action onStartUp;
        public event Action onForce;
        public event Action<string> onRequestCurtain;
        public void SceneRequest(GameSceneSO scene, bool isMenu) => onSceneRequest?.Invoke(scene, isMenu);
        public void ColdBootRequest() => onColdStartUp?.Invoke();
        public void RequestMainMenu() => onStartUp?.Invoke();
        public void RequestCurtain(string fade) => onRequestCurtain?.Invoke(fade);

        #endregion

        private void Awake()
        {
            instance = this;
        }

        #region LISTENERS
            private void OnEnable() 
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                onSceneRequest += ReceieveSceneTransition;
                onStartUp += RequestLoadMenu;
                #if UNITY_EDITOR
                    onColdStartUp += LoadGame;
                #endif

                Debug.Log("Scene Loader is Listening....");
            }

            private void OnDisable()
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                onSceneRequest -= ReceieveSceneTransition;
                onStartUp -= RequestLoadMenu;
                #if UNITY_EDITOR
                    onColdStartUp -= LoadGame;
                #endif

                Debug.Log("Scene Loader is not listening....");
            }
        #endregion

        private void ReceieveSceneTransition(GameSceneSO requestedScene, bool isMenu)
        {
            if(isMenu)
            {
                StartCoroutine(LoadMenu(requestedScene));
            }
            else
            {
                StartCoroutine(LoadLocation(requestedScene));
            }
        }
        private void RequestLoadMenu()
        {
            StartCoroutine(LoadMenu());
        }

    #if UNITY_EDITOR
        private IEnumerator ColdStartUp(GameSceneSO currentlyOpenedLocation)
        {
            _currentlyLoadedScene = currentlyOpenedLocation;
            Scene checkGameplay = SceneManager.GetSceneByBuildIndex(1);

            if(_currentlyLoadedScene.type == GameSceneSO.GameSceneType.Location)
            {
                if (!checkGameplay.isLoaded || checkGameplay == null)
                {
                    Debug.Log("Gameplay Loaded");
                    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_gameplayScene.sceneReference, LoadSceneMode.Additive);
                    while (!asyncLoad.isDone)
                    {
                        yield return null;
                    }
                }
            }
        }
    #endif

        private IEnumerator LoadLocation(GameSceneSO locationToLoad)
        {
            if(_isLoading) 
                yield break;
            Debug.Log("Loading Location...");
            _isLoading = true;
            _locationLoaded = false;
            Scene checkGameplay = SceneManager.GetSceneByBuildIndex(1);

            Debug.Log("Loading Scene Transition...");
            AsyncOperation asyncCurtain = SceneManager.LoadSceneAsync(_curtain.sceneReference, LoadSceneMode.Additive);
            yield return new WaitUntil(() => asyncCurtain.isDone);

            if (asyncCurtain.isDone)
            {
                //Call Curtain
                RequestCurtain("fadeIn");
                yield return new WaitUntil(() => Transition.current.isDone);

                //Attempt to Unload Previous Scene
                if (_previousScene != null)
                {
                    Debug.Log("Unloaded: " + _previousScene.name);
                    AsyncOperation asyncPrevious = SceneManager.UnloadSceneAsync(_previousScene.sceneReference);
                    yield return new WaitUntil(() => asyncPrevious.isDone);
                }

                //Attempt to Load Gameplay
                if (!checkGameplay.isLoaded || checkGameplay == null)
                {
                    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_gameplayScene.sceneReference, LoadSceneMode.Additive);
                    while (!asyncLoad.isDone)
                    {
                        yield return null;
                    }
                    if(asyncLoad.isDone) _gameplayLoaded = true;
                }

                //Attempt to Load Location
                AsyncOperation asyncLocation = SceneManager.LoadSceneAsync(locationToLoad.sceneReference, LoadSceneMode.Additive);
                yield return new WaitUntil(() => asyncLocation.isDone);

                if(asyncLocation.isDone)
                {
                    Scene newScene = SceneManager.GetSceneByPath(locationToLoad.GetScenePath());
                    SceneManager.SetActiveScene(newScene);
                    _locationLoaded = true;
                }

                //Call Curtain
                RequestCurtain("fadeOut");
                yield return new WaitUntil(() => Transition.current.isDone);

                //Unload Curtain Scene
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(_curtain.sceneReference);
                yield return new WaitUntil(() => asyncUnload.isDone);

                _previousScene = locationToLoad;
                _isLoading = false;
            }
        }

        public IEnumerator LoadMenu(GameSceneSO menuscene = null)
        {
            if(menuscene == null)
            {
                menuscene = _menuScene;
            }
            if(_isLoading) yield break;
            _isLoading = true;
            _menuLoaded = false;
            if (menuscene.type == GameSceneSO.GameSceneType.Menu)
            {
                Debug.Log("Loading Main Menu...");
                AsyncOperation asyncCurtain = SceneManager.LoadSceneAsync(_curtain.sceneReference, LoadSceneMode.Additive);

                yield return new WaitUntil(() => asyncCurtain.isDone);
                if(asyncCurtain.isDone)
                {
                    //Call Curtain
                    RequestCurtain("fadeIn");
                    yield return new WaitUntil(() => Transition.current.isDone);

                    //Attempt to Unload Previous Scene
                    if (_previousScene != null)
                    {
                        Debug.Log("Unloaded: " + _previousScene.name);
                        AsyncOperation asyncPrevious = SceneManager.UnloadSceneAsync(_previousScene.sceneReference);
                        yield return new WaitUntil(() => asyncPrevious.isDone);
                    }

                    //Attempt to Load Menu
                    AsyncOperation asyncMenu = SceneManager.LoadSceneAsync(menuscene.sceneReference, LoadSceneMode.Additive);
                    yield return new WaitUntil(() => asyncMenu.isDone);

                    if(asyncMenu.isDone)
                    {
                        _menuLoaded = true;
                    }

                    //Call Curtain
                    RequestCurtain("fadeOut");
                    yield return new WaitUntil(() => Transition.current.isDone);

                    //Unload Curtain Scene
                    AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(_curtain.sceneReference);
                    while (!asyncUnload.isDone)
                    {
                        yield return null;
                    }
                }
                Debug.Log("Menu Opertaion Done...");
                _isLoading = false;
                _previousScene = menuscene;
            }
            else
            {
                Debug.Log("Scene being requested is not a Menu Scene");
            }
        }
        public void LoadGame()
        {
            Scene checkGameplay = SceneManager.GetSceneByBuildIndex(1);

            if (!checkGameplay.isLoaded || checkGameplay == null)
            {
                if(_gameplayScene.type == GameSceneSO.GameSceneType.Gameplay)
                {
                    Debug.Log("Loading Gameplay....");
                    SceneManager.LoadSceneAsync(_gameplayScene.sceneReference, LoadSceneMode.Additive);
                }
                else
                {
                    Debug.Log("Scene being requested is not a Gameplay Scene");
                }
            }
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("SceneLoaded: " + scene.name);
        }

        public bool GetIsLoading()
        {
            return _isLoading;
        }
        public bool GetLocationStatus()
        {
            return _locationLoaded;
        }
        public bool GetGamePlayStatus()
        {
            return _gameplayLoaded;
        }
        public bool GetMenuStatus()
        {
            return _menuLoaded;
        }
    }
}
