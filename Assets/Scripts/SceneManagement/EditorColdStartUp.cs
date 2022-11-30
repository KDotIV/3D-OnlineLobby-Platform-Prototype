using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine.SceneManagement;
using RPG.Managers;

namespace RPG.StartUp
{
    public class EditorColdStartUp : MonoBehaviour
    {
        public static EditorColdStartUp instance;
#if UNITY_EDITOR

        [SerializeField] private GameSceneSO _currentScene;
        [SerializeField] private GameSceneSO _bootScene;
        private static bool isColdStart = false;

        public event Action coldStart;

        private void Awake()
        {
            instance = this;
            if(!SceneManager.GetSceneByBuildIndex(0).isLoaded)
            {
                isColdStart = true;
            }
        }
        private void OnEnable()
        {
        }
        private void OnDisable()
        {

        }
        private void Start()
        {
            StartCoroutine(Boot());
        }
        public static bool GetColdStart()
        {
            return isColdStart;
        }
        private IEnumerator Boot()
        {
            if(isColdStart)
            {
                AsyncOperation asyncStartUp = SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);
                while (!asyncStartUp.isDone)
                {
                    yield return null;
                }
                if(asyncStartUp.isDone) SceneLoader.instance.ColdBootRequest();
                StartUp.current.inProduction = false;
            }  
        }
        #endif
    }
}
