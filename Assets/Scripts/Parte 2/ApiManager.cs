using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public const string apiBase = "https://www.dnd5eapi.co";

    [Header("Race Fields")] public APIListResponse charListResponse;
    public APIListResponse traitsListResponse;

    public List<Race> racesList;
    public List<Trait> allTraits;

    [Header("Class Fields")] public APIListResponse classListResponse;
    public APIListResponse proficiencyListResponse;

    public List<Class> allClasses;

    [Header("Alignment field")] public APIListResponse alignmentListResponse;

    public List<Alignament> allAlignment;

    [Header("Weapon Fields")] public EquipmentCategory weaponListResponse;
    public List<Weapon> allWeapons;

    public Action OnFinishedInitLoad;
    public Action OnFinishedTraitLoad;
    public Action OnFinishedProfiLoad;

    public void Start()
    {
        StartCoroutine(InitGetRequest());
    }

    private IEnumerator InitGetRequest()
    {
        using UnityWebRequest charsWebRequest =
            UnityWebRequest.Get("https://www.dnd5eapi.co/api/races");
        yield return charsWebRequest.SendWebRequest();

        charListResponse = JsonUtility.FromJson<APIListResponse>(charsWebRequest.downloadHandler.text);

        for (int i = 0; i < charListResponse.count; i++)
        {
            using UnityWebRequest eachCharsWebRequest =
                UnityWebRequest.Get("https://www.dnd5eapi.co" + charListResponse.results[i].url);
            yield return eachCharsWebRequest.SendWebRequest();

            if (eachCharsWebRequest.responseCode == 200)
            {
                racesList.Add(JsonUtility.FromJson<Race>(eachCharsWebRequest.downloadHandler.text));
            }
        }

        using UnityWebRequest weaponWebRequest =
            UnityWebRequest.Get("https://www.dnd5eapi.co/api/equipment-categories/weapon");
        yield return weaponWebRequest.SendWebRequest();

        if (weaponWebRequest.responseCode != 200) Debug.Log("not 200");

        weaponListResponse = JsonUtility.FromJson<EquipmentCategory>(weaponWebRequest.downloadHandler.text);

        StartCoroutine(_LoadAlignments());
        StartCoroutine(_LoadClasses());
        yield return _LoadWeapons();
        StartCoroutine(_FillWeaponOptions());

        OnFinishedInitLoad?.Invoke();
    }

    public List<Trait> traits;

    public void LoadRaceTraits(Race p_race)
    {
        traits.Clear();
        StartCoroutine(_TraitsGetRequest(p_race));
    }

    public void LoadClassProficiencies(Class c)
    {
        StartCoroutine(ClassProficienciesGetRequest(c));
    }

    public List<Proficiency> currentClassProficiencies;

    private IEnumerator ClassProficienciesGetRequest(Class c)
    {
        // using UnityWebRequest traitsWebRequest =
        // UnityWebRequest.Get("https://www.dnd5eapi.co" + "/api/races/" + race.index + "/traits");
        // yield return traitsWebRequest.SendWebRequest();

        // race.traits = JsonUtility.FromJson<Traits>(traitsWebRequest.downloadHandler.text);
        var proficiencies = c.proficiencies;

        for (var i = 0; i < proficiencies.Length; i++)
        {
            using UnityWebRequest traitListWebRequest =
                UnityWebRequest.Get("https://www.dnd5eapi.co" + proficiencies[i].url);
            yield return traitListWebRequest.SendWebRequest();

            currentClassProficiencies.Add(JsonUtility.FromJson<Proficiency>(traitListWebRequest.downloadHandler.text));
        }

        OnFinishedProfiLoad?.Invoke();
    }

    private IEnumerator _TraitsGetRequest(Race race)
    {
        using UnityWebRequest traitsWebRequest =
            UnityWebRequest.Get("https://www.dnd5eapi.co" + "/api/races/" + race.index + "/traits");
        yield return traitsWebRequest.SendWebRequest();

        race.traits = JsonUtility.FromJson<Traits>(traitsWebRequest.downloadHandler.text);

        for (var i = 0; i < race.traits.count; i++)
        {
            using UnityWebRequest traitListWebRequest =
                UnityWebRequest.Get("https://www.dnd5eapi.co" + race.traits.results[i].url);
            yield return traitListWebRequest.SendWebRequest();

            traits.Add(JsonUtility.FromJson<Trait>(traitListWebRequest.downloadHandler.text));
        }

        OnFinishedTraitLoad?.Invoke();
    }

    private IEnumerator _LoadClasses()
    {
        using UnityWebRequest classesWebRequest =
            UnityWebRequest.Get("https://www.dnd5eapi.co/api/classes");
        yield return classesWebRequest.SendWebRequest();

        classListResponse = JsonUtility.FromJson<APIListResponse>(classesWebRequest.downloadHandler.text);

        for (int i = 0; i < classListResponse.count; i++)
        {
            using UnityWebRequest eachCharsWebRequest =
                UnityWebRequest.Get("https://www.dnd5eapi.co" + classListResponse.results[i].url);
            yield return eachCharsWebRequest.SendWebRequest();

            if (eachCharsWebRequest.responseCode == 200)
            {
                allClasses.Add(JsonUtility.FromJson<Class>(eachCharsWebRequest.downloadHandler.text));
            }
        }
    }

    private IEnumerator _LoadAlignments()
    {
        using UnityWebRequest alignWebRequest =
            UnityWebRequest.Get("https://www.dnd5eapi.co/api/alignments");
        yield return alignWebRequest.SendWebRequest();

        alignmentListResponse = JsonUtility.FromJson<APIListResponse>(alignWebRequest.downloadHandler.text);
        for (int i = 0; i < alignmentListResponse.count; i++)
        {
            using UnityWebRequest eachAlignWebRequest =
                UnityWebRequest.Get("https://www.dnd5eapi.co" + alignmentListResponse.results[i].url);
            yield return eachAlignWebRequest.SendWebRequest();

            if (eachAlignWebRequest.responseCode == 200)
            {
                allAlignment.Add(JsonUtility.FromJson<Alignament>(eachAlignWebRequest.downloadHandler.text));
            }
        }
    }

    private IEnumerator _LoadWeapons()
    {
        for (int i = 0; i < weaponListResponse.equipment.Length; i++)
        {
            using UnityWebRequest eachWeaponWebRequest =
                UnityWebRequest.Get("https://www.dnd5eapi.co" + weaponListResponse.equipment[i].url);
            yield return eachWeaponWebRequest.SendWebRequest();

            if (eachWeaponWebRequest.responseCode == 200)
            {
                allWeapons.Add(JsonUtility.FromJson<Weapon>(eachWeaponWebRequest.downloadHandler.text));
            }
        }
    }

    private IEnumerator _FillWeaponOptions()
    {
        for (int i = 0; i < allClasses.Count; i++)
        {
            int weaponQuantity = Random.Range(1, 5);
            for (int j = 0; j < weaponQuantity; j++)
            {
                int weaponID = Random.Range(0, weaponListResponse.equipment.Length);

                if (allClasses[i].starting_equipment_options == null) allClasses[i].starting_equipment_options = new();
                else allClasses[i].starting_equipment_options.Clear();

                if (!allClasses[i].starting_equipment_options.Contains(weaponListResponse.equipment[weaponID]))
                {
                    allClasses[i].starting_equipment_options.Add(weaponListResponse.equipment[weaponID]);
                }
            }
        }

        yield break;
    }


    public Weapon GetWeapon(string p_index)
    {
        for (int i = 0; i < allWeapons.Count; i++)
        {
            if (allWeapons[i].index == p_index)
            {
                return allWeapons[i];
            }
        }

        return null;
    }
}

