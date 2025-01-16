using UnityEngine;
using UnityEngine.UI;

public class WeaponUpgradeMatSlot : MonoBehaviour
{
    [SerializeField, Tooltip("0:Frame, 1:Icon")] Image[] images;
    [SerializeField] Button button;
    [SerializeField] Text cntText;
    EtcData data = null;
    WeaponUpgradeView view = null;
    public void Init(EtcData _data, WeaponUpgradeView _view)
    {
        images[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "WeaponManage_Slot_Button");
        SetData(_data);
        view = _view;
    }

    public void SetData() 
    {
        data = null;
        this.gameObject.SetActive(false);
    }

    public void SetData(EtcData _data)
    {
        data = _data;
        images[1].sprite = _data.GetIcon;
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
        if(view==null) { Debug.LogError("Error : Init"); return; }
        view.PressMatEctButton(data);
    }
}
