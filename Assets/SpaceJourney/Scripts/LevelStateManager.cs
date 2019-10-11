using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EazyEngine.Tools;
using EazyEngine.Tools.Space;
using System.Linq;
using DG.Tweening;
using EazyEngine.Space;
using UnityEngine.Events;
using EazyReflectionSupport;
using System.Reflection;
using System;

[System.Serializable]
public class UnityEventGameObject : UnityEvent<GameObject>
{

}

[System.Serializable]
public class UnityEvent2GameObject : UnityEvent<GameObject,GameObject>
{

}

[System.Flags]
[System.Serializable]
public enum EnemyType
{
    TYPE1 = 1 << 1,
    TYPE2 = 1 << 2,
    TYPE3 = 1 << 3,
    TYPE4 = 1 << 4,
    TYPE5 = 1 << 5,
    TYPE6 = 1 << 6,
    TYPE7 = 1 << 7,
    TYPE8 = 1 << 8,
    TYPE9 = 1 << 9,
    TYPE10 = 1 << 10,
    ROCKET = 1 << 11,
    ENEMY_FOLLOW = 1 << 12,
    TOWER_NORMAL = 1 << 13,
    TOWER_TARGET_PLAYER = 1 << 14,
    TOWER_ROTATE_4_NONG = 1 << 15,
    TOWER_LASER = 1 << 16,
}
[System.Serializable]
public enum TypeSpawn
{
    Random,
    Queue,
    RandomForAll
}
[System.Serializable]
public enum Formations
{
    None,
    DomainRandom,
    Grid,
    Circle,
    Triangle,
    Diamond
}
[System.Serializable]
public enum ConditionEndMoveType
{
    EndMove,
    DestroyAll,
    DestroyQuantity,
    CustomTrigger,
}
[System.Serializable]
public class ConditionEndMoveInfo
{
    public ConditionEndMoveType condition;
    [ShowIf("condition", ConditionEndMoveType.DestroyQuantity)]
    public int destroyQuantity = 1;
    [ShowIf("condition", ConditionEndMoveType.CustomTrigger)]
    public string triggerString;
}
[System.Serializable]
public enum AIBaseType
{
    Group,
    SelfEmiter
}
[System.Serializable]
public enum ConditionAttack
{
    None,
    OnSight,
    FaceDown,
    Trigger
}
[System.Serializable]
public enum TypeDirection
{
    Random,
    FaceSelf,
    MainPlayer,
}


[System.Serializable]
public class LootInfo
{
    [LabelText("Base Loot")]
    // [LabelWidth(100)]
    public AIBaseType baseLoot;
    public PickAbleItem[] items;
    public float percent = 1;
    public Vector2 dropCountRange = new Vector2(1,1);

    public LootInfo(PickAbleItem[] pItems ,Vector2 pRange)
    {
        items = pItems;
        dropCountRange = pRange;
    }
}
[System.Serializable]
public class FormationInfo
{
   //[Tooltip("Chọn được nhiều loại quái. (Xuất hiện theo trình tự hay ngẫu nhiên dựa vào typeSpwan)")]
    public GameObject[] prefabEnemies;
    [LabelWidth(100)]
   //[Tooltip("Số lượng quái xuất hiện")]
    public int quantity = 1;
    [LabelWidth(100)]
   //[Tooltip("Kiểu chọn ra loại quái spawn trong arrayEnemy theo trình tự hay random")]

    public TypeSpawn typeSpawn = TypeSpawn.Random;
    [LabelWidth(100)]
   //[Tooltip("Kiểu sắp xếp đội hình 1 Điểm ,ngẫu nhiên,grid,tròn,tam giác,diamond")]
    public Formations typeFomation = Formations.None;
   //[Tooltip("Điểm bắt đầu")]
    public Vector2 startSpawnPos = new Vector2(0,3);

