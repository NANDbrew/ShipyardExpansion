using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SE_Bridge
{
    public class ClothColToggler : MonoBehaviour
    {
        float radius;
        CapsuleCollider col;

        public void Awake()
        {
            col = GetComponent<CapsuleCollider>();
            radius = col.radius;
        }
        public void OnEnable()
        {
            col.radius = radius;
        }
        public void OnDisable()
        {
            col.radius = 0;
        }
    }
}
