using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Creek.Extensions
{
    public static class Extension
    {

        /// <summary>
        /// Wählt einen zusammenhängenden Bereich einer Sequenz aus, der zwischen 2 Elementen liegt, die durch je eine Bedingung ermittelt wurden.<para/>
        /// Es können mehrere solche zusammenhängende Bereiche vereint werden.
        /// </summary>
        /// <typeparam name="T">Der Typ von dem die Elemente der Sequenz sind.</typeparam>
        /// <param name="source">Die Quellliste.</param>
        /// <param name="startCondition">Die Bedingung mit der das Startelement eines Bereichs ermittelt wird.</param>
        /// <param name="endCondition">Die Bedingung mit der das Endelement eines Bereichs ermittelt wird.</param>
        /// <param name="includeStart"><c>true</c>, wenn das Startelement mit zurück gegeben werden soll, andernfalls <c>false</c>.</param>
        /// <param name="includeEnd"><c>true</c>, wenn das Endelement mit zurück gegeben werden soll, andernfalls <c>false</c>.</param>
        /// <param name="multipleRanges"><c>true</c>, wenn bei einem weiteren Fund eines Startelements der neu gefundene Bereich ebenfalls zurück gegeben wird; andernfalls <c>false</c>.</param>
        /// <returns>Eine Sequenz von Elementen aus <paramref name="source"/>, welche zwischen Elementen liegen, die den angegebenen Bedingungen entsprechen.</returns>
        /// <remarks>Beim erstellen der neuen Liste wird jedes Element durchlaufen. Zu Beginn wird jedes Element mit der <paramref name="startCondition"/>-Funktion geprüft. Sobald 
        /// ein Treffer erziehlt wurde, wird nur noch die <paramref name="endCondition"/>-Funktion zum prüfen aufgerufen. Sobald jetzt ein Treffer erziehlt wurde, beginnt die 
        /// Prüfung der Elemente wieder mit der <paramref name="startCondition"/>-Funktion. Wenn <paramref name="multipleRanges"/> <c>true</c> ist, wird nach dem ersten Treffer mit der
        /// <paramref name="endCondition"/>-Funktion die neue Sequenz zurück gegeben. </remarks>
        /// <exception cref="System.ArgumentNullException">Wird ausgelöst, wenn <paramref name="source"/>, <paramref name="startCondition"/> oder <paramref name="endCondition"/> <c>null</c> ist.</exception>
        public static IEnumerable<T> Range<T>(this IEnumerable<T> source, Func<T, bool> startCondition, Func<T, bool> endCondition, bool includeStart, bool includeEnd, bool multipleRanges)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (startCondition == null)
                throw new ArgumentNullException("startCondition");
            if (endCondition == null)
                throw new ArgumentNullException("endCondition");

            using (var enumerator = source.GetEnumerator())
            {
                bool ret = false;
                while (enumerator.MoveNext())
                {
                    if (ret)
                    {
                        if (endCondition(enumerator.Current))
                        {
                            if (includeEnd)
                                yield return enumerator.Current;
                            if (multipleRanges)
                                ret = false;
                            else
                                break;
                        }
                        else
                            yield return enumerator.Current;
                    }
                    else
                    {
                        if (startCondition(enumerator.Current))
                        {
                            ret = true;
                            if (includeStart)
                                yield return enumerator.Current;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Bestimmt ob ein Wert in einem bestimmten Bereich liegt.
        /// </summary>
        /// <param name="val">Der zu prüfende Wert.</param>
        /// <param name="min">Die Untergrenze des Bereichs.</param>
        /// <param name="max">Die Obergrenze des Bereichs.</param>
        /// <param name="rangeType">Bestimmt ob die Grenzen inklusiv oder exklusiv sind.</param>
        /// <returns><c>True</c>, wenn <paramref name="value"/> in den angegebenen Grenzen liegt. Andernfalls <c>False</c>.</returns>
        public static bool IsInRange(this double val, double min, double max, RangeTypes rangeType)
        {
            return ((rangeType == RangeTypes.Excl || rangeType == RangeTypes.ExclIncl) ? val > min : val >= min)
                && ((rangeType == RangeTypes.Excl || rangeType == RangeTypes.InclExcl) ? val < max : val <= max);
        }

        public static Color FromHex(this Color c, string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length != 6) throw new Exception("Color not valid");

            return Color.FromArgb(
                int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
        }

        public static void AddEventListener(this object e, string name, Delegate action)
        {
            e.GetType().GetEvent(name).AddEventHandler(e, action);
        }

        public enum RangeTypes
        {
            /// <summary>
            /// Die Untergrenze wird ein- und die Obergrenze ausgeschlossen.
            /// </summary>
            InclExcl,
            /// <summary>
            /// Die Untergrenze wird aus- und die Obergrenze eingeschlossen.
            /// </summary>
            ExclIncl,
            /// <summary>
            /// Beide Grenzen werden eingeschlossen.
            /// </summary>
            Incl,
            /// <summary>
            /// Beide Grenzen werden ausgeschlossen.
            /// </summary>
            Excl,
        }

        public static T As<T>(this string input)
        {
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(input);
        }

        public static byte[] ToByteArray(this string str)
        {
            var enc = new System.Text.ASCIIEncoding();
            return enc.GetBytes(str);
        }

        public static T To<T>(this object o)
        {
            return (T)o;
        }

        public static string toString(this object o)
        {
            return o.ToString();
        }
    }
}