    [ShowIf("typeFomation", Formations.Grid)]
   //[Tooltip("false trình tự sẽ theo hàng trước true trình tự sẽ theo cột trước")]
    public bool AxisX = false;
    [ShowIf("typeFomation", Formations.DomainRandom)]
   //[Tooltip("Vùng quái random bên trong")]
    public Vector2 sizeDomain = new Vector2(1,1);
    [ShowIf("validateFormationDistance")]
   //[Tooltip("Khoảng cách giữa các con quái")]
    public Vector2 distanceEmiter = new Vector2(1,1);
    [ShowIf("typeFomation", Formations.Grid)]
   //[Tooltip("Max collum when AxisX = true -> max Row")]
    public int maxCollum = 1;
    [ShowIf("typeFomation", Formations.Circle)]
   //[Tooltip("Bán kính vòng tròn")]
    public float radius = 1;
   //[Tooltip("Hướng ban đầu dùng cho rocket và tránh quái bị quay đầu smooth lúc đầu không đúng theo ý")] 
    public Vector2 directionStart  = Vector2.down;

    public bool validateFormationDistance()
    {
        return typeFomation != Formations.None && typeFomation != Formations.Circle;
    }
}

public struct MessageGamePlayEvent
{
    public string _message;
    public object[] _objects;
    public MessageGamePlayEvent(string pMess,params object[] pObjects)
    {
        _message = pMess;
        _objects = pObjects;
    }
}
[System.Serializable]
public enum TypeLook
{
    LookDirection,
    LookMainPlayer,
    LookDown,
    None,
}
[System.Serializable]
public class MoveInfo
{
    [HideInInspector]
    public int indexStateMove = 0;
    [LabelWidth(100)]
    public float DelayStart = 0;
    [LabelWidth(100)]
    public float RowDelay = 0;
    [HideLabel]
  [ButtonChangeValue("formatInfo/startSpawnPos")]
    public BezierSplineRaw splineRaw = new BezierSplineRaw();
    [LabelWidth(100)]
   //[Tooltip("Hướng mặt của quái khi di chuyển nhìn theo hướng đi, nhìn theo player, nhĩn xuống ,giữ nguyên như hướng mặt lúc đầu")]
    public TypeLook lookType;
    [HorizontalGroup("group3")]
    [LabelWidth(100)]
    public float durationMove = 1;
    [HorizontalGroup("group3")]
    [LabelWidth(100)]
    public bool speedBase = false;
    [LabelWidth(100)]
    public AnimationCurve curvesMoving = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
    [LabelWidth(100)]
    public int loop = 1;
    //[Tooltip("Điều kiện để chuyển sang state move tiếp theo")]
    public EventNodeMove[] onCompleteNode;
    public ConditionEndMoveInfo[] conditionComplete = new ConditionEndMoveInfo[] { new ConditionEndMoveInfo() };
    public UnityEventGameObject onComplete;
    public UnityEventGameObject onStart;

}
[System.Serializable]
public struct EventNodeMove
{
    public int nodeListen;
    public UnityEventGameObject onComplate;
}
[System.Serializable]
public enum ConditionMaChineStart
{
    Disable,
    OnStart,
    InSideScreen,
    Trigger
}
[System.Serializable]
public class AIAttackInfo
{
    [LabelText("Base AI")]
    [LabelWidth(100)]
   //[Tooltip("Thống số AI dựa trên toàn bộ group hoặc từng con quái")]
    public AIBaseType BaseAI;
    [HideLabel]
    public AIElementAttackInfo infoAI;
}
[System.Serializable]
public class AIElementAttackInfo
{
    [LabelText("Condition Start AI")]
   //[Tooltip("Điều kiện bắt đầu suy nghĩ: Ngay khi sinh ra,nằm trong màn hình,chờ khi có 1 trigger gọi từ ngoài")]
    public ConditionMaChineStart conditionStart;
    [ShowIf("conditionStart", ConditionMaChineStart.Trigger)]
    public string triggerStringMachine = "StartMachine";
   //[Tooltip("Điều kiện bắt đầu tấn công: Ngẫu nhiên, mặt đối mặt,khi quay mặt xuông dưới")]
    public ConditionAttack conditionAttack = ConditionAttack.None;
    [ShowIf("conditionAttack",ConditionAttack.Trigger)]
    public string triggerStringAttack = "StartAttack";
    [LabelText("First time Delay")]
    public Vector2 firstDelay = new Vector2(0, 0);
    [LabelText("Random Reload Time")]
   //[Tooltip("Thời gian nghỉ giữa các đợt tấn công")]
    public Vector2 reloadTime = new Vector2(2, 3);
    [LabelText("Random Count Attack Per Turn")]
   //[Tooltip("Ngẫu nhiên trong khoảng x->y số lần bắn trong 1 đợt")]
    public Vector2 countShoot = new Vector2(1, 2);
    [LabelText("Delay Per Attack")]
    //[InfoBox("min reloadtime should greater than delayPerAttack * countattack")]
   //[Tooltip("trong 1 đợt bắn nhiều lần khoảng cách nghỉ")]
    public Vector2 delayPerAttack = new Vector2(0.2f, 0.5f);
   //[Tooltip("Hướng bắn của viên đạn độc lập không liên quan đến hướng mặt của quái dùng trong trường hợp quái không thực tế quay 1 đằng bắn 1 nẻo")]
    public TypeDirection _typeDirection = TypeDirection.FaceSelf;
   //[Tooltip("semi bắn từng viên kiểu tap tap,auto mỗi lần bắn là bắn liên tục đến khi hết đạn")]
    //[InfoBox("kiểu auto phải cấu hình trong characterhandleWeapon của quái tích changeWeaponNotEnough (thay súng khi hết đạn) để bắn 2 3 lần với kiểu khác nhau Weapon cũng phải có WeaponAmmo (kho đạn) để giới hạn số lượng đạn")]
 //   public TypeFire typeFire = TypeFire.Semi;
}
[System.Serializable]
public enum TypeAttack
{
    Shoot,
    Sacrifice
}
[System.Serializable]
public enum CompleteAction
{
    Nothing,
    DestroyLeft,
}

