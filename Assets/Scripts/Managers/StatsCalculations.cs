using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AW
{
    public static class StatsCalculations
    {
        public static int CalculateBaseDamage(WeapenStats w, CharacterStats st)
        {
            int physical = w.physical - st.physical;
            int strike = w.strike - st.vs_strike;
            int slash = w.slash - st.vs_slash;
            int thrust = w.thrust - st.vs_thrust;
            int sum = physical + strike + slash + thrust;

            int magic = w.magic - st.magic;
            int fire = w.fire - st.fire;
            int dark = w.dark - st.dark;
            int lighting = w.lighting - st.lighting;
            sum += magic + fire + dark + lighting;

            if (sum <= 0)
                sum = 1;

            return sum;
        }
    }
}
