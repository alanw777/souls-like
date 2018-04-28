using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AW
{
    public class ResourcesManager : MonoBehaviour
    {
        public List<Weapen> weapenList = new List<Weapen>();
        public Dictionary<string,int> weapen_Ids = new Dictionary<string, int>();
        public Dictionary<string, int> spell_Ids = new Dictionary<string, int>();

 
        public static ResourcesManager singleton;

        void Awake()
        {
            singleton = this;
            LoadWeapenIds();
            LoadSpellIds();
        }

        private void LoadSpellIds()
        {
            SpellItemScriptableObject obj = Resources.Load("AW.SpellItemScriptableObject") as SpellItemScriptableObject;
            if (obj == null)
            {
                Debug.Log("could't find AW.SpellItemScriptableObject");
                return;
            }
            for (int i = 0; i < obj.spell_items.Count; i++)
            {
                if (spell_Ids.ContainsKey(obj.spell_items[i].itemName))
                {
                    Debug.Log(obj.spell_items[i].itemName + " item is a duplicae");
                }
                else
                {
                    spell_Ids.Add(obj.spell_items[i].itemName, i);
                }
            }
        }

        private void LoadWeapenIds()
        {
            WeapenScriptableObject obj = Resources.Load("AW.WeapenScriptableObject") as WeapenScriptableObject;
            if (obj == null)
            {
                Debug.Log("could't find AW.WeapenScriptableObject");
                return;
            }
            for (int i = 0; i < obj.weapen_all.Count; i++)
            {
                if (weapen_Ids.ContainsKey(obj.weapen_all[i].itemName))
                {
                    Debug.Log(obj.weapen_all[i].itemName + " item is a duplicae");
                }
                else
                {
                    weapen_Ids.Add(obj.weapen_all[i].itemName,i);
                }
            }
        }

        int GetWeapenIdFromString(string itemName)
        {
            int index = -1;
            if (weapen_Ids.TryGetValue(itemName, out index))
            {
                return index;
            }
            return -1;
        }

        public Weapen GetWeapen(string itemName)
        {
            WeapenScriptableObject obj = Resources.Load("AW.WeapenScriptableObject") as WeapenScriptableObject;
            int index = GetWeapenIdFromString(itemName);
            if (index == -1)
                return null;
            return obj.weapen_all[index];
        }

        int GetSpellIdFromString(string itemName)
        {
            int index = -1;
            if (spell_Ids.TryGetValue(itemName, out index))
            {
                return index;
            }
            return -1;
        }

        public Spell GetSpell(string itemName)
        {
            SpellItemScriptableObject obj = Resources.Load("AW.SpellItemScriptableObject") as SpellItemScriptableObject;
            int index = GetSpellIdFromString(itemName);
            if (index == -1)
            {
                Debug.Log("get spell null!");
                return null;
            }
            
            return obj.spell_items[index];
        }
    }

}

