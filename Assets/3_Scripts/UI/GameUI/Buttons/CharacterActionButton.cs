using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterActionButton : MonoBehaviour
{
    [SerializeField] ActionBtnType actionType;
    [SerializeField, Tooltip("0 : Frame, 1 : Fill, 2 : Key")] Image[] images;
    [SerializeField] Text coolDownText;
    Coroutine coolDownCor = null;
    bool canPress = true;
    protected enum ActionBtnType
    {
        NormalAtk = 0,
        Skill,
        UltimateSkill
    }   

    public void PressActionBtn(float _time)
    {
        if (!canPress)
            return;
        // To Do ~~~~ : Mobile
        coolDownCor = StartCoroutine(CCoolDown(_time));
    }

    public void Init()
    {
        SetImages();
        images[1].fillAmount = 1;
        coolDownText.text = string.Empty;
    }

    public void SetImages()
    {
        string atlasName = "JoyStick_Atlas";
        ResourceMgr res = SharedMgr.ResourceMgr;
        Sprite inputIndicateSprite = null;
        switch (actionType)
        {
            case ActionBtnType.NormalAtk:
                inputIndicateSprite = res.GetSpriteAtlas(atlasName, "Attack_Button");
                images[0].sprite = inputIndicateSprite;
                images[1].sprite = inputIndicateSprite;
                images[2].sprite = res.GetSpriteAtlas(atlasName, "Input_Q");
                break;
            case ActionBtnType.Skill:
                inputIndicateSprite = res.GetSpriteAtlas(atlasName, "Skill_Button");
                images[0].sprite = inputIndicateSprite;
                images[1].sprite = inputIndicateSprite;
                images[2].sprite = res.GetSpriteAtlas(atlasName, "Input_E");
                break;
            case ActionBtnType.UltimateSkill:
                inputIndicateSprite = res.GetSpriteAtlas(atlasName, "Ultimate_Button");
                images[0].sprite = inputIndicateSprite;
                images[1].sprite = inputIndicateSprite;
                images[2].sprite = res.GetSpriteAtlas(atlasName, "Input_R");
                break;
        }
    }

    public void ChangeData()
    {
        if (coolDownCor != null)
        {
            StopCoroutine(coolDownCor);
            coolDownCor = null;
        }
        // To Do ~~~ change
    }

    IEnumerator CCoolDown(float _time)
    {
        images[1].fillAmount = 0;
        float coolDownTime = _time;
        float time = 0;
        while (coolDownTime >=0 )
        {
            if (coolDownTime < 0)
                break;
            images[1].fillAmount = time/_time;
            coolDownText.text = coolDownTime.ToString("F1");
            coolDownTime -= Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }
        images[1].fillAmount = 1f;
        coolDownText.text = string.Empty;
        coolDownCor = null;
        canPress = true;
    }
}
