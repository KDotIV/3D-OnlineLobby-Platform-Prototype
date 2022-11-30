using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSaveSystem", menuName = "SaveSystem")]
public class SaveSystem : ScriptableObject
{
    public string saveFilename = "dap.dat";
    public string backupSaveFilename = "dap.dat.bak";
    public Save saveData = new Save();

    private void OnEnable() 
    {
        CharacterSelect.onSaveLoadedCharacter += SetCharacterData;
    }
    private void OnDisable()
    {
        CharacterSelect.onSaveLoadedCharacter -= SetCharacterData;
    }

    public bool LoadSaveDataFromDisk()
    {
        if(FileManager.LoadFromFile(saveFilename, out var json))
        {
            saveData.LoadFromJson(json);
            return true;
        }
        return false;
    }
    public void SaveDataToDisk()
    {
        if(FileManager.MoveFile(saveFilename, backupSaveFilename))
        {
            if(FileManager.WriteToFile(saveFilename, saveData.ToJson()))
            {
                Debug.Log("Save Successful");
            }
        }
    }
    public void WriteEmptySaveFile()
    {
        FileManager.WriteToFile(saveFilename, "");
    }
    public void SetNewGameData()
    {
        FileManager.WriteToFile(saveFilename, "");
        SaveDataToDisk();
    }

    private void SetCharacterData(CharacterDbContext loadedCharacter)
    {
        saveData._characterData = loadedCharacter;
    }
    public CharacterDbContext GetLoadedCharacterData()
    {
        return saveData._characterData;
    }
}
