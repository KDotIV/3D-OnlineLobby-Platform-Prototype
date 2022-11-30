using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Managers
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager instance;
        [SerializeField] private SaveSystem _saveSystem;
        [SerializeField] private bool _hasSaveData;

        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            
        }
        private void OnEnable()
        {
            _hasSaveData = _saveSystem.LoadSaveDataFromDisk();
        }
        private void OnDisable()
        {
            
        }
        public bool CheckSaveData()
        {
            return _hasSaveData;
        }
        public SaveSystem GetSaveSystem()
        {
            return _saveSystem;
        }
    }
}
