using UnityEngine;
using System.Collections;
using Games.LogicObj;
using Games.GlobeDefine;
using GCGame;
using Games.UserCommonData;
using Games.Item;
using GCGame.Table;
public class SGAutoMedicine : MonoBehaviour
{
    public static SGAutoMedicine Instance;
    public UISprite HPMaskCD;
    public UISprite MPMaskCD;
    public UISprite HPMask;
    public UISprite MPMask;
    public UISprite HPAdd;
    public UISprite MPAdd;
    public UISprite HPSprite;
    public UISprite MPSprite;
    MainPlayerViewModel playerModel;
    GameItemContainer BackPack;
    void Start() 
    {
        Instance = this;
        playerModel = GameViewModel.Get<MainPlayerViewModel>();
        BackPack = GameManager.gameManager.PlayerDataPool.BackPack;
        playerModel.CanAutoHp.Binding(OnCanAutoHP);
        playerModel.CanAutoMp.Binding(OnCanAutoMP);
        playerModel.HPCD.Binding(OnHPCD);
        playerModel.MPCD.Binding(OnMPCD);
    }

    void OnDestroy() 
    {
        playerModel.CanAutoHp.UnBinding(OnCanAutoHP);
        playerModel.CanAutoMp.UnBinding(OnCanAutoMP);
        playerModel.HPCD.UnBinding(OnHPCD);
        playerModel.MPCD.UnBinding(OnMPCD);
    }
    void OnHPCD(float newValue) 
    {
        if (playerModel.CanAutoHp.Value)
        {
            HPMask.gameObject.SetActive(newValue == 0 ? false : true);
        }
        HPMaskCD.gameObject.SetActive(newValue == 0 ? false : true);
        HPMaskCD.fillAmount = newValue;
    }
    void OnMPCD(float newValue) 
    {
        if (playerModel.CanAutoMp.Value)
        {
            MPMask.gameObject.SetActive(newValue == 0 ? false : true);
        }
        MPMaskCD.gameObject.SetActive(newValue == 0 ? false : true);
        MPMaskCD.fillAmount = newValue; 
    }
    void OnCanAutoHP(bool newValue) 
    {
        if (newValue)
        {
            HPSprite.gameObject.SetActive(true);
            HPAdd.gameObject.SetActive(false);
            if (playerModel.HPCD.Value == 0)
            {
                HPMask.gameObject.SetActive(false);
            }
        }
        else 
        {
            HPSprite.gameObject.SetActive(false);
            HPAdd.gameObject.SetActive(true);
            HPMask.gameObject.SetActive(true);
        }
    }

    void OnCanAutoMP(bool newValue) 
    {
        if (newValue)
        {
            MPSprite.gameObject.SetActive(true);
            MPAdd.gameObject.SetActive(false);
            if (playerModel.MPCD.Value == 0)
            {
                MPMask.gameObject.SetActive(false);
            }
        }
        else
        {
            MPSprite.gameObject.SetActive(false);
            MPAdd.gameObject.SetActive(true);
            MPMask.gameObject.SetActive(true);
        }
    }

    public void OnAutoHPClick()
    {
       if(!playerModel.CanAutoHp.Value)
       {
           UIManager.ShowUI(UIInfo.SysShop);
           return;
       }
       if (Singleton<ObjManager>.Instance.MainPlayer.AutoPercent((int)MedicSubClass.HP) == false)
       {
           Singleton<ObjManager>.Instance.MainPlayer.UpdateSelectDrug(); //重新选择药
           Singleton<ObjManager>.Instance.MainPlayer.AutoPercent((int)MedicSubClass.HP);
       }
    }
    public void OnAutoMPClick()
    {
        if (!playerModel.CanAutoMp.Value)
        {
            UIManager.ShowUI(UIInfo.SysShop);
            return;
        }
        if (Singleton<ObjManager>.Instance.MainPlayer.AutoPercent((int)MedicSubClass.MP) == false)
        {
            Singleton<ObjManager>.Instance.MainPlayer.UpdateSelectDrug(); //重新选择药
            Singleton<ObjManager>.Instance.MainPlayer.AutoPercent((int)MedicSubClass.MP);
        }
    }
    
}

