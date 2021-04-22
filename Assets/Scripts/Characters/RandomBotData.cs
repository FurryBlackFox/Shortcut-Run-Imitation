using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomBotData : MonoBehaviour
{
    public static RandomBotData S;

    [SerializeField] private BotData botData = default;
    [Space]
    [SerializeField] private bool parseNicnamesFromTxt = false;
    [SerializeField] private TextAsset nicknamesTxt = default;


    private List<string> nicknames;

    private List<Sprite> countries;
    private List<Color> plankColors;
    private void Awake()
    {
        if (!S)
            S = this;
        
        if (parseNicnamesFromTxt)
        {
            botData.nicknames = new List<string>(nicknamesTxt.text.Split(new char[] {'\n', (char)13},
                StringSplitOptions.RemoveEmptyEntries));
        }

        nicknames = botData.nicknames;
        countries = botData.countries;
        plankColors = botData.plankColors;
       
    }

    public string GetRandomNickname()
    {
        var index = Random.Range(0, nicknames.Count - 1);
        return nicknames[index];
    }

    public Sprite GetRandomCountry()
    {
        var index = Random.Range(0, countries.Count - 1);
        return countries[index];
    }

    public void GetRandomMaterials(out Material opaqueMaterial, out Material transparentMaterial)
    {
        var index = Random.Range(0, plankColors.Count - 1);
        opaqueMaterial = new Material(GameDataKeeper.S.GameSettings.PlanksOpaqueMaterial) 
            {color = plankColors[index]};
        
        var transparentColor = plankColors[index];
        transparentColor.a = GameDataKeeper.S.BotSettings.PlanksTranspMult;
        transparentMaterial = new Material(GameDataKeeper.S.GameSettings.PlanksTransparentMaterial)
            {color = transparentColor};
        
    }
}
