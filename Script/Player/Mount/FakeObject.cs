//********************************************************************
// 文件名: FakeObject.cs
// 描述: 坐骑 模型
// 作者: HeWenpeng
// 创建时间: 2014-1-2
// 功能说明：FakeObject动作模型， 用于UI显示， FakeObject.txt表配置。
// 修改历史:
//********************************************************************

using UnityEngine;
using System.Collections;
using GCGame.Table;
using Games.ObjAnimModule;
using Module.Log;
using Games.Item;
using Games.GlobeDefine;
using System.Collections.Generic;

namespace Games.FakeObject
{
    public class FakeObject
    {
        private ObjAnimModel m_ObjAnim;
        public ObjAnimModel ObjAnim
        {
            get { return m_ObjAnim; }
            set { m_ObjAnim = value; }
        }

        private GameObject m_FakeObjNode;
        public GameObject FakeObjNode
        {
            get { return m_FakeObjNode; }
            set { m_FakeObjNode = value; }
        }

        // 创建 坐骑Obj 唯一标识strName+NMountID,不可重复
        public bool initFakeObject(int nFakeObjID, GameObject fakeObjNode)
        {
            if (m_ObjAnim)
            {
                Destroy();
            }

            FakeObjNode = fakeObjNode;
            if (FakeObjNode == null)
            {
                LogModule.DebugLog("error: initFakeObject FakeObjNode is null");
                return false ;
            }

            Tab_FakeObject FakeObjTable = TableManager.GetFakeObjectByID(nFakeObjID, 0);
            if (FakeObjTable == null)
            {
                return false;
            }

            GameObject Obj = ResourceManager.InstantiateResource("Prefab/Model/NPCRoot", nFakeObjID.ToString()) as GameObject;
            if (Obj != null)
            {
                if (FakeObjTable.IsPlayer == 1)
                {
                    if (Singleton<ObjManager>.Instance.MainPlayer.ModelVisualID == GlobeVar.INVALID_ID)
                    {
                        Singleton<ObjManager>.GetInstance().ReloadModel(Obj, FakeObjTable.FakeObjModel, Singleton<ObjManager>.GetInstance().AsycLoadFakeObjOver, FakeObjTable, this);
                    }
                    else
                    {
                        InitFashionFakeObj(Obj, Singleton<ObjManager>.Instance.MainPlayer.ModelVisualID, FakeObjTable);
                    }
                }
                else
                {
                    Singleton<ObjManager>.GetInstance().ReloadModel(Obj, FakeObjTable.FakeObjModel, Singleton<ObjManager>.GetInstance().AsycLoadFakeObjOver, FakeObjTable, this);
                }

                m_ObjAnim = Obj.AddComponent<ObjAnimModel>() as ObjAnimModel;

                return true;
            }

            return false;
        }

        public bool initFakeObject(int nFakeObjID, GameObject fakeObjNode, out GameObject FakeObj)
        {
            FakeObj = null;

            if (m_ObjAnim)
            {
                Destroy();
            }

            FakeObjNode = fakeObjNode;
            if (FakeObjNode == null)
            {
                LogModule.DebugLog("error: initFakeObject FakeObjNode is null");
                return false;
            }

            Tab_FakeObject FakeObjTable = TableManager.GetFakeObjectByID(nFakeObjID, 0);
            if (FakeObjTable == null)
            {
                return false;
            }

            FakeObj = ResourceManager.InstantiateResource("Prefab/Model/NPCRoot", nFakeObjID.ToString()) as GameObject;
            if (FakeObj != null)
            {
                if (FakeObjTable.IsPlayer == 1)
                {
                    if (Singleton<ObjManager>.Instance.MainPlayer.ModelVisualID == GlobeVar.INVALID_ID)
                    {
                        Singleton<ObjManager>.GetInstance().ReloadModel(FakeObj, FakeObjTable.FakeObjModel, Singleton<ObjManager>.GetInstance().AsycLoadFakeObjOver, FakeObjTable, this);
                    }
                    else
                    {
                        InitFashionFakeObj(FakeObj, Singleton<ObjManager>.Instance.MainPlayer.ModelVisualID, FakeObjTable);
                    }
                }
                else
                {
                    Singleton<ObjManager>.GetInstance().ReloadModel(FakeObj, FakeObjTable.FakeObjModel, Singleton<ObjManager>.GetInstance().AsycLoadFakeObjOver, FakeObjTable, this);
                }

                m_ObjAnim = FakeObj.AddComponent<ObjAnimModel>() as ObjAnimModel;

                return true;
            }

            return false;
        }
        
        public void Destroy()
        {
            if (m_ObjAnim != null)
            {
                GameObject.Destroy(ObjAnim.gameObject);
                m_ObjAnim = null;
            }
        }

