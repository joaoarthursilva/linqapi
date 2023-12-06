using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class Linq : MonoBehaviour
{
    public Players playersList;

    [Serializable]
    public class Ex1and5Text
    {
        public TextMeshProUGUI id;
        public TextMeshProUGUI name;
    }

    [Header("Ordenar por país os jogadores que ainda não criaram heróis")]
    public List<Player> ex1;

    public List<Ex1and5Text> ex1Texts;


    [Header("Qual é a classe de herói mais criada e a menos criada")]
    public Ex2 ex2;

    public TextMeshProUGUI mostCreated;
    public TextMeshProUGUI leastCreated;

    [Header("Qual é o país com mais jogadores")]
    public string ex3;

    public TextMeshProUGUI ex3txt;

    [Header("Plataforma que possui os jogadores com melhores pontos")]
    public string ex4;

    public TextMeshProUGUI ex4txt;

    [Header("Top 10 jogadores com maior total de gold")]
    public List<Player> ex5;

    public List<Ex1and5Text> ex5Texts;

    [Serializable]
    public class Ex2
    {
        public string leastCreatedHeroClass;

        public string mostCreatedHeroClass;
    }


    private void Start()
    {
        var json = File.ReadAllText(Application.dataPath + "/data.json");

        playersList = JsonUtility.FromJson<Players>(json);

        // 1
        ex1 = playersList.players.Where(p => p.heroes.Count == 0).OrderBy(p => p.countryName).ToList();
        for (var i = 0; i < ex1.Count; i++)
        {
            ex1Texts[i].id.text = ex1[i].id.ToString();
            ex1Texts[i].name.text = ex1[i].name;
        }

        // 2
        var createdClasses = playersList.players.Select(p => p.heroes).SelectMany(hl => hl).ToList();

        var count = (from heroClass in createdClasses
            group heroClass by heroClass.heroClassName
            into g
            let heroCount = g.Count()
            orderby heroCount descending
            select new {ClassName = g.Key, TimesCreated = heroCount}).ToList();

        ex2 = new Ex2
        {
            leastCreatedHeroClass = count.Last().ClassName,
            mostCreatedHeroClass = count.First().ClassName
        };

        mostCreated.text = $"mais: {ex2.mostCreatedHeroClass}";
        leastCreated.text = $"menos: {ex2.leastCreatedHeroClass}";

        // 3
        ex3 = playersList.players.Select(p => p.countryName).GroupBy(g => g).Select(s => new
        {
            key = s.Key,
            value = s.Count()
        }).OrderByDescending(o => o.value).ToList().First().key;
        ex3txt.text = ex3;

        // 4
        ex4 = playersList.players.GroupBy(g => g.platformName).Select(s => new
        {
            key = s.Key,
            value = s.Average(a => a.points)
        }).OrderByDescending(o => o.value).ToList().First().key;
        ex4txt.text = ex4;

        // 5
        ex5 = playersList.players.OrderByDescending(o => o.heroes.Sum(s => s.gold)).Take(10).ToList();
        for (var i = 0; i < ex5.Count; i++)
        {
            ex5Texts[i].id.text = ex5[i].id.ToString();
            ex5Texts[i].name.text = ex5[i].name;
        }
    }

    [Serializable]
    public class Players
    {
        public List<Player> players;
    }

    [Serializable]
    public class Player
    {
        public int id;
        public string name;
        public string username;
        public int points;
        public int platformIndex;
        public string platformName;
        public int countryIndex;
        public string countryName;
        public List<Hero> heroes;
    }

    [Serializable]
    public class Hero
    {
        public int id;
        public string name;
        public int level;
        public int heroClassIndex;
        public string heroClassName;
        public int gold;
    }
}