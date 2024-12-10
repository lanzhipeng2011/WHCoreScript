using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Clojure;
using GCGame.Table;
using Games.GlobeDefine;
using Games.Animation_Modle;
using Games.LogicObj;
public class ActionFuncDefine 
{

    //上一个创建的单位
    public static GameObject LastCreateUnit;

    public static void Define() 
    {
        Context context = GameTrigger.Instance.ClojureContext;

        context.Define("last-unit", new ObjectNode(LastCreateUnit));
        context.Define("vec3",new FunctionNode(_Vec3));
        context.Define("set-player-pos",new FunctionNode(_set_player_pos));
        context.Define("npc-number",new FunctionNode(_npc_number));
		context.Define("destroyZhaLan",new FunctionNode(destroyZhanLan));
        context.Define("turn-off",new FunctionNode(_turn_off));
        context.Define("turn-on", new FunctionNode(_turn_on));
        context.Define("send-tutorial-step",new FunctionNode(_send_tutorial_step));
        context.Define("dis-connect", new FunctionNode(_disconnect));
        context.Define("re-connect",new FunctionNode(_reconnect));
        context.Define("event-mask",new FunctionNode(_show_event_mask));
        context.Define("close-event-mask",new FunctionNode(_close_event_mask));
        context.Define ("show-ui",new FunctionNode(_show_ui));
        context.Define("close-ui",new FunctionNode(_close_ui));
        context.Define("show-guide-arrow", new FunctionNode(_show_guide_arrow));
        context.Define("hide-guide-arrow", new FunctionNode(_hide_guide_arrow));
        context.Define("enable-move", new FunctionNode(enable_move));
        context.Define("change-fight",new FunctionNode(_change_npc_fight));
        context.Define("player-near-pos?", new FunctionNode(player_near_pos));
        context.Define("start-movie-mode",new FunctionNode(_start_movie_mode));
        context.Define("end-movie-mode",new FunctionNode(_end_movie_mode));
        context.Define("set-camera",new FunctionNode(set_camera));
        context.Define("create-unit",new FunctionNode(_create_unit));
        context.Define("play-anim",new FunctionNode(_play_anim));
        context.Define("destory-unit",new FunctionNode(_destory_unit));
        context.Define("close-trigger",new FunctionNode(_close_trigger));
        context.Define("id2point", new FunctionNode(_GetPointById));
        context.Define("def",new FunctionNode(_def_var));
        context.Define("get-yinghong",new FunctionNode(_get_obj_by_type));
        context.Define("obj-pos",new FunctionNode(_obj_pos));
        context.Define("near-pos?", new FunctionNode(near_pos));
        context.Define("call-event",new FunctionNode(_call_event));
		context.Define("_play_camera_animation",new FunctionNode(_play_camera_animation));
		context.Define("_play_juqingitem_effect",new FunctionNode(_play_juqingitem_effect));
		context.Define("_set_camera_state",new FunctionNode(_set_camera_state));
		context.Define("_set_isfirst_yexidaying",new FunctionNode(_set_isfirst_yexidaying));
        context.Define("change-scene",new FunctionNode(_change_scene));
	}

