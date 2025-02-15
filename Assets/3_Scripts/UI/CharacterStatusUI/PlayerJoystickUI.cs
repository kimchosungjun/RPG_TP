using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJoystickUI : MonoBehaviour
{
    [SerializeField, Tooltip("0:Area, 1:Joystick , 2:Normal, 3: Q, 4:Skill, 5:W, 6 :Ultimate, 7 :R")] Image[] images;
    [SerializeField] CharacterActionButton[] actionButtons;

    public void Init()
    {
        SetImages();
        for (int i = 0; i < 3; i++)
        {
            actionButtons[i].Init();
        }
    }

    public void SetImages()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        string atlasName = "JoyStick_Atlas";
        images[0].sprite = res.GetSpriteAtlas(atlasName, "JoyStick_Frame");
        images[1].sprite = res.GetSpriteAtlas(atlasName, "JoyStick_Input_Icon");
    }

    public void InputNormalAttack()
    {
        actionButtons[0].PressActionBtn(0.1f);
    }

    public void InputSkill(float _time)
    {
        actionButtons[1].PressActionBtn(_time);
    }

    public void InputUltimate(float _time)
    {
        actionButtons[2].PressActionBtn(_time);
    }
}
