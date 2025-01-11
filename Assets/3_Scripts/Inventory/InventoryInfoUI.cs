using UnityEngine;
using UnityEngine.UI;
using ItemEnums;

public class InventoryInfoUI : MonoBehaviour
{
    [Header("Infos")]
    [SerializeField] GameObject[] InfoObject;
    [SerializeField] GameObject topIndicateObject;
    int currentIndex = 0;

    Sprite[] typeIcons;

    public void TurnOffCurrentInfo()
    {
        if (InfoObject[currentIndex].activeSelf == false) return;

        SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.CurrentItemData = null;
        InfoObject[currentIndex].SetActive (false);
        topIndicateObject.SetActive(false);
    }

    public void Init()
    {
        SetImage();
    }

    public void SetImage()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        typeIcons = new Sprite[InfoObject.Length];
        Sprite descSprite = res.GetSpriteAtlas("Bar_Atlas_2", "InvenDesc_Bar");
        typeIcons[0] = res.GetSpriteAtlas("Icon_Atlas_3", "Type_Etc");
        typeIcons[1] = res.GetSpriteAtlas("Icon_Atlas_3", "Type_Consume");
        typeIcons[2] = res.GetSpriteAtlas("Icon_Atlas_3", "Type_Weapon");
        
        etcIcons[2].sprite = descSprite;
        consumeIcons[2].sprite = descSprite;
        weaponIcons[2].sprite = descSprite;

        consumeIcons[3].sprite = res.GetSpriteAtlas("Button_Atlas","Black_Frame");
        weaponIcons[3].sprite = res.GetSpriteAtlas("Icon_Atlas", "Attack_Icon");
    }

    #region Etc
    [Header("Etc")]
    [SerializeField, Tooltip("0:Icon ,1:TypeIcon, 2:Desc")] Image[] etcIcons;
    [SerializeField] Text etcName;
    [SerializeField] Text etcCntText;
    [SerializeField] Text etcDescriptionText;

    public void ShowInfo(EtcData _itemData)
    {
        currentIndex = (int)ITEMTYPE.ITEM_ETC;
        etcIcons[0].sprite = _itemData.itemIcon;
        etcIcons[1].sprite = typeIcons[0];
        etcName.text = _itemData.itemName;
        etcCntText.text = "X"+_itemData.itemCnt;
        etcDescriptionText.text = _itemData.itemDescription;
        InfoObject[currentIndex].SetActive(true);
        if (topIndicateObject.activeSelf == false)
            topIndicateObject.SetActive(true);
    }
    #endregion

    #region Consume

    [Header("Consume")]
    [SerializeField, Tooltip("0:Icon ,1:TypeIcon, 2:Desc")] Image[] consumeIcons;
    [SerializeField] Text consumeName;
    [SerializeField] Text consumeCntText;
    [SerializeField] Text consumeDescriptionText;
    ConsumeData consumeData = null;

    public void ShowInfo(ConsumeData _itemData)
    {
        consumeData = _itemData;
        currentIndex = (int)ITEMTYPE.ITEM_COMSUME;
        consumeIcons[0].sprite = _itemData.itemIcon;
        consumeIcons[1].sprite = typeIcons[1];
        consumeName.text = _itemData.itemName;
        consumeCntText.text = "X" + _itemData.itemCnt;
        consumeDescriptionText.text = _itemData.itemDescription;
        InfoObject[currentIndex].SetActive(true);
        if(topIndicateObject.activeSelf==false)
            topIndicateObject.SetActive(true);
    }

    public void PressUse()
    {
        SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.DecideUse(consumeData);
    }

    #endregion

    #region Weapon

    [Header("Weapon")]
    [SerializeField, Tooltip("0:Icon ,1:TypeIcon, 2:Desc, 3:First_Att, 4:Second_Att")] Image[] weaponIcons;
    [SerializeField] Text weaponName;
    [SerializeField] Text weaponDescriptionText;
    [SerializeField] Slider weaponLevelSlide;
    [SerializeField] Text weaponLevelText;
    [SerializeField] Text weaponAttackText;
    [SerializeField] Text[] weaponAdditionTexts;

    public void ShowInfo(WeaponData _itemData)
    {
        currentIndex = (int)ITEMTYPE.ITEM_WEAPON;
        weaponIcons[0].sprite = _itemData.itemIcon;
        weaponIcons[1].sprite = typeIcons[2];
 
        weaponName.text = _itemData.itemName;
        weaponDescriptionText.text = _itemData.itemDescription;
        weaponLevelText.text = "Lv."+_itemData.weaponCurrentLevel;
        if (_itemData.weaponMaxExp == 0)
            weaponLevelSlide.value = 1;
        else
            weaponLevelSlide.value = (float)_itemData.weaponCurrentExp / _itemData.weaponMaxExp;
        weaponAttackText.text = _itemData.attackValue.ToString();
     
        switch (_itemData.WeaponEffect)
        {
            case WEAPONEFFECT.WEAPON_ATTACK:
                weaponAdditionTexts[0].text = "공격력 % 증가";
                weaponIcons[4].sprite = weaponIcons[3].sprite;
                break;
            case WEAPONEFFECT.WEAPON_CRITICAL:
                weaponAdditionTexts[0].text = "크리티컬 증가";
                weaponIcons[4].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas","Critical_Icon");
                break;
        }
        weaponAdditionTexts[1].text = (int)(_itemData.effectValue * 100) + "%";
        InfoObject[currentIndex].SetActive(true);
        InfoObject[currentIndex].SetActive(true);
        if (topIndicateObject.activeSelf == false)
            topIndicateObject.SetActive(true);
    }
    #endregion

}