    public static ASTNode _call_event(List<ASTNode> args, Context context) 
    {
        string eventName = ((StringNode)args[0]).Val;
        GameTrigger.Instance.CallEvent(eventName);
        return null;
    }
    public static ASTNode _obj_pos(List<ASTNode> args, Context context) 
    {
        args = EvalArgs(args,context);
        if (args[0] == null)
        {
            return new ObjectNode(Vector3.zero);
        }
        GameObject obj = (GameObject)((ObjectNode)args[0]).Val;
        
        return new ObjectNode(new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, obj.transform.localPosition.z));
    }
    public static ASTNode _get_obj_by_type(List<ASTNode> args, Context context) 
    {
        string Force = (string)((StringNode)args[0]).Val;
        ObjectNode astobj = new ObjectNode(ObjManager.Instance.FindCharacterByName(Force)[0].gameObject);
        return astobj;
    }

    public static ASTNode _def_var(List<ASTNode> args, Context context) 
    {
        args[1] = RT.Evaluate(args[1], context);
        string varName = ((SymbolNode)args[0]).Val;
        ASTNode node = context.Find(varName);
        if (node == null)
        {
            context.Define(varName, args[1]);
        }
        else 
        {
            context.Replace(varName,args[1]);
        }
        return null;
    }
    public static ASTNode _GetPointById(List<ASTNode> args, Context context) 
    {
        args = EvalArgs(args,context);
        Tab_Points point = TableManager.GetPointsByID((int)((NumberNode)args[0]).NumberVal)[0];
        return new ObjectNode(new Vector3(point.PosX,point.PosY,point.PosZ));
    }
    public static ASTNode _close_trigger(List<ASTNode> args,Context context) 
    {
        int triggerId = (int)((NumberNode)args[0]).NumberVal;
        GameTrigger.Instance.Triggers[triggerId].CloseTrigger();
        return null;
    }
    public static ASTNode _close_ui(List<ASTNode> args, Context context) 
    {
        args = EvalArgs(args, context);
        string UIName = ((StringNode)args[0]).Val;
        UIPathData pathData = (UIPathData)typeof(UIInfo).GetField(UIName).GetValue(null);
        UIManager.CloseUI(pathData);
        return null;
    }
    public static ASTNode _destory_unit(List<ASTNode> args, Context context) 
    {
        args = EvalArgs(args,context);
        if (((ObjectNode)args[0]).Val != null)
        {
            GameObject obj = (GameObject)((ObjectNode)args[0]).Val;
            GameObject.Destroy(obj);
        }
        return null;
    }
    //
    public static ASTNode _play_anim(List<ASTNode> args, Context context) 
    {
        args = EvalArgs(args,context);
        GameObject obj = (GameObject)((ObjectNode)args[0]).Val;
        Animation anim = obj.GetComponent<Animation>();
        for (int i = 1; i < args.Count;i++)
        {
            anim.Play(((StringNode)args[i]).Val);
        }
        return null;
    }
    public static ASTNode _create_unit(List<ASTNode> args, Context context) 
    {
        args = EvalArgs(args,context);
        int attrId = (int)((NumberNode)args[0]).NumberVal;
        Tab_RoleBaseAttr tab_attr = TableManager.GetRoleBaseAttrByID(attrId)[0];
        Tab_CharModel char_model = TableManager.GetCharModelByID(tab_attr.CharModelID)[0];
        GameObject new_obj = (GameObject) GameObject.Instantiate( (GameObject)Resources.Load("Model/"+ char_model.ResPath) );
        new_obj.transform.localPosition = (Vector3)((ObjectNode)args[1]).Val;
        if (args.Count == 3)
        {
            new_obj.transform.localRotation = Quaternion.Euler((Vector3)((ObjectNode)args[2]).Val);
        }

        ((ObjectNode)context.Find("last-unit")).Val = new_obj;
        return null;
    }
    public static ASTNode set_camera(List<ASTNode> args, Context context) 
    {
        args =EvalArgs(args,context);
        Vector3 pos =(Vector3) ((ObjectNode)args[0]).Val;
        Vector3 rotation = (Vector3)((ObjectNode)args[1]).Val;
        Camera.main.transform.localPosition = pos;
        Camera.main.transform.localRotation = Quaternion.Euler(rotation);
        return null; 
    }

    public static ASTNode _start_movie_mode(List<ASTNode> args, Context context) 
    {
        CameraController cc  = ObjManager.Instance.MainPlayer.gameObject.GetComponent<CameraController>();
        cc.enabled = false;
        GameObject.Destroy(cc);
		if (ObjManager.Instance.MainPlayer.ObjEffectLogic!=null)
		{
			
			ObjManager.Instance.MainPlayer.ObjEffectLogic.StopEffect(-1);
		}
        if (ObjManager.Instance.MainPlayer.SkillCore!=null)
        {
		
          ObjManager.Instance.MainPlayer.SkillCore.SkillFinsh();
        }
        ObjManager.Instance.MainPlayer.gameObject.SetActive(false);
        ObjManager.Instance.MainPlayer.CloseNameBoard();
        
         Games.LogicObj.Obj_Fellow fellow = ObjManager.Instance.MainPlayer.GetCurFellow();
         if (fellow != null) 
         {
             fellow.gameObject.SetActive(false);
         }
        GameManager.EnableMove = false;
        return null;
    }

    public static ASTNode _end_movie_mode(List<ASTNode> args, Context context) 
    {
        ObjManager.Instance.MainPlayer.CameraController = ObjManager.Instance.MainPlayer.gameObject.AddComponent<CameraController>();
       // UIManager.Instance().ShowAllUILayer();
        ObjManager.Instance.MainPlayer.gameObject.SetActive(true);
        ObjManager.Instance.MainPlayer.ShowNameBoard();
        GameManager.EnableMove = true;
        Games.LogicObj.Obj_Fellow fellow = ObjManager.Instance.MainPlayer.GetCurFellow();
        if (fellow != null)
        {
            fellow.gameObject.SetActive(true);
        }
        return null;
    }
    public static ASTNode player_near_pos(List<ASTNode> args, Context context) 
    {
        BoolNode bnode = new BoolNode();
        bnode.Val = false;
        Vector3 vec3Pos = ObjManager.Instance.MainPlayer.transform.localPosition;
        args = EvalArgs(args,context);
        Vector3 dstPos = (Vector3)((ObjectNode)args[0]).Val;
        float number = (float)((NumberNode)args[1]).NumberVal;
        if (Vector3.Distance(dstPos, vec3Pos) < number) 
        {
            bnode.Val = true;
        }
        return bnode;
    }
      
    public static ASTNode near_pos(List<ASTNode> args, Context context)
    {
        args = EvalArgs(args, context);
        BoolNode bnode = new BoolNode();
        bnode.Val = false;
        Vector3 vec3Pos = (Vector3)((ObjectNode)args[0]).Val;
 
        Vector3 dstPos = (Vector3)((ObjectNode)args[1]).Val;
        float number = (float)((NumberNode)args[2]).NumberVal;
        if (Vector3.Distance(dstPos, vec3Pos) < number)
        {
            bnode.Val = true;
        }
        return bnode;
    }
    public static ASTNode enable_move(List<ASTNode> args, Context context) 
    {
        bool b = ((BoolNode)args[0]).Val;
        GameManager.EnableMove = b;
        return null;
    }
    public static List<ASTNode> EvalArgs(List<ASTNode> args, Context context)
    {
        for (int i = 0; i < args.Count; i++)
        {
            args[i] = RT.Evaluate(args[i], context);
        }
        return args;
    }
    public static ASTNode _set_player_pos(List<ASTNode> args, Context context)
    {
        args[0] = RT.Evaluate(args[0],context);
        Vector3 vec3 =(Vector3)((ObjectNode)args[0]).Val;
        NavMeshAgent agent = ObjManager.Instance.MainPlayer.GetComponent<NavMeshAgent>();
        agent.enabled = false;
        ObjManager.Instance.MainPlayer.gameObject.transform.localPosition = vec3;
        agent.enabled = true;
        return null;
    }

    public static ASTNode _Vec3(List<ASTNode> args, Context context)
    {
        float X =(float)((NumberNode)args[0]).NumberVal;
        float Y = (float)((NumberNode)args[1]).NumberVal;
        float Z = (float)((NumberNode)args[2]).NumberVal;
        Vector3 vec = new Vector3(X,Y,Z);
        return new ObjectNode(vec);
    }

    public static ASTNode _npc_number(List<ASTNode> args, Context context) 
    {
        NumberNode numberNode = new NumberNode();
        numberNode.NumberVal =  ObjManager.Instance.GetShowNPCNum();
        numberNode.Type = NumberNode.NumberType.INT;
        return numberNode;
    }
	
	public static ASTNode   destroyZhanLan(List<ASTNode> args, Context context) 
	{
		Obj_ZhaLan obj = Obj_ZhaLan.GetInstance ();
		if (obj != null) 
		{
			obj.playdeath();

		}
		return   null;
	}

    /// <summary>
    /// 关闭某个触发器
    /// </summary>
    /// <param name="args">0 触发器ID</param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static ASTNode _turn_off(List<ASTNode> args, Context context) 
    {
        for (int i = 0; i < args.Count; i++) 
        {
            int triggerID = (int)((NumberNode)args[i]).NumberVal;
            GameTrigger.Instance.Triggers[triggerID].TriggerEnable = false;
        }
        return null;
    }
    /// <summary>
    /// 打开某个触发器.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static ASTNode _turn_on(List<ASTNode> args, Context context)
    {
        args[0] = RT.Evaluate(args[0],context);
        int triggerID = (int)((NumberNode)args[0]).NumberVal;
        GameTrigger.Instance.Triggers[triggerID].TriggerEnable = true;
        return null;
    }

    public static ASTNode _send_tutorial_step(List<ASTNode> args, Context context) 
    {
        args[0] = RT.Evaluate(args[0],context);
        CG_GUIDE_STEP packet = (CG_GUIDE_STEP)PacketDistributed.CreatePacket(MessageID.PACKET_CG_GUIDE_STEP);
        packet.StepID = (int)((NumberNode)args[0]).NumberVal;
        packet.SendPacket();
        return null;
    }

    
    public static ASTNode _disconnect(List<ASTNode> args, Context context) 
    {
        NetWorkLogic.GetMe().DisconnectServer();
        return null;
    }

    public static ASTNode _reconnect(List<ASTNode> args, Context context) 
    {
        NetManager.Instance().OnReconnect();
        return null;
    }

    public static ASTNode _show_event_mask(List<ASTNode> args, Context context) 
    {
        args = EvalArgs(args,context);
        Vector3 center =(Vector3)((ObjectNode)args[0]).Val;
        Vector3 size = (Vector3)((ObjectNode)args[1]).Val;
        UIManager.ShowUI(UIInfo.UIEventMask, (bool bSuccess, object param) => 
        {
            SGUIEventMask.Instance.Create(center,size);
        });
        return null; 
    }

    public static ASTNode _close_event_mask(List<ASTNode> args,Context context)
    {
        UIManager.CloseUI(UIInfo.UIEventMask);
        return null;
    }

    public static ASTNode _show_ui(List<ASTNode> args, Context context) 
    {
        args = EvalArgs(args,context);
        string UIName = ((StringNode)args[0]).Val;
        UIPathData pathData = (UIPathData)typeof(UIInfo).GetField(UIName).GetValue(null);
        UIManager.ShowUI(pathData);
        return null;
    }

    public static ASTNode _show_guide_arrow(List<ASTNode> args, Context context) 
    {
        args = EvalArgs(args,context);
        Vector3 pos = (Vector3)((ObjectNode)args[0]).Val;
        if (GuideArrow.instance != null) 
        {
            GuideArrow.instance.Show(ObjManager.Instance.MainPlayer.gameObject,pos);
        }
        return null;
    }

    public static ASTNode _hide_guide_arrow(List<ASTNode> args, Context context) 
    {
        if (GuideArrow.instance != null) 
        {
            GuideArrow.instance.Hide();
        }
        return null;
    }

    public static ASTNode _change_npc_fight(List<ASTNode> args, Context context) 
    {
        CG_CH_MON_FIGHT packet = (CG_CH_MON_FIGHT)PacketDistributed.CreatePacket(MessageID.PACKET_CG_CH_MON_FIGHT);
        packet.Fight = 1;
        packet.Sceneclassid = 13;
        packet.Sceneinstid = 1;
        packet.SendPacket();
        return null;
    }
	public static ASTNode _play_camera_animation(List<ASTNode> args, Context context) 
	{
		CameraAnimationLogic cameraAni=GameManager.gameManager.ActiveScene.MainCamera.GetComponent<CameraAnimationLogic> ();
	
	
		args[0] = RT.Evaluate(args[0],context);
		int anid = (int)((NumberNode)args[0]).NumberVal;
		if (cameraAni != null)
		{
			cameraAni.Play(anid);
		}
		Singleton<ObjManager>.Instance.MainPlayer.IsInModelStory=true; 
		return null;
	}
	public static ASTNode _play_juqingitem_effect(List<ASTNode> args, Context context) 
	{
//		args = EvalArgs(args,context);
//		string strName = ((StringNode)args[0]).Val;
//		GameObject gItemObj = Singleton<ObjManager>.GetInstance().FindOtherGameObj(strName);
//		if (gItemObj==null)
//		{
//			return null;
//		}
//
//		gItemObj.GetComponent<Obj_JuqingItem> ().PlayEffect (304);
	//	gItemObj.GetComponent<Obj_JuqingItem> ().DelayRemoveEffct ();
		return null;
	}
	public static ASTNode _set_camera_state(List<ASTNode> args, Context context) 
	{
		args[0] = RT.Evaluate(args[0],context);
		int isreturn = (int)((NumberNode)args[0]).NumberVal;
		Obj_MainPlayer mainplayer = Singleton<ObjManager>.Instance.MainPlayer;
		CameraController cam = null;
		if (mainplayer != null)
		{
			if (mainplayer.CameraController) 
            {
	         cam=mainplayer.CameraController;
			}
		}
				
		if (isreturn == 1) 
		{
		    cam.IsCameraInAnimation = true;	
			Singleton<ObjManager>.Instance.MainPlayer.IsInModelStory=true; 
		} 
		else
		{
			cam.IsCameraInAnimation = false;	
			Singleton<ObjManager>.Instance.MainPlayer.IsInModelStory=false; 
		}
		return null;
	}

	public static ASTNode _set_isfirst_yexidaying(List<ASTNode> args, Context context) 
	{
		//GameManager.gameManager.PlayerDataPool.IsFirstYeXiDaYing=true;
		UserConfigData.AddPlayerSpecial(Singleton<ObjManager>.Instance.MainPlayer.GUID.ToString(),new PlayerSpecialData(true));
		return null;
	}

    public static ASTNode _change_scene(List<ASTNode> args,Context context)
    {
        args = EvalArgs(args,context);
        int classId = (int)((NumberNode)args[0]).NumberVal;
        SceneData.RequestChangeScene((int)CG_REQ_CHANGE_SCENE.CHANGETYPE.POINT, 0, classId, 0);
        return null;
    }
			
}
		
		