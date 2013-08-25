using System;
using System.Xml;
using System.Xml.Linq;

namespace XmlGoodies
{
    public static class XmlExtensions
    {
        /// <summary>
        /// Gets the first (in document order) child element with specified nam. If element with such name is missing, exception will be thrown.
        /// </summary>
        /// <param name="element">An <c>XElement</c> containing child element.</param>
        /// <param name="name">The <c>XName</c> to match.</param>
        /// <returns>A <c>XElement</c> that matches the specified <c>XName</c>.</returns>
        /// <exception cref="XmlException">Element doesn't have child element with specified name.</exception>
        public static XElement MandatoryElement(this XElement element, XName name)
        {
            XElement childElement = element.Element(name);
            if (childElement == null)
            {
                throw new XmlException(string.Format("The element '{0}' doesn't contain mandatory child element '{1}'.", element.Name, name));
            }

            return childElement;
        }

        /// <summary>
        /// Gets the first (in document order) child element with specified name or generated empty element if there is no matching element.
        /// </summary>
        /// <param name="element">An <c>XElement</c> containing child element.</param>
        /// <param name="name">The <c>XName</c> to match.</param>
        /// <returns>A <c>XElement</c> that matches the specified <c>XName</c>, or generated empty <c>XElement</c> with specified <c>XName</c></returns>
        public static XElement ElementOrEmpty(this XElement element, XName name)
        {
            XElement childElement = element.Element(name);
            
            return childElement ?? new XElement(name);
        }
        /// <summary>
        /// Gets the <c>XAttribute</c> of this <c>XElement</c> that has the specified <c>XName</c>. If attribute is missing, exception will be thrown.
        /// </summary>
        /// <param name="element">An <c>XElement</c> containing attribute.</param>
        /// <param name="name">The <c>XName</c> to match.</param>
        /// <returns>The matched <c>XAttribute</c>.</returns>
        /// <exception cref="XmlException">The <c>XElement</c> doesn't contain an <c>XAttribute</c> with specified <c>XName</c>.</exception>
        public static XAttribute MandatoryAttribute(this XElement element, XName name)
        {
            var attribute = element.Attribute(name);
            if (attribute == null)
            {
                throw new XmlException(string.Format("Mandatory attribute '{0}' is missing in the element '{1}'.", name, element.Name));
            }
            
            return attribute;
        }

        /// <summary>
        /// Gets the <c>XAttribute</c> of this <c>XElement</c> that has the specified <c>XName</c>. 
        /// If attribute is missing, new <c>XAttribute</c> will be generated with specified <c>XName</c> and default value.
        /// </summary>
        /// <param name="element">An <c>XElement</c> containing attribute.</param>
        /// <param name="name">The <c>XName</c> to match.</param>
        /// <param name="defaultValue">The default value which will be used as a value of the generated attribute.</param>
        /// <returns>The matched <c>XAttribute</c> or generated <c>XAttribute</c> with default value.</returns>
        public static XAttribute AttributeOrEmpty(this XElement element, XName name, string defaultValue = null)
        {
            XAttribute attribute = element.Attribute(name);
            return attribute ?? new XAttribute(name, defaultValue ?? string.Empty);
        }

        /// <summary>
        /// Gets <c>enum</c> value of this <c>XAttribute</c>.
        /// </summary>
        /// <typeparam name="T">An <c>enum</c> type to which attribute value will be converted.</typeparam>
        /// <param name="attribute">The <c>XAttribute</c> with <c>enum</c> value as a string.</param>
        /// <param name="defaultValue">The default value which will be returned if attribute has <c>string.Empty</c> value.</param>
        /// <returns>Value of the specified type.</returns>
        /// <exception cref="XmlException">The value of an attribute cannot be converted to specified type.</exception>
        /// <remarks>The conversion is case-insensitive.</remarks>
        public static T EnumValue<T>(this XAttribute attribute, T defaultValue = default(T)) where T : struct, IConvertible
        {
            Type typeOfT = typeof(T);
            if (!typeOfT.IsEnum)
            {
                throw new XmlException(string.Format("Type '{0}' must be an enumeration.", typeOfT.Name));
            }

            if (string.IsNullOrEmpty(attribute.Value))
            {
                return defaultValue;
            }

            T value;
            if (!Enum.TryParse(attribute.Value, /* ignoreCase = */ true, out value))
            {
                throw new XmlException(string.Format("The attribute '{0}' has value '{1}' which cannot be converted to the value of type '{2}'.", attribute.Name, attribute.Value, typeOfT.Name));
            }

            return value;
        }

        /// <summary>
        /// Asserts current <c>XElement</c> has expected <c>XName</c>.
        /// </summary>
        /// <param name="element">The <c>XElement</c> to assert.</param>
        /// <param name="expectedName">The expected <c>XName</c> of the element.</param>
        /// <exception cref="XmlException">The element has different name.</exception>
        public static void AssertName(XElement element, XName expectedName)
        {
            if (element.Name != expectedName)
            {
                throw new XmlException(string.Format("Assertion failed. Wrong XML element received. Expected element with name '{0}', but was element with name '{1}'.", element.Name, expectedName));
            }
        }

        /// <summary>
        /// Outputs new <c>XElement</c> if it has non-default value.
        /// </summary>
        /// <param name="name">The <c>XName</c> of the <c>XElement</c> to generate.</param>
        /// <param name="value">The node value.</param>
        /// <param name="defaultValue">The default value which shouldn't be outputted.</param>
        /// <returns>New <c>XElement</c> if <c>value</c> is not <c>null</c> or default; <c>null</c> otherwise.</returns>
        /// <remarks>This method allows to skip generation of empty <c>XElement</c> nodes.</remarks>
        public static XElement NewOptionalXElement(XName name, object value, object defaultValue = null)
        {
            if (value != null && !value.Equals(defaultValue))
            {
                return new XElement(name, value);
            }
            
            return null;
        }

        /// <summary>
        /// Outputs new <c>XAttribute</c> if it has non-default value.
        /// </summary>
        /// <param name="name">The <c>XName</c> of the <c>XAttribute</c> to generate.</param>
        /// <param name="value">The attribute value.</param>
        /// <param name="defaultValue">The default value which shouldn't be outputted.</param>
        /// <returns>New <c>XAttribute</c> if <c>value</c> is not <c>null</c> or default; <c>null</c> otherwise.</returns>
        /// <remarks>This method allows to skip generation of attributes with empty or default values.</remarks>
        public static XAttribute NewOptionalXAttribute(XName name, object value, object defaultValue = null)
        {
            if (value != null && !value.Equals(defaultValue))
            {
                return new XAttribute(name, value);
            }

            return null;
        }
    }
}

