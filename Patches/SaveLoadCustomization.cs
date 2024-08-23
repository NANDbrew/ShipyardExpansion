using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace ShipyardExpansion.Patches
{
    [HarmonyPatch(typeof(SaveableBoatCustomization))]
    internal class SaveablaBoatCustomizationPatches
    {
        [HarmonyPatch("GetData")]
        [HarmonyPostfix]
        public static void GetDataPatch(BoatRefs ___refs)
        {
            SaveLoader.SaveSailConfig(___refs);
        }
        [HarmonyPatch("LoadData")]
        [HarmonyPostfix]
        public static void LoadDataPatch(BoatRefs ___refs)
        {
            SaveLoader.LoadSailConfig(___refs);
        }
    }

    [HarmonyPatch(typeof(SaveLoadManager), "LoadModData")]
    internal class SaveLoadPatch00
    {

        public static void Postfix()
        {
            foreach(GameObject gameObject in GameObject.FindGameObjectsWithTag("Boat")) 
            {
                if (gameObject.GetComponent<BoatRefs>() != null)
                {
                    SaveLoader.LoadSailConfig(gameObject.GetComponent<BoatRefs>());
                }
            }
        }

    }

    public static class SaveLoader
    {
        public static void LoadSailConfig(BoatRefs refs)
        {
            string boat = "SEboatSails." + refs.GetComponent<SaveableObject>().sceneIndex;
            Debug.Log("attempting to load data");
            if (!GameState.modData.ContainsKey(boat))
            {
                Debug.LogError("modData does not contain config");
                return;
            }
            string slug = GameState.modData[boat];
            string[] masts = slug.Split(new char[] { ')' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string mast in masts)
            {
                Debug.Log($"{mast}");
                string[] foo = mast.Split('(');
                int mastIndex = Convert.ToInt32(foo[0]);
                string[] sails = foo[1].Split(new char[] { ']' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < refs.masts[mastIndex].sails.Count; i++)
                {
                    GameObject installedSail = refs.masts[mastIndex].sails[i];
                    string[] sailInfo = sails[i].Split(',');
                    if (installedSail.GetComponent<Sail>().prefabIndex == Convert.ToInt32(sailInfo[0]))
                    {
                        SailScaler component = installedSail.GetComponent<SailScaler>();
                        component.SetScaleAbs(Convert.ToSingle(sailInfo[1]), Convert.ToSingle(sailInfo[2]));
                        if (sailInfo.Length >= 4)
                        {
                            component.SetAngle(Convert.ToSingle(sailInfo[3]));
                            Debug.Log("sail angle = " + sailInfo[3]);
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
                    text += component2.prefabIndex.ToString() + ",";

                    //text += mast.orderIndex.ToString() + ",";

                    text += component.scale.x.ToString() + ",";
                    text += component.scale.y.ToString() + ",";
                    text += component.angle.ToString() + "]";
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
            Debug.Log("data = " + text);

        }
    }

}
