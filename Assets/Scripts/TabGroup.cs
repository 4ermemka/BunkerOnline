using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    [SerializeField] private List<TabButton> tabButtons;
    [SerializeField] private Sprite tabIdle;
    [SerializeField] private Sprite tabHover;
    [SerializeField] private Sprite tabActive;

    private TabButton selectedTab;

    public void Subscribe(TabButton button)
    {
        if(tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }
        tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button) 
    {
        ResetTabs();
        if(selectedTab == null || selectedTab != button)
            button.SetBackground(tabHover);
    }

    public void OnTabExit(TabButton button) 
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button) 
    {
        ResetTabs();
        if(selectedTab != null)
        {
            selectedTab.Deselect();
            selectedTab.SetBackground(tabIdle);
        }
        
        if(selectedTab == button)
        {
            selectedTab = null;
        }
        else
        {
            selectedTab = button;
            button.SetBackground(tabActive);
            selectedTab.Select();
        }
    }   

    public void ResetTabs()
    {
        foreach(TabButton tab in tabButtons)
        {
            if(selectedTab == null || selectedTab != tab)
                tab.SetBackground(tabIdle);
        }
    }
}
