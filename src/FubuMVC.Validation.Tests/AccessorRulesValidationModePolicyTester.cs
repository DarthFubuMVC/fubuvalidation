﻿using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Validation.Tests
{
	[TestFixture]
	public class AccessorRulesValidationModePolicyTester
	{
		private IServiceLocator services()
		{
			var rules = new AccessorRules();

			var overrides = new AccessorRulesValidationOverrides();
			overrides.As<IAccessorRulesRegistration>().AddRules(rules);

			var services = new InMemoryServiceLocator();
			services.Add(rules);

			return services;
		}

		private ValidationMode modeFor(Expression<Func<AccessorRulesValidationModeTarget, object>> expression)
		{
			var accessor = SingleProperty.Build(expression);
			return new AccessorRulesValidationModePolicy().DetermineMode(services(), accessor);
		}

		private bool matches(Expression<Func<AccessorRulesValidationModeTarget, object>> expression)
		{
			var accessor = SingleProperty.Build(expression);
			return new AccessorRulesValidationModePolicy().Matches(services(), accessor);
		}

		[Test]
		public void matches_requests_for_properties_with_live_validation()
		{
			matches(x => x.Live).ShouldBeTrue();
		}

		[Test]
		public void matches_requests_for_properties_with_triggered_validation()
		{
			matches(x => x.Triggered).ShouldBeTrue();
		}

		[Test]
		public void matches_requests_for_properties_with_no_validation_mode()
		{
			matches(x => x.Ignored).ShouldBeFalse();
		}

		[Test]
		public void mode_is_determined_from_accessor_rules_live()
		{
			modeFor(x => x.Live).ShouldEqual(ValidationMode.Live);
		}


		[Test]
		public void mode_is_determined_from_accessor_rules_triggered()
		{
			modeFor(x => x.Triggered).ShouldEqual(ValidationMode.Triggered);
		}

		public class AccessorRulesValidationModeTarget
		{
			public string Live { get; set; }
			public string Triggered { get; set; }
			public string Ignored { get; set; }
		}

		public class AccessorRulesValidationOverrides : OverridesFor<AccessorRulesValidationModeTarget>
		{
			public AccessorRulesValidationOverrides()
			{
				Property(x => x.Live).LiveValidation();
				Property(x => x.Triggered).TriggeredValidation();
			}
		}
	}
}