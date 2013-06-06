﻿using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuLocalization;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuValidation.Tests
{
	public class FieldEqualityTarget
	{
		public string Value1 { get; set; }
		public string Value2 { get; set; }

		public string Other { get; set; }

		public int DifferentType { get; set; }
	}

	[TestFixture]
	public class FieldEqualityRuleTester
	{
		private FieldEqualityTarget theTarget;
		private FieldEqualityRule theRule;

		[SetUp]
		public void SetUp()
		{
			theTarget = new FieldEqualityTarget();
			theRule = FieldEqualityRule.For<FieldEqualityTarget>(t => t.Value1, t => t.Value2);
		}

		private Notification theNotification
		{
			get
			{
				var scenario = ValidationScenario<FieldEqualityTarget>.For(x =>
				{
					x.Model = theTarget;
					x.Rule(theRule);
				});

				return scenario.Notification;
			}
		}

		[Test]
		public void throws_when_property_types_do_not_match()
		{
			Exception<InvalidOperationException>
				.ShouldBeThrownBy(() => FieldEqualityRule.For<FieldEqualityTarget>(x => x.Value1, x => x.DifferentType));
		}

		[Test]
		public void throws_when_targeting_messages_to_an_unknown_accessor()
		{
			var rule = FieldEqualityRule.For<FieldEqualityTarget>(x => x.Value1, x => x.Value2);
			var accessor = SingleProperty.Build<FieldEqualityTarget>(x => x.Other);

			Exception<ArgumentOutOfRangeException>
				.ShouldBeThrownBy(() => rule.ReportMessagesFor(accessor));
		}

		[Test]
		public void uses_the_default_token()
		{
			FieldEqualityRule.For<FieldEqualityTarget>(t => t.Value1, t => t.Value2)
			                 .Token.ShouldEqual(ValidationKeys.FieldEquality);
		}

		[Test]
		public void registers_a_message_for_property1_when_property1_is_specified()
		{
			theTarget.Value1 = "hello//";

			theRule.ReportMessagesFor(theRule.Property1);

			var messages = theNotification.MessagesFor<FieldEqualityTarget>(x => x.Value1);
			messages.Single().StringToken.ShouldEqual(ValidationKeys.FieldEquality);

			theNotification.MessagesFor<FieldEqualityTarget>(x => x.Value2).ShouldHaveCount(0);
		}

		[Test]
		public void registers_a_message_for_property2_when_property2_is_specified()
		{
			theTarget.Value1 = "hello";

			theRule.ReportMessagesFor(theRule.Property2);

			var messages = theNotification.MessagesFor<FieldEqualityTarget>(x => x.Value2);
			messages.Single().StringToken.ShouldEqual(ValidationKeys.FieldEquality);

			theNotification.MessagesFor<FieldEqualityTarget>(x => x.Value1).ShouldHaveCount(0);
		}

		[Test]
		public void registers_a_message_for_both_proeprties_when_both_are_specified()
		{
			theTarget.Value1 = "hello";

			theRule.ReportMessagesFor(theRule.Property1);
			theRule.ReportMessagesFor(theRule.Property2);

			theNotification
				.MessagesFor<FieldEqualityTarget>(x => x.Value1)
				.Single()
				.StringToken
				.ShouldEqual(ValidationKeys.FieldEquality);

			theNotification
				.MessagesFor<FieldEqualityTarget>(x => x.Value2)
				.Single()
				.StringToken
				.ShouldEqual(ValidationKeys.FieldEquality);
		}

		[Test]
		public void no_message_when_the_properties_match()
		{
			theTarget.Value1 = "hello";
			theTarget.Value2 = "hello";

			theRule.ReportMessagesFor(theRule.Property1);
			theRule.ReportMessagesFor(theRule.Property2);

			theNotification.MessagesFor<FieldEqualityTarget>(x => x.Value1).Any().ShouldBeFalse();
		}

		[Test]
		public void renders_the_substitutions()
		{
			theTarget.Value1 = "hello";
			theTarget.Value2 = "asdf";

			theRule.ReportMessagesFor(theRule.Property1);

			var message = theNotification.MessagesFor<FieldEqualityTarget>(x => x.Value1).Single();
			message.GetMessage().ShouldEqual("Value1 must equal Value2");
		}

		[Test]
		public void no_message_when_the_value_is_empty()
		{
			theTarget.Value1 = "";

			theRule.ReportMessagesFor(theRule.Property1);
			theRule.ReportMessagesFor(theRule.Property2);

			theNotification.MessagesFor<FieldEqualityTarget>(x => x.Value1).Any().ShouldBeFalse();
		}	

		[Test]
		public void builds_the_localized_properties()
		{
			var values = theRule.ToValues();
			
			var prop1 = values.Child("property1");
			prop1.Get<string>("field").ShouldEqual(theRule.Property1.Name);
			prop1.Get<string>("label").ShouldEqual(LocalizationManager.GetHeader(theRule.Property1.InnerProperty));

			var prop2 = values.Child("property2");
			prop2.Get<string>("field").ShouldEqual(theRule.Property2.Name);
			prop2.Get<string>("label").ShouldEqual(LocalizationManager.GetHeader(theRule.Property2.InnerProperty));
		}

		[Test]
		public void builds_the_token()
		{
			var values = theRule.ToValues();

			values.Get<string>("message").ShouldEqual(theRule.Token.ToString());
		}

		[Test]
		public void builds_the_targets()
		{
			theRule.ReportMessagesFor(theRule.Property1);
			theRule.ReportMessagesFor(theRule.Property2);

			var values = theRule.ToValues();

			values.Get<IEnumerable<string>>("targets").ShouldHaveTheSameElementsAs(theRule.Property1.Name, theRule.Property2.Name);
		}
	}
}