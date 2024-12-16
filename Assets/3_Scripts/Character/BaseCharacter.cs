using UnityEngine;
using UtilEnums;

public abstract class BaseCharacter : MonoBehaviour
{
    /**************************************/
    /************** 레이어 ***************/
    /************** 테이블 ***************/
    /**************************************/

    #region Protected : Layer Value
    protected int noneInteractionLayer = 0; 
    protected int bitLayer;
    protected int intLayer;
    protected TABLE_FOLDER_TYPES characterTableType;
    #endregion

    #region Property
    public int GetBitLayer { get { return bitLayer; } }
    public int GetIntLayer { get { return intLayer; } } 
    public TABLE_FOLDER_TYPES GetCharacterTableType { get { return characterTableType; } }
    #endregion

    #region Abstract Method 
    /// <summary>
    /// Layer와 캐릭터 테이블 데이터 타입을 설정한다.
    /// </summary>
    public abstract void SetCharacterType();
    public virtual void SetNoneInteractionType() { this.gameObject.layer = noneInteractionLayer; }
    #endregion 
}
