using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyEngine.Space
{
    public struct DamageTakenEvent
    {
        public Character AffectedCharacter;
        public GameObject Instigator;
        public float CurrentHealth;
        public float DamageCaused;
        public float PreviousHealth;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoreMountains.CorgiEngine.MMDamageTakenEvent"/> struct.
        /// </summary>
        /// <param name="affectedCharacter">Affected character.</param>
        /// <param name="instigator">Instigator.</param>
        /// <param name="currentHealth">Current health.</param>
        /// <param name="damageCaused">Damage caused.</param>
        /// <param name="previousHealth">Previous health.</param>
        public DamageTakenEvent(Character affectedCharacter, GameObject instigator, float currentHealth, float damageCaused, float previousHealth)
        {
            AffectedCharacter = affectedCharacter;
            Instigator = instigator;
            CurrentHealth = currentHealth;
            DamageCaused = damageCaused;
            PreviousHealth = previousHealth;
        }
    }
    [System.Serializable]
    public struct DamageGivenEvent
    {
        public GameObject Affected;
        public GameObject Instigator;
        public float CurrentHealth;
        public float DamageCaused;
        public float PreviousHealth;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoreMountains.CorgiEngine.MMDamageTakenEvent"/> struct.
        /// </summary>
        /// <param name="affectedCharacter">Affected character.</param>
        /// <param name="instigator">Instigator.</param>
        /// <param name="currentHealth">Current health.</param>
        /// <param name="damageCaused">Damage caused.</param>
        /// <param name="previousHealth">Previous health.</param>
        public DamageGivenEvent(GameObject affectedCharacter, GameObject instigator, float currentHealth, float damageCaused, float previousHealth)
        {
            Affected = affectedCharacter;
            Instigator = instigator;
            CurrentHealth = currentHealth;
            DamageCaused = damageCaused;
            PreviousHealth = previousHealth;
        }
    }

    public struct PickEvent
    {

        public string _nameItem;
        public int _quantityItem;
        public GameObject _object,_owner;
        public FlowCanvas.FlowScript flow;
        public Dictionary<string,object> Variables;
        public PickEvent(string pNameItem,int pQuantityItem,GameObject pObject,GameObject pOwner, FlowCanvas.FlowScript pFlow = null,Dictionary<string,object> pVars = null)
        {
            _nameItem = pNameItem;
            _quantityItem = pQuantityItem;
            _object = pObject;
            _owner = pOwner;
            flow = pFlow;
            Variables = pVars;
        }
    }
}
