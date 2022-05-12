/// Used to draw custom inspectors for unrecognised file types, which Unity imports as "DefaultAsset"
/// To do this, create a new editor class extending DefaultAssetInspector
/// Return true in the IsValid function if the file extension of the file matches the type you'd like to draw.
/// The DefaultAssetEditor class will then hold a reference to the new instance of your editor class and call the appropriate methods for drawing.
/// An example can be found at the bottom of the file.

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Used to draw custom inspectors for unrecognised file types, which Unity imports as "DefaultAsset"
/// </summary>
[CustomEditor(typeof(DefaultAsset), true)]
public class DefaultAssetEditor : Editor {

	private DefaultAssetInspector inspector;

	private void OnEnable () {
		inspector = FindObjectInspector ();
		if(inspector != null) {
			inspector.editor = this;
			inspector.serializedObject = serializedObject;
			inspector.target = target;
			inspector.OnEnable();
		}
	}

	private void OnDisable () {
		if(inspector != null)
			inspector.OnDisable();
	}

	protected override void OnHeaderGUI () {
		if(inspector != null) {
			inspector.OnHeaderGUI();
		}
		else if (target.GetType() != typeof(UnityEditor.DefaultAsset))
			base.OnHeaderGUI();
	}

	public void DrawDefaultHeaderGUI () {
		base.OnHeaderGUI();
	}

	public override void OnInspectorGUI () {
		if(inspector != null) {
			GUI.enabled = true;
			inspector.OnInspectorGUI();
		}
		else if (target.GetType() != typeof(UnityEditor.DefaultAsset))
			base.OnInspectorGUI();
	}

	private DefaultAssetInspector FindObjectInspector () {
		List<string> assembliesToCheck = new List<string>{"Assembly-CSharp-Editor", "Assembly-CSharp-Editor-firstpass", "Assembly-UnityScript-Editor", "Assembly-UnityScript-Editor-firstpass"};
		string assetPath = AssetDatabase.GetAssetPath(target);
		Assembly[] referencedAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
		for(int i = 0; i < referencedAssemblies.Length; ++i) {
			if(!assembliesToCheck.Contains(referencedAssemblies[i].GetName().Name))
				continue;
			foreach(var type in referencedAssemblies[i].GetTypes()) {
				if(!type.IsSubclassOf(typeof(DefaultAssetInspector))) 
					continue;
				DefaultAssetInspector objectInspector = (DefaultAssetInspector)Activator.CreateInstance(type);
				if(objectInspector.IsValid(assetPath)) {
					objectInspector.target = target;
					return objectInspector;
				}
			}
		}
		return null;
	}
}

/// <summary>
/// Default asset inspector. Used by DefaultAssetEditor
/// </summary>
public abstract class DefaultAssetInspector {
	// Reference to the actual editor we draw to
	public DefaultAssetEditor editor;
	// Shortcut to the target object
	public UnityEngine.Object target;
	// Shortcut to the serializedObject
	public SerializedObject serializedObject;

	public abstract bool IsValid(string assetPath);
	public virtual void OnEnable () {}
	public virtual void OnDisable () {}
	// An example of how Unity draws headers can be found at https://github.com/MattRix/UnityDecompiled/blob/master/UnityEditor/UnityEditor/Editor.cs
	public virtual void OnHeaderGUI () {
		editor.DrawDefaultHeaderGUI();
	}
	public virtual void OnInspectorGUI() {}
}

// EXAMPLE FOR A .SAVE FILE
public class SaveFileInspector : DefaultAssetInspector {
	public override bool IsValid(string assetPath) {
		return Path.GetExtension(assetPath) == ".toml";
	}

	public override void OnInspectorGUI () {
		//	 Call to redraw every frame
		editor.Repaint();
		serializedObject.Update();
//	 	Use System.IO.File to read the file contents and draw it here if you wish!
		var assetPath = AssetDatabase.GetAssetPath(editor.target);
		var absolutePath = System.IO.Path.Combine(Application.dataPath.Substring(0, Application.dataPath.Length-7), assetPath);
		var text = System.IO.File.ReadAllText(absolutePath);
		EditorGUILayout.TextArea(text);
		serializedObject.ApplyModifiedProperties();
	}
}