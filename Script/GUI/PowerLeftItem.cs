using UnityEngine;
using System.Collections;
using GCGame;
using GCGame.Table;
using System.Collections.Generic;

public class PowerLeftItem : MonoBehaviour {

    public UILabel labelScore;
    public UILabel labelLevel;
    public UISprite SpriteLevel;

    public int ItemLevel { set; get; }

	// Use this for initialization
	void Start () {
	
	}
	

    public void SetData(int type, int score, int level, bool setSlider = false ,int index = -1)
    {
        if (Singleton<ObjManager>.Instance.MainPlayer == null)
            return;

        if( type > (int)BePowerData.BePowerType.BPTDEFINE_Null &&
            type < (int)BePowerData.BePowerType.BPTDEFINE_MAX)
        {
            BePowerData.BePowerType eType = (BePowerData.BePowerType)type;
			//========
			SetLevel(eType, score,level, setSlider);	
			//========
            switch (eType)
            {
                case BePowerData.BePowerType.BPTDEFINE_BELLE:
//                   int bellCombat = BelleData.GetActiveMatrixPowerNum();
//                    SetLevel(eType, bellCombat, setSlider);		
                    break;
                case BePowerData.BePowerType.BPTDEFINE_EQUIP:
//                    int equipCombat = Singleton<ObjManager>.Instance.MainPlayer.GetTotalEquipCombatValue();
//                    SetLevel(eType, equipCombat, setSlider);					
                    break;
                case BePowerData.BePowerType.BPTDEFINE_FELLOW:
//                    int fellowCombat = Singleton<ObjManager>.Instance.MainPlayer.GetTotalFellowCombatValue();
//                    SetLevel(eType, fellowCombat, setSlider);					
                    break;
                case BePowerData.BePowerType.BPTDEFINE_GEM:
//                    int gemCombat = Singleton<ObjManager>.Instance.MainPlayer.GetTotalGemCombatValue();
//                    SetLevel(eType, gemCombat, setSlider);
                    break;
                case BePowerData.BePowerType.BPTDEFINE_SKILL:
//                   int skillCombat = Singleton<ObjManager>.Instance.MainPlayer.GetTotalStudySkillCombatValue();
//                    SetLevel(eType, skillCombat, setSlider);
//                    break;
                case BePowerData.BePowerType.BPTDEFINE_XIAKE:
//                    int xkCombat = GameManager.gameManager.PlayerDataPool.SwordsManCombat;
//                    SetLevel(eType, xkCombat, setSlider);
                    break;
                default:
                    break;
            }
        }

		//===========
		switch(index)
		{
		case 0:
			labelScore.text = Utils.GetDicByID(4245);
			break;
		case 1:
			labelScore.text = Utils.GetDicByID(4246);
			break;
		case 2:
			labelScore.text = Utils.GetDicByID(4247);
			break;
		case 3:
			labelScore.text = Utils.GetDicByID(4248);
			break;
		case 4:
			labelScore.text = Utils.GetDicByID(4249);
			break;
		case 5:
			labelScore.text = Utils.GetDicByID(4250);
			break;

		}
		//===========

    }

    void SetLevel( BePowerData.BePowerType type, int combat,int level, bool setSlider )
    {
        if (labelLevel == null)
            return;
	
//        int level = 5;
//        Tab_BePowerLevel powerLevel = TableManager.GetBePowerLevelByID((int)type, 0);
//        if (powerLevel != null)
//        {
//            for (int i = 0; i < powerLevel.getValueCount(); i++)
//            {
//                if (powerLevel.GetValuebyIndex(i) >= combat)
//                {
//                    level = powerLevel.GetLevelbyIndex(i);
//                    break;
//                }
//            }
//        }
//        else
//        {
//            level = 1;
//        }
//
        ItemLevel = level;

        switch (level)
        {
            case 1:
                labelLevel.text = Utils.GetDicByID(2376);
                SpriteLevel.spriteName = "ui_pub_084";//"pingmin";
                break;
            case 2:
                labelLevel.text = Utils.GetDicByID(2377);
                SpriteLevel.spriteName = "ui_pub_085";
                break;
            case 3:
                labelLevel.text = Utils.GetDicByID(2378);
                SpriteLevel.spriteName = "ui_pub_086";
                break;
            case 4:
                labelLevel.text = Utils.GetDicByID(2379);
                SpriteLevel.spriteName = "ui_pub_087";
                break;
            case 5:
                labelLevel.text = Utils.GetDicByID(2380);
                SpriteLevel.spriteName = "ui_pub_088";
                break;
            default:
                labelLevel.text = Utils.GetDicByID(2376);
                SpriteLevel.spriteName = "ui_pub_084";
                break;
        }

//2376	死亡推送_UI_20140623	[FFFFFF]平民
//2377	死亡推送_UI_20140623	[33CC66]侠客
//2378	死亡推送_UI_20140623	[33CCFF]英雄
//2379	死亡推送_UI_20140623	[CC66FF]霸主
//2380	死亡推送_UI_20140623	[FF9933]宗师
    }
}
