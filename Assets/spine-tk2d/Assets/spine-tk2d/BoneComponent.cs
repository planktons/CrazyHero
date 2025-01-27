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
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Spine;

/// <summary>Sets a GameObject's transform to match a bone on a Spine skeleton.</summary>
[ExecuteInEditMode]
[AddComponentMenu("Spine/BoneComponent")]
public class BoneComponent : MonoBehaviour {
	[System.NonSerialized]
	public bool valid;

	public SkeletonRenderer skeletonRenderer;
	public Bone bone;

	public bool followZPosition = true;
	public bool followBoneRotation = true;

	public SkeletonRenderer SkeletonRenderer {
		get { return skeletonRenderer; }
		set {
			skeletonRenderer = value;
			Reset ();
		}
	}

	// TODO: Make the rotation behavior more customizable
	// public bool followTransformRotation = false;

	// TODO: Make transform follow bone scale? too specific? This is really useful for shared shadow assets.
	//public bool followBoneScale = false;

	/// <summary>If a bone isn't set, boneName is used to find the bone.</summary>
	public String boneName;

	protected Transform cachedTransform;
	protected Transform skeletonTransform;
	
	public void Reset () {
		bone = null;
		cachedTransform = transform;
		valid = skeletonRenderer != null && skeletonRenderer.valid;
		if (!valid) return;
		skeletonTransform = skeletonRenderer.transform;
	}

	public void Awake () {
		Reset();
	}

	public void LateUpdate () {
		if (!valid) {
			Reset();
			return;
		}

		if (bone == null) {
			if (boneName == null || boneName.Length == 0) return;
			bone = skeletonRenderer.skeleton.FindBone(boneName);
			if (bone == null) {
				Debug.LogError("Bone not found: " + boneName, this);
				return;
			}
		}

		Spine.Skeleton skeleton = skeletonRenderer.skeleton;
		float flipRotation = (skeleton.flipX ^ skeleton.flipY) ? -1f : 1f;

		if (cachedTransform.parent == skeletonTransform) {
			cachedTransform.localPosition = new Vector3(bone.worldX, bone.worldY, followZPosition ? 0f : cachedTransform.localPosition.z);

			if(followBoneRotation) {
				Vector3 rotation = cachedTransform.localRotation.eulerAngles;
				cachedTransform.localRotation = Quaternion.Euler(rotation.x, rotation.y, bone.worldRotation * flipRotation);
			}

		} else {
			Vector3 targetWorldPosition = skeletonTransform.TransformPoint(new Vector3(bone.worldX, bone.worldY, 0f));
			if(!followZPosition) targetWorldPosition.z = cachedTransform.position.z;

			cachedTransform.position = targetWorldPosition;

			if(followBoneRotation) {
				Vector3 rotation = skeletonTransform.rotation.eulerAngles;
				cachedTransform.rotation = Quaternion.Euler(rotation.x, rotation.y, 
				                                            skeletonTransform.rotation.eulerAngles.z + (bone.worldRotation * flipRotation) );
			}
		}
	}
}