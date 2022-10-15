using System;
using UnityEngine;

namespace CucuTools.Injects
{
    /// <summary>
    /// Attribute for injection field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CucuArgAttribute : PropertyAttribute
    {
    }
}