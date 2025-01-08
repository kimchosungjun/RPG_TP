using UnityEngine;
using UnityEngine.UI;

public class ConversationAutoButton : MonoBehaviour
{
    [SerializeField] Image effectImage;
    public void SetImage()
    {
        // Atlas
    }

    public void OnOffEffect(bool _isActive) { effectImage.gameObject.SetActive(_isActive); }
}
