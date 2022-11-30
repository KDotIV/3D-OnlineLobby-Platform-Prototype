using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using RPG.Managers;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private List<Selector> _selectorList;
    [SerializeField] private List<CharacterDbContext> _loadedCharacterList;
    [SerializeField] private CharacterDbContext _loadedCharacter;

    public static event Action<string> onCharacterAPICall;
    public static event Action<CharacterDbContext> onSaveLoadedCharacter;

    private void Awake()
    {
        _loadedCharacterList = new List<CharacterDbContext>();
        RefreshCharacterList();
    }

    public void OnEnable()
    {
        APIGateway.onAPISuccess += GetCharacterList;
        Selector.onCharacterSelect += SelectCharacter;
    }
    private void OnDisable()
    {
        APIGateway.onAPISuccess -= GetCharacterList;
        Selector.onCharacterSelect -= SelectCharacter;
    }
    public void RefreshCharacterList()
    {
        var currentUser = APIGateway.instance.GetCurrentUserID();

        onCharacterAPICall?.Invoke(currentUser);
    }

    private void GetCharacterList()
    {
        _loadedCharacterList = APIGateway.instance.GetCurrentCharacterList();

        if (_loadedCharacterList != null)
        {
            for (var i = 0; i < _selectorList.Count; i++)
            {
                if (i == _loadedCharacterList.Count)
                {
                    break;
                }
                var openSlot = _selectorList[i];
                Debug.Log(_selectorList[i].name);

                _selectorList[i].SetCharacter(_loadedCharacterList[i].CharacterName);
                _selectorList[i].SetActiveUI(true);

                Debug.Log($"{_selectorList.Count}");
            }
        }
    }

    public void SelectCharacter(string selectedName)
    {
        foreach (var found in _loadedCharacterList)
        {
            if(found.CharacterName == selectedName)
            {
                _loadedCharacter = found;
                Debug.Log($"{_loadedCharacter.CharacterName}: Is Ready...");
                onSaveLoadedCharacter?.Invoke(_loadedCharacter);
            }
        }
    }
}
