using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Resources;
using System.Reflection;
using System.Windows.Markup;
using Xvue.Framework.API.Attributes;

//public enum MyEnum
//{
//    [DisplayString("My Value")]
//    Value,
//    [DisplayString("This means On")]
//    On,
//    [DisplayString("Off means not On")]
//    Off,
//    [DisplayString("The great unknown")]
//    Unknown,
//    [DisplayString("I ain't got none")]
//    None
//}

//with localisation , must add the right entries in the corresponding resx for each language
//public enum MyEnum
//{
//    [DisplayString(ResourceKey = "MyEnum_Value")]
//    Value,
//    [DisplayString(ResourceKey = "MyEnum_On")]
//    On,
//    [DisplayString(ResourceKey = "MyEnum_Off")]
//    Off,
//    [DisplayString(ResourceKey = "MyEnum_Unknown")]
//    Unknown,
//    [DisplayString(ResourceKey = "MyEnum_None")]
//    None
//}

//xmlns:sys="clr-namespace:System;assembly=mscorlib" 

//use it in WPF as follows
//<Application.Resources>
//        <sm:EnumDisplayer Type="{x:Type FlickrNet:ContentType}" x:Key="contentTypes">
//</Application.Resources>
//And use it as a data source and value converter:
//<ComboBox ItemsSource="{Binding Source={StaticResource contentTypes},Path=DisplayNames}"
//          SelectedValue="{Binding Path=Batch.Photos/ContentType,
//                          Converter={StaticResource contentTypes}}" />

//
//<sm:EnumDisplayer Type="{x:Type FlickrNet:ContentType}" x:Key="contentTypes">
//    <sm:EnumDisplayEntry EnumValue="Photo" DisplayString="Photo (Default)"/>
//    <sm:EnumDisplayEntry EnumValue="Screenshot" DisplayString="Screenshot"/>
//    <sm:EnumDisplayEntry EnumValue="Other" DisplayString="Other"/>
//</sm:EnumDisplayer>

////<Window
////  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
////  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
////  xmlns:sys="clr-namespace:System;assembly=mscorlib"
////  SizeToContent="WidthAndHeight"
////  Title="Show Enums in a ListBox using Binding">
////  <Window.Resources>
////    <ObjectDataProvider MethodName="GetValues"
////                        ObjectType="{x:Type sys:Enum}"
////                        x:Key="AlignmentValues">
////      <ObjectDataProvider.MethodParameters>
////        <x:Type TypeName="HorizontalAlignment" />
////      </ObjectDataProvider.MethodParameters>
////    </ObjectDataProvider>
////  </Window.Resources>
////  <Border Margin="10" BorderBrush="Aqua"
////          BorderThickness="3" Padding="8">
////    <StackPanel Width="300">
////      <TextBlock>Choose the HorizontalAlignment
////                 value of the Button:</TextBlock>
////      <ListBox Name="myComboBox" SelectedIndex="0" Margin="8"
////               ItemsSource="{Binding Source={StaticResource
////                                             AlignmentValues}}"/>
////      <Button Content="Click Me!"
////              HorizontalAlignment="{Binding ElementName=myComboBox,
////                                            Path=SelectedItem}"/>
////    </StackPanel>
////  </Border>
////</Window>
namespace Xvue.Framework.API.Converters
{
    public class EnumDisplayEntry
    {
        public string EnumValue { get; set; }
        public string DisplayString { get; set; }
        public bool ExcludeFromDisplay { get; set; }
    }

    [ContentProperty("OverriddenDisplayEntries")]
    public class EnumDisplayerConverter : IValueConverter
    {
        private Type _enumType;
        private IDictionary displayValues;
        private IDictionary reverseValues;
        private List<EnumDisplayEntry> overriddenDisplayEntries;

        public EnumDisplayerConverter()
        {
        }

        public EnumDisplayerConverter(Type type)
        {
            if (!_enumType.IsEnum)
                throw new ArgumentException("parameter is not an Enumerated type", "type");
            _enumType = type;
        }

        public List<EnumDisplayEntry> OverriddenDisplayEntries
        {
            get
            {
                if (overriddenDisplayEntries == null)
                    overriddenDisplayEntries = new List<EnumDisplayEntry>();
                return overriddenDisplayEntries;
            }
        }

        public ReadOnlyCollection<string> DisplayNames
        {
            get
            {
                Type displayValuesType = typeof(Dictionary<,>)
                                            .GetGenericTypeDefinition().MakeGenericType(typeof(string), _enumType);
                this.displayValues = (IDictionary)Activator.CreateInstance(displayValuesType);

                this.reverseValues =
                   (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>)
                            .GetGenericTypeDefinition()
                            .MakeGenericType(_enumType, typeof(string)));

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
                        reverseValues.Add(displayString, enumValue);
                    }
                }
                return new List<string>((IEnumerable<string>)displayValues.Values).AsReadOnly();
            }
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

        private string GetBackupDisplayStringValue(object enumValue)
        {
            if (overriddenDisplayEntries != null && overriddenDisplayEntries.Count > 0)
            {
                EnumDisplayEntry foundEntry = overriddenDisplayEntries.Find(delegate(EnumDisplayEntry entry)
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

    }
}
