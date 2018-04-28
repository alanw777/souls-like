using System.IO;
using UnityEngine;

namespace AW
{
    public static class StaticStrings
    {
        //inputs
        public static string Vertical = "Vertical";
        public static string Horizontal = "Horizontal";
        public static string B = "B";
        public static string A = "A";
        public static string Y = "Y";
        public static string X = "X";
        public static string RT = "RT";
        public static string LT = "LT";
        public static string LB = "LB";
        public static string RB = "RB";
        public static string L = "L";
        public static string Pad_X = "Pad_X";
        public static string Pad_Y = "Pad_Y";

        //animator
        public static string mirror = "mirror";
        public static string changeWeapen = "changeWeapen";
        public static string onGround = "onGround";
        public static string interacting = "interacting";
        public static string blocking = "blocking";
        public static string isLeft = "isLeft";
        public static string canMove = "canMove";
        public static string lockon = "lockon";
        public static string animSpeed = "animSpeed";
        public static string parry_attack = "parry_attack";
        public static string vertical = "vertical";
        public static string horizontal = "horizontal";
        public static string Rolls = "Rolls";
        public static string run = "run";
        public static string attack_interrupt = "attack_interrupt";
        public static string parry_recieved = "parry_recieved";
        public static string getting_backstabbed = "getting_backstabbed";
        public static string damage1 = "damage_1";
        public static string damage2 = "damage_2";
        public static string damage3 = "damage_3";
        public static string emptyBoth = "Empty Both";
        public static string emptyLeft = "Empty Left";
        public static string emptyRight = "Empty Right";

        //data
        public static string itemFolder = "/Items/";
        

        public static string SaveLocation()
        {
            string r = Application.streamingAssetsPath;
            if (!Directory.Exists(r))
            {
                Directory.CreateDirectory(r);
            }
            return r;
        }




    }
}

