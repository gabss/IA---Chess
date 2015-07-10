using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Manage e mantain persistent of the choices made on the first gui (difficulty and team)
/// </summary>
public class Director : MonoBehaviour {

	public Toggle[] iaDifficult;
	public int difficulty = 0;
	public string team = "White";

	void Awake(){
		DontDestroyOnLoad(gameObject);
	}


	public void toggleChanged(Toggle tog){
		if(tog.isOn){
			foreach(Toggle cTog in iaDifficult){
				if(cTog != tog) cTog.isOn = false;
			}
		}
	}

	public void goToGame(GameObject btn){
		for(int i=0; i<iaDifficult.Length; i++)
			if(iaDifficult[i].isOn) difficulty = i+1;
		if(difficulty != 0){
			team = btn.name;
			Application.LoadLevel("chess");
		}
	}
}
