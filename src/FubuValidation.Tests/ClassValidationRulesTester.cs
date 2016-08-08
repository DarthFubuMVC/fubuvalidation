using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuLocalization;
using FubuTestingSupport;
using FubuValidation.Fields;
using NUnit.Framework;

namespace FubuValidation.Tests
{
    [TestFixture]
    public class ClassValidationRulesTester
    {
        private ClassValidationRules<ClassValidationRulesTarget> theRules;

        [SetUp]
        public void SetUp()
        {
            theRules = new ClassValidationRules<ClassValidationRulesTarget>();
        }

        private IEnumerable<IFieldValidationRule> rulesFor(Expression<Func<ClassValidationRulesTarget, object>> expression)
        {
            var registry = new FieldRulesRegistry(new IFieldValidationSource[0], new TypeDescriptorCache());
            var graph = ValidationGraph.For(registry);

            graph.Import(theRules);

            return registry.RulesFor(typeof (ClassValidationRulesTarget)).RulesFor(expression.ToAccessor());
        }

        private IEnumerable<IValidationRule> classRules()
        {
            var graph = ValidationGraph.For(theRules);

            return graph.Sources.SelectMany(source => source.RulesFor(typeof(ClassValidationRulesTarget)));
        }

        [Test]
        public void no_rules()
        {
            rulesFor(x => x.Name).Any().ShouldBeFalse();
            rulesFor(x => x.Address1).Any().ShouldBeFalse();
        }

        [Test]
        public void register_a_single_required_rule()
        {
            theRules.Require(x => x.Name);

            rulesFor(x => x.Name).Single().ShouldBeOfType<RequiredFieldRule>();
        }

        [Test]
        public void register_a_single_required_rule_with_a_conditional()
        {
            theRules.Require(x => x.Province).If(x => x.Country == "Canada");

            var conditionalRule = rulesFor(x => x.Province).Single().ShouldBeOfType<ConditionalFieldRule<ClassValidationRulesTarget>>();

            conditionalRule.Inner.ShouldBeOfType<RequiredFieldRule>();



            conditionalRule.Condition.Matches(null, ValidationContext.For(new ClassValidationRulesTarget() { Country = "United States" })).ShouldBeFalse();
            conditionalRule.Condition.Matches(null, ValidationContext.For(new ClassValidationRulesTarget() { Country = "Canada" })).ShouldBeTrue();
        }

        [Test]
        public void register_custom_rule()
        {
            theRules.Property(x => x.Name)
                .Custom(x => x.Age + 1 > 12);

            rulesFor(x => x.Name).Single()
                .ShouldBeOfType<CustomFieldValidationRule<ClassValidationRulesTarget>>();
        }

        [Test]
        public void register_custom_rule_with_token()
        {
            var token = StringToken.FromKeyString("Test");
            theRules.Property(x => x.Name)
                .Custom(x => x.Age + 1 > 12, token);

            rulesFor(x => x.Name).Single()
                .ShouldBeOfType<CustomFieldValidationRule<ClassValidationRulesTarget>>()
                .Token.ShouldEqual(token);
        }

        [Test]
        public void register_multiple_required_rules()
        {
            theRules.Require(x => x.Name, x => x.Address1);

            rulesFor(x => x.Name).Single().ShouldBeOfType<RequiredFieldRule>();
            rulesFor(x => x.Address1).Single().ShouldBeOfType<RequiredFieldRule>();
        }

        [Test]
        public void register_maximum_length()
        {
            theRules.Property(x => x.Name).MaximumLength(19);
            rulesFor(x => x.Name).Single().ShouldBeOfType<MaximumLengthRule>()
                .Length.ShouldEqual(19);
        }

        [Test]
        public void register_maximum_length_conditionally()
        {
            var filter = FieldRuleCondition.For<ClassValidationRulesTarget>(x => x.Country == "Canada");
            theRules.Property(x => x.Name).MaximumLength(19).If(filter);
            rulesFor(x => x.Name).Single().ShouldBeOfType<ConditionalFieldRule<ClassValidationRulesTarget>>()
                .Inner
                .ShouldBeOfType<MaximumLengthRule>()
                .Length.ShouldEqual(19);
        }

        [Test]
        public void register_greater_than_zero()
        {
            theRules.Property(x => x.Age).GreaterThanZero();
            rulesFor(x => x.Age).Single().ShouldBeOfType<GreaterThanZeroRule>();
        }

        [Test]
        public void register_greater_or_equal_to_zero()
        {
            theRules.Property(x => x.Age).GreaterOrEqualToZero();
            rulesFor(x => x.Age).Single().ShouldBeOfType<GreaterOrEqualToZeroRule>();
        }

        [Test]
        public void register_required_for_a_single_property()
        {
            theRules.Property(x => x.Name).Required();
        }

        [Test]
        public void register_multiple_rules_for_a_single_property()
        {
            theRules.Property(x => x.Name).Required().MaximumLength(10);

            var nameRules = rulesFor(x => x.Name);
            nameRules.Count().ShouldEqual(2);
            nameRules.Any(x => x is RequiredFieldRule).ShouldBeTrue();
            nameRules.ShouldContain(new MaximumLengthRule(10));
        }

        [Test]
        public void register_class_level_rule_by_type()
        {
            theRules.Register<SimpleClassLevelRule>();

            var classLevelRules = classRules();
            classLevelRules.Any(x => x is SimpleClassLevelRule).ShouldBeTrue();
        }

        [Test]
        public void register_class_level_rule_by_instance()
        {
            theRules.Register(new ComplexClassLevelRule<ClassValidationRulesTarget>(x => x.Province, x => x.Country));

            var classLevelRules = classRules();
            classLevelRules.Any(x => x is ComplexClassLevelRule<ClassValidationRulesTarget>);
        }

		[Test]
		public void configure_a_field_equality_rule()
		{
			var myToken = StringToken.FromKeyString("MyKeys:MyToken", "Passwords must match");
			
			theRules
				.Property(x => x.Password)
				.Matches(x => x.ConfirmPassword)
				.ReportErrorsOn(x => x.ConfirmPassword)
				.UseToken(myToken);

			var rule = theRules.As<IValidationSource>().RulesFor(typeof(ClassValidationRulesTarget)).OfType<FieldEqualityRule>().Single();
			
			rule.Property1.ShouldEqual(SingleProperty.Build<ClassValidationRulesTarget>(x => x.Password));
			rule.Property2.ShouldEqual(SingleProperty.Build<ClassValidationRulesTarget>(x => x.ConfirmPassword));

			rule.Token.ShouldEqual(myToken);
		}
    }

    public class ClassValidationRulesTarget
    {
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Province { get; set;}
        public string Country { get; set; }

		public string Password { get; set; }
		public string ConfirmPassword { get; set; }

        public int Age { get; set; }
    }

    public class SimpleClassLevelRule : IValidationRule
    {
        public void Validate(ValidationContext context)
        {
            
        }
    }

    public class ComplexClassLevelRule<T> : IValidationRule where T : class
    {
        private readonly Accessor _firstAccessor;
        private readonly Accessor _secondAccessor;

        public ComplexClassLevelRule(Expression<Func<T, object>> firstField, Expression<Func<T, object>> secondField)
        {
            _firstAccessor = firstField.ToAccessor();
            _secondAccessor = secondField.ToAccessor();
        }


        public void Validate(ValidationContext context)
        {
           
        }
    }
}