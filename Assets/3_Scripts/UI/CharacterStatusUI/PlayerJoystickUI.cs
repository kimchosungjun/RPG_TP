using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJoystickUI : MonoBehaviour
{
    [SerializeField, Tooltip("0:Area, 1:Joystick , 2:Normal, 3: Q, 4:Skill, 5:W, 6 :Ultimate, 7 :R")] Image[] images;

    public void Init()
    {
        SetImages();
    }

    public void SetImages()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        string atlasName = "JoyStick_Atlas";
        images[0].sprite = res.GetSpriteAtlas(atlasName, "JoyStick_Frame");
        images[1].sprite = res.GetSpriteAtlas(atlasName, "JoyStick_Input_Icon");
        images[2].sprite = res.GetSpriteAtlas(atlasName, "Attack_Button");
        images[3].sprite = res.GetSpriteAtlas(atlasName, "Input_Q");
        images[4].sprite = res.GetSpriteAtlas(atlasName, "Skill_Button");
        images[5].sprite = res.GetSpriteAtlas(atlasName, "Input_E");
        images[6].sprite = res.GetSpriteAtlas(atlasName, "Ultimate_Button");
        images[7].sprite = res.GetSpriteAtlas(atlasName, "Input_R");
    }

    public void InputNormalAttack()
    {

    }

    public void InputSkill()
    {
    }

    public void InputUltimate()
    {

    }



}
