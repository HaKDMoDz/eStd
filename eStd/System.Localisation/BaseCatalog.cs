using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Creek.I18N.Gettext.Plural;

namespace Creek.I18N.Gettext
{
    /// <summary>
    /// Base catalog.
    /// </summary>
    public abstract class BaseCatalog : ICatalog
    {
        /// <summary>
        /// Context glue (&lt;EOT&gt; symbol)
        /// </summary>
        public const char CONTEXT_GLUE = '\u0004';

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCatalog"/> class that has no translations,
        /// with default plural form formulas, using given culture info.
        /// </summary>
        /// <param name="cultureInfo">Locale of this catalog.</param>
        protected BaseCatalog(CultureInfo cultureInfo)
        {
            CultureInfo = cultureInfo;
            PluralForms = PluralFormProcessor.Default;
            Translations = new Dictionary<string, string[]>();
        }

        #region ICatalog implementation

        /// <summary>
        /// Returns <paramref name="text"/> translated into the selected language.
        /// Similar to <c>gettext</c> function.
        /// </summary>
        /// <param name="text">Text to translate.</param>
        /// <returns>Translated text.</returns>
        public virtual string GetString(string text)
        {
            return GetStringDefault(text, text);
        }

        /// <summary>
        /// Returns <paramref name="text"/> translated into the selected language.
        /// Similar to <c>gettext</c> function.
        /// </summary>
        /// <param name="text">Text to translate.</param>
        /// <param name="args">Optional arguments for <see cref="System.String.Format(string, object[])"/> method.</param>
        /// <returns>Translated text.</returns>
        public virtual string GetString(string text, params object[] args)
        {
            return String.Format(GetStringDefault(text, text), args);
        }

        /// <summary>
        /// Returns the plural form for <paramref name="n"/> of the translation of <paramref name="text"/>.
        /// Similar to <c>Lib.Gettext</c> function.
        /// </summary>
        /// <param name="text">Singular form of message to translate.</param>
        /// <param name="pluralText">Plural form of message to translate.</param>
        /// <param name="n">Value that determines the plural form.</param>
        /// <returns>Translated text.</returns>
        public virtual string GetPluralString(string text, string pluralText, long n)
        {
            return GetPluralStringDefault(text, text, pluralText, n);
        }

        /// <summary>
        /// Returns the plural form for <paramref name="n"/> of the translation of <paramref name="text"/>.
        /// Similar to <c>Lib.Gettext</c> function.
        /// </summary>
        /// <param name="text">Singular form of message to translate.</param>
        /// <param name="pluralText">Plural form of message to translate.</param>
        /// <param name="n">Value that determines the plural form.</param>
        /// <param name="args">Optional arguments for <see cref="System.String.Format(string, object[])"/> method.</param>
        /// <returns>Translated text.</returns>
        public virtual string GetPluralString(string text, string pluralText, long n, params object[] args)
        {
            return String.Format(GetPluralStringDefault(text, text, pluralText, n), args);
        }

        /// <summary>
        /// Returns <paramref name="text"/> translated into the selected language using given <paramref name="context"/>.
        /// Similar to <c>pgettext</c> function.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="text">Text to translate.</param>
        /// <returns>Translated text.</returns>
        public virtual string GetParticularString(string context, string text)
        {
            return GetStringDefault(context + CONTEXT_GLUE + text, text);
        }

        /// <summary>
        /// Returns <paramref name="text"/> translated into the selected language using given <paramref name="context"/>.
        /// Similar to <c>pgettext</c> function.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="text">Text to translate.</param>
        /// <param name="args">Optional arguments for <see cref="System.String.Format(string, object[])"/> method.</param>
        /// <returns>Translated text.</returns>
        public virtual string GetParticularString(string context, string text, params object[] args)
        {
            return String.Format(GetStringDefault(context + CONTEXT_GLUE + text, text), args);
        }

