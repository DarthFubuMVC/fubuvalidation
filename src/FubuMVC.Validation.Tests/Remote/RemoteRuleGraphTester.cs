using FubuCore.Reflection;
using FubuMVC.Validation.Remote;
using FubuTestingSupport;
using FubuValidation.Fields;
using NUnit.Framework;

namespace FubuMVC.Validation.Tests.Remote
{
    [TestFixture]
    public class RemoteRuleGraphTester
    {
        private RemoteRuleGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            theGraph = new RemoteRuleGraph();
        }

        [Test]
        public void registers_rules()
        {
            var a1 = ReflectionHelper.GetAccessor<RuleGraphModel>(x => x.FirstName);
            var a2 = ReflectionHelper.GetAccessor<RuleGraphModel>(x => x.LastName);
            
            var r1 = new RequiredFieldRule();
            var r2 = new MinimumLengthRule(5);

            theGraph.RegisterRule(a1, r1);
            theGraph.RegisterRule(a1, r2);

            theGraph.RegisterRule(a2, r1);

            theGraph.RulesFor(a1).ShouldHaveTheSameElementsAs(RemoteFieldRule.For(a1, r1), RemoteFieldRule.For(a1, r2));
            theGraph.RulesFor(a2).ShouldHaveTheSameElementsAs(RemoteFieldRule.For(a2, r1));
        }

        [Test]
        public void finds_the_rule_by_the_hash()
        {
            var a1 = ReflectionHelper.GetAccessor<RuleGraphModel>(x => x.FirstName);
            var r1 = new RequiredFieldRule();

            theGraph.RegisterRule(a1, r1);

            var remote = RemoteFieldRule.For(a1, r1);

            theGraph.RuleFor(remote.ToHash()).ShouldEqual(remote);
        }

        public class RuleGraphModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    }
}