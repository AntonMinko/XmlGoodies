using System;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;

namespace XmlGoodies
{
    [TestFixture]
    public class XmlExtensionsTests
    {
        [Test]
        public void MandatoryElementTest()
        {
            var element = new XElement("Test", new XElement("Child", 1));

            var childElement = element.MandatoryElement("Child");

            Assert.IsNotNull(childElement, "childElement != null");
            Assert.AreEqual("Child", childElement.Name.ToString(), "childElement.Name");

            var ex = Assert.Throws<XmlException>(() => element.MandatoryElement("AbsentElement"));
            Assert.IsTrue(ex.Message.Contains("Test") && ex.Message.Contains("AbsentElement"), "Error message");
        }

        [Test]
        public void ElementOrEmptyTest()
        {
            var element = new XElement("Test", new XElement("Child", "value"));

            var childElement = element.ElementOrEmpty("Child");
            Assert.IsNotNull(childElement, "childElement != null");
            Assert.AreEqual("Child", childElement.Name.ToString(), "childElement.Name");
            Assert.AreEqual("value", childElement.Value, "childElement.Name");

            var dummyElement = element.ElementOrEmpty("AbsentElement");
            Assert.IsNotNull(dummyElement, "dummyElement != null");
            Assert.AreEqual("AbsentElement", dummyElement.Name.ToString(), "dummyElement != null");

            element = null;
            dummyElement = element.ElementOrEmpty("AbsentElement");
            Assert.IsNotNull(dummyElement, "dummyElement != null");
            Assert.AreEqual("AbsentElement", dummyElement.Name.ToString(), "dummyElement != null");
        }

        [Test]
        public void MandatoryAttributeTest()
        {
            var element = new XElement("Test", new XAttribute("attribute", 1));

            var attribute = element.MandatoryAttribute("attribute");

            Assert.IsNotNull(attribute, "attribute != null");
            Assert.AreEqual("attribute", attribute.Name.ToString(), "attribute.Name");

            var ex = Assert.Throws<XmlException>(() => element.MandatoryAttribute("absentAttribute"));
            Assert.IsTrue(ex.Message.Contains("Test") && ex.Message.Contains("absentAttribute"), "Error message");
        }

        [Test]
        public void AttributeOrEmptyTest()
        {
            var element = new XElement("Test", new XAttribute("attribute", 1));

            var attribute = element.AttributeOrEmpty("attribute");

            Assert.IsNotNull(attribute, "attribute != null");
            Assert.AreEqual("attribute", attribute.Name.ToString(), "attribute.Name");
            Assert.AreEqual("1", attribute.Value, "attribute.Value");

            var dummyAttribute = element.AttributeOrEmpty("dummyAttribute");
            Assert.IsNotNull(dummyAttribute, "dummyAttribute != null");
            Assert.AreEqual("dummyAttribute", dummyAttribute.Name.ToString(), "dummyAttribute.Name");
            Assert.AreEqual(string.Empty, dummyAttribute.Value, "dummyAttribute.Value is empty");

            dummyAttribute = element.AttributeOrEmpty("dummyAttribute", "default value");
            Assert.AreEqual("default value", dummyAttribute.Value, "dummyAttribute.Value default value");

            element = null;
            dummyAttribute = element.AttributeOrEmpty("dummyAttribute");
            Assert.IsNotNull(dummyAttribute, "dummyAttribute != null");
            Assert.AreEqual("dummyAttribute", dummyAttribute.Name.ToString(), "dummyAttribute.Name");
        }

        [Test]
        public void ValueWithBuiltInTypesTest()
        {
            var intAttribute = new XAttribute("int", "10");
            Assert.AreEqual(10, intAttribute.Value<int>());

            var uintAttribute = new XAttribute("uint", "123");
            Assert.AreEqual(123U, uintAttribute.Value<uint>());

            var shortAttribute = new XAttribute("short", "-12");
            Assert.AreEqual(-12, shortAttribute.Value<short>());

            var ushortAttribute = new XAttribute("ushort", "123");
            Assert.AreEqual(123, ushortAttribute.Value<ushort>());

            var byteAttribute = new XAttribute("byte", "0");
            Assert.AreEqual(0, byteAttribute.Value<byte>());

            var sbyteAttribute = new XAttribute("sbyte", "-10");
            Assert.AreEqual(-10, sbyteAttribute.Value<sbyte>());

            var charAttribute = new XAttribute("char", "c");
            Assert.AreEqual('c', charAttribute.Value<char>());

            var longAttribute = new XAttribute("long", "999");
            Assert.AreEqual(999L, longAttribute.Value<long>());

            var ulongAttribute = new XAttribute("ulong", "100");
            Assert.AreEqual(100UL, ulongAttribute.Value<ulong>());

            var floatAttribute = new XAttribute("float", 90.90F.ToString());
            Assert.AreEqual(90.90F, floatAttribute.Value<float>());

            var doubleAttribute = new XAttribute("double", 10.10D.ToString());
            Assert.AreEqual(10.10D, doubleAttribute.Value<double>());

            var decimalAttribute = new XAttribute("decimal", 10.10M.ToString());
            Assert.AreEqual(10.10M, decimalAttribute.Value<decimal>());

            var boolAttribute = new XAttribute("bool", "true");
            Assert.AreEqual(true, boolAttribute.Value<bool>());

            var dateTimeAttribute = new XAttribute("DateTime", new DateTime(3000, 1, 1).ToString());
            Assert.AreEqual(new DateTime(3000, 1, 1), dateTimeAttribute.Value<DateTime>());
        }

