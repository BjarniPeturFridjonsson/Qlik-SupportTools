using System;
using Eir.Common.Common;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Eir.Common.Test.Common
{
    [TestFixture]
    public class SeededDateTimeProviderTest : TestBase
    {
        [Test]
        public void SeededTimeRetainsDifferenceToReferenceTime()
        {
            var currentReferenceTime = new DateTime(2016, 6, 16, 12, 40, 0);
            var referenceTimeProviderMock = new Mock<IDateTimeProvider>();
            referenceTimeProviderMock
                .Setup(x => x.Time())
                .Returns(() => currentReferenceTime);

            var seededTimeProvider = new SeededDateTimeProvider(referenceTimeProviderMock.Object);

            // seed the time provider with a time that is 23 seconds behind the current reference time
            var firstSeededTime = currentReferenceTime.Subtract(TimeSpan.FromSeconds(23));
            seededTimeProvider.SetTimeSeed(firstSeededTime);

            // move reference time forward by 40 seconds
            currentReferenceTime = currentReferenceTime.Add(TimeSpan.FromSeconds(40));
            var secondSeededTime = seededTimeProvider.Time();

            Assert.That(secondSeededTime - firstSeededTime, Is.EqualTo(TimeSpan.FromSeconds(40)));
            Assert.That(currentReferenceTime - secondSeededTime, Is.EqualTo(TimeSpan.FromSeconds(23)));
        }


        [Test]
        public void FreezeTimeReturnsSameTimeOnSubsequentCalls()
        {
            var currentReferenceTime = new DateTime(2016, 6, 16, 12, 40, 0);
            var referenceTimeProviderMock = new Mock<IDateTimeProvider>();
            referenceTimeProviderMock
                .Setup(x => x.Time())
                .Returns(() => currentReferenceTime);

            var seededTimeProvider = new SeededDateTimeProvider(referenceTimeProviderMock.Object);
            seededTimeProvider.FreezeTime();
            var firstTime = seededTimeProvider.Time();

            // fast-forward reference time one minute. This should not affect
            // the seeded time provider.
            currentReferenceTime = currentReferenceTime.AddMinutes(1);
            var secondTime = seededTimeProvider.Time();

            firstTime.ShouldBe(secondTime);
        }

        [Test]
        public void TimeIsResumedWhenSeededAfterFreeze()
        {
            var currentReferenceTime = new DateTime(2016, 6, 16, 12, 40, 0);
            var referenceTimeProviderMock = new Mock<IDateTimeProvider>();
            referenceTimeProviderMock
                .Setup(x => x.Time())
                .Returns(() => currentReferenceTime);

            var seededTimeProvider = new SeededDateTimeProvider(referenceTimeProviderMock.Object);
            seededTimeProvider.FreezeTime();
            var firstTime = seededTimeProvider.Time();

            // fast-forward reference time one minute. This should not affect
            // the seeded time provider.
            currentReferenceTime = currentReferenceTime.AddMinutes(1);
            var secondTime = seededTimeProvider.Time();

            // Now, seed the timer with a new time (which is one minute later than the previous)
            seededTimeProvider.SetTimeSeed(currentReferenceTime);
            var thirdTime = seededTimeProvider.Time();

            firstTime.ShouldBe(secondTime);
            thirdTime.ShouldBe(secondTime.AddMinutes(1));
        }
    }
}