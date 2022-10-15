using System;
using UnityEngine;

namespace CucuTools.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CucuReadOnlyAttribute : PropertyAttribute
    {
    }
}