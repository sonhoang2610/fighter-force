using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using UnityEngine.Events;
using UnityEngine.UI;
using EazyEngine.Tools.Space;
using Sirenix.OdinInspector;
using NodeCanvas.Framework;

namespace EazyEngine.Space
{
    [System.Serializable]
    public class UnityEventInt : UnityEvent<int>
    {

    }
    public class Health : MonoBehaviour, IRespawn,IListenerTriggerAnimator
    {
        [Sirenix.OdinInspector.ReadOnly]
        public int currentHealth;
        /// If this is true, this object can't take damage
        [Sirenix.OdinInspector.ReadOnly]
        public bool invulnerable = false;
        public bool InvulnerableIfOutSide = true;
        public bool invuOnStart = false;
        public float timeInvuAfterRevie = 0;
        public bool disableParrentOnDeath = false;
        public bool disableOnDeath  = true;
        public int deffense = 0;
        public int MaxiumHealth;
        public int InitialHealth;
        public GameObject containerHealthBar;
        public Image HealthBar;
        public AudioClip DamageSfx;
        public GameObject DeathEffect;
        public GameObject DamagedEffect;
        public GameObject HealingEffect;
        public GameObject HealingEffectTimer;

        public bool subHealth = false;
        [ShowIf("subHealth")]
        public int countChildMin = 0;
        [ShowIf("subHealth")]
        public Health[] childHealth;
        [ShowIf("subHealth")]
        public UnityEvent onMinChild;
        public UnityEvent onDeath;
        public UnityEventInt onTakenDamage;
        public UnityEvent onRevie;
        private Character _character;
        public bool animDeath = false;
        public string triggerAnimDeath = "";
        public string listenerDeathAnimFinish = "";

        protected int  healDestiny = 0;
        protected float currentTime = 0;
        protected float timeplan = 0.1f;
        protected float lastFrom = 0;
        protected float currentDeffense = 0;
	    protected int currentChildAlive = 0;
	    protected float currentDurationResetDamaged = 0;
	    protected int indexDamaged = 0;
        protected Health parentHealth;
        private void Awake()
        {
            if (DeathEffect && GameManager.Instance.inGame)
            {
                ParticleEnviroment.Instance.preloadEffect(5, DeathEffect, transform.position, 1);
            }
        }
        public Health[] getAllAvailableHealth()
        {
            List<Health> pHealths = new List<Health>();
            for(int i = 0; i < childHealth.Length; ++i)
            {
                pHealths.AddRange(childHealth[i].getAllAvailableHealth());
            }
            if(!subHealth  || currentChildAlive <= countChildMin)
            {
                pHealths.Add(this);
            }
            return pHealths.ToArray();
        }

        public void onDeathChild()
        {
            currentChildAlive--;
            if (currentChildAlive <= countChildMin)
            {
                if (CurrentHealth > 0)
                {
                    if (currentChildAlive <= countChildMin && Invulnerable)
                    {
                        onMinChild.Invoke();
                        Invulnerable = false;
                    }
                }
                else
                {
                    if (animDeath)
                    {
                        GetComponent<Collider2D>().enabled = false;
                        CharacterMain.SetTrigger(triggerAnimDeath);
                    }
                    else
                    {
                        Kill();
                    }
              
                }
            }
        }

        public float PercentageHealth
        {
            get
            {
                return (float)CurrentHealth / (float)MaxiumHealth;
            }
        }
        public Character CharacterMain
        {
            get
            {
                return _character;
            }

            set
            {
                _character = value;
            }
        }