        /// <summary>
        /// Returns the plural form for <paramref name="n"/> of the translation of <paramref name="text"/> using given <paramref name="context"/>.
        /// Similar to <c>npgettext</c> function.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="text">Singular form of message to translate.</param>
        /// <param name="pluralText">Plural form of message to translate.</param>
        /// <param name="n">Value that determines the plural form.</param>
        /// <returns>Translated text.</returns>
        public virtual string GetParticularPluralString(string context, string text, string pluralText, long n)
        {
            return GetPluralStringDefault(context + CONTEXT_GLUE + text, text, pluralText, n);
        }

        /// <summary>
        /// Returns the plural form for <paramref name="n"/> of the translation of <paramref name="text"/> using given <paramref name="context"/>.
        /// Similar to <c>npgettext</c> function.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="text">Singular form of message to translate.</param>
        /// <param name="pluralText">Plural form of message to translate.</param>
        /// <param name="n">Value that determines the plural form.</param>
        /// <param name="args">Optional arguments for <see cref="System.String.Format(string, object[])"/> method.</param>
        /// <returns>Translated text.</returns>
        public virtual string GetParticularPluralString(string context, string text, string pluralText, long n,
                                                        params object[] args)
        {
            return String.Format(GetPluralStringDefault(context + CONTEXT_GLUE + text, text, pluralText, n), args);
        }

        #endregion

        /// <summary>
        /// Current catalog locale.
        /// </summary>
        public CultureInfo CultureInfo { get; protected set; }

        /// <summary>
        /// Current plural form processor.
        /// </summary>
        public PluralFormProcessor PluralForms { get; protected set; }

        /// <summary>
        /// Loaded raw translation strings.
        /// (msgctxt&lt;EOT&gt;)msgid => msgstr[]
        /// </summary>
        public Dictionary<string, string[]> Translations { get; protected set; }

        /// <summary>
        /// Returns translated string for given <paramref name="messageId"/> or <paramref name="defaultMessage"/> on fail.
        /// </summary>
        /// <param name="messageId">Message ID</param>
        /// <param name="defaultMessage">Default message</param>
        /// <returns>Translated string</returns>
        public virtual string GetStringDefault(string messageId, string defaultMessage)
        {
            string[] translations = GetTranslations(messageId);

            if (translations == null || translations.Length == 0)
            {
                Trace.WriteLine(String.Format("Translation not found for message id \"{0}\".", messageId), "Lib.Gettext");
                return defaultMessage;
            }

            return translations[0];
        }

        /// <summary>
        /// Returns translated plural string for given <paramref name="messageId"/> or 
        /// <paramref name="defaultMessage"/> or <paramref name="defaultPluralMessage"/> on fail.
        /// </summary>
        /// <param name="messageId">Message ID</param>
        /// <param name="defaultMessage">Default message singular form</param>
        /// <param name="defaultPluralMessage">Default message plural form</param>
        /// <param name="n">Value that determines the plural form</param>
        /// <returns>Translated string</returns>
        public virtual string GetPluralStringDefault(string messageId, string defaultMessage,
                                                     string defaultPluralMessage, long n)
        {
            string[] translations = GetTranslations(messageId);
            int pluralIndex = PluralForms.GetPluralFormIndex(CultureInfo, n);

            if (translations == null || translations.Length <= pluralIndex)
            {
                Trace.WriteLine(String.Format("Translation not found (plural) for message id \"{0}\".", messageId),
                                "Lib.Gettext");
                return (n == 1) ? defaultMessage : defaultPluralMessage;
            }

            return translations[pluralIndex];
        }

        /// <summary>
        /// Returns all translations for given <paramref name="messageId"/>.
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns>Returns all translations for given <paramref name="messageId"/> or null if not found.</returns>
        public virtual string[] GetTranslations(string messageId)
        {
            if (String.IsNullOrEmpty(messageId)) return null;
            if (!Translations.ContainsKey(messageId)) return null;

            return Translations[messageId];
        }
    }
}