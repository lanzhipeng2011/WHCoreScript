using UnityEngine;
using System.Collections;
using Games.LogicObj;
using GCGame.Table;


public class GuideArrow : MonoSingleton<GuideArrow>
{
    GameObject FollowSprite;
    Vector3 TargetPosition;
    bool IsRuning = false;

	private GameObject TargetObject = null;

    public void Show(GameObject sprite,Vector3 targetPos,GameObject targetSprite = null) 
    {
        this.IsRuning = true;
        this.FollowSprite = sprite;
        this.TargetPosition = targetPos;
        gameObject.SetActive(true);

		if(targetSprite != null)
		{
			TargetObject = targetSprite;
		}
    }

    public void Hide() 
    {
        this.IsRuning = false;
        gameObject.SetActive(false);
        this.FollowSprite = null;
    }

	private Obj_Character YHChatacter = null;

    void Update() 
    {
		//===
		if(TargetObject != null)
		{
			if(YHChatacter == null)
				YHChatacter = Singleton<ObjManager>.GetInstance().FindObjCharacterInSceneByName(StrDictionary.GetClientDictionaryString("#{6008}"));
			if(YHChatacter != null)
			{
				GameObject npcGameObject = YHChatacter.gameObject as GameObject;//140201
				if(npcGameObject != null)
				{
					transform.localPosition = FollowSprite.transform.localPosition+new Vector3(0,0.3f,0);
					transform.LookAt(npcGameObject.transform.position);
					transform.rotation = Quaternion.Euler(90f,transform.rotation.eulerAngles.y-90f,transform.rotation.eulerAngles.z);
				}
			}

			return;
		}
		//=====end

        if (this.IsRuning == false || this.FollowSprite == null) 
        {
            return;
        }
        transform.localPosition = FollowSprite.transform.localPosition+new Vector3(0,0.3f,0);
        transform.LookAt(TargetPosition);
        transform.rotation = Quaternion.Euler(90f,transform.rotation.eulerAngles.y-90f,transform.rotation.eulerAngles.z);
     
    }
   

}
