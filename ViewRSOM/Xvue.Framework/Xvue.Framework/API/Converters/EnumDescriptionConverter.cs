using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Collections;
using System.Collections.ObjectModel;
using System.Resources;
using System.Reflection;
using System.Windows.Markup;
using Xvue.Framework.API.Attributes;
namespace Xvue.Framework.API.Converters
{

    [ContentProperty("OverriddenDisplayEntries")]
    public class EnumDescriptionConverter : IValueConverter
    {
        readonly Type _enumType;
        IDictionary displayValues;
        IDictionary reverseValues;
        List<EnumDisplayEntry> _overriddenDisplayEntries;

        public EnumDescriptionConverter(Type type)
        {            
            if (!type.IsEnum)
                throw new ArgumentException("parameter is not an Enumerated type", "type");
            _enumType = type;
            CreateDictionaries();
        }

        public List<EnumDisplayEntry> OverriddenDisplayEntries
        {
            get
            {
                if (_overriddenDisplayEntries == null)
                    _overriddenDisplayEntries = new List<EnumDisplayEntry>();
                return _overriddenDisplayEntries;
            }
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value);
        }
        protected object Convert(object value)
        {
            return displayValues[value];
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack(value);
        }
        protected object ConvertBack(object value)
        {
            return reverseValues[value];
        }

        private string GetDisplayStringValue(DisplayStringAttribute[] a)
        {
            if (a == null || a.Length == 0) return null;
            DisplayStringAttribute dsa = a[0];
            if (!string.IsNullOrEmpty(dsa.ResourceKey))
            {
                ResourceManager rm = new ResourceManager(_enumType);
                return rm.GetString(dsa.ResourceKey);
            }
            return dsa.Value;
        }

        private string GetBackupDisplayStringValue(object enumValue)
        {
            if (_overriddenDisplayEntries != null && _overriddenDisplayEntries.Count > 0)
            {
                EnumDisplayEntry foundEntry = _overriddenDisplayEntries.Find(delegate(EnumDisplayEntry entry)
                {
                    object e = Enum.Parse(_enumType, entry.EnumValue);
                    return enumValue.Equals(e);
                });
                if (foundEntry != null)
                {
                    if (foundEntry.ExcludeFromDisplay) return null;
                    return foundEntry.DisplayString;

                }
            }
            return Enum.GetName(_enumType, enumValue);
        }

        private void CreateDictionaries()
        {
            Type displayValuesType = typeof(Dictionary<,>)
                                        .GetGenericTypeDefinition().MakeGenericType(_enumType,typeof(string));
            this.displayValues = (IDictionary)Activator.CreateInstance(displayValuesType);

            this.reverseValues =
                (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>)
                        .GetGenericTypeDefinition()
                        .MakeGenericType(typeof(string),_enumType));

            var fields = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                DisplayStringAttribute[] a = (DisplayStringAttribute[])
                                            field.GetCustomAttributes(typeof(DisplayStringAttribute), false);

                string displayString = GetDisplayStringValue(a);
                object enumValue = field.GetValue(null);

                if (displayString == null)
                {
                    displayString = GetBackupDisplayStringValue(enumValue);
                }
                if (displayString != null)
                {
                    displayValues.Add(enumValue, displayString);
                    reverseValues.Add(displayString,enumValue);
                }
            }
        }
        public ReadOnlyCollection<string> DisplayNames
        {
            get
            {
                //CreateDictionaries();
                return new List<string>((IEnumerable<string>)displayValues.Values).AsReadOnly();
            }
        }

        public object GetField(int index)
        {
            var fields = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            object enumValue = fields[index].GetValue(null);
            return enumValue;
        }

        public static string GetFriendlyName<T>(T value)
        {
            if (value == null)
                return "";

            try
            {
                Xvue.Framework.API.Converters.EnumSegment<T> helper = new Xvue.Framework.API.Converters.EnumSegment<T>(new List<T>());
                string friendlyName = helper.SelectedEnumName(value);
                return friendlyName;
            }
            catch
            {
                return "";
            }
        }
    }

    public class EnumSegment<T>
    {

        Array EnumList { get; set; }

        EnumDescriptionConverter converter;

        public Type GetMainType
        {
            get { return typeof(T); }
        }

        public ObservableCollection<string> DisplayNameList { get; private set; }

        public EnumSegment(List<T> enumExcluded)
        {
            List<T> currentEnumList = new List<T>();
            
            EnumList = Enum.GetValues(typeof(T));            
            converter = new EnumDescriptionConverter(typeof(T));            

            DisplayNameList = new ObservableCollection<string>();
            for (int i = 0; i < converter.DisplayNames.Count; i++)
            {
                foreach (T item in EnumList)
                {
                    if (Enum.Equals(item, converter.GetField(i)))
                    {
                        currentEnumList.Add(item);
                        break;
                    }
                }
            }
            EnumList = currentEnumList.ToArray();
            int index = 0;
            foreach (var item in EnumList)
            {
                bool found = false;
                if (enumExcluded != null)
                {
                    foreach (T exclude in enumExcluded)
                    {
                        if (Enum.Equals(item, exclude))
                        {
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                    DisplayNameList.Add(converter.DisplayNames[index]);

                index++;
            }
        }

        public object SelectedEnum(string displayName)
        {
            int index = 0;
            foreach (var item in EnumList)
            {
                if (converter.DisplayNames[index] == displayName)
                {
                    return item;
                }
                index++;
            }
            return null;
        }

        public string SelectedEnumName(T selectedEnum)
        {
            int index = 0;
            string selectedEnumString = "";
            foreach (var item in EnumList)
            {
                if (Enum.Equals(item, selectedEnum))
                {
                    selectedEnumString = converter.DisplayNames[index];
                    break;
                }
                index++;
            }
            foreach (string displayName in DisplayNameList)
            {
                if (displayName == selectedEnumString) return displayName;
            }
            return "";
        }

        public bool IsSelectedEnumName(T selectedEnum)
        {
            int index = 0;
            string selectedEnumString = "";
            foreach (var item in EnumList)
            {
                if (Enum.Equals(item, selectedEnum))
                {
                    selectedEnumString = converter.DisplayNames[index];
                    break;
                }
                index++;
            }
            foreach (string displayName in DisplayNameList)
            {
                if (displayName == selectedEnumString) return true;
            }
            return false;
        }

        public object FirstEnumItem()
        {
            int index = 0;
            foreach (var item in EnumList)
            {
                foreach (string displayName in DisplayNameList)
                {
                    if (converter.DisplayNames[index] == displayName)
                    {
                        return item;
                    }
                }
                index++;
            }
            return null;
        }
    }
}
