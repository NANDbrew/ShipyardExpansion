using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ShipyardExpansion
{
    public static class SailDataManager
    {
        public static void LoadSailConfig(BoatRefs refs)
        {
            string boat = "SEboatSails." + refs.GetComponent<SaveableObject>().sceneIndex;
            Debug.Log("attempting to load data");
            if (!GameState.modData.ContainsKey(boat))
            {
                Debug.Log("modData does not contain config");
                return;
            }
            string slug = GameState.modData[boat];
            //Debug.Log("loading data: " + slug);
            string[] masts = slug.Split(new char[] { ')' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string mast in masts)
            {
                //Debug.Log($"{mast}");
                string[] foo = mast.Split('(');
                int mastIndex = Convert.ToInt32(foo[0]);
                string[] sails = foo[1].Split(new char[] { ']' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < refs.masts[mastIndex].sails.Count; i++)
                {
                    GameObject installedSail = refs.masts[mastIndex].sails[i];
                    string[] sailInfo = sails[i].Split(',');
                    if (installedSail.GetComponent<Sail>().prefabIndex == Convert.ToInt32(sailInfo[0], CultureInfo.InvariantCulture))
                    {
                        SailScaler component = installedSail.GetComponent<SailScaler>();
                        component.SetScaleAbs(Convert.ToSingle(sailInfo[1], CultureInfo.InvariantCulture), Convert.ToSingle(sailInfo[2], CultureInfo.InvariantCulture));
                        if (sailInfo.Length >= 4)
                        {
                            component.SetAngle(Convert.ToSingle(sailInfo[3], CultureInfo.InvariantCulture));
                            //Debug.Log("sail angle = " + sailInfo[3]);
                        }
                    }
                }

            }
        }

        public static void SaveSailConfig(BoatRefs refs)
        {
            Debug.Log("attempting to save data");
            string boat = "SEboatSails." + refs.GetComponent<SaveableObject>().sceneIndex.ToString();
            string text = "";
            Mast[] masts = refs.masts;
            foreach (Mast mast in masts)
            {
                if (!mast || !mast.gameObject.activeInHierarchy || mast.sails.Count < 1)
                {
                    continue;
                }
                text += mast.orderIndex.ToString() + "(";
                foreach (GameObject sail in mast.sails)
                {
                    SailScaler component = sail.GetComponent<SailScaler>();
                    Sail component2 = sail.GetComponent<Sail>();
                    text += component2.prefabIndex.ToString(CultureInfo.InvariantCulture) + ",";

                    //text += mast.orderIndex.ToString() + ",";

                    text += component.scale.x.ToString(CultureInfo.InvariantCulture) + ",";
                    text += component.scale.y.ToString(CultureInfo.InvariantCulture) + ",";
                    text += component.angle.ToString(CultureInfo.InvariantCulture) + "]";
                }
                text += ")";
            }
            if (GameState.modData.ContainsKey(boat))
            {
                GameState.modData[boat] = text;
            }
            else
            {
                GameState.modData.Add(boat, text);
            }
            //Debug.Log("saving data: " + text);

        }
    }

}
