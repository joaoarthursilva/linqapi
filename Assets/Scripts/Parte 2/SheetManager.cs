using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SheetManager : MonoBehaviour
{
    public TMP_Dropdown raceDropdown;
    public TMP_Dropdown classDropdown;
    public TMP_Dropdown alignmentDropdown;
    public TextMeshProUGUI abilitiesText;
    public TextMeshProUGUI traitText;
    public TextMeshProUGUI profiText;
    public TextMeshProUGUI alignText;
    private ApiManager _apiManager;

    private void Start()
    {
        _apiManager = ApiManager.Instance;
        _apiManager.OnFinishedInitLoad += SetRaceChoiceData;
        _apiManager.OnFinishedInitLoad += SetClassChoiceData;
        _apiManager.OnFinishedInitLoad += SetAlignmentChoiceData;
        _apiManager.OnFinishedTraitLoad += SetTraitText;
        _apiManager.OnFinishedProfiLoad += UpdateProfiText;
    }

    private void SetRaceChoiceData()
    {
        var races = _apiManager.racesList;
        raceDropdown.ClearOptions();
        raceDropdown.options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < races.Count; i++)
        {
            raceDropdown.options.Add(new TMP_Dropdown.OptionData(races[i].name));
        }
    }

    private void SetClassChoiceData()
    {
        var classes = _apiManager.allClasses;
        classDropdown.ClearOptions();
        classDropdown.options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < classes.Count; i++)
        {
            classDropdown.options.Add(new TMP_Dropdown.OptionData(classes[i].name));
        }
    }

    private void SetAlignmentChoiceData()
    {
        var alignments = _apiManager.allAlignment;
        alignmentDropdown.ClearOptions();
        alignmentDropdown.options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < alignments.Count; i++)
        {
            alignmentDropdown.options.Add(new TMP_Dropdown.OptionData(alignments[i].name));
        }
    }

    public void ChooseRace(int id)
    {
        var currentRace = _apiManager.racesList[id];
        var abilities = currentRace.ability_bonuses;
        string txt = "";
        foreach (var abilityBonus in abilities)
        {
            txt += abilityBonus.ability_score.name + " +" + abilityBonus.bonus + "\n";
        }

        abilitiesText.text = txt;
        _apiManager.LoadRaceTraits(currentRace);
    }

    private void SetTraitText()
    {
        string txt = "";
        for (var index = 0; index < _apiManager.traits.Count; index++)
        {
            var trait = _apiManager.traits[index];
            txt += index + 1 + "\n" + trait.name + "\n" + trait.desc[0] + "\n";
        }

        traitText.text = txt;
    }

    public void ChooseClass(int id)
    {
        var currentClass = _apiManager.allClasses[id];

        _apiManager.LoadClassProficiencies(currentClass);
    }

    public void UpdateProfiText()
    {
        var proficiencies = _apiManager.currentClassProficiencies;
        profiText.text = "";
        for (int i = 0; i < proficiencies.Count; i++)
        {
            profiText.text += proficiencies[i].name + "\n";
        }
    }

    public void ChooseAlignment(int id)
    {
        var currentAlignment = _apiManager.allAlignment[id];
        alignText.text = $"Alignment Desc: \n\n {currentAlignment.desc}";
    }
}