        public int CurrentHealth
        {
            get
            {
                return currentHealth;
            }

            set
            {
                if (value < currentHealth)
                {
                    if(value > MaxiumHealth)
                    {
                        value = MaxiumHealth;
                    }
                    if (containerHealthBar != null)
                    {
                        if (!containerHealthBar.gameObject.activeSelf)
                        {
                            containerHealthBar.gameObject.SetActive(true);
                        }
                    
                    }
                }
                if (HealthBar)
                {
                    HealthBar.fillAmount = (float)value / (float)MaxiumHealth;
                }
          
                currentHealth = value;
            }
        }
        public int Deffense
        {
            set
            {
                deffense = value;
                currentDeffense = value;
            }
            get
            {
                return deffense;
            }
        }
        public bool Invulnerable {
            get {
                if (InvulnerableIfOutSide)
                {
                    if (LevelManger.InstanceRaw != null && LevelManger.Instance.mainPlayCamera.Rect().Contains(transform.position))
                    {
                        return invulnerable || invuSelf;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return invulnerable;
                }
            } set => invulnerable = value; }

        public Health ParentHealth { get => parentHealth; set => parentHealth = value; }
        public void triggerToParent(string pTrigger)
        {
            if (ParentHealth)
            {
                ParentHealth.GetComponent<GraphOwner>().SendEvent(pTrigger);
            }
        }

        public void addHealth(int pHealth)
        {
            CurrentHealth += pHealth;
            if (HealingEffect)
            {
                ParticleEnviroment.Instance.createEffect(HealingEffect, transform.position);
            }
        }

        public void ignoreWithLayer(string layer)
        {
            LevelManger.Instance.addIgnoreObjectWithLayer(gameObject, LayerMask.NameToLayer(layer));
        }

        public void clearIgnoreSelf()
        {
            LevelManger.Instance.removeIgnoreObject(gameObject);
        }

        public void healHealthTime(float pTime,int pHealth)
        {
           if(healDestiny == 0)
            {
                healDestiny = currentHealth + pHealth;
            }
            else
            {
                healDestiny += pHealth;
            }
           if(healDestiny >= MaxiumHealth)
            {
                healDestiny = MaxiumHealth;
            }
            currentTime = 0;
            timeplan = pTime;
            lastFrom = currentHealth;
            if (HealingEffectTimer)
            {
                HealingEffectTimer.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (healDestiny != 0)
            {
                currentTime += Time.deltaTime;
                float percent = currentTime / timeplan;
                int pHealth = (int)Mathf.Lerp(lastFrom, healDestiny, percent);
                if (pHealth > CurrentHealth)
                {
                    EzEventManager.TriggerEvent(new DamageTakenEvent(_character, gameObject, pHealth, pHealth - CurrentHealth, currentHealth));
                }
                CurrentHealth = pHealth;
                if(percent >= 1)
                {
                    healDestiny = 0;
                    if (HealingEffectTimer)
                    {
                        HealingEffectTimer.gameObject.SetActive(false);
                    }
                }
            }
        }
        private void Start()
        {
            Initialization();
        }

        protected virtual void Initialization()
        {

            currentDeffense = deffense;
            CharacterMain = GetComponent<Character>();
            CurrentHealth = InitialHealth;
            if (!invuOnStart)
            {
                Invulnerable = false;
            }
            else
            {
                Invulnerable = true;
            }
            if (subHealth)
            {
                for (int i = 0; i < childHealth.Length; ++i)
                {
                    childHealth[i].onDeath.AddListener(onDeathChild);
                    childHealth[i].ParentHealth = this;
                }
                    currentChildAlive = childHealth.Length;
                if (currentChildAlive > countChildMin)
                {
                    Invulnerable = true;
                }
            }
        }

        public virtual void Damage(int damage, GameObject instigator, float flickerDuration, float invincibilityDuration)
        {
            if (Invulnerable) return;
            if (CurrentHealth <= 0) return;
            
            float previousHealth = CurrentHealth;
            if(currentDeffense != 0)
            {
                if(currentDeffense > 0)
                {
                    damage =(int)( (float)damage * 10000 / (10000 + currentDeffense));
                }
                else
                {
                    damage = (int)((float)damage * (2 - 10000 / (10000 - currentDeffense)));
                }
            }
	        if(damage > 0 ){
	        	int pDownDamaged = 0;
	        	for(int i  = 0 ; i < pDownDamaged ; ++i){
	        		pDownDamaged += 15*i;
	        	}
	        	if(pDownDamaged > 90){
	        		pDownDamaged = 90;
	        	}
	        	damage -= (int)((float)damage* (float)pDownDamaged/10.0f);
	        	indexDamaged++;
	        }
            CurrentHealth -= damage;
            if(damage > 0)
            {
            	if(currentDurationResetDamaged <= 0){
	            	currentDurationResetDamaged = 0.1f;
            	}
                if (DamagedEffect)
                {
                    ParticleEnviroment.Instance.createEffect(DamagedEffect, transform.position, 1);
                }
                onTakenDamage.Invoke(damage);
                if (CharacterMain)
                {
                    if (CharacterMain.machine != null)
                    {
                        CharacterMain.machine.SetTrigger("Hurt");
                        if (CharacterMain.machine.model)
                        {
                            var fill = CharacterMain.machine.model.GetComponent<FillColor>();
                            if (fill)
                            {
                                fill.flash(0.1f);
                            }
                        }
                    }
                }
            }
            EzEventManager.TriggerEvent<DamageTakenEvent>(new DamageTakenEvent(CharacterMain, instigator, CurrentHealth, damage, previousHealth));

            if(CurrentHealth <= 0)
            {
                if (animDeath)
                {
                    GetComponent<Collider2D>().enabled = false;
                    CharacterMain.SetTrigger(triggerAnimDeath);
                }
                else
                {
                    Kill();
                }         
            }
        }
        public virtual void onKill()
        {
            
        }
        public  void Kill()
        {
            Kill(true);
        }

        public void Kill(bool isPing)
        {
            if (subHealth)
            {
                for (int i = 0; i < childHealth.Length; ++i)
                {
                    if (childHealth[i].CurrentHealth > 0)
                    {
                        childHealth[i].Kill(false);
                    }
                }
            }
            if (isPing)
            {
                onDeath.Invoke();
            }
            if (disableParrentOnDeath)
            {
                transform.parent.gameObject.SetActive(false);
            }
            if (disableOnDeath)
            {
                gameObject.SetActive(false);
            }
            if (containerHealthBar)
            {
                containerHealthBar.gameObject.SetActive(false);
            }
            if (DeathEffect)
            {
                ParticleEnviroment.Instance.createEffect(DeathEffect, transform.position);
            }
            onKill(); deathtime++;
        }

        public void showDeathEffect()
        {
            if (DeathEffect)
            {
                ParticleEnviroment.Instance.createEffect(DeathEffect, transform.position, 1);
            }
        }

        public void onRespawn()
        {
            
        }
        bool revieing = false;
        public void Revive(bool pActiveObject)
        {
            onRevie.Invoke();
         
            revieing = true;
            healDestiny = 0;
            CurrentHealth = InitialHealth;
            //GetComponent<Collider2D>().enabled = true;
            if (!invuOnStart)
            {
                Invulnerable = false;
            }
            else
            {
                Invulnerable = true;
            }
            if (subHealth && deathtime > 0)
            {
                currentChildAlive = childHealth.Length;
                if(currentChildAlive > 0)
                {
                    Invulnerable = true;
                }
            }
            if (HealthBar)
            {
                HealthBar.fillAmount = (float)CurrentHealth / (float)MaxiumHealth;
                containerHealthBar.gameObject.SetActive(false);
            }
             var pRespawns = GetComponents<IRespawn>();
            foreach(var pRespawn in pRespawns)
            {
                pRespawn.onRespawn();
            }
            revieing = false;
            if (subHealth && deathtime > 0)
            {
                for (int i = 0; i < childHealth.Length; ++i)
                {
                    childHealth[i].Revive();
                }
                if(timeInvuAfterRevie > 0)
                {
                    invuSelf = true;
                    Invoke("disableInvu", timeInvuAfterRevie);
                }
               
            }
            if (pActiveObject && !gameObject.activeSelf)
            {
                gameObject.SetActive(pActiveObject);
            }
        }
        protected bool invuSelf = false;
        public void disableInvu()
        {
            invuSelf = false;
        }
        protected int deathtime = 0;
        public void Revive()
        {
            Revive(true);
        }

        public bool registerListen()
        {
            throw new System.NotImplementedException();
        }
       
        public void TriggerFromAnimator(AnimationEvent pEvent)
        {
            if(pEvent.stringParameter == listenerDeathAnimFinish)
            {
                if (animDeath)
                {
                    Kill();
                }
            }
        }
	    // LateUpdate is called every frame, if the Behaviour is enabled.
	    protected void LateUpdate()
	    {
	    	if(currentDurationResetDamaged > 0){
	    		currentDurationResetDamaged -= Time.deltaTime;
	    		if(currentDurationResetDamaged <= 0){
	    			indexDamaged = 0;
	    		}
	    	}
	    }
    }
}
