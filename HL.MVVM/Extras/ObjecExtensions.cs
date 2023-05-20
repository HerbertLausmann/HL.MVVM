using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Provides extension methods for adding and retrieving custom properties to and from objects.
    /// </summary>
    public static class ObjectExtensions
    {
        private static readonly ConditionalWeakTable<object, PropertyBag> Properties = new ConditionalWeakTable<object, PropertyBag>();

        /// <summary>
        /// Sets a custom property of type T with a specified name and value for a given object.
        /// </summary>
        /// <typeparam name="T">The type of the custom property.</typeparam>
        /// <param name="obj">The object for which the custom property is being set.</param>
        /// <param name="propertyName">The name of the custom property.</param>
        /// <param name="value">The value of the custom property.</param>
        /// <exception cref="ArgumentException">Thrown if the propertyName is null or empty.</exception>
        public static void SetProperty<T>(this object obj, string propertyName, T value)
        {
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentException("Property name cannot be null or empty.", nameof(propertyName));

            PropertyBag propertyBag;
            if (!Properties.TryGetValue(obj, out propertyBag))
            {
                propertyBag = new PropertyBag();
                Properties.Add(obj, propertyBag);
            }

            propertyBag[propertyName] = value;
        }

        /// <summary>
        /// Retrieves the custom property of type T with the specified name for a given object.
        /// </summary>
        /// <typeparam name="T">The type of the custom property.</typeparam>
        /// <param name="obj">The object for which the custom property is being retrieved.</param>
        /// <param name="propertyName">The name of the custom property.</param>
        /// <returns>The value of the custom property if it exists; otherwise, the default value of type T.</returns>
        /// <exception cref="ArgumentException">Thrown if the propertyName is null or empty.</exception>
        public static T GetProperty<T>(this object obj, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentException("Property name cannot be null or empty.", nameof(propertyName));

            if (Properties.TryGetValue(obj, out var propertyBag) && propertyBag.TryGetValue(propertyName, out var value))
            {
                return (T)value;
            }

            return default;
        }

        /// <summary>
        /// Represents a collection of custom properties associated with an object.
        /// </summary>
        private class PropertyBag : System.Collections.Generic.Dictionary<string, object> { }
    }
}
