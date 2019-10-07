using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EazyEngine.Tools;
using System;

namespace EazyEngine.Space
{
    //#if UNITY_EDITOR
    //    using UnityEditor;

    //    public class AbilityContainerCreator
    //    {
    //        [MenuItem("Assets/Create/EazyEngine/Space/AbilityContainer")]
    //        public static void CreateMyAsset()
    //        {
    //            AbilityContainer asset = ScriptableObject.CreateInstance<AbilityContainer>();
    //            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
    //            if (string.IsNullOrEmpty(path))
    //            {
    //                path = "Assets";
    //            }
    //            AssetDatabase.CreateAsset(asset, path + "/AbilityContainer.asset");
    //            AssetDatabase.SaveAssets();

    //            EditorUtility.FocusProjectWindow();

    //            Selection.activeObject = asset;
    //        }
    //    }
    //#endif

    public interface ILevelSetter
    {
        void setLevel(int pLevel);
    }
    [System.Serializable]
    public class UnitDefineLevel : ILevelSetter, IConvertible
    {
        [ShowIf("@(!isSpecifiedUnit || algrothimTypeUpgrade == AlgrothimPriceItem.StepConstrainEachLevel)")]
        public int startUnit = 0;

        public AlgrothimPriceItem algrothimTypeUpgrade;
        [ShowIf("algrothimTypeUpgrade", AlgrothimPriceItem.StepConstrainEachLevel)]
        public int stepUnit;
        [ShowIf("algrothimTypeUpgrade", AlgrothimPriceItem.ConstrainDefineFromeSpecifiedLevel)]
        public bool isSpecifiedUnit = false;
        [ShowIf("algrothimTypeUpgrade", AlgrothimPriceItem.ConstrainDefineFromeSpecifiedLevel)]
        public SpecifiedLevelStepUnit[] unitDefines;

        private int currentLevel;
        [ShowInInspector]
        [Sirenix.OdinInspector. ReadOnly]
        protected int CurrentLevel { get => currentLevel; set => currentLevel = value; }

        public UnitDefineLevel(int pStartUnit)
        {
            startUnit = pStartUnit;
        }
        public override string ToString()
        {
            return getUnit(CurrentLevel).ToString();
        }
        public static bool operator !=(UnitDefineLevel obj1, int obj2)
        {
            return obj1.getUnit(obj1.CurrentLevel).Equals(obj2);
        }
        public static bool operator ==(UnitDefineLevel obj1, int obj2)
        {
            return obj1.getUnit(obj1.CurrentLevel).Equals(obj2);
        }
        public static implicit operator int( UnitDefineLevel obj2)
        {
            return obj2.getUnit(obj2.CurrentLevel);
        }
        public int getUnit(int pLevel)
        {
            if(algrothimTypeUpgrade == AlgrothimPriceItem.StepConstrainEachLevel)
            {
                return startUnit + pLevel * stepUnit;
            }
            if(unitDefines == null || unitDefines.Length == 0)
            {
                return startUnit;
            }
            int pGrowth = 0;
            int indexPrice = unitDefines.Length - 1;
            int pLEvel = pLevel;
            while (pLEvel > 0)
            {
                if (indexPrice < unitDefines.Length && indexPrice >= 0)
                {
                    if (pLEvel >= unitDefines[indexPrice].levelRequire)
                    {
                        if(pLEvel == unitDefines[indexPrice].levelRequire && isSpecifiedUnit)
                        {
                            return unitDefines[indexPrice].unit;
                        }
                        if (isSpecifiedUnit)
                        {
                            pGrowth = unitDefines[indexPrice].unit;
                        }
                        else
                        {
                            pGrowth += unitDefines[indexPrice].unit;
                        }                 
                        pLEvel--;
                    }
                    else
                    {
                        indexPrice--;
                    }
                }
                else
                {
                    pLEvel = 0;
                }

            }
            if (isSpecifiedUnit)
            {
                return pGrowth;
            }
            else
            {
                return startUnit + pGrowth;
            }
        
        }