        [Flags]
        private enum Colors
        {
            None = 0,
            Red = 1,
            Green = 2,
            Blue = 4
        }

        [Test]
        public void SingleEnumValueTest()
        {
            var attribute = new XAttribute("color", Colors.Blue);
            Assert.AreEqual("Blue", attribute.Value);

            Colors colors = attribute.Value<Colors>();
            Assert.AreEqual(Colors.Blue, colors, "Blue");
        }

        [Test]
        public void SingleEnumValueWrongCaseTest()
        {
            var attribute = new XAttribute("color", "green");

            Colors colors = attribute.Value<Colors>();
            Assert.AreEqual(Colors.Green, colors, "Green");
        }

        [Test]
        public void MultipleEnumValueTest()
        {
            var attribute = new XAttribute("color", Colors.Red | Colors.Green);
            Assert.AreEqual("Red, Green", attribute.Value);

            Colors colors = attribute.Value<Colors>();
            Assert.AreEqual(Colors.Red | Colors.Green, colors, "Red | Green");
        }

        [Test]
        public void DefaultEnumValueTest()
        {
            var attribute = new XAttribute("color", string.Empty);

            Colors colors = attribute.Value<Colors>();
            Assert.AreEqual(Colors.None, colors, "None");

            colors = attribute.Value<Colors>(Colors.Green);
            Assert.AreEqual(Colors.Green, colors, "Green");

            attribute = null;
            colors = attribute.Value<Colors>();
            Assert.AreEqual(Colors.None, colors, "Null -> None");
        }

        [Test]
        public void ValueErrorHandlingTest()
        {
            var enumAttribute = new XAttribute("colorAttribute", "Purple");

            var ex = Assert.Throws<XmlException>(() => enumAttribute.Value<Colors>());
            Assert.IsTrue(
                ex.Message.Contains("colorAttribute") && ex.Message.Contains("Purple") && ex.Message.Contains("Colors"),
                "Enum value exception");

            var intAttribute = new XAttribute("number", "one");
            ex = Assert.Throws<XmlException>(() => intAttribute.Value<int>());
            Assert.IsTrue(ex.Message.Contains("number") && ex.Message.Contains("one") && ex.Message.Contains("Int32"),
                "int value exception");
        }

        [Test]
        public void ElementValueTest()
        {
            var intElement = new XElement("int", "10");
            Assert.AreEqual(10, intElement.Value<int>());

            var enumElement = new XElement("color", Colors.Red | Colors.Green); // Value = "Red, Green"
            Assert.AreEqual("Red, Green", enumElement.Value);
            Colors colors = enumElement.Value<Colors>();
            Assert.AreEqual(Colors.Red | Colors.Green, colors, "Red | Green");
        }

        [Test]
        public void AttributeValueOfCustomTypeTest()
        {
            var urlAttribute = new XAttribute("url", "http://google.com");
            Assert.AreEqual(new Uri("http://google.com"), urlAttribute.Value(s => new Uri(s)));
        }

        [Test]
        public void ElementValueOfCustomTypeTest()
        {
            var urlElement = new XElement("Url", "http://google.com");
            Assert.AreEqual(new Uri("http://google.com"), urlElement.Value(s => new Uri(s)));
        }

        [Test]
        public void AssertNameTest()
        {
            var element = new XElement("Element");

            Assert.DoesNotThrow(() => XmlExtensions.AssertName(element, "Element"));

            var ex = Assert.Throws<XmlException>(() => XmlExtensions.AssertName(element, "AnotherName"));
            Assert.IsTrue(ex.Message.Contains("'AnotherName'") && ex.Message.Contains("'Element'"), "AnotherName");

            ex =
                Assert.Throws<XmlException>(
                    () => XmlExtensions.AssertName(element, "{http://schemas.xyz.com/namespaceUri}Element"));
            Assert.IsTrue(
                ex.Message.Contains("'{http://schemas.xyz.com/namespaceUri}Element'") &&
                ex.Message.Contains("'Element'"), "with namespace");
        }

        [Test]
        public void NewOptionalXElementTest()
        {
            var element = XmlExtensions.NewOptionalXElement("Element", "value");
            Assert.IsNotNull(element);

            element = XmlExtensions.NewOptionalXElement("Element", (int?) null, defaultValue: 1);
            Assert.IsNotNull(element);

            element = XmlExtensions.NewOptionalXElement("Element", 1, defaultValue: 1);
            Assert.IsNull(element);
        }

        [Test]
        public void NewOptionalXAttributeTest()
        {
            var element = XmlExtensions.NewOptionalXAttribute("attribute", "value");
            Assert.IsNotNull(element);

            element = XmlExtensions.NewOptionalXAttribute("attribute", null);
            Assert.IsNull(element);

            element = XmlExtensions.NewOptionalXAttribute("attribute", null, defaultValue: 1);
            Assert.IsNull(element);

            element = XmlExtensions.NewOptionalXAttribute("attribute", 1, defaultValue: 1);
            Assert.IsNull(element);
        }

        [Test]
        public void NewOptionalXElementsTest()
        {
            var element = XmlExtensions.NewOptionalXElements("Element", () => true, "value");
            Assert.IsNotNull(element);

            element = XmlExtensions.NewOptionalXElements("Element", () => false, "value");
            Assert.IsNull(element);
        }
    }
}
