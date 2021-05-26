using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Player : Character
{

    public event Action OnPlayerFinished;
    public event Action<int> OnPlanksOverload;
    public event Action<Vector3> OnPlankCollected;
    
    
    public bool inputEnabled = true;

    [SerializeField] protected PlayerData playerData = default;
    public PlayerData PlayerData => playerData;
    [SerializeField] protected bool enableAutoRun = false;
    public bool EnableAutoRun => enableAutoRun;
    
 


    private PlayerMovementController playerMovementController;
    private PlayerPlanksController playerPlanksController;
    

    protected override void Awake()
    {
        base.Awake();
        playerMovementController = GetComponent<PlayerMovementController>();
        playerPlanksController = GetComponent<PlayerPlanksController>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
                
        PlayerInput.OnJump += playerMovementController.Jump; //TODO: delete
        PlayerInput.OnMovementInput += playerMovementController.ApplyInput;

        playerPlanksController.OnPlanksOverload += OnPlanksOverloadHandler;
        playerPlanksController.OnPlankCollected += OnPlankCollectedHandler;
        GameManager.OnPlayerDeath += OnPlayerDeathHandler;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
                
        PlayerInput.OnJump -= playerMovementController.Jump; //TODO: delete
        PlayerInput.OnMovementInput -= playerMovementController.ApplyInput;

        playerPlanksController.OnPlanksOverload -= OnPlanksOverloadHandler;
        playerPlanksController.OnPlankCollected -= OnPlankCollectedHandler;
        GameManager.OnPlayerDeath -= OnPlayerDeathHandler;
    }

    protected override void Start()
    {
        base.Start();
        
        characterName = playerData.PlayerName;
        uiCharacterNameText.SetText(characterName);

    }


    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.CompareTag("Finish"))
        {
            OnPlayerFinished?.Invoke();
            inputEnabled = false;
        }
    }
    
    protected override void FixedUpdate()
    {
        base.FixedUpdate();


    }

    private void OnPlanksOverloadHandler(int count)
    {
        OnPlanksOverload?.Invoke(count);
    }

    private void OnPlankCollectedHandler(Vector3 lastPlankPos)
    {
        OnPlankCollected?.Invoke(lastPlankPos);
    }

    private void OnPlayerDeathHandler()
    {
        inputEnabled = false;
    }
    
    #region Economics

    public bool AddCoins(int value)
    {
        if (value < 0)
            return false;
        playerData.CoinsCount += value;
        return true;
    }

    public bool DebitCoins(int value)
    {
        if (value < 0 | playerData.CoinsCount < value)
            return false;
        playerData.CoinsCount -= value;
        return true;
    }

    #endregion



}
