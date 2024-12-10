using UnityEngine;
using System.Collections;
using Games.GlobeDefine;
public class SGDamageBoardManager :Singleton<SGDamageBoardManager>
{
    /*
     Client
      AbiliAttack  
       if(KeyDown(J))
       {
          
       }
     
     Server
      AbiliAttack
     */
    /*
     SceneLogic
        Ability (AbiliMove AbiliAttack)
        Actor
        Unit
          Ability
          Unit
      UnitManager (CreateUnit{ New Unit } )
      SceneLogic
      
       Ability  -> Unit
       Ability <-SceneLogic
     */
    public void ShowDamageBoard(GameDefine_Globe.DAMAGEBOARD_TYPE typ) 
    {
        //设置内禁用伤害信息.
        if (PlayerPreferenceData.SystemDamageBoardEnable == false) 
        {
            string[] lst = new string[] { "AA", "BB", "CC" };
           
            return;

        }

    }
}
