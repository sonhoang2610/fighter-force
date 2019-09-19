using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EazyEngine.Space
{
    public class AttachmentContainer : MonoBehaviour
    {
        public ObjectGroupAttachMent[] infos;
        protected List<GameObject> currentAttachMent;
        protected List<ObjectGroupAttachMent> attachMentSelected = new List<ObjectGroupAttachMent>();

        private void Awake()
        {
            attachMentSelected = new List<ObjectGroupAttachMent>();
            if(infos.Length > 0)
                 attachMentSelected.Add(infos[0]);
        }
        public List<GameObject> CurrentAttachMent
        {
            get
            {
                if(currentAttachMent == null)
                {
                    currentAttachMent = new List<GameObject>();
                }
                if(currentAttachMent.Count <= 0)
                {
                    for(int i = attachMentSelected.Count-1; i >= 0; i--)
                    {
                        currentAttachMent.AddRange(attachMentSelected[i].attachMentPosStart);
                    }
                }
                return currentAttachMent;
            }
        }
        public void setAttachMentInts(string pAttachMent)
        {
            attachMentSelected.Clear();
            var pStrs = pAttachMent.Split('|');
            for(int i = 0; i < pStrs.Length; ++i)
            {
                int a = -1;
               if( int.TryParse(pStrs[i],out a) && a < infos.Length)
                {
                    attachMentSelected.Add(infos[a]);
                }  
            }

        }
        public void addAttachMentInts(string pAttachMent)
        {
            var pStrs = pAttachMent.Split('|');
            for (int i = 0; i < pStrs.Length; ++i)
            {
                int a = -1;
                if (int.TryParse(pStrs[i], out a) && a < infos.Length)
                {
                    attachMentSelected.Add(infos[a]);
                }
            }
        }

        public void UpdateWeapon()
        {
            //var pWeapon = GetComponent<ProjectileMultipeWeapon>();
            //pWeapon.set
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
