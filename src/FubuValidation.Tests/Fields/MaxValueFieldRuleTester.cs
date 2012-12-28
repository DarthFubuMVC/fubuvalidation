﻿using System.Linq;
using FubuTestingSupport;
using FubuValidation.Fields;
using NUnit.Framework;

namespace FubuValidation.Tests.Fields
{
    [TestFixture]
    public class MaxValueFieldRuleTester
    {
        private MaxValueTarget theTarget;

        [SetUp]
        public void SetUp()
        {
            theTarget = new MaxValueTarget();
        }

        private Notification theNotification
        {
            get
            {
                var scenario = ValidationScenario<MaxValueTarget>.For(x =>
                {
                    x.Model = theTarget;
                    x.FieldRule(new MaxValueFieldRule(5));
                });

                return scenario.Notification;
            }
        }

        [Test]
        public void registers_a_message_when_the_value_is_greater_than_max()
        {
            theTarget.Value = 6;
            var messages = theNotification.MessagesFor<MaxValueTarget>(x => x.Value);
            messages.Single().StringToken.ShouldEqual(ValidationKeys.MaxValue);
        }

        [Test]
        public void no_message_when_equal_to_max()
        {
            theTarget.Value = 5;
            theNotification.MessagesFor<MaxValueTarget>(x => x.Value).Any().ShouldBeFalse();
        }

        [Test]
        public void no_message_when_less_than_max()
        {
            theTarget.Value = 1;
            theNotification.MessagesFor<MaxValueTarget>(x => x.Value).Any().ShouldBeFalse();
        }

        [Test]
        public void renders_the_substitutions()
        {
            theTarget.Value = 7;
            var message = theNotification.MessagesFor<MaxValueTarget>(x => x.Value).Single();
            message.GetMessage().ShouldEqual("Value must be less than or equal to 5");
        }


        public class MaxValueTarget
        {
            public int Value { get; set; }
        }
    }
}