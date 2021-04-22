using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPositionChecker : MonoBehaviour
{
    public static event Action<Character> OnFirstPositionObtained;
    public static event Action<Character> OnFirstPositionLost; 
    public static event Action<int> OnPlayerPositionUpdated;

    [SerializeField, Min(0f)] private float activationDelay = 5f;
    [SerializeField, Range(0f, 5f)] private float checkDelay = 1f;
    
    
    private WaitForSeconds delay;
    private List<Character> characters;
    private Character player;
    private int cashedPlayerPos;
    private Character cashedFirstPosCharacter;

    private void Start()
    {
        characters = GameDataKeeper.S.characters;
        player = GameDataKeeper.S.Player;
        delay = new WaitForSeconds(checkDelay);
        StartCoroutine(CheckPosition());
    }
    
    private IEnumerator CheckPosition()
    {
        yield return new WaitForSeconds(activationDelay);
        var finishPos = Finish.Position;
        finishPos.y = 0;
        while (enabled)
        {
            var playerPos = 1;

            var characterPos = player.transform.position;
            characterPos.y = 0;
            var playerDistance = (finishPos - characterPos).sqrMagnitude;
            
            var firstCharacter = player;
            var firstCharacerDistance = playerDistance;
        
            for (var i = 1; i < characters.Count; i++)
            {
                characterPos = characters[i].transform.position;
                characterPos.y = 0;
                var currentCharacterDistance = (finishPos - characterPos).sqrMagnitude;
                if (playerDistance > currentCharacterDistance)
                    playerPos++;
                if (firstCharacerDistance > currentCharacterDistance)
                {
                    firstCharacter = characters[i];
                    firstCharacerDistance = currentCharacterDistance;
                }
            }
            
            if (playerPos != cashedPlayerPos)
            {
                cashedPlayerPos = playerPos;
                OnPlayerPositionUpdated?.Invoke(playerPos);
            }

            if (cashedFirstPosCharacter != firstCharacter)
            {
                OnFirstPositionLost?.Invoke(cashedFirstPosCharacter);
                OnFirstPositionObtained?.Invoke(firstCharacter);
                cashedFirstPosCharacter = firstCharacter;
            }
            
            
            yield return delay;
        }

    }
}
