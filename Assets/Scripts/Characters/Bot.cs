using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
[RequireComponent(typeof(TrailRenderer))]
public class Bot : Character
{

    #region Variables
    
    [Header("Editor Tools")] 
    [SerializeField] private bool showTrace = true;

    [SerializeField] private BotNavigationSphere navigationSphere;
    public BotNavigationSphere NavigationSphere => navigationSphere;


    private WaitForSeconds planksTranspCheckWaitForSeconds;
    
    private BotSettings botSettings;
    private BotMovementController botMovementController;
    private BotPlanksController botPlanksController;
    
    
    internal int PlanksCount => botPlanksController.PlanksCount;
    
    #endregion

    
    protected override void Awake()
    {
        base.Awake();
        botMovementController = GetComponent<BotMovementController>();
        botPlanksController = GetComponent<BotPlanksController>();
        
        botSettings = GameDataKeeper.S.BotSettings;
        
    }


    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    
    protected override void Start()
    {
        base.Start();
        
        GetComponent<TrailRenderer>().enabled = showTrace;
        GetRandomCharacterData();
    }
    

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.CompareTag("TransparentPlanksTrigger") && botSettings.EnablePlanksTransp)
        {
            botPlanksController.ApplyNewMaterial(BotPlanksController.PlanksMaterial.Transparent);
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TransparentPlanksTrigger") && botSettings.EnablePlanksTransp)
        {
            botPlanksController.ApplyNewMaterial(BotPlanksController.PlanksMaterial.Opaque);
        }
    }
    
   
    #region Other

 
    private void GetRandomCharacterData()
    {
        characterName = RandomBotData.S.GetRandomNickname();
        uiCharacterNameText.SetText(characterName);
        RandomBotData.S.GetRandomMaterials(out botPlanksController.planksMaterialOpaque, 
            out botPlanksController.planksMaterialTransparent);
        botPlanksController.currentPlanksMaterial = botPlanksController.planksMaterialOpaque;
    }

    #endregion

}
