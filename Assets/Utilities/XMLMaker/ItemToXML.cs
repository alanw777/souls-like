using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using AW;

namespace AW.Utilities
{
    [ExecuteInEditMode]
    public class ItemToXML : MonoBehaviour
    {
        public bool make;
        public List<RuntimeWeapen> canidates = new List<RuntimeWeapen>();
        public string xml_vertion;
        public string targetName;

        void Update()
        {
            if(!make)
                return;
            make = false;

            string xml = xml_vertion;//<?xml version="1.0" encoding="UTF-8"?>
            
            xml +="\n" + "<root>" + "\n";

            foreach (RuntimeWeapen i in canidates)
            {
                Weapen w = i.instance;

                xml += "<weapen>" + "\n";
                xml += "<oh_idle>" + w.oh_idle + "</oh_idle>" + "\n";
                xml += "<th_idle>" + w.th_idle + "</th_idle>" + "\n";
                xml += ActionListToString(w.actions, "actions");
                xml += ActionListToString(w.two_handenActions, "two_handenActions");
                xml += "<parryMultiplier>" + w.parryMultiplier + "</parryMultiplier>" + "\n";
                xml += "<backstabMultiplier>" + w.backstabMultiplier + "</backstabMultiplier>" + "\n";
                xml += "<leftHandMirror>" + w.leftHandMirror + "</leftHandMirror>" + "\n";

//                xml += "<mp_x>" + w.model_pos.x + "</mp_x>" + "\n";
//                xml += "<mp_y>" + w.model_pos.y + "</mp_y>" + "\n";
//                xml += "<mp_z>" + w.model_pos.z + "</mp_z>" + "\n";
//
//                xml += "<me_x>" + w.model_eulers.x + "</me_x>" + "\n";
//                xml += "<me_y>" + w.model_eulers.y + "</me_y>" + "\n";
//                xml += "<me_z>" + w.model_eulers.z + "</me_z>" + "\n";

                xml += "<ms_x>" + w.model_scale.x + "</ms_x>" + "\n";
                xml += "<ms_y>" + w.model_scale.y + "</ms_y>" + "\n";
                xml += "<ms_z>" + w.model_scale.z + "</ms_z>" + "\n";

                xml += "</weapen>" + "\n";

            }

            xml += "</root>";

            string path = StaticStrings.SaveLocation() + StaticStrings.itemFolder;
            if (string.IsNullOrEmpty(targetName))
            {
                targetName = "items_database.xml";
            }
           
            path += targetName;
            File.WriteAllText(path,xml);
        }

        private string ActionListToString(List<Action> actions,string nodeName)
        {
            string xml = "";
            foreach (Action a in actions)
            {
                xml += "<" + nodeName + ">" + "\n";
                xml += "<ActionInput>" + a.input + "</ActionInput>" + "\n";
                xml += "<ActionType>" + a.type + "</ActionType>" + "\n";
                xml += "<targetAnim>" + a.targetAnim + "</targetAnim>" + "\n";
                xml += "<mirror>" + a.mirror + "</mirror>" + "\n";
                xml += "<canBenParried>" + a.canBenParried + "</canBenParried>" + "\n";
                xml += "<changeSpeed>" + a.changeSpeed + "</changeSpeed>" + "\n";
                xml += "<animSpeed>" + a.animSpeed + "</animSpeed>" + "\n";
                xml += "<canParry>" + a.canParry + "</canParry>" + "\n";
                xml += "<canBackstab>" + a.canBackstab + "</canBackstab>" + "\n";
                xml += "<overrideDamageAnim>" + a.overrideDamageAnim + "</overrideDamageAnim>" + "\n";
                xml += "<damageAnim>" + a.damageAnim + "</damageAnim>" + "\n";

                WeapenStats s = a.weapenStats;
                xml += "<physical>" + s.physical + "</physical>" + "\n";
                xml += "<strike>" + s.strike + "</strike>" + "\n";
                xml += "<slash>" + s.slash + "</slash>" + "\n";
                xml += "<thrust>" + s.thrust + "</thrust>" + "\n";
                xml += "<magic>" + s.magic + "</magic>" + "\n";
                xml += "<fire>" + s.fire + "</fire>" + "\n";
                xml += "<lighting>" + s.lighting + "</lighting>" + "\n";
                xml += "<dark>" + s.dark + "</dark>" + "\n";

                xml += "</" + nodeName + ">" + "\n";

            }
            return xml;
        }

        

    }

}

