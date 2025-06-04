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
            Debug.Log("attempting to load data for " + refs.name);
            if (!GameState.modData.ContainsKey(boat))
            {
                Debug.Log("SE boat sails: modData does not contain config for " + refs.name);
                return;
            }
            if (Plugin.skipSailData.Value) return;

            string slug = GameState.modData[boat];
            //Debug.Log("loading data: " + slug);
            string[] masts = slug.Split(new char[] { ')' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string mast in masts)
            {
                //Debug.Log($"{mast}");
                string[] foo = mast.Split('(');

                int mastIndex = Convert.ToInt32(foo[0]);
                if (mastIndex >= refs.masts.Length) break;

                #region convert from SE v4.5
                if (Plugin.convertSave.Value && VersionManager.saveVersion2[1] < 5 && mastIndex > 30 && mastIndex < 50)
                {
                    Debug.Log("mast index? " + mastIndex);
                    mastIndex += 20;
                    Debug.Log("mast index! " + mastIndex);
                }
                #endregion

                Mast mastComp = refs.masts[mastIndex];
                if (mastComp == null) continue;

                string[] sails = foo[1].Split(new char[] { ']' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < mastComp.sails.Count; i++)
                {
                    GameObject installedSail = mastComp.sails[i];
                    string[] sailInfo = sails[i].Split(',');
                    //if (installedSail.GetComponent<Sail>().prefabIndex == Convert.ToInt32(sailInfo[0], CultureInfo.InvariantCulture))
                    //{
                        SailScaler component = installedSail.GetComponent<SailScaler>();
                        if (component == null)
                        {
                            Debug.Log("No sail scaler component found");
                            continue;
                        }
                        component.SetScaleAbs(Convert.ToSingle(sailInfo[1], CultureInfo.InvariantCulture), Convert.ToSingle(sailInfo[2], CultureInfo.InvariantCulture));
                        if (sailInfo.Length >= 4)
                        {
                            component.SetAngle(Convert.ToSingle(sailInfo[3], CultureInfo.InvariantCulture));
                            //Debug.Log("sail Angle = " + sailInfo[3]);
                            if (sailInfo.Length >= 5 && Boolean.Parse(sailInfo[4]))
                            {
                                component.FlipJib(true);
                            }
                        }
                    //}
                }

            }
        }

        public static void SaveSailConfig(BoatRefs refs)
        {
            if (Plugin.skipSailData.Value) return;

            Debug.Log("attempting to save data");
            string boat = "SEboatSails." + refs.GetComponent<SaveableObject>().sceneIndex.ToString();
            string text = "";
            Mast[] masts = refs.masts;
            foreach (Mast mast in masts)
            {
                if (mast == null || mast.sails.Count == 0)
                {
                    continue;
                }
                text += mast.orderIndex.ToString() + "(";
                foreach (GameObject sail in mast.sails)
                {
                    SailScaler component = sail.GetComponent<SailScaler>();
                    if (component == null)
                    {
                        Debug.LogError("No sail scaler component found! Aborting data for this boat");
                        return;
                    }

                    Sail component2 = sail.GetComponent<Sail>();
                    text += component2.prefabIndex.ToString(CultureInfo.InvariantCulture) + ",";
                    text += component.Scale.x.ToString(CultureInfo.InvariantCulture) + ",";
                    text += component.Scale.y.ToString(CultureInfo.InvariantCulture) + ",";
                    text += component.Angle.ToString(CultureInfo.InvariantCulture);
                    if (component.flippable) text += "," + component.Flipped.ToString(CultureInfo.InvariantCulture);
                    text += "]";
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
