﻿using System.Linq;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuValidation.Tests
{
    [TestFixture]
    public class IValidatedRuleSourceTester
    {
        private Validator theValidator;

        [SetUp]
        public void SetUp()
        {
            theValidator = new Validator(new TypeResolver(), new ValidationQuery(new IValidationSource[0]), MockRepository.GenerateMock<IServiceLocator>());
        }

        [Test]
        public void validator_should_call_through_to_validate_method()
        {
            theValidator.Validate(new ValidatedClass { IsValid = true }).IsValid().ShouldBeTrue();
            theValidator.Validate(new ValidatedClass
            {
                IsValid = false
            }).MessagesFor<ValidatedClass>(x => x.Name)
                .Single().StringToken.ShouldEqual(ValidationKeys.REQUIRED);
        }
    }

    public class ValidatedClass : IValidated
    {
        public void Validate(ValidationContext context)
        {
            if (!IsValid)
            {
                context.Notification.RegisterMessage<ValidatedClass>(x => x.Name, ValidationKeys.REQUIRED);
            }
        }

        public bool IsValid { get; set; }
        public string Name { get; set; }
    }
}