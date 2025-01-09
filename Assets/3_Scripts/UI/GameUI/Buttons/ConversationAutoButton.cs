using UnityEngine;
using UnityEngine.UI;

public class ConversationAutoButton : MonoBehaviour
{
    [SerializeField] Image buttonImage;
    [SerializeField] Image effectImage;
    public void SetImage()
    {
        buttonImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "Auto_Frame");
    }

    public void OnOffEffect(bool _isActive) { effectImage.gameObject.SetActive(_isActive); }
}