[System.Serializable]

public class LevelState
{
   // private AIMachine machine;
    [HideInInspector]
    public bool isExpaned = false;
    [ExpanedListEvent("$nameGroup", "expand", "collapse")]
    [LabelWidth(100)]
    public string nameState = "None";
    [ExpanedListEvent("$nameGroup", "expand", "collapse")]
    [LabelWidth(100)]
    public string comment = "None";
    // [HideInInspector]
    [CustomValueDrawer("customName")]
    public string nameGroup = "";

    public string customName(string pValue,GUIContent pLabel)
    {
        return nameState + " (" + comment + ")";
    }
    public void expand()
    {
        if (isExpaned) return;
        isExpaned = true;
#if UNITY_EDITOR
        UnityEditor.SceneView.RepaintAll();
#endif
    }

    public void collapse()
    {
        if (!isExpaned) return;
        isExpaned = false;
#if UNITY_EDITOR
        UnityEditor.SceneView.RepaintAll();
#endif
    }

    #region Spawn

    //[FoldoutGroup("$nameGroup/Spwan")]
    //[Tooltip("Chọn được nhiều loại quái. (Xuất hiện theo trình tự hay ngẫu nhiên dựa vào typeSpwan)")]
    //public GameObject[] prefabEnemies;
    //[FoldoutGroup("$nameGroup/Spwan")]
    //[LabelWidth(100)]
    //[Tooltip("Số lượng quái xuất hiện")]
    //public int quantity = 1;
    //[FoldoutGroup("$nameGroup/Spwan")]
    //[LabelWidth(100)]
    //[Tooltip("Kiểu chọn ra loại quái spawn trong arrayEnemy theo trình tự hay random")]
    //public TypeSpawn typeSpawn = TypeSpawn.Random;
    [FoldoutGroup("$nameGroup/Spwan")]
    public bool isManual = false;
    [FoldoutGroup("$nameGroup/Spwan")]
    [HideIf("isManual")]
    [HideLabel]
  //  [CustomValueDrawer("convert")]
    public FormationInfo formatInfo = new FormationInfo();
    [ShowIf("isManual")]
    [FoldoutGroup("$nameGroup/Spwan")]
    public GroupManager groupManager;
    //public FormationInfo convert(FormationInfo formatInfo ,GUIContent pLabel)
    //{
    //    formatInfo.quantity = quantity;
    //    formatInfo.prefabEnemies = prefabEnemies;
    //    formatInfo.typeSpawn = typeSpawn;
    //    return formatInfo;
    //}
    #endregion
    #region Moving
    [FoldoutGroup("$nameGroup/Moving")]
    [CustomContextMenu("FlipX","FlipX")]
    [OnValueChanged("addNewMove")]
    [ListDrawerSettings(ShowIndexLabels = true)]
   //[Tooltip("Right click để flip")]
   // [CustomValuerWrap("setUpIndex")]
    public MoveInfo[] moveInfos = new MoveInfo[] { new MoveInfo() };
    //public MoveInfo setUpIndex(MoveInfo pInfo,GUIContent pLabel)
    //{
    //    pInfo.indexStateMove = moveInfos.findIndex(pInfo);
    //    return pInfo;
    //}
    public void addNewMove()
    {
        if (moveInfos.Length > 1)
        {
            moveInfos[moveInfos.Length - 1].splineRaw.SetControlPoint(0, moveInfos[moveInfos.Length - 2].splineRaw.GetControlPoint(moveInfos[moveInfos.Length - 2].splineRaw.ControlPointCount - 1),true);
        }
    }