[Serializable]
public class Race
{
    public string index;
    public string name;
    public AbilityBonus[] ability_bonuses;
    public Traits traits;
}

[Serializable]
public class AbilityBonus
{
    public int bonus;
    public INUClass ability_score;
}

[Serializable]
public class Traits
{
    public int count;
    public INUClass[] results;
}

[Serializable]
public class Trait
{
    public string index;
    public string name;
    [TextArea(2, 6)] public string[] desc;
}


[Serializable]
public class Class
{
    public string index;
    public string name;
    public StartingEquipment[] starting_equipment;
    public List<INUClass> starting_equipment_options;
    public INUClass[] proficiencies;
}

[Serializable]
public class EquipmentCategory
{
    public string index;
    public string name;
    public INUClass[] equipment;
}

[Serializable]
public class StartingEquipment
{
    public int quantity;
    public INUClass equipment;
}

[Serializable]
public class StartingEquipmentOptions
{
    public string desc;
    public int choose;
    public string type;
    public OptionSet from;
}

[Serializable]
public class OptionSet
{
    public string option_set_type;
    public Option[] options_array;
}

[SerializeField]
public class Option
{
    public string option_type;
    public INUClass item;
}


[Serializable]
public class Alignament
{
    public string index;
    public string name;
    public string url;
    public string desc;
    public string abbreviation;
}


[Serializable]
public class Weapon
{
    public string index;
    public string name;
    public string url;
    public string[] desc;
    public string[] special;
    public INUClass equipment_category;
    public string weapon_category;
    public string weapon_range;
    public string category_range;
    public DamageData damage;
    public DamageData two_handed_damage;
    public Range range;
    public float weight;
}


[Serializable]
public class Range
{
    public float normal;
    public float Long;
}

[Serializable]
public class DamageData
{
    public string damage_dice;
    public INUClass damage_type;
}

[Serializable]
public class Proficiency
{
    public string index;
    public string name;
    public string desc; //there's no description on API
}

[Serializable]
public class INUClass
{
    public string index;
    public string name;
    public string url;
}


[Serializable]
public class APIListResponse
{
    public int count;
    public INUClass[] results;
}