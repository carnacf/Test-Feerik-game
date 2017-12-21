using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Manager_script : MonoBehaviour {
	
	private string tex_folder = "./Assets/_Textures/";
	private string tex_txt = "./Assets/textures.txt";

	// Use this for initialization
	void Start () {
		List<Transform> need_tex = new List<Transform>(GameObject.Find ("Need_tex").GetComponentsInChildren<Transform>());
		for (int i = 0; i < need_tex.Count; i++) {
			if (need_tex [i] == GameObject.Find ("Need_tex").transform) {
				need_tex.RemoveAt (i);
			}
		}
		string [] tex_url = System.IO.File.ReadAllLines (tex_txt);
		LoadAllTex (need_tex, tex_url,tex_folder);
	}



	void LoadAllTex(List<Transform> need_tex,string [] tex_url,string tex_folder){
		foreach(Transform t in need_tex){
			string tex_name = t.GetComponent<Object_script> ().tex_name;
			string url = findName (tex_url, tex_name);
			if (url != "") {
				loadTexture (t,url,tex_folder);
			}
		}
	}

	void loadTexture(Transform go, string url,string tex_folder){
		byte[] tex;
		if (!isLoaded (url, tex_folder)) {
			StartCoroutine(downloadImg (go,url, tex_folder));
		} else {
			tex = File.ReadAllBytes(tex_folder+ url.Split('/')[url.Split('/').Length - 1]);
			changeTex (go, tex);
		}
	}

	IEnumerator downloadImg(Transform go,string url,string folder){
		WWW img = new WWW (url);
		while(!img.isDone)
			yield return new WaitForEndOfFrame();
		Debug.Log (url);
		byte[] img_raw = img.bytes;
		Debug.Log ("2");
		File.WriteAllBytes (folder + getFileNameInUrl(url),img_raw);
		changeTex (go, img_raw);
	}

	void changeTex(Transform go, byte[] file){
		Texture2D tex = new Texture2D (1,1);
		tex.LoadImage (file);
		go.GetComponent<Renderer> ().material.mainTexture = tex;
	}

	bool isLoaded(string url,string tex_folder){
		string [] splited = url.Split('/');
		string file_name = splited[splited.Length - 1];
		return File.Exists (tex_folder + file_name);
	}

	string findName(string[] all_url,string file_name){
		for (int i = 0; i < all_url.Length; i++) {
			if (getFileNameInUrl (all_url [i]) == file_name)
				return all_url [i];
				
		}
		return "";
	}

	string getFileNameInUrl(string url){
		string [] splited = url.Split('/');
		return splited[splited.Length - 1];
	}
}
