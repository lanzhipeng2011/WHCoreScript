using UnityEngine;
using System.Collections;

public class HeadInfoDropItemManager : MonoBehaviour {


	public UILabel l_ItemNameLabel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowNameLabel(string itemName)
	{
		this.gameObject.SetActive (true);
		l_ItemNameLabel.text = itemName;
	}

	public void OnAnimationFinished()
	{
		this.gameObject.SetActive (false);
		Destroy (this.gameObject);
	}

}
