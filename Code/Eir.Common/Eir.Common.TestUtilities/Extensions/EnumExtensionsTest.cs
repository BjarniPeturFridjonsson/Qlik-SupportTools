using Eir.Common.Extensions;
using NUnit.Framework;
using Shouldly;

namespace Eir.Common.Test.Extensions
{
    [TestFixture]
    public class EnumExtensionsTest
    {
        private enum TestEnum
        {
            Undefined,
            FirstValue
        }

        [Test]
        public void CanConvertEnumValueToStorageString()
        {
            var enumValue = TestEnum.FirstValue;
            var valueAsStorageString = enumValue.ToStorageString();

            valueAsStorageString.ShouldBe("FirstValue");
        }


        [Test]
        public void CanConvertStorageStringToEnumValue()
        {
            var valueAsStorageString = "FirstValue";
            var enumValue = valueAsStorageString.FromStorageString<TestEnum>();

            enumValue.ShouldBe(TestEnum.FirstValue);
        }
    }
}