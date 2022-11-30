using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "NewSceneSO", menuName = "Scene Data")]
public class GameSceneSO : SerializableScriptableObject
{
    [Header("SceneData")]
    public SceneReference sceneReference;
    public string shortDescription;
    public GameSceneType type;

    public string GetScenePath()
    {
        return sceneReference.ScenePath;
    }

    public enum GameSceneType
    {
        Location,
        Menu,
        Boot,
        PersistentManagers,
        Gameplay,
    }
}
