using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using Sirenix.OdinInspector;

namespace EazyEngine.Space
{

    public class CharacterInstancedConfigGroupSO : EzScriptTableObject
    {
        [HideLabel]
        [OnValueChanged("Dirty",true)]
        public CharacterInstancedConfigGroup info;

        public void Dirty()
        {
#if UNITY_EDITOR
            UnityEditor.SerializedObject pObject = new UnityEditor.SerializedObject(this);
            pObject.ApplyModifiedProperties();
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

    }
}