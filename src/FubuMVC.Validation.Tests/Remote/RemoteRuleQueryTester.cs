﻿using FubuMVC.Validation.Remote;
using FubuTestingSupport;
using FubuValidation.Fields;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Validation.Tests.Remote
{
    [TestFixture]
    public class RemoteRuleQueryTester
    {
        private RequiredFieldRule theRule;
        private IRemoteRuleFilter f1;
        private IRemoteRuleFilter f2;
        private RemoteRuleQuery theQuery;


        [SetUp]
        public void SetUp()
        {
            theRule = new RequiredFieldRule();
            f1 = MockRepository.GenerateStub<IRemoteRuleFilter>();
            f2 = MockRepository.GenerateStub<IRemoteRuleFilter>();

            theQuery = new RemoteRuleQuery(new[] { f1, f2 });
        }


        [Test]
        public void matches_if_any_filters_match()
        {
            f1.Stub(x => x.Matches(theRule)).Return(true);
            f2.Stub(x => x.Matches(theRule)).Return(false);

            theQuery.IsRemote(theRule).ShouldBeTrue();
        }

        [Test]
        public void does_not_match_no_filters_match()
        {
            f1.Stub(x => x.Matches(theRule)).Return(false);
            f2.Stub(x => x.Matches(theRule)).Return(false);

            theQuery.IsRemote(theRule).ShouldBeFalse();
        }
    }
}