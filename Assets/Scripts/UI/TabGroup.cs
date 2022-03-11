using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [SerializeField]
    private List<Tab> tabs;

    private Tab activeTab;
    

    private void Start()
    {
        foreach (var tab in tabs)
        {
            tab.SubscribeToTabGroup(this);
        }

        if (tabs.Count > 0)
        {
            activeTab = tabs[0];
            activeTab.IsActive(true);
        }
    }


    public void OnTabEnter(Tab tab)
    {

    }

    public void OnTabExit(Tab tab)
    {
       
    }

    public void OnTabClick(Tab tab)
    {
        activeTab.IsActive(false);
        tab.IsActive(true);
        activeTab = tab;
    }
}
