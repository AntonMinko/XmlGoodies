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
        }

        [Flags]
        enum Colors
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

            Colors colors = attribute.EnumValue<Colors>();
            Assert.AreEqual(Colors.Blue, colors, "Blue");
        }

        [Test]
        public void MultipleEnumValueTest()
        {
            var attribute = new XAttribute("color", Colors.Red | Colors.Green);
            Assert.AreEqual("Red, Green", attribute.Value);

            Colors colors = attribute.EnumValue<Colors>();
            Assert.AreEqual(Colors.Red | Colors.Green, colors, "Red | Green");
        }

        [Test]
        public void DefaultEnumValueTest()
        {
            var attribute = new XAttribute("color", string.Empty);
            
            Colors colors = attribute.EnumValue<Colors>();
            Assert.AreEqual(Colors.None, colors, "None");

            colors = attribute.EnumValue<Colors>(Colors.Green);
            Assert.AreEqual(Colors.Green, colors, "Green");
        }

        [Test]
        public void EnumValueErrorHandlingTest()
        {
            var attribute = new XAttribute("colorAttribute", "Purple");

            var ex = Assert.Throws<XmlException>(() => attribute.EnumValue<DateTime>());
            Assert.IsTrue(ex.Message.Contains("DateTime"), "Type exception");

            ex = Assert.Throws<XmlException>(() => attribute.EnumValue<Colors>());
            Assert.IsTrue(ex.Message.Contains("colorAttribute") && ex.Message.Contains("Purple") && ex.Message.Contains("Colors"), "Value exception");
        }

        [Test]
        public void AssertNameTest()
        {
            var element = new XElement("Element");

            Assert.DoesNotThrow(() => XmlExtensions.AssertName(element, "Element"));
            
            var ex = Assert.Throws<XmlException>(() => XmlExtensions.AssertName(element, "AnotherName"));
            Assert.IsTrue(ex.Message.Contains("'AnotherName'") && ex.Message.Contains("'Element'"), "AnotherName");

            ex = Assert.Throws<XmlException>(() => XmlExtensions.AssertName(element, "{http://schemas.xyz.com/namespaceUri}Element"));
            Assert.IsTrue(ex.Message.Contains("'{http://schemas.xyz.com/namespaceUri}Element'") && ex.Message.Contains("'Element'"), "with namespace");
        }

        [Test]
        public void NewOptionalXElementTest()
        {
            var element = XmlExtensions.NewOptionalXElement("Element", "value");
            Assert.IsNotNull(element);

            element = XmlExtensions.NewOptionalXElement("Element", null);
            Assert.IsNull(element);

            element = XmlExtensions.NewOptionalXElement("Element", null, defaultValue: 1);
            Assert.IsNull(element);

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
    }
}
