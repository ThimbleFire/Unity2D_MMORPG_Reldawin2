using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LowCloud.Reldawin
{
    public class Inventory : SceneBehaviour
    {
        [SerializeField] private AudioClip pickupItem;
        [SerializeField] private AudioClip dropItem;
        private byte emptySlots = 20;

        public byte GetEmptySlots
        {
            get { return emptySlots; }
        }

        private void Awake()
        {
            base.Awake();

            EventProcessor.AddInstructionParams( Packet.YieldInteract, Add );
        }

        private void Add( object[] obj )
        {
            foreach ( UI_Slot slot in slots )
            {
                if ( slot.IsEmpty )
                {
                    GameObject a = Instantiate( EmptyItemPrefab );
                    a.transform.SetParent( slot.transform );
                    a.GetComponent<Item>().Build( Convert.ToInt32(obj[0]) );

                    emptySlots--;

                    if ( emptySlots == 0 )
                        ClientTCP.SendInterrupt();

                    break;
                }
            }
        }

        private void Drop()
        {
            //not yet implemented
        }

        public GameObject EmptyItemPrefab;
        public UI_Slot[] slots;
    }
}