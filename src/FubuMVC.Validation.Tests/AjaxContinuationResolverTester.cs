using FubuMVC.Core.Ajax;
using FubuTestingSupport;
using FubuValidation;
using NUnit.Framework;

namespace FubuMVC.Validation.Tests
{
    [TestFixture]
    public class AjaxContinuationResolverTester
    {
        private AjaxContinuationResolver theResolver;
        private RecordingAjaxContinuationModifier theModifier;
        private Notification theNotification;

        [SetUp]
        public void SetUp()
        {
            theModifier = new RecordingAjaxContinuationModifier();
            theResolver = new AjaxContinuationResolver(new[] { theModifier });
            theNotification = new Notification();

            theResolver.Resolve(theNotification);
        }

        [Test]
        public void modifies_the_continuation()
        {
            theModifier.Continuation.ShouldNotBeNull();
            theModifier.Notification.ShouldBeTheSameAs(theNotification);
        }

        public class RecordingAjaxContinuationModifier : IAjaxContinuationModifier
        {
            public AjaxContinuation Continuation { get; private set; }
            public Notification Notification { get; private set; }

            public void Modify(AjaxContinuation continuation, Notification notification)
            {
                Continuation = continuation;
                Notification = notification;
            }
        }
    }
}