using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using RPG.Managers;

namespace RPG.StartUp
{
    public class StartUp : MonoBehaviour
    {
        public static StartUp current;

        public bool inProduction = true;
        public bool isServer = false;
        [SerializeField] private GameSceneSO serverSceneSO;
        void Awake()
        {
            current = this;
            Debug.Log("Booting game....");
        }
        private void Start()
        {
            Initialize();
        }
        private void Initialize()
        {
#if UNITY_EDITOR
            if (EditorColdStartUp.GetColdStart()) inProduction = false;
#endif
            if (isServer)
            {
                SceneLoader.instance.SceneRequest(serverSceneSO, false);
                NetworkManager.singleton.StartServer();
                return;
            }
            if(inProduction) SceneLoader.instance.RequestMainMenu();
        }
    }
}
