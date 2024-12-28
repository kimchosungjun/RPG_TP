using UnityEngine;

public class ConsumeUseUI : MonoBehaviour
{
    ConsumeData consumeData = null;
    [SerializeField] GameObject consumeObject;
    public void Active(ConsumeData _data)
    {
        consumeData = _data;
        consumeObject.SetActive(true);
    }

    public void InActive()
    {
        consumeData = null;
        consumeObject.SetActive(false);
    }

    public void Use()
    {
        consumeData.Use();
    }

    public void NotUse()
    {
        InActive();
    }
}
