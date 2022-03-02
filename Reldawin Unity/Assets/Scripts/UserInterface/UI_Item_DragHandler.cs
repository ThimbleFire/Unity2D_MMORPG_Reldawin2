using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LowCloud.Reldawin
{
    public class UI_Item_DragHandler : MonoBehaviour, /*IBeginDragHandler,*/ IDragHandler, IEndDragHandler, IInitializePotentialDragHandler
    {
        public static GameObject itemBeingDragged;
        private Vector3 startPosition;
        private Transform startParent;

        //void Start()
        //{
        //
        //}

        //void Update()
        //{
        //
        //}

        public void OnInitializePotentialDrag /*OnBeginDrag*/( PointerEventData eventData )
        {
            itemBeingDragged = gameObject;
            startPosition = transform.position;
            startParent = transform.parent;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            AudioDevice.Instance.Play( Sound.Common.Pickup );
        }

        public void OnDrag( PointerEventData eventData )
        {
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag( PointerEventData eventData )
        {
            //itemBeingDragged.GetComponent<Item>()
            itemBeingDragged = null;
            GetComponent<CanvasGroup>().blocksRaycasts = true;

            if ( transform.parent == startParent )
            {
                transform.position = startPosition;
            }
        }
    }
}