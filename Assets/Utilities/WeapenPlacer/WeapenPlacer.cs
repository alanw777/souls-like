using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AW.Utilities
{
    [ExecuteInEditMode]
    public class WeapenPlacer : MonoBehaviour
    {
        public string weapenId;
        public GameObject weapenModel;
        public bool lefthand;
        public bool saveWeapen;

	    void Update () {
		    if(!saveWeapen)
                return;
	        saveWeapen = false;
            if(weapenModel==null)
                return;
	        if (string.IsNullOrEmpty(weapenId))
	            return;
            WeapenScriptableObject obj = Resources.Load("AW.WeapenScriptableObject") as WeapenScriptableObject;
            if(obj==null)
                return;
	        for (int i = 0; i < obj.weapen_all.Count; i++)
	        {
	            if (obj.weapen_all[i].itemName == weapenId)
	            {
	                Weapen w = obj.weapen_all[i];
	                if (lefthand)
	                {
	                    w.l_model_pos = weapenModel.transform.localPosition;
	                    w.l_model_eulers = weapenModel.transform.localEulerAngles;
	                }
	                else
	                {
                        w.r_model_pos = weapenModel.transform.localPosition;
                        w.r_model_eulers = weapenModel.transform.localEulerAngles;
	                }
	                w.model_scale = weapenModel.transform.localScale;
                    return;
	            }
	        }

            Debug.Log("weapenId not find in WeapenScriptableObject");
	    }
    }


}
