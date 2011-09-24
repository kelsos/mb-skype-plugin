using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using SKYPE4COMLib;

namespace MusicBeePlugin
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public partial class Plugin
    {
        /// <summary>
        /// 
        /// </summary>
        private const char Musicnote = '\u266B';
        //True if Skype is Running.
        /// <summary>
        /// 
        /// </summary>
        private static bool _skypeRunning;
        /// <summary>
        /// 
        /// </summary>
        private readonly PluginInfo _about = new PluginInfo();
        /// <summary>
        /// 
        /// </summary>
        private string _album;
        /// <summary>
        /// 
        /// </summary>
        private string _albumArtist;
        /// <summary>
        /// 
        /// </summary>
        private string _artist;
        /// <summary>
        /// 
        /// </summary>
        private Panel _configPanel;
        /// <summary>
        /// 
        /// </summary>
        private ContextMenuStrip _conmen;
        //The nowPlayingPattern & The string that will get displaying the mood box of Skype.
        /// <summary>
        /// 
        /// </summary>
        private bool _displayNote;
        /// <summary>
        /// 
        /// </summary>
        private bool _displayNowPlayingString;
        /// <summary>
        /// 
        /// </summary>
        private MusicBeeApiInterface _mbApiInterface;
        /// <summary>
        /// 
        /// </summary>
        private string _nowPlayingPattern;
        /// <summary>
        /// 
        /// </summary>
        private string _nowPlayingString;
        /// <summary>
        /// 
        /// </summary>
        private SquareButton _openContext;
        /// <summary>
        /// 
        /// </summary>
        private string _settingFile;
        /// <summary>
        /// 
        /// </summary>
        private SkypeClass _skype;
        /// <summary>
        /// 
        /// </summary>
        private TextBox _textBox;
        /// <summary>
        /// 
        /// </summary>
        private string _title;
        /// <summary>
        /// 
        /// </summary>
        private string _year;

        /// <summary>
        /// Gets the running processes and checks if Skype is running.
        /// </summary>
        /// <remarks></remarks>
        private static void CheckIfRunning()
        {
            foreach (var clsProcess in Process.GetProcesses())
            {
                if (!(clsProcess.ProcessName.Contains("skype") || clsProcess.ProcessName.Contains("Skype") ||
                    clsProcess.ProcessName.Contains("SKYPE"))) continue;
                _skypeRunning = true;
                break;
            }
        }

        /// <summary>
        /// Reads the values of the boolean variables and creates the Now Playing String to be Displayed
        /// </summary>
        /// <returns>the now playing string</returns>
        /// <remarks></remarks>
        private string GetNowPlayingString()
        {
            if (_displayNote == false && _displayNowPlayingString)
            {
                return "Now Playing: " + _nowPlayingString;
            }
            if (_displayNote == false && _displayNowPlayingString == false)
            {
                return _nowPlayingString;
            }
            if (_displayNote && _displayNowPlayingString == false)
            {
                return Musicnote + _nowPlayingString;
            }
            return String.Format("{0}Now Playing: {1}", Musicnote, _nowPlayingString);
        }

        /// <summary>
        /// Initialises the specified API interface PTR.
        /// </summary>
        /// <param name="apiInterfacePtr">The API interface PTR.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            _mbApiInterface =
                (MusicBeeApiInterface) Marshal.PtrToStructure(apiInterfacePtr, typeof (MusicBeeApiInterface));
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            _skypeRunning = false;
            _displayNote = true;
            _displayNowPlayingString = true;
            _about.PluginInfoVersion = PluginInfoVersion;
            _about.Name = "Skype: Now Playing";
            _about.Description = "Changes the skype mood to the currently playing track";
            _about.Author = "Kelsos";
            _about.TargetApplication = "Skype";
                // current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
            _about.Type = PluginType.General;
            _about.VersionMajor = Convert.ToInt16(v.Major); // your plugin version
            _about.VersionMinor = Convert.ToInt16(v.Minor);
            _about.Revision = Convert.ToInt16(v.Revision);
            _about.MinInterfaceVersion = MinInterfaceVersion;
            _about.MinApiRevision = MinApiRevision;
            _about.ReceiveNotifications = ReceiveNotificationFlags.PlayerEvents;
            _about.ConfigurationPanelHeight = 100; //Height of the panel.
            _settingFile = _mbApiInterface.Setting_GetPersistentStoragePath() + "mb_skypenp.ini";
            InitializeSkypeConnection();
            //Persistent Settings are saved in mb_skypenp.ini file in the application settings folder.
            LoadSettings(); //Loading the saved pattern.
            //Stop timer initialization at 5 seconds. The timer is disabled at start.
            //_stopTimer.Interval = 5000;
            //_stopTimer.Enabled = false;
            //_stopTimer.Tick += StopTimerTick;
            return _about;
        }

        /// <summary>
        /// Handles the tick event of the Stop Timer. On the event the method restores the default message, 
        /// and disables the timer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void StopTimerTick(object sender, EventArgs e)
        {
            RestoreDefaultMessage();
            //_stopTimer.Enabled = false;
        }

        /// <summary>
        /// Initializes the skype connection.
        /// </summary>
        /// <remarks></remarks>
        private void InitializeSkypeConnection()
        {
            try
            {
                CheckIfRunning();
                if (_skypeRunning)
                {
                    _skype = new SkypeClass(); //Skype Class object is used to access the Skype.
                    if (!GetRestoredStatusFromXml())
                    {
                        _skype.CurrentUserProfile.MoodText = GetPreviousMoodMessageFromXml();
                        SetRestoredStatusToXml(true);
                    }
                    else
                    {
                        SetPreviousMessageToXml(_skype.CurrentUserProfile.MoodText);
                    }
                    //previousMoodMessage = skype.CurrentUserProfile.MoodText; //The current Mood Text is saved. (It will be restored at pause/stop or application/plugin close).
                }
            }
            catch
            {
                _skypeRunning = false;
            }
        }

        /// <summary>
        /// Configures the specified panel handle.
        /// </summary>
        /// <param name="panelHandle">The panel handle.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Configure(IntPtr panelHandle)
        {
            // panelHandle will only be set if you set about.ConfigurationPanelHeight to a non-zero value
            // keep in mind the panel width is scaled according to the font the user has selected
            if (panelHandle != IntPtr.Zero)
            {
                using (var tp = new ToolTip())
                {
                    _textBox = new TextBox();
                    _configPanel = (Panel) Control.FromHandle(panelHandle);

                    //Label
                    using (
                        var patternBoxLabel = new Label
                                                  {
                                                      Bounds = new Rectangle(0, 0, _configPanel.Width, 22),
                                                      Text = "The pattern to be displayed:"
                                                  })
                    {
                        tp.SetToolTip(_textBox,
                                      "Tag indentifiers that can be used are: <Artist>, <AlbumArtist>, <Title>, <Year> and <Album>");
                        //Text Box
                        _textBox.Text = _nowPlayingPattern;
                        _textBox.Bounds = new Rectangle(0, patternBoxLabel.Height + 2, _configPanel.Width,
                                                        _textBox.Height);
                        _textBox.BackColor =
                            Color.FromArgb(_mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputControl,
                                                                                        ElementState.ElementStateDefault,
                                                                                        ElementComponent.
                                                                                            ComponentBackground));
                        _textBox.ForeColor =
                            Color.FromArgb(_mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputControl,
                                                                                        ElementState.ElementStateDefault,
                                                                                        ElementComponent.
                                                                                            ComponentForeground));
                        _textBox.BorderStyle = BorderStyle.FixedSingle;
                        _textBox.HideSelection = false;
                        //Button Creation
                        _openContext = new SquareButton
                                           {
                                               Bounds =
                                                   new Rectangle(_textBox.Right - (_textBox.Height + 1), 2,
                                                                 _textBox.Height - 1,
                                                                 _textBox.Height - 1),
                                               ButtonColor =
                                                   Color.FromArgb(
                                                       _mbApiInterface.Setting_GetSkinElementColour(
                                                           SkinElement.SkinInputPanelLabel,
                                                           ElementState.ElementStateDefault,
                                                           ElementComponent.ComponentBackground)),
                                               FontColor =
                                                   Color.FromArgb(
                                                       _mbApiInterface.Setting_GetSkinElementColour(
                                                           SkinElement.SkinInputControl,
                                                           ElementState.ElementStateDefault,
                                                           ElementComponent.ComponentBackground)),
                                               Parent = _textBox
                                           };
                        _openContext.BringToFront();
                        _openContext.TextAlign = ContentAlignment.MiddleCenter;
                        _openContext.Text = "...";
                        _textBox.Controls.Add(_openContext);
                        //CheckBox for "Now Playing" Display
                        using (var nowPlayingCheck = new CheckBox())
                        {
                            nowPlayingCheck.Bounds = new Rectangle(0, _textBox.Bottom + 2, _configPanel.Right,
                                                                   nowPlayingCheck.Height);
                            nowPlayingCheck.Text = "Display \"Now Playing:\" text in front of the pattern";
                            nowPlayingCheck.Checked = _displayNowPlayingString;
                            nowPlayingCheck.FlatStyle = FlatStyle.Flat;
                            nowPlayingCheck.AutoSize = true;
                            //CheckBox for \u266B Unicode Character Display
                            using (
                                var noteDisplayCheck = new CheckBox
                                                           {
                                                               Bounds =
                                                                   new Rectangle(0, nowPlayingCheck.Bottom,
                                                                                 _configPanel.Right,
                                                                                 nowPlayingCheck.Height),
                                                               Text =
                                                                   String.Format(
                                                                       "Display {0} char infront of \"Now Playing:\"",
                                                                       Musicnote),
                                                               Checked = _displayNote,
                                                               FlatStyle = FlatStyle.Flat,
                                                               AutoSize = true
                                                           })
                            {
                                _configPanel.Controls.AddRange(new Control[]
                                                                   {
                                                                       patternBoxLabel, _textBox, nowPlayingCheck,
                                                                       noteDisplayCheck
                                                                   });
                                //EventHandlers Created.
                                _openContext.MouseClick += OpenContextMouseClick;
                                _textBox.TextChanged += TextBoxTextChanged;
                                nowPlayingCheck.CheckedChanged += NowPlayingCheckChanged;
                                noteDisplayCheck.CheckedChanged += NoteDisplayCheckChanged;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Event Handler that changes the value of the _displayNowPlayingString when the corresponding checkbox changes value.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void NowPlayingCheckChanged(object sender, EventArgs e)
        {
            _displayNowPlayingString = !_displayNowPlayingString;
            SaveSettings();
        }

        /// <summary>
        /// Event Handler that changes the value of the _displayNote when the corresponding checkbox changes value.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void NoteDisplayCheckChanged(object sender, EventArgs e)
        {
            _displayNote = !_displayNote;
            SaveSettings();
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
                var pattern = xmlDoc.CreateElement(nodeName);
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
            if (!File.Exists(_settingFile))
            {
                var xmNew = new XmlDocument();

                //Writing the XML Declaration
                var xmlDec = xmNew.CreateXmlDeclaration("1.0", "utf-8", "yes");

                //Creating the root element
                var rootNode = xmNew.CreateElement("Settings");
                xmNew.InsertBefore(xmlDec, xmNew.DocumentElement);
                xmNew.AppendChild(rootNode);
                xmNew.Save(_settingFile);
            }
            var xmD = new XmlDocument();
            xmD.Load(_settingFile);
            WriteXmlNode(xmD, "pattern", _nowPlayingPattern);
            WriteXmlNode(xmD, "displaynote", _displayNote.ToString());
            WriteXmlNode(xmD, "displayNowPlaying", _displayNowPlayingString.ToString());
            xmD.Save(_settingFile);
        }

        /// <summary>
        /// Gets the previous Mood message from the Settings XML file.
        /// </summary>
        /// <returns>The stored message if the file exists, or a null string if the file is non existant</returns>
        /// <remarks></remarks>
        private string GetPreviousMoodMessageFromXml()
        {
            if (File.Exists(_settingFile))
            {
                var xmD = new XmlDocument();
                xmD.Load(_settingFile);
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
            if (!File.Exists(_settingFile)) return;
            var xmD = new XmlDocument();
            xmD.Load(_settingFile);
            WriteXmlNode(xmD, "previousMoodMessage", previousMoodMessage);
            xmD.Save(_settingFile);
        }

        /// <summary>
        /// Gets the restored status from XML.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool GetRestoredStatusFromXml()
        {
            if (File.Exists(_settingFile))
            {
                var xmD = new XmlDocument();
                xmD.Load(_settingFile);


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
            if (!File.Exists(_settingFile))
                return;
            var xmD = new XmlDocument();
            xmD.Load(_settingFile);
            WriteXmlNode(xmD, "moodRestored", restoredStatus.ToString());
            xmD.Save(_settingFile);
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
            if (!File.Exists(_settingFile))
            {
                _nowPlayingPattern = "<Artist> - <Title>";
                _displayNote = true;
                _displayNowPlayingString = true;
            }
            else
            {
                var xmD = new XmlDocument();
                xmD.Load(_settingFile);
                _nowPlayingPattern = ReadPatternFromXml(xmD, "pattern");
                _displayNote = ReadBooleanValuesFromXml(xmD, "displaynote");
                _displayNowPlayingString = ReadBooleanValuesFromXml(xmD, "displayNowPlaying");
            }
        }

        /// <summary>
        /// Creates the context menu.
        /// </summary>
        /// <remarks></remarks>
        private void ContextMenuCreator()
        {
            _conmen = new ContextMenuStrip();

            //Creation of the ToolStripMenuItems
            var separator = new ToolStripSeparator();
            var defaultFormat = new ToolStripMenuItem();
            var artist = new ToolStripMenuItem();
            var title = new ToolStripMenuItem();
            var setNull = new ToolStripMenuItem();
            var year = new ToolStripMenuItem();
            var album = new ToolStripMenuItem();
            var albumArtist = new ToolStripMenuItem();

            //Setting the text values of the new ToolStripMenuItems
            setNull.Text = "Empty Field";
            defaultFormat.Text = "Default";
            artist.Text = "Artist";
            albumArtist.Text = "Album Artist";
            title.Text = "Title";
            year.Text = "Year";
            album.Text = "Album";

            //Adding the MenuItems to the Context menu.
            _conmen.Items.Add(setNull);
            _conmen.Items.Add(separator);
            _conmen.Items.Add(defaultFormat);
            _conmen.Items.Add(artist);
            _conmen.Items.Add(albumArtist);
            _conmen.Items.Add(title);
            _conmen.Items.Add(year);
            _conmen.Items.Add(album);

            //Creating the EventHandlers for the Click Event oft
            defaultFormat.Click += DefaultFormatClicked;
            setNull.Click += SetNullClicked;
            artist.Click += ArtistClicked;
            albumArtist.Click += AlbumArtistClicked;
            title.Click += TitleClicked;
            year.Click += YearClicked;
            album.Click += AlbumClicked;

            _conmen.BackColor =
                Color.FromArgb(_mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputPanel,
                                                                            ElementState.ElementStateDefault,
                                                                            ElementComponent.ComponentBackground));
            _conmen.ForeColor =
                Color.FromArgb(_mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputPanel,
                                                                            ElementState.ElementStateDefault,
                                                                            ElementComponent.ComponentForeground));
        }

        /// <summary>
        /// Handles the textbox text changed event and saves the settings.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            // save the value
            _nowPlayingPattern = _textBox.Text;
            SaveSettings();
        }

        // MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
        /// <summary>
        /// Closes the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <remarks></remarks>
        public void Close(PluginCloseReason reason)
        {
            if (GetRestoredStatusFromXml()) return;
            _skype.CurrentUserProfile.MoodText = GetPreviousMoodMessageFromXml();
            SetRestoredStatusToXml(true);
        }

        /// <summary>
        /// Creates the nowPlayingString by replacing the Pattern witht he values of the respective fields.
        /// </summary>
        /// <remarks></remarks>
        private void GetNowPlayingTrackString()
        {
            _nowPlayingString = _nowPlayingPattern;

            //Get the values
            _artist = _mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Artist);
            _title = _mbApiInterface.NowPlaying_GetFileTag(MetaDataType.TrackTitle);
            _albumArtist = _mbApiInterface.NowPlaying_GetFileTag(MetaDataType.AlbumArtist);
            _year = _mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Year);
            _album = _mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Album);

            //Regular Expressions for each supported TAG.
            var artistExpression = new Regex("<Artist>");
            var titleExpression = new Regex("<Title>");
            var albumArtistExpression = new Regex("<AlbumArtist>");
            var yearExpression = new Regex("<Year>");
            var albumExpression = new Regex("<Album>");

            //Replacing each tag with the current value of the specific tag
            _nowPlayingString = artistExpression.Replace(_nowPlayingString, _artist);
            _nowPlayingString = titleExpression.Replace(_nowPlayingString, _title);
            _nowPlayingString = albumArtistExpression.Replace(_nowPlayingString, _albumArtist);
            _nowPlayingString = yearExpression.Replace(_nowPlayingString, _year);
            _nowPlayingString = albumExpression.Replace(_nowPlayingString, _album);
        }

        // uninstall this plugin - clean up any persisted files
        /// <summary>
        /// Uninstalls this instance.
        /// </summary>
        /// <remarks></remarks>
        public void Uninstall()
        {
            if (File.Exists(_settingFile))
            {
                File.Delete(_settingFile);
            }
        }

        // receive event notifications from MusicBee
        // only required if about.ReceiveNotificationFlags = PlayerEvents
        /// <summary>
        /// Receives the notification.
        /// </summary>
        /// <param name="sourceFileUrl">The source file URL.</param>
        /// <param name="type">The type.</param>
        /// <remarks></remarks>
        public void ReceiveNotification(string sourceFileUrl, NotificationType type)
        {
            // perform some action depending on the notification type
            switch (type)
            {
                case NotificationType.PluginStartup:
                    break;
                case NotificationType.TrackChanged:
                    //if (_stopTimer.Enabled)
                    //    _stopTimer.Enabled = false;
                    GetNowPlayingTrackString();
                    if (!_skypeRunning)
                    {
                        InitializeSkypeConnection();
                    }
                    if (_skypeRunning)
                    {
                        if (GetRestoredStatusFromXml())
                        {
                            SetRestoredStatusToXml(false);
                        }
                        _skype.CurrentUserProfile.MoodText = GetNowPlayingString();
                    }
                    break;
                case NotificationType.PlayStateChanged:
                    switch (_mbApiInterface.Player_GetPlayState())
                    {
                        case PlayState.Undefined:
                            break;
                        case PlayState.Loading:
                            break;
                        case PlayState.Playing:
                            break;
                        case PlayState.Paused:
                            break;
                        case PlayState.Stopped:
                            // When the State is activated so is the _stopTimer. 
                            //_stopTimer.Enabled = true;
                            break;
                    }
                    break;
                case NotificationType.AutoDjStarted:
                    break;
                case NotificationType.AutoDjStopped:
                    break;
                case NotificationType.VolumeMuteChanged:
                    break;
                case NotificationType.VolumeLevelChanged:
                    break;
                case NotificationType.NowPlayingListChanged:
                    break;
                case NotificationType.NowPlayingArtworkReady:
                    break;
                case NotificationType.NowPlayingLyricsReady:
                    break;
                case NotificationType.TagsChanged:
                    break;
            }
        }

        // return an array of lyric or artwork provider names this plugin supports
        // the providers will be iterated through one by one and passed to the RetrieveLyrics/ RetrieveArtwork function in order set by the user in the MusicBee Tags(2) preferences screen until a match is found
        /// <summary>
        /// Gets the providers.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string[] GetProviders()
        {
            return null;
        }

        // return lyrics for the requested artist/title from the requested provider
        // only required if PluginType = LyricsRetrieval
        // return null if no lyrics are found
        /// <summary>
        /// Retrieves the lyrics.
        /// </summary>
        /// <param name="sourceFileUrl">The source file URL.</param>
        /// <param name="artist">The artist.</param>
        /// <param name="trackTitle">The track title.</param>
        /// <param name="album">The album.</param>
        /// <param name="synchronisedPreferred">if set to <c>true</c> [synchronised preferred].</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string RetrieveLyrics(string sourceFileUrl, string artist, string trackTitle, string album,
                                            bool synchronisedPreferred, string provider)
        {
            return null;
        }

        // return Base64 string representation of the artwork binary data from the requested provider
        // only required if PluginType = ArtworkRetrieval
        // return null if no artwork is found
        /// <summary>
        /// Retrieves the artwork.
        /// </summary>
        /// <param name="sourceFileUrl">The source file URL.</param>
        /// <param name="albumArtist">The album artist.</param>
        /// <param name="album">The album.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string RetrieveArtwork(string sourceFileUrl, string albumArtist, string album, string provider)
        {
            //Return Convert.ToBase64String(artworkBinaryData)
            return null;
        }

        /// <summary>
        /// Sets the null clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void SetNullClicked(object sender, EventArgs e)
        {
            _textBox.Text = "";
        }

        /// <summary>
        /// Artists the clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void ArtistClicked(object sender, EventArgs e)
        {
            if (_textBox.SelectionLength > 0)
            {
                _textBox.SelectedText = "<Artist>";
            }
            else
            {
                _textBox.Text += "<Artist>";
            }
        }

        /// <summary>
        /// Albums the artist clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void AlbumArtistClicked(object sender, EventArgs e)
        {
            if (_textBox.SelectionLength > 0)
            {
                _textBox.SelectedText = "<AlbumArtist>";
            }
            else
            {
                _textBox.Text += "<AlbumArtist>";
            }
        }

        /// <summary>
        /// Titles the clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void TitleClicked(object sender, EventArgs e)
        {
            if (_textBox.SelectionLength > 0)
            {
                _textBox.SelectedText = "<Title>";
            }
            else
            {
                _textBox.Text += "<Title>";
            }
        }

        /// <summary>
        /// Years the clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void YearClicked(object sender, EventArgs e)
        {
            if (_textBox.SelectionLength > 0)
            {
                _textBox.SelectedText = "<Year>";
            }
            else
            {
                _textBox.Text += "<Year>";
            }
        }

        /// <summary>
        /// Albums the clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void AlbumClicked(object sender, EventArgs e)
        {
            if (_textBox.SelectionLength > 0)
            {
                _textBox.SelectedText = "<Album>";
            }
            else
            {
                _textBox.Text += "<Album>";
            }
        }

        /// <summary>
        /// Opens the context mouse click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void OpenContextMouseClick(object sender, MouseEventArgs e)
        {
            ContextMenuCreator();
            _conmen.Show(_openContext, new Point(_openContext.Width, 0));
        }

        /// <summary>
        /// Defaults the format clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void DefaultFormatClicked(object sender, EventArgs e)
        {
            _textBox.Text = "<Artist> - <Title>";
        }
        private void RestoreDefaultMessage()
        {
            if (!_skypeRunning) return;
            if (GetRestoredStatusFromXml()) return;
            _skype.CurrentUserProfile.MoodText = GetPreviousMoodMessageFromXml();
            SetRestoredStatusToXml(true);
        }
    }
}