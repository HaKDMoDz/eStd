using System;
using System.Collections.Generic;
using System.Text;

namespace Creek.IO.FilterBuilder
{
    /// <summary>
    /// Represents a filter group.
    /// </summary>
    public class FilterGroup
    {

        #region " Constructor "

        /// <summary>
        /// Creates a new FilterGroup object.
        /// </summary>
        /// <param name="name">
        /// The name of the group (e.g. "Documents")
        /// </param>
        public FilterGroup(string name)
        {
            sFilters = new List<string>();
            GroupName = name;
        }

        #endregion

        #region " Private members "

        /// <summary>
        /// The string builder to return the filter string.
        /// </summary>

        private StringBuilder sb = new StringBuilder();
        /// <summary>
        /// To hold the string filters internally.
        /// </summary>

        private List<string> sFilters;
        #endregion

        #region " Private methods "

        /// <summary>
        /// Checks if the specified filter is contained within the filter group.
        /// </summary>
        /// <param name="filter">
        /// The filter to check for.
        /// </param>
        /// <returns>
        /// Boolean as to whether the filter was found in the collection.
        /// </returns>
        private bool Contains(Creek.IO.FilterBuilder.FilterBuilder.Filters filter)
        {

            //Check the filter and return boolean
            return sFilters.Contains(Helpers.ReturnFilterAsString(filter));

        }

        #endregion

        #region " Public members "

        /// <summary>
        /// The name of the group.
        /// </summary>

        public readonly string GroupName;
        #endregion

        #region " Public methods "

        /// <summary>
        /// Add a filter to the current filter group.
        /// </summary>
        /// <param name="filter">
        /// The filter to add.
        /// </param>

        public void Add(Creek.IO.FilterBuilder.FilterBuilder.Filters filter)
        {
            //Add filter to the list
            sFilters.Add(Helpers.ReturnFilterAsString(filter));

        }

        /// <summary>
        /// Add filters to the current filter group.
        /// </summary>
        /// <param name="filters">
        /// The filters to add.
        /// </param>

        public void Add(Creek.IO.FilterBuilder.FilterBuilder.Filters[] filters)
        {
            //Add filter to the list
            foreach (Creek.IO.FilterBuilder.FilterBuilder.Filters item in filters)
            {
                sFilters.Add(Helpers.ReturnFilterAsString(item));
            }

        }

        /// <summary>
        /// Adds a system file type to the current filter
        /// group (gets description from computer).
        /// </summary>
        /// <param name="extension">
        /// The system file extension (e.g. "mp3").
        /// </param>
        /// <param name="plural">
        /// Optional. Specifies whether to try and make the system
        /// description plural ("Files" instead of "File") for 
        /// consistency with other filters. Default is true.
        /// </param>

        public void Add(string extension, bool plural = true)
        {
            //Make sure extension starts with a dot
            extension = extension.StartsWith(".") == false ? "." + extension : extension;

            //Try to get the system file type desription
            string description = "";
            try
            {
                description = new Creek.IO.FilterBuilder.FilterBuilder().GetSystemFileTypeDescription(extension);
            }
            catch (Exception)
            {
                description = "";
            }


            //Return is description is nothing
            if (string.IsNullOrEmpty(description))
            {
                return;
            }

            //Check if needs to be plural
            if (plural)
            {
                //Add an 's' to the end if it doesn't have one
                description = description.EndsWith("s") == false ? description + "s" : description;
            }

            //Get the raw extension (without spaces, dots and stars)
            string ext = extension.Trim(' ', '.', '*');

            //Make the filter string by combining the description and extension
            //properly
            string s = (description.Trim() + " (*." + ext + ")|*." + ext);

            //Add filter to the list
            sFilters.Add(s);

        }

        /// <summary>
        /// Add a custom filter to the current filter group.
        /// </summary>
        /// <param name="description">
        /// The custom description (e.g. "Foo Files").
        /// </param>
        /// <param name="extension">
        /// The custom file extension (e.g. ".foo").
        /// </param>

        public void Add(string description, string extension)
        {
            //Check if description or extension are empty, if so return
            if (string.IsNullOrEmpty(description) || string.IsNullOrEmpty(extension))
            {
                return;
            }

            //Get the raw extension (without spaces, dots and stars)
            string ext = extension.Trim(' ', '.', '*');

            //Make the filter string by combining the description and extension
            //properly
            string s = (description.Trim() + " (*." + ext + ")|*." + ext);

            //Add filter to the list
            sFilters.Add(s);

        }

        /// <summary>
        /// Returns a filter string for the items in this group.
        /// </summary>
        /// <returns>The filter string.</returns>
        public string GetFilterString()
        {

            //To hold the file extensions
            var ext = new List<string>();

            //Get file extensions

            foreach (string s in sFilters)
            {
                string[] sArray = s.Split('|');
                //Split filter with "|"

                //Make sure there are two parts
                if (sArray.Length < 2)
                {
                    return null;
                }



                //Add the extension after trimming spaces, and dots
                ext.Add(sArray[1].Trim(' ', '.'));
                //Replace ; with ,

            }

            string fs = GroupName.Trim(' ', '*', '.') + " (";
            //Start of filter string

            //Cycle through the extentions adding to the filter string

            for (int i = 0; i <= ext.Count - 1; i++)
            {
                //Not last item
                if (i != ext.Count - 1)
                {

                    //Add the extension and a comma
                    //Check if it already has a star
                    if (!ext[i].StartsWith("*."))
                    {
                        fs += ("*." + ext[i] + ",").Replace(";", ",");
                    }
                    else
                    {
                        fs += (ext[i] + ",").Replace(";", ",");
                    }


                }
                else
                {
                    //Last item - add extension and bracket
                    //Check if it already has a star
                    if (!ext[i].StartsWith("*."))
                    {
                        fs += ("*." + ext[i] + ")").Replace(";", ",");
                    }
                    else
                    {
                        fs += (ext[i] + ")").Replace(";", ",");
                    }

                }

            }

            //Update filter string
            fs += "|";
            //Last part - add a straight line

            //Cycle through the extentions adding to the last part of filter string

            for (int i2 = 0; i2 <= ext.Count - 1; i2++)
            {
                //Not last item
                if (i2 != ext.Count - 1)
                {

                    //Check if it already has a star
                    if (!ext[i2].StartsWith("*."))
                    {
                        fs += ("*." + ext[i2] + ";").Replace(",", ";");
                        //Add the extension and a semi-colon
                    }
                    else
                    {
                        fs += (ext[i2] + ";").Replace(",", ";");
                    }


                }
                else
                {
                    //Check if it already has a star
                    if (!ext[i2].StartsWith("*."))
                    {
                        fs += ("*." + ext[i2]).Replace(",", ";");
                        //Last item - Add the extension
                    }
                    else
                    {
                        fs += (ext[i2]).Replace(",", ";");
                        //Last item - Add the extension
                    }

                }

            }

            return fs;

        }

        #endregion

    }
}