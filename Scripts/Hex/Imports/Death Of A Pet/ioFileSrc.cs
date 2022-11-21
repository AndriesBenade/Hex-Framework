using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ioFileSrc {
	
	public class ioFile {
		
		public string filename = "";
		
		public ioFile (string ffilename) {
			filename = ffilename;
		}
		
		public float filesize () {
			FileInfo f = new FileInfo (filename);
			long size = 0;
			if (f.Exists) {
				size = f.Length;
			}
			return (float)size;
		}
		
		public string writedate () {
			FileInfo f = new FileInfo (filename);
			string s = "";
			if (f.Exists) {
				s = f.LastWriteTime.ToLongDateString();
			}
			return s;
		}
		
		public string writetime () {
			FileInfo f = new FileInfo (filename);
			string s = "";
			if (f.Exists) {
				s = f.LastWriteTime.ToLongTimeString();
			}
			return s;
		}
		
		public string accessdate () {
			FileInfo f = new FileInfo (filename);
			string s = "";
			if (f.Exists) {
				s = f.LastAccessTime.ToLongDateString();
			}
			return s;
		}
		
		public string accesstime () {
			FileInfo f = new FileInfo (filename);
			string s = "";
			if (f.Exists) {
				s = f.LastAccessTime.ToLongTimeString();
			}
			return s;
		}
		
		public string creationdate () {
			FileInfo f = new FileInfo (filename);
			string s = "";
			if (f.Exists) {
				s = f.CreationTime.ToLongDateString();
			}
			return s;
		}
		
		public string creationtime () {
			FileInfo f = new FileInfo (filename);
			string s = "";
			if (f.Exists) {
				s = f.CreationTime.ToLongTimeString();
			}
			return s;
		}
		
		public bool isreadonly () {
			FileInfo f = new FileInfo (filename);
			bool b = false;
			if (f.Exists) {
				b = f.IsReadOnly;
			}
			return b;
		}
		
		public string fullname () {
			FileInfo f = new FileInfo (filename);
			string s = "";
			if (f.Exists) {
				s = f.FullName;
			}
			return s;
		}
		
		public string name () {
			FileInfo f = new FileInfo (filename);
			string s = "";
			if (f.Exists) {
				s = f.Name;
			}
			return s;
		}
		
		public string extension () {
			FileInfo f = new FileInfo (filename);
			string s = "";
			if (f.Exists) {
				s = f.Extension;
			}
			return s;
		}
		
		public string directory () {
			FileInfo f = new FileInfo (filename);
			string s = "";
			if (f.Exists) {
				s = f.DirectoryName;
			}
			return s;
		}
		
		public FileAttributes attributes () {
			FileInfo f = new FileInfo (filename);
			FileAttributes a = FileAttributes.Normal;
			if (f.Exists) {
				a = f.Attributes;
			}
			return a;
		}
		
		public void addclassitem (string classname, string key, string value) {
			additem (classname + "." + key, value);
		}
		
		public bool hasclass (string classname) {
			bool b = false;
			foreach (string s in loadlines()) {
				string c = extractkey (s);
				if (c.Contains (".")) {
					c = c.Substring (0, c.IndexOf ("."));
					if (c.ToLower() == classname.ToLower()) {
						b = true;
						break;
					}
				}
			}
			return b;
		}
		
		public void removeclass(string classname) {
			if (hasclass (classname)) {
				List<string> ls = new List<string>();
				foreach (string s in loadlines()) {
					string c = extractkey (s);
					if (c.Contains (".")) {
						c = c.Substring (0, c.IndexOf ("."));
						if (c.ToLower() != classname.ToLower()) {
							ls.Add (s);
						}
					}
				}
				writelines (ls.ToArray ());
			}
		}
		
		public bool hasitem (string key) {
			bool b = false;
			foreach (string s in loadlines()) {
				if (extractkey (s).ToLower() == key.ToLower()) {
					b = true;
					break;
				}
			}
			return b;
		}
		
		public string getitem (string key) {
			string item = "";
			foreach (string s in loadlines()) {
				if (extractkey (s).ToLower() == key.ToLower()) {
					item = s;
					break;
				}
			}
			return item;
		}
		
		public void additem (string key, string value) {
			appendline (key + "=" + value );
		}
		
		public void removeitem(string key) {
			if (hasitem (key)) {
				List<string> ls = new List<string>();
				foreach (string s in loadlines()) {
					if (extractkey (s).ToLower() != key.ToLower()) {
						ls.Add (s);
					}
				}
				writelines (ls.ToArray ());
			}
		}
		
		public void removeclassitem(string classname, string key) {
			if (hasitem (classname + "." + key)) {
				removeitem (classname + "." + key);
			}
		}
		
		public void updateitem(string key, string value) {
			if (hasitem (key)) {
				List<string> ls = new List<string>();
				foreach (string s in loadlines()) {
					if (extractkey (s).ToLower() != key.ToLower()) {
						ls.Add (s);
					} else {
						ls.Add (key + "=" + value);
					}
				}
				writelines (ls.ToArray ());
			}
		}
		
		public void updateclassitem(string classname, string key, string value) {
			if (hasitem (classname + "." + key)) {
				List<string> ls = new List<string>();
				foreach (string s in loadlines()) {
					if (extractkey (s).ToLower() != (classname + "." + key).ToLower()) {
						ls.Add (s);
					} else {
						ls.Add (classname + "." + key + "=" + value);
					}
				}
				writelines (ls.ToArray ());
			}
		}
		
		public int getclasscount () {
			return getclasses ().Length;
		}
		
		public string[] getclasses () {
			string[] lines = loadlines ();
			List <string> classes = new List <string> ();
			foreach (string line in lines) {
				string c = extractkey (line);
				if (c.Contains (".")) {
					c = c.Substring (0, c.IndexOf (".")).ToLower ();
					if (!classes.Contains (c)) {
						classes.Add (c);
					}
				}
			}
			return classes.ToArray ();
		}
		
		public string getitemvalue (string key) {
			string value = "";
			if (hasitem (key)) {
				string item = getitem (key);
				value = extractvalue (item);
			}
			return value;
		}
		
		public string getclassitemvalue (string classname, string key) {
			string value = "";
			if (hasitem (classname + "." + key)) {
				string item = getitem (classname + "." + key);
				value = extractvalue (item);
			}
			return value;
		}
		
		public float getitemvalueasfloat (string key) {
			string value = "";
			if (hasitem (key)) {
				string item = getitem (key);
				value = extractvalue (item);
			}
			float val = 0;
			bool b = float.TryParse (value, out val);
			return val;
		}
		
		public int getitemvalueasint (string key) {
			string value = "";
			if (hasitem (key)) {
				string item = getitem (key);
				value = extractvalue (item);
			}
			int val = 0;
			bool b = int.TryParse (value, out val);
			return val;
		}
		
		public float getclassitemvalueasfloat (string classname, string key) {
			string value = "";
			if (hasitem (classname + "." + key)) {
				string item = getitem (classname + "." + key);
				value = extractvalue (item);
			}
			float val = 0;
			bool b = float.TryParse (value, out val);
			return val;
		}
		
		public int getclassitemvalueasint (string classname, string key) {
			string value = "";
			if (hasitem (classname + "." + key)) {
				string item = getitem (classname + "." + key);
				value = extractvalue (item);
			}
			int val = 0;
			bool b = int.TryParse (value, out val);
			return val;
		}
		
		public bool getitemvalueasbool (string key) {
			string value = "";
			if (hasitem (key)) {
				string item = getitem (key);
				value = extractvalue (item);
			}
			bool b = false;
			switch (value.ToLower ()) {
				case "true" :
					b = true;
					break;
				case "t" :
					b = true;
					break;
				case "yes" :
					b = true;
					break;
				case "y" :
					b = true;
					break;
				case "1" :
					b = true;
					break;
				case "+" :
					b = true;
					break;
				case "positive" :
					b = true;
					break;
				case "p" :
					b = true;
					break;
				default :
					b = false;
					break;
			}
			return b;
		}
		
		public bool getclassitemvalueasbool (string classname, string key) {
			string value = "";
			if (hasitem (classname + "." + key)) {
				string item = getitem (classname + "." + key);
				value = extractvalue (item);
			}
			bool b = false;
			switch (value.ToLower ()) {
				case "true" :
					b = true;
					break;
				case "t" :
					b = true;
					break;
				case "yes" :
					b = true;
					break;
				case "y" :
					b = true;
					break;
				case "1" :
					b = true;
					break;
				case "+" :
					b = true;
					break;
				case "positive" :
					b = true;
					break;
				case "p" :
					b = true;
					break;
				default :
					b = false;
					break;
			}
			return b;
		}
		
		public string extractvalue (string item) {
			string value = "";
			value = item.Remove (0, item.IndexOf ("=") + 1);
			return value;
		}
		
		public string extractkey (string item) {
			string key = "";
			key = item.Substring (0, item.IndexOf ("="));
			key = key.Replace (" ", "");
			return key;
		}
		
		public string loadtext () {
			string[] lines = loadlines ();
			string s = "";
			int i = 1;
			foreach (string line in lines) {
				s += line;
				if (i <= lines.Length) {
					s += "\n";
				}
				i ++;
			}
			return s;
		}
		
		public string[] loadlines () {
			//return File.ReadAllLines (filename);
			List<string> ls = new List<string> ();
			if (exists ()) {
				FileStream fstream = new FileStream (filename, FileMode.Open);
				StreamReader freader = new StreamReader (fstream);
				while (!freader.EndOfStream) {
					string sline = freader.ReadLine ();
					if (sline.Length != 0) {
						if (sline[0].ToString () != "#") {
							ls.Add (sline);
						}
					}
				}
				freader.Close();
				fstream.Close();
			}
			return ls.ToArray ();
		}
		
		public void appendlines (string[] flines) {
			if (exists ()) {
				FileStream fstream = new FileStream (filename, FileMode.Append);
				StreamWriter fwriter = new StreamWriter (fstream);
				foreach (string s in flines) {
					fwriter.WriteLine (s);
				}
				fwriter.Close();
				fstream.Close();
			}
		}
		
		public void appendline (string fline) {
			if (exists ()) {
				FileStream fstream = new FileStream (filename, FileMode.Append);
				StreamWriter fwriter = new StreamWriter (fstream);
				fwriter.WriteLine (fline);
				fwriter.Close();
				fstream.Close();
			}
		}
		
		public void writelines (string[] flines) {
			if (exists ()) {
				string[] slines = loadlines();
				FileStream fstream = new FileStream (filename, FileMode.Create);
				StreamWriter fwriter = new StreamWriter (fstream);
				foreach (string s in flines) {
					fwriter.WriteLine (s);
				}
				fwriter.Close();
				fstream.Close();
			}
		}
		
		public void write (string ftext) {
			if (exists ()) {
				FileStream fstream = new FileStream (filename, FileMode.Create);
				StreamWriter fwriter = new StreamWriter (fstream);
				fwriter.Write (ftext);
				fwriter.Close();
				fstream.Close();
			}
		}
		
		public void create () {
			if (exists ()) {
				FileStream fstream = new FileStream (filename, FileMode.Create);
				fstream.Close();
			} else {
				FileStream fstream = new FileStream (filename, FileMode.CreateNew);
				fstream.Close();
			}
		}
		
		public void delete () {
			if (exists ()) {
				FileInfo f = new FileInfo (filename);
				f.Delete ();
			}
		}
		
		public bool exists () {
			return File.Exists (filename);
		}
		
	}
	
}