    public void FlipX()
    {
        Vector3 pDir = formatInfo.directionStart;
        pDir.x = -pDir.x;
        formatInfo.directionStart = pDir;
        Vector3 pStart = formatInfo.startSpawnPos;
        pStart.x = -pStart.x;
        formatInfo.startSpawnPos = pStart;
        for (int i = 0; i < moveInfos.Length; ++i)
        {
            BezierSplineRaw pSpline = moveInfos[i].splineRaw;
            for(int j = 0; j < pSpline.ControlPointCount; ++j)
            {
               Vector3 pPoint =  pSpline.GetControlPoint(j);
                pPoint.x = -pPoint.x;
                pSpline.SetControlPoint(j, pPoint, true);
            }
        }
    }
    #endregion
    #region AI
    [FoldoutGroup("$nameGroup/AI")]
    [HideLabel]
    public AIAttackInfo attackInfo;
    #endregion
    #region Loot

    [FoldoutGroup("$nameGroup/Loot")]
    public LootInfo[] lootItems;

    #endregion
    //public AIMachine Machine
    //{
    //    get
    //    {
    //        return machine;
    //    }

    //    set
    //    {
    //        machine = value;
    //    }
    //}
    [FoldoutGroup("$nameGroup/Complete")]
    public CompleteAction _completeAction = CompleteAction.DestroyLeft;
    [FoldoutGroup("$nameGroup/Complete")]
    public UnityEvent onCompleteState;
    

    [ExpanedListEvent("$nameGroup")]
    [Button("Preview")]

    public void preview()
    {
        if (Application.isPlaying)
        {

            EzEventManager.TriggerEvent(new BehaviorStateEvent(BehaviorState.RunState, this));
        }
    }



    public static LevelState clipboard;
    [SerializeField]
    [EazyEngine.Tools.ReadOnly]
    [HideInEditorMode]
    public int totalDeath = 0;
    protected int totalComplete = 0;
    protected bool isComplete = false;


