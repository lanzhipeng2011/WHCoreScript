using UnityEngine;
using System.Collections;
using Games.GlobeDefine;

namespace Games.LogicObj
{
    public class Obj_Client : Obj_Character
    {
        public Obj_Client()
        {
            m_ObjType = GameDefine_Globe.OBJ_TYPE.OBJ_CLIENT;
        }

        void Awake()
        {
            m_ObjTransform = transform;
        }

        void FixedUpdate()
        {
            UpdateTargetMove();
            if (null != m_AnimLogic)
            {
                m_AnimLogic.AnimationUpdate();
            }
        }

        public override void OnAnimationFinish(int animationID)
        {
            base.OnAnimationFinish(animationID);
            if (gameObject.name == "play-effect")
            {
                if (animationID == GlobeVar.YanMenGuan_QiaoFengAni)
                {
                    Obj_MainPlayer mainplayer = Singleton<ObjManager>.Instance.MainPlayer;
                    if (mainplayer != null)
                    {
                        if (mainplayer.CameraController)
                        {
                            mainplayer.CameraController.InitCameraRock(4);

                            if (BackCamerControll.Instance() != null)
                            {
                                BackCamerControll.Instance().IsCloseIdle = false;
                                BackCamerControll.Instance().InitBlackScreenTween(1.0f, 1.0f, 1.0f);
                                BackCamerControll.Instance().PlayBlackScreenTween();

                                BackCamerControll.Instance().InitBlackScreenLabel("几日后......");
                                BackCamerControll.Instance().InitBlackScreenLabelTween(0.5f, 1.0f, 2.0f);
                                BackCamerControll.Instance().PlayBlackScreenLabelTween();
                            }


                            mainplayer.CameraController.ResetCameraToMainPlayer();
                            if (null != mainplayer.AnimLogic)
                                mainplayer.AnimLogic.Play((int)CharacterDefine.CharacterAnimId.Die);

                            if (mainplayer.Profession == (int)CharacterDefine.PROFESSION.SHAOLIN)
                            {
                                GameManager.gameManager.SoundManager.PlaySoundEffect(5);     //attack1_shaolin
                            }
                            else if (mainplayer.Profession == (int)CharacterDefine.PROFESSION.TIANSHAN)
                            {
                                GameManager.gameManager.SoundManager.PlaySoundEffect(6);    //attack1_tianshan
                            }
                            else if (mainplayer.Profession == (int)CharacterDefine.PROFESSION.DALI)
                            {
                                GameManager.gameManager.SoundManager.PlaySoundEffect(1);    //attack1_dali
                            }
                            else if (mainplayer.Profession == (int)CharacterDefine.PROFESSION.XIAOYAO)
                            {
                                GameManager.gameManager.SoundManager.PlaySoundEffect(7);    //attack1_xiaoyao
                            }

                            StoryDialogLogic.ShowStory(GlobeVar.YanMenGuan_BossStory2ID);
                            mainplayer.IsNoMove = true;
                        }                        
                    }
                }
            }
        }
    }

}