/******************************************************************************
 * Spine Runtimes Software License
 * Version 2.1
 * 
 * Copyright (c) 2013, Esoteric Software
 * All rights reserved.
 * 
 * You are granted a perpetual, non-exclusive, non-sublicensable and
 * non-transferable license to install, execute and perform the Spine Runtimes
 * Software (the "Software") solely for internal use. Without the written
 * permission of Esoteric Software (typically granted by licensing Spine), you
 * may not (a) modify, translate, adapt or otherwise create derivative works,
 * improvements of the Software or develop new applications using the Software
 * or (b) remove, delete, alter or obscure any trademarks or any copyright,
 * trademark, patent or other intellectual property or proprietary rights
 * notices on or in the Software, including any copy thereof. Redistributions
 * in binary or source form must include this license and terms.
 * 
 * THIS SOFTWARE IS PROVIDED BY ESOTERIC SOFTWARE "AS IS" AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
 * EVENT SHALL ESOTERIC SOFTARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
 * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
 * OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkeletonRenderer))]
public class SkeletonRendererInspector : Editor {
	protected SerializedProperty skeletonDataAsset, initialSkinName, normals, tangents, meshes, immutableTriangles;

	protected virtual void OnEnable () {
		skeletonDataAsset = serializedObject.FindProperty("skeletonDataAsset");
		initialSkinName = serializedObject.FindProperty("initialSkinName");
		normals = serializedObject.FindProperty("calculateNormals");
		tangents = serializedObject.FindProperty("calculateTangents");
		meshes = serializedObject.FindProperty("renderMeshes");
		immutableTriangles = serializedObject.FindProperty("immutableTriangles");
	}

	protected virtual void gui () {
		SkeletonRenderer component = (SkeletonRenderer)target;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PropertyField(skeletonDataAsset);
		float reloadWidth = GUI.skin.label.CalcSize(new GUIContent("Reload")).x + 20;
		if (GUILayout.Button("Reload", GUILayout.Width(reloadWidth))) {
			if (component.skeletonDataAsset != null) {
				component.skeletonDataAsset.Reset();
			}
			component.Reset();
		}
		EditorGUILayout.EndHorizontal();

		if (!component.valid) {
			component.Reset();
			component.LateUpdate();
			if (!component.valid) return;
		}


		// Initial skin name.
		{
			String[] skins = new String[component.skeleton.Data.Skins.Count];
			int skinIndex = 0;
			for (int i = 0; i < skins.Length; i++) {
				String name = component.skeleton.Data.Skins[i].Name;
				skins[i] = name;
				if (name == initialSkinName.stringValue)
					skinIndex = i;
			}
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Initial Skin", GUILayout.Width(EditorGUIUtility.labelWidth));
			skinIndex = EditorGUILayout.Popup(skinIndex, skins);
			EditorGUILayout.EndHorizontal();
			
			initialSkinName.stringValue = skins[skinIndex];
		}
		
		EditorGUILayout.PropertyField(meshes,
			new GUIContent("Render Meshes", "Disable to optimize rendering for skeletons that don't use meshes"));
		EditorGUILayout.PropertyField(immutableTriangles,
			new GUIContent("Immutable Triangles", "Enable to optimize rendering for skeletons that never change attachment visbility"));
		EditorGUILayout.PropertyField(normals);
		EditorGUILayout.PropertyField(tangents);
	}

	override public void OnInspectorGUI () {
		serializedObject.Update();
		gui();
		if (serializedObject.ApplyModifiedProperties() ||
			(Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed")
		) {
			if (!Application.isPlaying) ((SkeletonRenderer)target).Reset();
		}
	}
}