        public void PlayAnim(int nAnimID)
        {
            if (null != m_ObjAnim && null != m_ObjAnim.AnimLogic)
                m_ObjAnim.AnimLogic.Play(nAnimID);
        }

        void InitFashionFakeObj(GameObject Obj, int nModelVisualID, Tab_FakeObject FakeObjTable)
        {
            if (Singleton<ObjManager>.Instance.MainPlayer == null)
            {
                return;
            }

            // 重载模型
            Tab_ItemVisual tabItemVisual = TableManager.GetItemVisualByID(nModelVisualID, 0);
            if (tabItemVisual == null)
            {
                return;
            }

            int nCharModelID = Singleton<ObjManager>.Instance.MainPlayer.GetCharModelID(tabItemVisual);

            Tab_CharModel tabCharModel = TableManager.GetCharModelByID(nCharModelID, 0);
            if (tabCharModel == null)
            {
                return;
            }

            Singleton<ObjManager>.GetInstance().ReloadModel(Obj, tabCharModel.ResPath, Singleton<ObjManager>.GetInstance().AsycLoadFakeObjOver, FakeObjTable, this);
        }

        public void InitFakeObjWeapon(GameObject Obj, int nCurWeaponID, Tab_FakeObject FakeObjTable)
        {
            // 重载武器
            bool defaultVisual = false;
            Tab_ItemVisual WeaponVisual = null;
            Tab_EquipAttr tabEquipAttr = TableManager.GetEquipAttrByID(nCurWeaponID, 0);
            if (tabEquipAttr != null)
            {
                Tab_ItemVisual tabWeaponVisual = TableManager.GetItemVisualByID(tabEquipAttr.ModelId, 0);
                if (tabWeaponVisual != null)
                {
                    WeaponVisual = tabWeaponVisual;
                }
                else
                {
                    defaultVisual = true;
                }
            }
            else
            {
                defaultVisual = true;
            }

            if (defaultVisual)
            {
				Tab_ItemVisual tabDefaultVisual = TableManager.GetItemVisualByID(nCurWeaponID, 0);//GlobeVar.DEFAULT_VISUAL_ID
                if (tabDefaultVisual == null)
                {
                    return;
                }

                WeaponVisual = tabDefaultVisual;
            }

            if (WeaponVisual == null)
            {
                return;
            }

            int nWeaponModelID = Singleton<ObjManager>.Instance.MainPlayer.GetWeaponModelID(WeaponVisual);

            Tab_WeaponModel tabWeaponModel = TableManager.GetWeaponModelByID(nWeaponModelID, 0);
            if (tabWeaponModel == null)
            {
                return;
            }

            if (Singleton<ObjManager>.Instance.MainPlayer == null)
            {
                return;
            }

            if (Singleton<ObjManager>.Instance.MainPlayer.Profession == (int)CharacterDefine.PROFESSION.TIANSHAN)
            {
               // LoadWeapon(Obj, "Weapon_L", tabWeaponModel.ResPath + "_L");
				LoadWeapon(Obj, "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand/HH_weaponHandLf",
				           tabWeaponModel.ResPath + "_L");
                //LoadWeapon(Obj, "Weapon_R", tabWeaponModel.ResPath + "_R");
				LoadWeapon(Obj, "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt",
				           tabWeaponModel.ResPath + "_R");
            }
           // else if (Singleton<ObjManager>.Instance.MainPlayer.Profession == (int)CharacterDefine.PROFESSION.XIAOYAO)
            //{
            //    LoadWeapon(Obj, "Weapon_L", tabWeaponModel.ResPath);
           // }
            else
            {

				if(Singleton<ObjManager>.Instance.MainPlayer.Profession == (int)CharacterDefine.PROFESSION.SHAOLIN||Singleton<ObjManager>.Instance.MainPlayer.Profession == (int)CharacterDefine.PROFESSION.XIAOYAO)
				{
                    //剑客的剑应该背在背上

					LoadWeapon(Obj, "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/HH_weaponback",
					           tabWeaponModel.ResPath);

				}
				else
				{
                //LoadWeapon(Obj, "Weapon_R", tabWeaponModel.ResPath);
				LoadWeapon(Obj, "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/HH_weaponHandRt",
				           tabWeaponModel.ResPath);
				}
            }
        }

        private void LoadWeapon(GameObject Obj, string szBindPoint, string szResName)
        {
            List<object> param = new List<object>();
            param.Add(szBindPoint);
            param.Add(Singleton<ObjManager>.Instance.MainPlayer.WeaponEffectGem);
            param.Add(null);
            param.Add(Singleton<ObjManager>.Instance.MainPlayer.Profession);

            Singleton<ObjManager>.GetInstance().ReloadWeapon(Obj, szResName, Singleton<ObjManager>.GetInstance().AsycReloadWeaponOver, param);
        }
    }
}

