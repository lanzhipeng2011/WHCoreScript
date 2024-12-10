/********************************************************************************
 *	文件名：Obj_OtherGameObj.cs
 *	全路径：	\Script\OBJ\Obj_OtherGameObj.cs
 *	创建人：	贺文鹏
 *	创建时间：2014-02-17
 *
 *	功能说明： 非Obj的一些gameobject的身上数据。
 *	       
 *	修改记录：
*********************************************************************************/
using UnityEngine;
using System.Collections;

namespace Games.LogicObj
{
    public class Obj_OtherGameObj : MonoBehaviour {

        public const int MaxParamCount = 4;
        private int[] IntParam = new int[MaxParamCount];
        private string[] StrParam = new string[MaxParamCount];

        public Obj_OtherGameObj()
        {
            CleanUp();
        }

        void CleanUp()
        {
            for (int i = 0; i< MaxParamCount;i++)
            {
                IntParam[i] = 0;
                StrParam[i] = "";
            }
        }
		public void Update()
		{
			//by dsy 旋转去掉
			//this.transform.Rotate (Vector3.up * 10 * Time.deltaTime, Space.Self);
		}
        public void SetIntParam(int nIndex, int nValue)
        {
            if (nIndex >= 0 && nIndex < MaxParamCount)
            {
                IntParam[nIndex] = nValue;
            }
        }

        public int GetIntParamByIndex(int nIndex)
        {
            if (nIndex >= 0 && nIndex < MaxParamCount)
            {
                return IntParam[nIndex];
            }

            return 0;
        }

        public void SetStrParam(int nIndex, string strValue)
        {
            if (nIndex >= 0 && nIndex < MaxParamCount)
            {
                StrParam[nIndex] = strValue;
            }
        }

        public string GetStrParamByIndex(int nIndex)
        {
            if (nIndex >= 0 && nIndex < MaxParamCount)
            {
                return StrParam[nIndex];
            }

            return "";
        }
    }
}
