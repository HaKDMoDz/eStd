using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace System.IO
{
    public class xConfiguration : Dictionary<string, Dictionary<string, string>>
    {
        public xConfiguration(string file)
        {
            string configData;

            using (StreamReader reader = new StreamReader(file))
            {
                configData = reader.ReadToEnd();

                Regex regx = new Regex(@"@.*?{.*?\n}", RegexOptions.Singleline);
                MatchCollection mCollection = regx.Matches(configData);

                foreach (Match match in mCollection)
                {
                    string matchStr = match.ToString();
                    string sectionName = matchStr.Substring(1, matchStr.IndexOf("{") - 1).Trim();

                    string sectionBlock = matchStr.Substring(matchStr.IndexOf("{") + 1, matchStr.LastIndexOf("}") - matchStr.IndexOf("{") - 1).Trim();

                    string[] strProperties = sectionBlock.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    Dictionary<string, string> propertyCollection = new Dictionary<string, string>();

                    foreach (string strPoperty in strProperties)
                    {
                        string[] propertyValuePair = strPoperty.Split(new string[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries);

                        if (propertyValuePair.Length == 2)
                        {
                            string propertyName = propertyValuePair[0].Trim();
                            string propertyValue = propertyValuePair[1].Trim();

                            propertyCollection.Add(propertyName, propertyValue);
                        }
                        else
                        {
                            throw new Exception(string.Format("Property/value error ({0}) ", propertyValuePair[0].Trim()));
                        }
                    }

                    if (this.ContainsKey(sectionName))
                    {
                        Dictionary<string, string> collection = this[sectionName];

                        foreach (KeyValuePair<string, string> pair in propertyCollection)
                        {
                            collection[pair.Key] = pair.Value;
                        }

                    }
                    else
                    {
                        this[sectionName] = propertyCollection;
                    }

                    Dictionary<string, string> cfcollection;

                    switch (sectionName)
                    {
                        case "clone":
                            cfcollection = this[sectionName];

                            foreach (KeyValuePair<string, string> item in cfcollection)
                            {
                                this[item.Key] = new Dictionary<string, string>(this[item.Value]);
                            }

                            this.Remove("clone");
                            break;

                        case "import":
                            cfcollection = this[sectionName];

                            foreach (KeyValuePair<string, string> item in cfcollection)
                            {
                                xConfiguration newConfig;

                                try
                                {
                                    newConfig = new xConfiguration(item.Value);
                                }
                                catch (FileNotFoundException e)
                                {
                                    throw new Exception(string.Format("Unable to import configuration file. {0}", e.Message));
                                }

                                if (item.Key == "*")
                                {
                                    List<string> sections = newConfig.GetSectionNames;

                                    foreach (string imSectionName in sections)
                                    {
                                        this[imSectionName] = new Dictionary<string, string>(newConfig.GetSection(imSectionName));
                                    }
                                }
                                else
                                {
                                    this[item.Key] = new Dictionary<string, string>(newConfig.GetSection(item.Key));
                                }
                            }

                            this.Remove("import");
                            break;
                    }
                }
            }
        }

        public string GetProperty(string sectionName, string propertyName)
        {
            try
            {
                return this[sectionName][propertyName];
            }
            catch
            {
                throw new Exception("Section or property does not exist");
            }
        }

        public Dictionary<string, string> GetSection(string sectionName)
        {
            try
            {
                return this[sectionName];
            }
            catch
            {
                throw new Exception("Section does not exist");
            }
        }

        public List<string> GetSectionNames
        {
            get
            {
                return this.Keys.ToList<string>();
            }
        }
    }
}