using UnityEngine;
using System.Collections;

public class AdditionData : MonoBehaviour {

    public static int doubleExpDisableTime = 0;

    public static void UpdateAdditionData(GC_ADDITIONINFO_UPDATE data)
    {
        if (data.HasDoubleExpDurtaion)
        {
            doubleExpDisableTime = (int)Time.realtimeSinceStartup + data.DoubleExpDurtaion;
        }
    }
}
