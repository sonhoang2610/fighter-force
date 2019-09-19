#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Reflection;
using System.Reflection.Emit;

public class ExpanedListEventDrawer : FoldoutGroupAttributeDrawerGeneric<ExpanedListEvent>,IDisposable
{
    private delegate void InstanceDelegateWithLabel(ref object owner);
    public override void changedGroup(bool pBool)
    {
        var attribute = this.Attribute;
        var entry = this.Property.Parent.ValueEntry;

        var context = entry.Context.Get(this, "context", (Context)null);
        if (!pBool)
        {
  
            if (context != null && context.Value.CollapseEvent != null)
            {
                var val = context.Value.MemberProperty.ValueEntry.WeakSmartValue;
                context.Value.CollapseEvent(ref val);
            }
        }
        else
        {
            if (context != null && context.Value.ExpanedEvent != null)
            {
                     var val = context.Value.MemberProperty.ValueEntry.WeakSmartValue;
                    context.Value.ExpanedEvent(ref val);
            }
        }
    }
    private class Context
    {
        public string ErrorMessage;
        public InstanceDelegateWithLabel ExpanedEvent;
        public InstanceDelegateWithLabel CollapseEvent;
        internal InspectorProperty MemberProperty;
    }
    protected override void Initialize()
    {
        base.Initialize();
        var attribute = this.Attribute;
        var entry = this.Property.Parent.ValueEntry;

        var context = entry.Context.Get(this, "context", (Context)null);

        if (context.Value == null)
        {
            context.Value = new Context();
            context.Value.MemberProperty = entry.Property;
            var parentType = context.Value.MemberProperty.BaseValueEntry.TypeOfValue;
            MethodInfo methodInfo = null, methodInfoCollapse = null;
            if (!string.IsNullOrEmpty(attribute.methodNameExpaned))
            {

                methodInfo = parentType
                   .FindMember()
                   .IsNamed(attribute.methodNameExpaned)
                   .IsMethod()
                   .ReturnsVoid()
                   .HasNoParameters()
                   .GetMember<MethodInfo>(out context.Value.ErrorMessage);
            }
            if (!string.IsNullOrEmpty(attribute.methodNameExpaned))
            {
                methodInfoCollapse = parentType
                  .FindMember()
                 .IsNamed(attribute.methodNameCollapsed)
                 .IsMethod()
                .ReturnsVoid()
                .HasNoParameters()
                .GetMember<MethodInfo>(out context.Value.ErrorMessage);
            }
            if (context.Value.ErrorMessage == null)
            {
                if (methodInfo.IsStatic())
                {
                    //context.Value.CustomValueDrawerStaticWithLabel = (Func<T, GUIContent, T>)Delegate.CreateDelegate(typeof(Func<T, GUIContent, T>), methodInfo);
                }
                else
                {
                    if (methodInfo != null)
                    {
                        DynamicMethod emittedMethod;

                        emittedMethod = new DynamicMethod("CustomValueDrawerAttributeDrawer." + typeof(void).GetNiceFullName() + entry.Property.Path + attribute.methodNameExpaned, typeof(void), new Type[] { typeof(object).MakeByRefType() });

                        var il = emittedMethod.GetILGenerator();

                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldind_Ref);
                        il.Emit(OpCodes.Castclass, parentType);
                        il.Emit(OpCodes.Callvirt, methodInfo);
                        il.Emit(OpCodes.Ret);
                        //il.Emit(OpCodes.Ldind_Ref);
                        //il.Emit(OpCodes.Castclass, parentType);

                        //il.Emit(OpCodes.Callvirt, methodInfo);
                        //il.Emit(OpCodes.Ret);

                        context.Value.ExpanedEvent = (InstanceDelegateWithLabel)emittedMethod.CreateDelegate(typeof(InstanceDelegateWithLabel));
                    }
                    if (methodInfoCollapse != null)
                    {
                        DynamicMethod emittedMethod;

                        emittedMethod = new DynamicMethod("CustomValueDrawerAttributeDrawer." + typeof(void).GetNiceFullName() + entry.Property.Path + attribute.methodNameCollapsed, typeof(void), new Type[] { typeof(object).MakeByRefType() });

                        var il = emittedMethod.GetILGenerator();

                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldind_Ref);
                        il.Emit(OpCodes.Castclass, parentType);
                        il.Emit(OpCodes.Callvirt, methodInfoCollapse);
                        il.Emit(OpCodes.Ret);

                        context.Value.CollapseEvent = (InstanceDelegateWithLabel)emittedMethod.CreateDelegate(typeof(InstanceDelegateWithLabel));
                    }
                }
                if (!this.IsVisible.Value)
                {

                    if (context != null && context.Value.CollapseEvent != null)
                    {
                        var val = context.Value.MemberProperty.ValueEntry.WeakSmartValue;
                        context.Value.CollapseEvent(ref val);
                    }
                }
                else
                {
                    if (context != null && context.Value.ExpanedEvent != null)
                    {
                        var val = context.Value.MemberProperty.ValueEntry.WeakSmartValue;
                        context.Value.ExpanedEvent(ref val);
                    }
                }
            }
        }
    }
    protected override void DrawPropertyLayout(GUIContent label)
    {
  
        //if (attribute.Expanded)
        //{
        //    var val = context.Value.MemberProperty.ValueEntry.WeakSmartValue;
        //    context.Value.ExpanedEvent(ref val);
        //}
        //else
        //{
        //    var val = context.Value.MemberProperty.ValueEntry.WeakSmartValue;
        //    context.Value.CollapseEvent(ref val);
        //}
        base.DrawPropertyLayout(label);
    }

    public void Dispose()
    {
        var attribute = this.Attribute;
        var entry = this.Property.Parent.ValueEntry;

        var context = entry.Context.Get(this, "context", (Context)null);
        if(context != null && context.Value.CollapseEvent != null)
        {
            var val = context.Value.MemberProperty.ValueEntry.WeakSmartValue;
            context.Value.CollapseEvent(ref val);
        }
    }
}

#endif