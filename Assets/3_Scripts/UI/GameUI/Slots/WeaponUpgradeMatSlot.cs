using UnityEngine;
using UnityEngine.UI;

public class WeaponUpgradeMatSlot : MonoBehaviour
{
    [SerializeField, Tooltip("0:Frame, 1:Icon")] Image[] images;
    [SerializeField] Button button;
    [SerializeField] Text cntText;
    EtcData data = null;

    public void Init(EtcData _data)
    {
        images[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "WeaponManage_Slot_Button");
        SetData(data);
    }

    public void SetData(EtcData _data)
    {
        data = _data; 
        images[1].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas(_data.atlasName,_data.fileName+"_Icon");
        cntText.text = "X"+data.itemCnt;

        if (images[1].gameObject.activeSelf == false)
            images[1].gameObject.SetActive(true);
        if (cntText.gameObject.activeSelf == false)
            cntText.gameObject.SetActive(true);
    }

    public void ClearData()
    {
        data = null;
        if (images[1].gameObject.activeSelf )
            images[1].gameObject.SetActive(false);
        if (cntText.gameObject.activeSelf )
            cntText.gameObject.SetActive(false);
    }


    public void PressMatSlot()
    {
        Debug.Log("snf ");
    }
}
