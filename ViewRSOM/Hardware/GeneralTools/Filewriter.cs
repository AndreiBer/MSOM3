/////////////////////////////////////////////////////////////
// "class FileWriter"
//
// provides methods to write values and comments in a text File
// each method returns true, when writing was successfull
//////////////////////////////////////////////////////////////
using System;
using System.IO;

namespace General.Tools.Filewriter
{

    public class Filewriter
    {

        
        public bool CreateFileAndWriteHeader(string filename, string header)
        {

            string output_filename = filename;
            StreamWriter writer = new StreamWriter(File.Open(output_filename, FileMode.Create));

            try
            {
                writer.WriteLine(""+header+"");

            }
            catch
            {
                return false;
            }
            writer.Close();
            return true;
        }
        public bool writeStringToFile(string filename, string mystring)
        {

            string output_filename = filename;
            StreamWriter writer = new StreamWriter(File.Open(output_filename, FileMode.Append));

            try
            {
                writer.WriteLine(mystring);

            }
            catch
            {
                return false;
            }
            writer.Close();
            return true;
        }
         
        //overload for one double value
        public bool writeDoublesToFileAsString(string filename, double value1)
        {

            string output_filename = filename;
            StreamWriter writer = new StreamWriter(File.Open(output_filename, FileMode.Append));

            try
            {
                writer.WriteLine(Convert.ToString(value1));
                
            }
            catch
            {
                return false;
            }
            writer.Close();
            return true;
        }
        //overload for two double values
        public bool writeDoublesToFileAsString(string filename, double value1, double value2)
        {

            string output_filename = filename;
            StreamWriter writer = new StreamWriter(File.Open(output_filename, FileMode.Append));

            try
            {
                writer.WriteLine(Convert.ToString(value1)+";"+Convert.ToString(value2));
            }
            catch
            {
                return false;
            }
            writer.Close();
            return true;
        }

        //overload for five double values
        public bool writeDoublesToFileAsString(string filename, double value1, double value2, double value3, double value4, double value5)
        {

            string output_filename = filename;
            StreamWriter writer = new StreamWriter(File.Open(output_filename, FileMode.Append));

            try
            {
                writer.WriteLine(Convert.ToString(value1) + ";" + Convert.ToString(value2) + ";" + Convert.ToString(value3) + ";" + Convert.ToString(value4) + ";" + Convert.ToString(value5));
                
            }
            catch
            {
                return false;
            }
            writer.Close();
            return true;
        }

        //overload for six double values
        public bool writeDoublesToFileAsString(string filename, double value1, double value2, double value3, double value4, double value5, double value6)
        {

            string output_filename = filename;
            StreamWriter writer = new StreamWriter(File.Open(output_filename, FileMode.Append));

            try
            {
                writer.WriteLine(Convert.ToString(value1) + ";" + Convert.ToString(value2) + ";" + Convert.ToString(value3) + ";" + Convert.ToString(value4) + ";" + Convert.ToString(value5) + ";" + Convert.ToString(value6));

            }
            catch
            {
                return false;
            }
            writer.Close();
            return true;
        }

    }
}