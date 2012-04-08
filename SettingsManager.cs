using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace MusicBeePlugin
{
    internal class SettingsManager
    {
        private static string _nowPlayingPattern;
        private static bool _displayNowPlayingString;
        private static string _settingsFilename;
        private static string _peristentStoragePath;

        public static string PersistentStorage
        {
            set
            {
                _peristentStoragePath = value;
                if (!Directory.Exists(_peristentStoragePath + "\\mb_skypenp\\"))
                    Directory.CreateDirectory(_peristentStoragePath + "\\mb_skypenp\\");
                _settingsFilename = _peristentStoragePath + "\\mb_skypenp\\settings.xml";
            }
        }

        public static string NowPlayingPattern
        {
            get { return _nowPlayingPattern; }
            set { _nowPlayingPattern = value; }
        }

        public static bool DisplayNowPlayingString
        {
            get { return _displayNowPlayingString; }
            set { _displayNowPlayingString = value; }
        }

        /// <summary>
        /// Writes an XML node.
        /// </summary>
        /// <param name="xmlDoc">The XML document.</param>
        /// <param name="nodeName">Name of the node.</param>
        /// <param name="value">The value.</param>
        /// <remarks></remarks>
        private static void WriteXmlNode(XmlDocument xmlDoc, string nodeName, string value)
        {
            XmlNode node = xmlDoc.SelectSingleNode("//" + nodeName);
            if (node == null)
            {
                XmlElement pattern = xmlDoc.CreateElement(nodeName);
                XmlNode root = xmlDoc.DocumentElement;
                pattern.InnerText = value;
                if (root != null) root.AppendChild(pattern);
            }
            else
            {
                node.InnerText = value;
            }
        }

        private static string ReadXmlNode(XmlDocument xmlDocument, string nodeName)
        {
            XmlNode node = xmlDocument.SelectSingleNode("//" + nodeName);
            return node != null ? node.InnerText : String.Empty;
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        /// <remarks></remarks>
        public static void SaveSettings()
        {
            XmlDocument settingsFile = OpenSettingsFile(_settingsFilename);
            settingsFile.Load(_settingsFilename);
            WriteXmlNode(settingsFile, "pattern", _nowPlayingPattern);
            WriteXmlNode(settingsFile, "displayNowPlaying",
                         _displayNowPlayingString.ToString(CultureInfo.InvariantCulture));
            settingsFile.Save(_settingsFilename);
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <remarks></remarks>
        public static void LoadSettings()
        {
            if (!File.Exists(_settingsFilename))
            {
                _nowPlayingPattern = "<Artist> - <Title>";
                _displayNowPlayingString = true;
                SaveSettings();
            }
            else
            {
                XmlDocument xmlDocument = OpenSettingsFile(_settingsFilename);
                _nowPlayingPattern = ReadXmlNode(xmlDocument, "pattern");
                Boolean.TryParse(ReadXmlNode(xmlDocument, "displayNowPlaying"),out _displayNowPlayingString);
            }
        }

        private static XmlDocument OpenSettingsFile(string filename)
        {
            if (!File.Exists(filename))
            {
                XmlDocument newXmlDocument = new XmlDocument();

                //Writing the XML Declaration
                XmlDeclaration xmlDeclaration = newXmlDocument.CreateXmlDeclaration("1.0", "utf-8", "yes");

                //Creating the root element
                XmlElement rootNode = newXmlDocument.CreateElement("Settings");
                newXmlDocument.InsertBefore(xmlDeclaration, newXmlDocument.DocumentElement);
                newXmlDocument.AppendChild(rootNode);
                newXmlDocument.Save(_settingsFilename);
            }
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filename);
            return xmlDocument;
        }

        public static void RemoveSettings()
        {
            if (File.Exists(_settingsFilename))
            {
                File.Delete(_settingsFilename);
            }
        }
    }
}