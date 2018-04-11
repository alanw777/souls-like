using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AW.UI
{
    public class QuickSlot : MonoBehaviour
    {
        public static QuickSlot singleton;
        public List<QSlots> slots = new List<QSlots>();

        void Awake()
        {
            singleton = this;
        }
        public void Init()
        {
            ClearIcons();
        }

        public void ClearIcons()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].icon.gameObject.SetActive(false);
            }
        }

        public void UpdateSlot(QSlotType stype, Sprite i)
        {
            QSlots q = GetQSlot(stype);
            q.icon.sprite = i;
            q.icon.gameObject.SetActive(true);
        }

        public QSlots GetQSlot(QSlotType t)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].type == t)
                {
                    return slots[i];
                }
            }
            return null;
        }
    }

    public enum QSlotType
    {
        rh,lh,item,spell
    }

    [System.Serializable]
    public class QSlots
    {
        public Image icon;
        public QSlotType type;
    }

}

