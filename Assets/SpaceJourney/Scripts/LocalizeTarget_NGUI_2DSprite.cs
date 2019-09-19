
#if NGUI

using UnityEngine;

namespace I2.Loc
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif

    public class LocalizeTarget_NGUI_2DSprite : LocalizeTarget<UI2DSprite>
    {
        static LocalizeTarget_NGUI_2DSprite() { AutoRegister(); }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] static void AutoRegister() { LocalizationManager.RegisterTarget(new LocalizeTargetDesc_Type<UI2DSprite, LocalizeTarget_NGUI_2DSprite>() { Name = "NGUI UI2DSprite", Priority = 100 }); }

        public override eTermType GetPrimaryTermType(Localize cmp) { return eTermType.Sprite; }
        public override eTermType GetSecondaryTermType(Localize cmp) { return eTermType.Sprite; }
        public override bool CanUseSecondaryTerm() { return true; }
        public override bool AllowMainTermToBeRTL() { return false; }
        public override bool AllowSecondTermToBeRTL() { return false; }

        public override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
        {
            primaryTerm = mTarget.mainTexture ? mTarget.mainTexture.name : "";
            if (mTarget.sprite2D != null && mTarget.sprite2D.name != primaryTerm)
                primaryTerm += "." + mTarget.sprite2D.name;
            var btn = mTarget.GetComponent<UIButton>();
            secondaryTerm = btn ? (btn.disabledSprite2D?  btn.disabledSprite2D.name : "" ) : "";
        }


        public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
        {
            Sprite Old = mTarget.sprite2D;
            bool bChanged = false;
            if (Old == null || Old.name != mainTranslation)
            {
                mTarget.sprite2D = cmp.FindTranslatedObject<Sprite>(mainTranslation);
                mTarget.nextSprite = cmp.FindTranslatedObject<Sprite>(mainTranslation);
                bChanged = true;
            }
            var btn = mTarget.GetComponent<UIButton>();
            if (btn)
            {
                if (bChanged)
                {
                    btn.normalSprite2D = cmp.FindTranslatedObject<Sprite>(mainTranslation);
                }
                Sprite OldDisable = btn.disabledSprite2D;
                if (!string.IsNullOrEmpty(secondaryTranslation) &&( OldDisable == null || OldDisable.name != secondaryTranslation))
                {
                    btn.disabledSprite2D = cmp.FindTranslatedObject<Sprite>(secondaryTranslation);
                }
            }
   
            // If the old value is not in the translatedObjects, then unload it as it most likely was loaded from Resources
            //if (!HasTranslatedObject(Old))
            //	Resources.UnloadAsset(Old);

            // In the editor, sometimes unity "forgets" to show the changes
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorUtility.SetDirty(mTarget);
#endif
            if (bChanged)
                mTarget.MakePixelPerfect();
        }
    }
}
#endif

