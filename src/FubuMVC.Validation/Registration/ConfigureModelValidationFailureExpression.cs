using System;
using System.Collections.Generic;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Validation.Registration
{
    public class ConfigureModelValidationFailureExpression
    {
        private readonly Func<Type, bool> _predicate;
        private readonly IList<ObjectDef> _policies;

        public ConfigureModelValidationFailureExpression(Func<Type, bool> predicate, IList<ObjectDef> policies)
        {
            _predicate = predicate;
            _policies = policies;
        }

        public void RedirectTo<T>()
            where T : class, new()
        {
            RedirectTo(new T());
        }

        public void RedirectTo(object target)
        {
            buildPolicy(FubuContinuation.RedirectTo(target));
        }

        public void TransferTo<T>()
            where T : class, new()
        {
            TransferTo(new T());
        }

        public void TransferTo(object target)
        {
            buildPolicy(FubuContinuation.TransferTo(target));
        }

        private void buildPolicy(FubuContinuation continuation)
        {
            var policy = new ObjectDef { Type = typeof(FubuContinuationFailurePolicy) };
            policy.DependencyByValue(typeof (Func<Type, bool>), _predicate);
            policy.DependencyByValue(typeof(FubuContinuation), continuation);

            _policies.Add(policy);
        }
    }
}