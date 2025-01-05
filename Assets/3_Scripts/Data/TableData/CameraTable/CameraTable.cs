using ItemTableClassGroup;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraTableClassGroup;

public partial class CameraTable : BaseTable
{
    /*************************************************************
    **************** 카메라 데이터 저장 Dictionary  **************
    *************************************************************/

    #region Camera Data Group : Dictionary 
    // Etc
    Dictionary<int, CameraShakeTableData> shakeDataGroup = new Dictionary<int, CameraShakeTableData>();

    #endregion
    /************************************************************
    **************** 카메라 데이터 반환 Methods *****************
    ************************************************************/

    #region Get Camera Data

    /// <summary>
    /// Get Camera Shake Table Data
    /// </summary>
    /// <param name="_id"></param>
    /// <returns></returns>
    public CameraShakeTableData GetCameraShakeData(int _id)
    {
        if (shakeDataGroup.ContainsKey(_id))
            return shakeDataGroup[_id];
        return null;
    }

    #endregion
}