    public bool IsComplete
    {
        get
        {
            return isComplete;
        }
    }
    public int TotalComplete
    {
        set
        {
            totalComplete = value;
            if(totalComplete >= formatInfo.quantity)
            {
                isComplete = true;
                onCompleteState.Invoke();
            }
        }
        get
        {
            return totalComplete;
        }
    }

    #region validate
#if UNITY_EDITOR
    public int validateQuantityDestroy(int pValue, GUIContent label)
    {
        return (int)UnityEditor.EditorGUILayout.Slider(label, pValue, 0, formatInfo.quantity);
    }
#endif


    #endregion

}

public enum BehaviorState
{
    RunState,
}

public struct BehaviorStateEvent
{
    public LevelState _state;
    public BehaviorState _behavior;
    public BehaviorStateEvent(BehaviorState pBehavior, LevelState pState)
    {
        _state = pState;
        _behavior = pBehavior;
    }
}

public class LevelStateManager : Singleton<LevelStateManager>, EzEventListener<BehaviorStateEvent>
{
    // [ExpanedListEvent("")]
    [ListDrawerSettings(NumberOfItemsPerPage = 10,ShowItemCount = true)]
    //[InfoBox("Nơi define các đợt quái việc khi nào ra đợt nào trình tự ra sao nằm ở component Behaviour Tree bên dưới ấn open, mỏ rộng từng state play game ấn button preview để xem trước từng đợt")]
    public LevelState[] states;
    public void changeStartValue(int indexState,Vector3 oldPos,Vector3 newPos)
    {
        for (int i =0; i < states[indexState].moveInfos.Length; ++i)
        {
            BezierSplineRaw pSpline = states[indexState].moveInfos[i].splineRaw;
            for (int j = 0; j < pSpline.ControlPointCount; ++j)
            {
                pSpline.SetControlPoint(j, pSpline.GetControlPoint(j) + (newPos - oldPos), pSpline.Relative);
                if (!pSpline.Relative && j == 0)
                {
                    return;
                }
            }

        }
    }
    public void ActiveThisObject(GameObject pObject)
    {
        if (pObject.GetComponent<Health>())
        {
            pObject.GetComponent<Health>().Invulnerable = false;
        }
    }

