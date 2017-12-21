using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.IO;

public class Manager_script : MonoBehaviour {

	[Range(1,10)]
	public int nbThreads;
	private string tex_folder = "./Assets/_Textures/";
	private string tex_txt = "./Assets/textures.txt";
	private HashSet<string> to_download;

	// Use this for initialization
	IEnumerator Start () {
		to_download = new HashSet<string> ();
		string [] tex_url = System.IO.File.ReadAllLines (tex_txt);
		List<Transform> need_tex = new List<Transform>(GameObject.Find ("Need_tex").GetComponentsInChildren<Transform>());
		for (int i = 0; i < need_tex.Count; i++) {
			if (need_tex [i] == GameObject.Find ("Need_tex").transform) {
				//Exctract parent object
				need_tex.RemoveAt (i);
			}

			if (!File.Exists(tex_folder+need_tex [i].GetComponent<Object_script> ().tex_name)) {
				string url = findName (tex_url, need_tex [i].GetComponent<Object_script> ().tex_name);
				if (url != "") {
					//If tex not dowload and url found add it to List
					Debug.Log("Add into to_dowload : " + url);
					to_download.Add (url);
				}
			}
		}

		foreach (string s in to_download) {
			using (WWW img = new WWW (s)) {
				yield return img;
				Debug.Log("Dowload : " + s);
				byte[] raw_img = img.bytes;
				File.WriteAllBytes (tex_folder + getFileNameInUrl(s),raw_img);
			}
		}

		//Start threading
		for(int i = 0;i<nbThreads;i++){
			int start = i * need_tex.Count / nbThreads;
			int end = (i+1) * need_tex.Count / nbThreads;
			StartCoroutine(LoadAllTex(start,end,need_tex,tex_folder));
		}
	}

	IEnumerator LoadAllTex(int start,int end, List<Transform> need_tex,string tex_folder){
		for(int i = start;i<end;i++){
			loadTexture (need_tex[i],tex_folder);
			yield return null;
		}
	}

	void loadTexture(Transform go,string tex_folder){
		string path = tex_folder + go.gameObject.GetComponent<Object_script> ().tex_name;
		if (File.Exists (path)) {
			Debug.Log("load : " + path);
			byte[] raw_tex = File.ReadAllBytes(path);
			Texture2D tex = new Texture2D (1,1);
			tex.LoadImage (raw_tex);
			go.GetComponent<Renderer> ().material.mainTexture = tex;
		}
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
