using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using AW;

namespace AW.Utilities
{
    [ExecuteInEditMode]
    public class XMLToResources : MonoBehaviour
    {
        public bool load;
        public ResourcesManager resourcesManager;
        public string weapenFileName = "items_database.xml";
        void Update()
        {
            if(!load)
                return;
            load = false;

            LoadWeapenData(resourcesManager);

        }

        public void LoadWeapenData(ResourcesManager rm)
        {
            string filePath = StaticStrings.SaveLocation() + StaticStrings.itemFolder;
            filePath += weapenFileName;

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            foreach (XmlNode w in doc.DocumentElement.SelectNodes("//weapen"))
            {
                Weapen _w = new Weapen();
                _w.actions = new List<Action>();
                _w.two_handenActions = new List<Action>();

                XmlNode weapenId = w.SelectSingleNode("weapenId");
                XmlNode oh_idle = w.SelectSingleNode("oh_idle");
                _w.oh_idle = oh_idle.InnerText;
                XmlNode th_idle = w.SelectSingleNode("th_idle");
                _w.th_idle = th_idle.InnerText;

                XmlNode parryMultiplier = w.SelectSingleNode("parryMultiplier");
                float.TryParse(parryMultiplier.InnerText, out _w.parryMultiplier);
                XmlNode backstabMultiplier = w.SelectSingleNode("backstabMultiplier");
                float.TryParse(backstabMultiplier.InnerText, out _w.backstabMultiplier);

                XmlNode leftHandMirror = w.SelectSingleNode("leftHandMirror");
                _w.leftHandMirror = (leftHandMirror.InnerText == "True");

//                _w.model_pos = XmlToVector3(w,"mp");
//                _w.model_eulers = XmlToVector3(w, "me");
//                _w.model_scale = XmlToVector3(w, "ms");

                XmlToActions(doc, "actions", ref _w);
                XmlToActions(doc, "two_handenActions", ref _w);

                resourcesManager.weapenList.Add(_w);
            }
        }

        private Vector3 XmlToVector3(XmlNode w, string prefix)
        {
            XmlNode x = w.SelectSingleNode(prefix+"_x");
            float _x = 0;
            float.TryParse(x.InnerText, out _x);
            XmlNode y = w.SelectSingleNode(prefix+"_y");
            float _y = 0;
            float.TryParse(y.InnerText, out _y);
            XmlNode z = w.SelectSingleNode(prefix+"_z");
            float _z = 0;
            float.TryParse(z.InnerText, out _z);
            return new Vector3(_x, _y, _z);
        }

        private void XmlToActions(XmlDocument doc, string nodeName, ref Weapen _w)
        {
            foreach (XmlNode a in doc.DocumentElement.SelectNodes("//"+nodeName))
            {
//                XmlNode weapenId = w.SelectSingleNode("weapenId");
//                _w.weapenId = weapenId.InnerText;
//
//                XmlNode parryMultiplier = w.SelectSingleNode("parryMultiplier");
//                float.TryParse(parryMultiplier.InnerText, out _w.parryMultiplier);


                Action _a = new Action();

                XmlNode actionInput = a.SelectSingleNode("ActionInput");
                _a.input = (ActionInput)Enum.Parse(typeof(ActionInput),actionInput.InnerText);

                XmlNode actionType = a.SelectSingleNode("ActionType");
                _a.type = (ActionType)Enum.Parse(typeof(ActionType), actionType.InnerText);

                XmlNode targetAnim = a.SelectSingleNode("targetAnim");
                _a.targetAnim = targetAnim.InnerText;

                XmlNode damageAnim = a.SelectSingleNode("damageAnim");
                _a.damageAnim = damageAnim.InnerText;

                XmlNode animSpeed = a.SelectSingleNode("animSpeed");
                float.TryParse(animSpeed.InnerText, out _a.animSpeed);

                XmlNode mirror = a.SelectSingleNode("mirror");
                _a.mirror = (mirror.InnerText == "True");

                XmlNode canBenParried = a.SelectSingleNode("canBenParried");
                _a.canBenParried = (canBenParried.InnerText == "True");

                XmlNode changeSpeed = a.SelectSingleNode("changeSpeed");
                _a.changeSpeed = (changeSpeed.InnerText == "True");

                XmlNode canParry = a.SelectSingleNode("canParry");
                _a.canParry = (canParry.InnerText == "True");

                XmlNode canBackstab = a.SelectSingleNode("canBackstab");
                _a.canBackstab = (canBackstab.InnerText == "True");

                XmlNode overrideDamageAnim = a.SelectSingleNode("overrideDamageAnim");
                _a.overrideDamageAnim = (overrideDamageAnim.InnerText == "True");

                _a.weapenStats = new WeapenStats();
                XmlNode physical = a.SelectSingleNode("physical");
                int.TryParse(physical.InnerText, out _a.weapenStats.physical);
                XmlNode strike = a.SelectSingleNode("strike");
                int.TryParse(strike.InnerText, out _a.weapenStats.strike);
                XmlNode slash = a.SelectSingleNode("slash");
                int.TryParse(slash.InnerText, out _a.weapenStats.slash);
                XmlNode thrust = a.SelectSingleNode("thrust");
                int.TryParse(thrust.InnerText, out _a.weapenStats.thrust);
                XmlNode magic = a.SelectSingleNode("magic");
                int.TryParse(magic.InnerText, out _a.weapenStats.magic);
                XmlNode fire = a.SelectSingleNode("fire");
                int.TryParse(fire.InnerText, out _a.weapenStats.fire);
                XmlNode lighting = a.SelectSingleNode("lighting");
                int.TryParse(lighting.InnerText, out _a.weapenStats.lighting);
                XmlNode dark = a.SelectSingleNode("dark");
                int.TryParse(dark.InnerText, out _a.weapenStats.dark);

                if (nodeName == "actions")
                {
                    _w.actions.Add(_a);
                }
                else
                {
                    _w.two_handenActions.Add(_a);
                }
            }
        }
    }

}