    public void DeActiveThisObject(GameObject pObject)
    {
        if (pObject.GetComponent<Health>())
        {
            pObject.GetComponent<Health>().Invulnerable = true;
        }
    }
    public void changeValue(int indexState,int indexMove,int indexPoint,Vector3 oldPos,Vector3 newPos)
    {
        if(indexMove < states[indexState].moveInfos.Length - 1 && indexPoint == states[indexState].moveInfos[indexMove].splineRaw.ControlPointCount-1)
        {
            for(int i = indexMove +1; i < states[indexState].moveInfos.Length; ++i)
            {
                BezierSplineRaw pSpline = states[indexState].moveInfos[i].splineRaw;
                for(int j = 0; j < pSpline.ControlPointCount; ++j)
                {
                    pSpline.SetControlPoint(j, pSpline.GetControlPoint(j) + (newPos - oldPos), pSpline.Relative);
                  
                    if (!pSpline.Relative && j == 0)
                    {
                        return;
                    }
                }
              
            }
     
        }
        if(indexState < states.Length)
        {
            if (indexPoint == 1 && indexMove > 0)
            {
                Vector3 pPos1 = states[indexState].moveInfos[indexMove].splineRaw.GetControlPoint(indexPoint);
                Vector3 pMid = states[indexState].moveInfos[indexMove].splineRaw.GetControlPoint(indexPoint - 1);
                Vector3 pPos2 = pMid * 2 - pPos1;
                states[indexState].moveInfos[indexMove - 1].splineRaw.SetControlPoint(states[indexState].moveInfos[indexMove - 1].splineRaw.ControlPointCount - 2, pPos2);
            }
        }
       //for(int i = 0; i < states.Length; ++i)
       // {
       //   Vector3 pOldPos = states[i].formatInfo.startSpawnPos;
       //   for (int j =0; j < states[i].moveInfos.Length; ++j)
       //     {
       //         states[i].moveInfos[j].splineRaw.SetControlPoint(0, pOldPos);
       //         pOldPos = states[i].moveInfos[j].splineRaw.GetControlPoint(states[i].moveInfos[j].splineRaw.ControlPointCount-1);
       //     }
       // }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
    public LevelState runState(string pState)
    {
        for(int i  =0; i< states.Length; ++i)
        {
            if(states[i].nameState == pState)
            {
                var pStatee = states[i].DeepClone();
                runState(pStatee);       
                return pStatee;
            }
        }
        return null;
    } 
    private void OnEnable()
    {
        EzEventManager.AddListener(this);
    }
    private void OnDisable()
    {
        EzEventManager.RemoveListener(this);
    }
    public void OnEzEvent(BehaviorStateEvent eventType)
    {
        if (eventType._behavior == BehaviorState.RunState)
        {
            runState(eventType._state.DeepClone());
        }
    }
    public void runState(LevelState pState)
    {
        if (!pState.isManual)
        {
            GroupManager pLeader = GroupManager.managers.Find(x => !x.gameObject.activeSelf);
            if (!pLeader)
            {
                GameObject pMainLeaderObject = new GameObject();
                pLeader = pMainLeaderObject.AddComponent<GroupManager>();
                pLeader.ParentState = pState;
                pMainLeaderObject.name = "manager";
                GroupManager.managers.Add(pLeader);
            }
            else
            {
                pLeader.gameObject.SetActive((true));
                pLeader.ParentState = pState;
            }
     
        }
        else
        {
            GameObject pMainLeaderObject = PoolManagerComon.Instance.createNewObject(pState.groupManager.gameObject);
            pMainLeaderObject.transform.localPosition = Vector3.zero;
            pMainLeaderObject.SetActive(true);
            pMainLeaderObject.GetComponent< GroupManager>().ParentState = pState;
        }
    }

    public void DeBugString(string pStr)
    {
        Debug.Log(pStr);
    }

    protected override void Awake()
    {
        base.Awake();
       var pTime = gameObject.AddComponent<EazyEngine.Timer.TimeControllerElement>();
        pTime._groupName = EazyEngine.Timer.TimeKeeper.Instance.getTimeLineIndex( "Global");

        gameObject.AddComponent<TimeControlBehavior>();

    }
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) {
            if (states == null) return;
           // if (!transform.parent || !transform.parent.gameObject.activeInHierarchy) return;
            for (int i = 0; i < states.Length; ++i)
            {
                if (states[i].isExpaned)
                {
                    if (states[i].moveInfos.Length > 0 && !states[i].isManual)
                    {
                        Vector2[][] pArrayPos = JourneySpaceUlities.getPointFormat(states[i].formatInfo, states[i].formatInfo.quantity);
                        float pDegrees = VectorExtension.FindDegree(states[i].formatInfo.directionStart.normalized);
                        Color pOldColor = Gizmos.color;
                        Gizmos.color = Color.yellow;
                        for (int j = 0; j < pArrayPos.Length; ++j)
                        {
                            for (int k = 0; k < pArrayPos[j].Length; ++k)
                            {                        
                                Gizmos.DrawSphere((pArrayPos[j][k]).Rotate(-pDegrees) + states[i].formatInfo.startSpawnPos, 0.3f);                             
                            }
                        }
                        Gizmos.color = pOldColor;
                    }
                    if (states[i].formatInfo.typeFomation == Formations.DomainRandom && !states[i].isManual)
                    {
                        MMDebug.DrawGizmoRectangle(states[i].formatInfo.startSpawnPos, states[i].formatInfo.sizeDomain, Color.yellow);
                    }

                    MMDebug.DrawGizmoArrow(states[i].formatInfo.startSpawnPos, states[i].formatInfo.directionStart.normalized*3, Color.yellow);
                }
            }
           
        }
    }
}
