﻿using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuTestingSupport;
using FubuValidation.Fields;
using NUnit.Framework;

namespace FubuMVC.Validation.Tests
{
    [TestFixture]
    public class AccessorRulesFieldSourceTester
    {
        private AccessorRules theRules;
        private AccessorRulesFieldSource theSource;
        private Accessor a1;
        private Accessor a2;
        private Accessor a3;

        [SetUp]
        public void SetUp()
        {
            theRules = new AccessorRules();
            a1 = accessor(x => x.Prop1);
            a2 = accessor(x => x.Prop2);
            a3 = accessor(x => x.Prop3);

            theRules.Add(a1, new RequiredFieldRule());
            theRules.Add(a3, new EmailFieldRule());
            theRules.Add(a3, new GreaterThanZeroRule());

            theSource = new AccessorRulesFieldSource(theRules);
        }

        private Accessor accessor(Expression<Func<AccessorRulesTarget, object>> expression)
        {
            return ReflectionHelper.GetAccessor(expression);
        }

        [Test]
        public void gets_the_rules_by_accessor()
        {
            theSource.RulesFor(a1.InnerProperty).ShouldHaveTheSameElementsAs(new RequiredFieldRule());
            theSource.RulesFor(a2.InnerProperty).ShouldHaveCount(0);
            theSource.RulesFor(a3.InnerProperty).ShouldHaveTheSameElementsAs(new EmailFieldRule(), new GreaterThanZeroRule());
        }


        public class AccessorRulesTarget
        {
            public string Prop1 { get; set; }
            public string Prop2 { get; set; }
            public string Prop3 { get; set; }
        }
    }
}