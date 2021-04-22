using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryPointsCounter : MonoBehaviour
{
    public static event Action<Character, int> OnCharacterFinish;
    public struct CharacterPoints
    {
        public Character character;
        public int points;

        public CharacterPoints(Character character, int points)
        {
            this.character = character;
            this.points = points;
        }
    }
    
    [SerializeField] private float accuralDelay = 2f;
    [SerializeField] private float pointsPerSecond = 1f;
    [SerializeField, Range(1, 10)] private float maxMultiplier = 4;
    [SerializeField, Range(0f, 1f)] private float multDecrease = 0.9f;


    private WaitForSeconds delay;
    private float secondsPassed = 0f;
    private float currentPoints;
    private List<CharacterPoints> charactersPoints;
    private float currentMultiplier;
    void Start()
    {
        var characters = GameDataKeeper.S.characters;
        charactersPoints = new List<CharacterPoints>();
        foreach (var character in characters)
        {
            charactersPoints.Add(new CharacterPoints(character, 0));
        }

        currentMultiplier = maxMultiplier;
        delay = new WaitForSeconds(accuralDelay);
        StartCoroutine(PointCounter());
    }

    private void OnEnable()
    {
        Finish.OnCharacterFinish += RecordPoints;
    }

    private void OnDisable()
    {
        Finish.OnCharacterFinish -= RecordPoints;
    }

    private IEnumerator PointCounter()
    {
        while (!GameManager.isGameEnded)
        {
            secondsPassed += accuralDelay;
            yield return delay;
        } 
    }

    private void RecordPoints(Character character)
    {
        for (var i = 0; i < charactersPoints.Count; i++)
        {
            if (character == charactersPoints[i].character)
            {
                var characterPoints = charactersPoints[i];
                characterPoints.points = Mathf.RoundToInt(pointsPerSecond * secondsPassed * currentMultiplier);
                charactersPoints[i] = characterPoints;
                OnCharacterFinish?.Invoke(charactersPoints[i].character, charactersPoints[i].points);
                currentMultiplier *= multDecrease;
                if (currentMultiplier < 1f)
                    currentMultiplier = 1f;
            }
        }
    }
}
