using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class UserInterfaceAudio
{
    public MainMenuUserInterfaceAudio MainMenuUserInterfaceAudio;
    
    public SelectionSceneUserInterfaceAudio SelectionSceneUserInterfaceAudio;
    
    public SceneLoadUserInterfaceAudio SceneLoadUserInterfaceAudio;
    
    public ButtonUserInterfaceAudio ButtonUserInterfaceAudio;
    
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

[System.Serializable]
public class SceneLoadUserInterfaceAudio
{
    public SpecificAudio SceneLoadStart;

    public SpecificAudio SceneLoadMiddle;
    
    public SpecificAudio SceneLoadEnd;
}

[System.Serializable]
public class ButtonUserInterfaceAudio
{
    public SpecificAudio[] ButtonPressedAudio;
}