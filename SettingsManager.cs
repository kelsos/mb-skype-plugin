using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace MusicBeePlugin
{
    class SettingsManager
    {
        private string _nowPlayingPattern;
        private bool _displayNowPlayingString;
        private static string _settingsFile;
        private static string _peristentStoragePath;

        public string PersistentStorage
        {
            set { _peristentStoragePath = value; }
        }
        
        public string NowPlayingPattern
        {
            get { return _nowPlayingPattern; }
            set { _nowPlayingPattern = value; }
        }

        public bool DisplayNowPlayingString
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

        /// <summary>
        /// Saves the settings.
        /// </summary>
        /// <remarks></remarks>
        private void SaveSettings()
        {
            if (!File.Exists(_settingsFile))
            {
                XmlDocument xmNew = new XmlDocument();

                //Writing the XML Declaration
                XmlDeclaration xmlDec = xmNew.CreateXmlDeclaration("1.0", "utf-8", "yes");

                //Creating the root element
                XmlElement rootNode = xmNew.CreateElement("Settings");
                xmNew.InsertBefore(xmlDec, xmNew.DocumentElement);
                xmNew.AppendChild(rootNode);
                xmNew.Save(_settingsFile);
            }
            XmlDocument xmD = new XmlDocument();
            xmD.Load(_settingsFile);
            WriteXmlNode(xmD, "pattern", _nowPlayingPattern);
            WriteXmlNode(xmD, "displayNowPlaying", _displayNowPlayingString.ToString(CultureInfo.InvariantCulture));
            xmD.Save(_settingsFile);
        }

        /// <summary>
        /// Gets the previous Mood message from the Settings XML file.
        /// </summary>
        /// <returns>The stored message if the file exists, or a null string if the file is non existant</returns>
        /// <remarks></remarks>
        private string GetPreviousMoodMessageFromXml()
        {
            if (File.Exists(_settingsFile))
            {
                XmlDocument xmD = new XmlDocument();
                xmD.Load(_settingsFile);
                return ReadPatternFromXml(xmD, "previousMoodMessage");
            }
            return "";
        }
        /// <summary>
        /// Writes the previous Mood message to the Settings XML file.
        /// </summary>
        /// <param name="previousMoodMessage">The previous mood message.</param>
        /// <remarks></remarks>
        private void SetPreviousMessageToXml(string previousMoodMessage)
        {
            if (!File.Exists(_settingsFile)) return;
            XmlDocument xmD = new XmlDocument();
            xmD.Load(_settingsFile);
            WriteXmlNode(xmD, "previousMoodMessage", previousMoodMessage);
            xmD.Save(_settingsFile);
        }

        /// <summary>
        /// Gets the restored status from XML.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool GetRestoredStatusFromXml()
        {
            if (File.Exists(_settingsFile))
            {
                XmlDocument xmD = new XmlDocument();
                xmD.Load(_settingsFile);


                return Convert.ToBoolean(ReadBooleanValuesFromXml(xmD, "moodRestored"));
            }
            return true;
        }

        /// <summary>
        /// Sets the restored status to XML.
        /// </summary>
        /// <param name="restoredStatus">if set to <c>true</c> [restored status].</param>
        /// <remarks></remarks>
        private void SetRestoredStatusToXml(bool restoredStatus)
        {
            if (!File.Exists(_settingsFile))
                return;
            XmlDocument xmD = new XmlDocument();
            xmD.Load(_settingsFile);
            WriteXmlNode(xmD, "moodRestored", restoredStatus.ToString(CultureInfo.InvariantCulture));
            xmD.Save(_settingsFile);
        }

        /// <summary>
        /// Reads the pattern from XML.
        /// </summary>
        /// <param name="xmlDoc">The XML doc.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static string ReadPatternFromXml(XmlDocument xmlDoc, string pattern)
        {
            var node = xmlDoc.SelectSingleNode("//" + pattern);
            return node != null ? node.InnerText : "<Artist> - <Title>";
        }

        /// <summary>
        /// Reads the boolean values from XML.
        /// </summary>
        /// <param name="xmlDoc">The XML doc.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static bool ReadBooleanValuesFromXml(XmlDocument xmlDoc, string pattern)
        {
            var node = xmlDoc.SelectSingleNode("//" + pattern);
            return node == null || Convert.ToBoolean(node.InnerText);
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <remarks></remarks>
        private void LoadSettings()
        {
            if (!File.Exists(_settingsFile))
            {
                _nowPlayingPattern = "<Artist> - <Title>";
                _displayNowPlayingString = true;
            }
            else
            {
                XmlDocument xmD = new XmlDocument();
                xmD.Load(_settingsFile);
                _nowPlayingPattern = ReadPatternFromXml(xmD, "pattern");
                _displayNowPlayingString = ReadBooleanValuesFromXml(xmD, "displayNowPlaying");
            }
        }
        public static void RemoveSettings()
        {
            if (File.Exists(_settingsFile))
            {
                File.Delete(_settingsFile);
            }
        }

        public void RestoreDefaultMessage()
        {
            //if (!_skypeRunning) return;
            //if (GetRestoredStatusFromXml()) return;
            //proxy.Connect();
            //proxy.SendMessage("SET PROFILE MOOD_TEXT " + GetPreviousMoodMessageFromXml());
            //SetRestoredStatusToXml(true);
        }
    }
}
