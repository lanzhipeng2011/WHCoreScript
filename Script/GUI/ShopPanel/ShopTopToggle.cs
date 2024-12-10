using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopTopToggle : MonoBehaviour
{
    public ShopManager ShopManager;
    public TabController TabController;

    void Start()
    {
        //for (int i = 0; i < toggles.Count; i++)
        //{
        //    int index = i;
        //    toggles[i].onClick.AddListener(f =>
        //    {
        //        ShopManager.SetContentData(index);
        //    });
        //}
        TabController.delTabChanged += (TabButton tb) =>
        {
            ShopManager.SetContentData(int.Parse(tb.gameObject.name));
        };
    }
}
