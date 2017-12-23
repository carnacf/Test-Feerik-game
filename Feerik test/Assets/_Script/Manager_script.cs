using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System;
using UnityEngine;


public class Manager_script : MonoBehaviour {

	[Range(1,10)]
	public int nbThreads;
	public TextAsset txt;
	private string tex_folder;

	private List<string> tex_list;
	private Dictionary<string,byte[]> to_load;
	private string [] tex_url;

	// Use this for initialization
	void Start () {
		ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

		tex_folder = Application.persistentDataPath + "/Textures/";
		tex_list = new List<string> ();
		to_load = new Dictionary<string,byte[]> ();
		tex_url = txt.text.Split(/*new [] {Environment.NewLine},StringSplitOptions.None*/' ');
		Debug.Log (tex_url.Length);
		List<Transform> need_tex = new List<Transform>(GameObject.Find ("Need_tex").GetComponentsInChildren<Transform>());

		for (int i = 0; i < need_tex.Count; i++) {
			if (need_tex [i] != GameObject.Find ("Need_tex").transform) {
				if (!tex_list.Contains (need_tex [i].GetComponent<Object_script> ().tex_name)) {
					tex_list.Add (need_tex [i].GetComponent<Object_script> ().tex_name);
					Debug.Log ("Add : " + need_tex [i].GetComponent<Object_script> ().tex_name);
				}
			}
		}

		//Start download and save thread
		Thread[] threads = new Thread[nbThreads];
		for(int i = 0;i<nbThreads;i++) {
			int start = i * tex_list.Count / nbThreads;
			int end = (i+1) * tex_list.Count / nbThreads;
			threads[i] = new Thread(()=>loadThread(start,end));
			threads [i].Start ();
		}

		for(int i = 0;i<nbThreads;i++) {
			threads [i].Join ();
		}
		//End


		for (int i = 0; i < need_tex.Count; i++) {
			if (need_tex [i] != GameObject.Find ("Need_tex").transform) {
				if (to_load.ContainsKey (need_tex [i].GetComponent<Object_script> ().tex_name)) {
					Texture2D tex = new Texture2D (1,1);
					tex.LoadImage (to_load[need_tex[i].GetComponent<Object_script> ().tex_name]);
					need_tex[i].GetComponent<Renderer> ().material.mainTexture = tex;
				}
			}
		}
	}


	void loadThread(int start, int end){
		for (int i = start; i < end; i++) {
			byte[] raw_img;
			//If texture already load in Dictionary do nothing
			if (!to_load.ContainsKey (tex_list [i])) {
				if (!File.Exists (tex_folder + tex_list [i])) {
					string url = findName (tex_url, tex_list [i]);
					if (!url.Equals("")) {
						raw_img = new System.Net.WebClient ().DownloadData (url);
						to_load.Add (tex_list [i], raw_img);
						Debug.Log ("write");
						(new FileInfo (tex_folder + tex_list [i])).Directory.Create ();
						File.WriteAllBytes (tex_folder + tex_list [i], raw_img);
					} else {
						to_load.Add (tex_list [i], null);
					}
				}else {
					Debug.Log ("file exist");
					raw_img = File.ReadAllBytes(tex_folder + tex_list[i]);
					to_load.Add (tex_list [i], raw_img);
				}
			}
		}
	}

	void loadTexture(Transform go,string tex_folder){
		string path = tex_folder + go.gameObject.GetComponent<Object_script> ().tex_name;
		if (File.Exists (path)) {
			byte[] raw_tex = File.ReadAllBytes(path);
			Texture2D tex = new Texture2D (1,1);
			tex.LoadImage (raw_tex);
			go.GetComponent<Renderer> ().material.mainTexture = tex;
		}
	}

	string findName(string[] all_url,string file_name){
		for (int i = 0; i < all_url.Length; i++) {
			if (getFileNameInUrl (all_url [i]).Equals(file_name)) {
				Debug.Log ("find : " + all_url [i] +" " + file_name);
				return all_url [i];
			}
				
				
				
		}
		return "";
	}

	string getFileNameInUrl(string url){
		string [] splited = url.Split('/');
		return splited[splited.Length - 1];
	}
}
