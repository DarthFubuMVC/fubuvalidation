﻿using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.UI.Elements;
using FubuValidation.Fields;

namespace FubuMVC.Validation.UI
{
    public interface IFieldValidationModifier
    {
        void ModifyFor(IFieldValidationRule rule, ElementRequest request);
    }

    public class FieldValidationModifier : IFieldValidationModifier
    {
        private readonly IEnumerable<IValidationAnnotationStrategy> _strategies;

        public FieldValidationModifier(IEnumerable<IValidationAnnotationStrategy> strategies)
        {
            _strategies = strategies;
        }

        public void ModifyFor(IFieldValidationRule rule, ElementRequest request)
        {
            _strategies
                .Where(x => x.Matches(rule))
                .Each(x => x.Modify(request, rule));
        }
    }
}