﻿using System;

namespace FubuValidation.Fields
{
    /// <summary>
    /// Marks an <see cref="IFieldValidationRule"/> to be ignored for client-side localization attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class IgnoreClientLocalizationAttribute : Attribute
    {
    }
}