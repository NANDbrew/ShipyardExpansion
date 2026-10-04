
using SE_Bridge;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipyardExpansion.Scripts
{
    public class ShipyardCutawayButton : GoPointerButton
    {
        private static TextMesh text;
        private static bool state = true;

        private static List<Renderer> renderers;
        private static Type[] types = new Type[] { typeof(HullPlayerCollider), typeof(GPButtonBoatPushCol), typeof(CleanableObject), typeof(SE_Cladding) };
        public static ShipyardCutawayButton instance;

        private void Awake()
        {
            text = transform.GetChild(0).GetComponent<TextMesh>();
            instance = this;
        }

        public override void OnActivate()
        {
            SetState(!state);

        }
        public static void SetState(bool newState)
        {
            state = newState;
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = newState;
            }
        }

        public static void FindRenderers(GameObject boat)
        {
            renderers = new List<Renderer>();
            foreach (var t in types)
            {
                var children = boat.GetComponentsInChildren(t);
                foreach (var child in children)
                {
                    var rend = child.GetComponent<Renderer>();
                    if (rend != null)
                    { 
                        renderers.Add(rend); 
                    }
                    
                    if (t == typeof(SE_Cladding))
                    {
                        foreach (var r in child.GetComponentsInChildren<Renderer>())
                        {
                            renderers.Add(r);
                        }
                    }

                }
            }

            // crazy chicanery for Jong
            if (boat.GetComponent<SaveableObject>().sceneIndex == 70)
            {
                var hull = boat.transform.Find("junk large (3)/hull");
                if (hull)
                {
                    renderers.Add(hull.GetComponent<Renderer>());
                }
            }
        }

    }
}
