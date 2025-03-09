using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class FloatDamageUI : MonoBehaviour
{
    [SerializeField] Transform[] numberLengthTransform;
    Dictionary<int, FlaotDamageGroupSet> damageGroup = new Dictionary<int, FlaotDamageGroupSet>();

    private void Awake()
    {
        FlaotDamageGroupSet group1 = new FlaotDamageGroupSet();   
        FlaotDamageGroupSet group10 = new FlaotDamageGroupSet();   
        FlaotDamageGroupSet group100 = new FlaotDamageGroupSet();   
        FlaotDamageGroupSet group1000 = new FlaotDamageGroupSet(); 
        
        damageGroup.Add(0, group1);
        damageGroup.Add(1, group10);
        damageGroup.Add(2, group100);
        damageGroup.Add(3, group1000);
    }

    public void ShowFloatDamage(int _number, Vector3 _position)
    {
        int num = _number;
        int len = 0;
        int mul = 1;
        while (true)
        {
            num /= 10;
            if (num == 0)
                break;
            len++;
            mul *= 10;
        }

        FloatDamageImage getImage = damageGroup[len].GetImage();
        
        if(getImage==null)
        {
            GameObject imgObject = Instantiate(SharedMgr.ResourceMgr.LoadResource<FloatDamageImage>($"UI/Damage/{mul}") 
                ,_position, Quaternion.identity, numberLengthTransform[len]).gameObject;
            getImage = imgObject.GetComponent<FloatDamageImage>();
            //getImage.ShowImage(_number, len);
            damageGroup[len].AddGroup(getImage);
        }
        else
        {
            getImage.transform.position = _position;
            //getImage.ShowImage(_number, len);
            getImage.gameObject.SetActive(true);
        }
        getImage.ShowImage(_number, len);
    }
}

public class FlaotDamageGroupSet
{
    List<FloatDamageImage> images = new List<FloatDamageImage>();
    
    public void AddGroup(FloatDamageImage _image)
    {
        images.Add(_image);
    }

    public FloatDamageImage GetImage()
    {
        int cnt = images.Count;
        for(int i=0; i<cnt; i++)
        {
            if(images[i].gameObject.activeSelf == false)
                return images[i];
        }

        return null;
    }
}
