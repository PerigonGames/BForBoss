using System;
using UnityEngine;

namespace BForBoss
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class Resolve : PropertyAttribute
    {
        public Resolve()
        {
        }
    }
}