        public void setLevel(int pLevel)
        {
            currentLevel = pLevel;
        }

        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }
        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            return this;
        }

        public long ToInt64(IFormatProvider provider)
        {
            return this;
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            return this;
        }

        public string ToString(IFormatProvider provider)
        {
            return this.ToString();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
    }
    [System.Serializable]
    public class AbilityConfig
    { 
        [EzSerializeField]
        [InlineEditor]
        public AbilityInfo _ability;
        public bool isDisplay = true;
        [EzSerializeField]
        public int startUnit = 0;
        [EzSerializeField]
        public int limitUnit = 0;
        public AlgrothimPriceItem algrothimTypeUpgrade;
        [ShowIf("algrothimTypeUpgrade", AlgrothimPriceItem.StepConstrainEachLevel)]
        public int priceStep;
        [ShowIf("algrothimTypeUpgrade", AlgrothimPriceItem.ConstrainDefineFromeSpecifiedLevel)]
        public SpecifiedLevelStepUnit[] priceDefines = new SpecifiedLevelStepUnit[] { new SpecifiedLevelStepUnit() { levelRequire = 0, unit = 0 } };
        public int ExtraDamage { get; set; }

        public int getUnitAtLevel(int oLevel)
        {
            if (algrothimTypeUpgrade == AlgrothimPriceItem.StepConstrainEachLevel)
            {
                return startUnit + oLevel * priceStep;
            }
            else
            {
                int pGrowth = 0;
                int indexPrice = priceDefines.Length - 1;
                int pLEvel = oLevel;
                while (pLEvel > 0)
                {
                    if (indexPrice < priceDefines.Length && indexPrice >= 0)
                    {
                        if (pLEvel >= priceDefines[indexPrice].levelRequire)
                        {
                            pGrowth += priceDefines[indexPrice].unit;
                            pLEvel--;
                        }
                        else
                        {
                            indexPrice--;
                        }
                    }
                    else
                    {
                        pLEvel = 0;
                    }

                }
                return startUnit + pGrowth + ExtraDamage;
            }
        }
        public int CurrentUnit
        {
            get
            {
                if (algrothimTypeUpgrade == AlgrothimPriceItem.StepConstrainEachLevel)
                {
                    return startUnit + CurrentLevel * priceStep;
                }
                else
                {
                    int pGrowth = 0;
                    int indexPrice = priceDefines.Length - 1;
                    int pLEvel = currentLevel;
                    while (pLEvel > 0)
                    {
                        if (indexPrice < priceDefines.Length && indexPrice>=  0)
                        {
                            if (pLEvel >= priceDefines[indexPrice].levelRequire)
                            {
                                pGrowth += priceDefines[indexPrice].unit;
                                pLEvel--;
                            }
                            else 
                            {
                                indexPrice--;
                            }
                        }
                        else
                        {
                            pLEvel = 0;
                        }

                    }
                    return startUnit + pGrowth + ExtraDamage;
                }
            }
        }
        public int NextUnit
        {
            get
            {
                if (algrothimTypeUpgrade == AlgrothimPriceItem.StepConstrainEachLevel)
                {
                    return startUnit + (CurrentLevel+1) * priceStep;
                }
                else
                {
                    int pGrowth = 0;
                    int indexPrice = priceDefines.Length - 1;
                    int pLEvel = (CurrentLevel+1);
                    while (pLEvel > 0)
                    {
                        if (indexPrice < priceDefines.Length && indexPrice >= 0)
                        {
                            if (pLEvel >= priceDefines[indexPrice].levelRequire)
                            {
                                pGrowth += priceDefines[indexPrice].unit;
                                pLEvel--;
                            }
                            else
                            {
                                indexPrice--;
                            }
                        }
                        else
                        {
                            pLEvel = 0;
                        }

                    }
                    return startUnit + pGrowth;
                }
            }
        }
        private int currentLevel = 0;
        public int CurrentLevel
        {
            set
            {
                currentLevel = value;
            }
            get
            {
                return currentLevel;
            }
        }
    }
    public class AbilityContainer : EzMonoBehaviorSerialize
    {
        [EzSerializeField]

        public AbilityConfig[] abilities;
    }
}
