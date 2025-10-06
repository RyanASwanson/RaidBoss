using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserInterfaceAudio
{
    public MainMenuUserInterfaceAudio MainMenuUserInterfaceAudio;
    
    public SelectionSceneUserInterfaceAudio SelectionSceneUserInterfaceAudio;
    
    public SpecificAudio[] GeneralUIAudio;
}

[System.Serializable]
public class MainMenuUserInterfaceAudio
{
    public SpecificAudio MainMenuPlay;
}

[System.Serializable]
public class SelectionSceneUserInterfaceAudio
{
    public SpecificAudio BossSelected;
    
    public SpecificAudio[] DifficultySelected;
    
    public SpecificAudio HeroSelected;
}