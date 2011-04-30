using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using SKYPE4COMLib;

namespace MusicBeePlugin
{
	public partial class Plugin
	{
		private MusicBeeApiInterface mbApiInterface;
		private PluginInfo about = new PluginInfo();
		private SkypeClass skype;

		//The initial mood is saved in order to restore it.
		private string previousMoodMessage;

		//Track tag related Variables.
		private string artist;
		private string title;
		private string albumArtist;
		private string year;
		private string album;

		//The nowPlayingPattern & The string that will get displaying the mood box of Skype.
		private string nowPlayingPattern;
		private string nowPlayingString;

		//The Path to the file where the settings will be saved.
		private string settingFile;

		//The Configuration panel items are declared in the DLL scope in order to be accessible from the various methods.
		private TextBox textBox;
		private SquareButton openContext;
		private Panel configPanel;
		private ContextMenuStrip conmen;

		//True if Skype is Running.
		private static bool skypeRunning;

		private bool displayNote;
		private bool displayNowPlayingString;
		private const char _MUSICNOTE = '\u266B';

		/// <summary>
		/// Gets the running processes and checks if Skype is running.
		/// </summary>
		private void checkIfRunning()
		{
			foreach (Process clsProcess in Process.GetProcesses())
			{
				if (clsProcess.ProcessName.Contains("skype") || clsProcess.ProcessName.Contains("Skype") || clsProcess.ProcessName.Contains("SKYPE"))
				{
					skypeRunning = true;
					break;
				}
			}
		}

		/// <summary>
		/// Reads the values of the boolean variables and creates the Now Playing String to be Displayed
		/// </summary>
		/// <returns>the now playing string</returns>
		private string getNowPlayingString()
		{
			if (displayNote == false && displayNowPlayingString == true)
			{
				return "Now Playing: " + nowPlayingString;
			}
			else if (displayNote == false && displayNowPlayingString == false)
			{
				return nowPlayingString;
			}
			else if (displayNote == true && displayNowPlayingString == false)
			{
				return _MUSICNOTE + nowPlayingString;
			}
			else
			{
				return _MUSICNOTE + "Now Playing: " + nowPlayingString;
			}
		}

		public PluginInfo Initialise(IntPtr apiInterfacePtr)
		{
			mbApiInterface = (MusicBeeApiInterface)Marshal.PtrToStructure(apiInterfacePtr, typeof(MusicBeeApiInterface));
			Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			skypeRunning = false;
			displayNote = true;
			displayNowPlayingString = true;
			about.PluginInfoVersion = PluginInfoVersion;
			about.Name = "Skype: Now Playing";
			about.Description = "Changes the skype mood to the currently playing track";
			about.Author = "Kelsos";
			about.TargetApplication = "Skype";   // current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
			about.Type = PluginType.General;
			about.VersionMajor = Convert.ToInt16(v.Major);  // your plugin version
			about.VersionMinor = Convert.ToInt16(v.Minor);
			about.Revision = Convert.ToInt16(v.Revision);
			about.MinInterfaceVersion = MinInterfaceVersion;
			about.MinApiRevision = MinApiRevision;
			about.ReceiveNotifications = ReceiveNotificationFlags.PlayerEvents;
			about.ConfigurationPanelHeight = 100;  //Height of the panel.
			initializeSkypeConnection();
			//Persistent Settings are saved in mb_skypenp.ini file in the application settings folder.
			settingFile = mbApiInterface.Setting_GetPersistentStoragePath() + "mb_skypenp.ini";
			loadSettings(); //Loading the saved pattern.
			return about;
		}

		private void initializeSkypeConnection()
		{
			try
			{
				checkIfRunning();
				if (skypeRunning)
				{
					skype = new SkypeClass(); //The SkypeClass object is used to access skype from the dll.
					previousMoodMessage = skype.CurrentUserProfile.MoodText; //The current Mood Text is saved. (It will be restored at pause/stop or application/plugin close).
				}
			}
			catch
			{
				skypeRunning = false;
			}
		}

		public bool Configure(IntPtr panelHandle)
		{
			// panelHandle will only be set if you set about.ConfigurationPanelHeight to a non-zero value
			// keep in mind the panel width is scaled according to the font the user has selected
			if (panelHandle != IntPtr.Zero)
			{
				ToolTip tp = new ToolTip();
				textBox = new TextBox();
				configPanel = (Panel)Panel.FromHandle(panelHandle);

				//Label
				Label lbl = new Label();
				lbl.Bounds = new Rectangle(0, 0, configPanel.Width, 22);
				lbl.Text = "The pattern to be displayed:";

				tp.SetToolTip(textBox, "Tag indentifiers that can be used are: <Artist>, <AlbumArtist>, <Title>, <Year> and <Album>");

				//Text Box
				textBox.Text = nowPlayingPattern;
				textBox.Bounds = new Rectangle(0, lbl.Height + 2, configPanel.Width, textBox.Height);
				textBox.BackColor = Color.FromArgb(mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputControl, ElementState.ElementStateDefault, ElementComponent.ComponentBackground));
				textBox.ForeColor = Color.FromArgb(mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputControl, ElementState.ElementStateDefault, ElementComponent.ComponentForeground));
				textBox.BorderStyle = BorderStyle.FixedSingle;
				textBox.HideSelection = false;

				//Button Creation
				openContext = new SquareButton();
				openContext.Bounds = new Rectangle(textBox.Right - (textBox.Height + 1), 2, textBox.Height - 1, textBox.Height - 1);
				openContext.ButtonColor = Color.FromArgb(mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputPanelLabel, ElementState.ElementStateDefault, ElementComponent.ComponentBackground));
				openContext.FontColor = Color.FromArgb(mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputControl, ElementState.ElementStateDefault, ElementComponent.ComponentBackground));
				openContext.Parent = textBox;
				openContext.BringToFront();
				openContext.TextAlign = ContentAlignment.MiddleCenter;
				openContext.Text = "...";
				textBox.Controls.Add(openContext);

				//CheckBox for "Now Playing" Display
				CheckBox nowPlayingCheck = new CheckBox();
				nowPlayingCheck.Bounds = new Rectangle(0, textBox.Bottom + 2, configPanel.Right, nowPlayingCheck.Height);
				nowPlayingCheck.Text = "Display \"Now Playing:\" text in front of the pattern";
				nowPlayingCheck.Checked = displayNowPlayingString;
				nowPlayingCheck.FlatStyle = FlatStyle.Flat;
				nowPlayingCheck.AutoSize = true;

				//CheckBox for \u266B Unicode Character Display
				CheckBox noteDisplayCheck = new CheckBox();
				noteDisplayCheck.Bounds = new Rectangle(0, nowPlayingCheck.Bottom, configPanel.Right, nowPlayingCheck.Height);
				noteDisplayCheck.Text = "Display " + _MUSICNOTE + " char infront of \"Now Playing:\"";
				noteDisplayCheck.Checked = displayNote;
				noteDisplayCheck.FlatStyle = FlatStyle.Flat;
				noteDisplayCheck.AutoSize = true;

				configPanel.Controls.AddRange(new Control[] { lbl, textBox, nowPlayingCheck, noteDisplayCheck });
				//EventHandlers Created.
				openContext.MouseClick += openContext_MouseClick;
				textBox.TextChanged += textBox_TextChanged;
				nowPlayingCheck.CheckedChanged += nowPlayingCheck_Changed;
				noteDisplayCheck.CheckedChanged += noteDisplayCheck_Changed;
			}
			return false;
		}

		private void nowPlayingCheck_Changed(object sender, EventArgs e)
		{
			displayNowPlayingString = !displayNowPlayingString;
			saveSettings();
		}

		private void noteDisplayCheck_Changed(object sender, EventArgs e)
		{
			displayNote = !displayNote;
			saveSettings();
		}

		private void writeXmlNode(XmlDocument xmlDoc, string nodeName, string value)
		{
			XmlNode node = xmlDoc.SelectSingleNode("//" + nodeName);
			if (node == null)
			{
				XmlElement pattern = xmlDoc.CreateElement(nodeName);
				XmlNode root = xmlDoc.DocumentElement;
				pattern.InnerText = value;
				root.AppendChild(pattern);
			}
			else
			{
				node.InnerText = value;
			}
		}

		/// <summary>
		/// Saves the settings.
		/// </summary>
		private void saveSettings()
		{
			if (!File.Exists(settingFile))
			{
				XmlDocument xmNew = new XmlDocument();

				//Writing the XML Declaration
				XmlDeclaration xmlDec = xmNew.CreateXmlDeclaration("1.0", "utf-8", "yes");

				//Creating the root element
				XmlElement rootNode = xmNew.CreateElement("Settings");
				xmNew.InsertBefore(xmlDec, xmNew.DocumentElement);
				xmNew.AppendChild(rootNode);
				xmNew.Save(settingFile);
			}
			XmlDocument xmD = new XmlDocument();
			xmD.Load(settingFile);
			writeXmlNode(xmD, "pattern", nowPlayingPattern);
			writeXmlNode(xmD, "displaynote", displayNote.ToString());
			writeXmlNode(xmD, "displayNowPlaying", displayNowPlayingString.ToString());
			xmD.Save(settingFile);
		}

		private string readPatternFromXml(XmlDocument xmlDoc, string pattern)
		{
			XmlNode node = xmlDoc.SelectSingleNode("//" + pattern);
			if (node != null)
			{
				return node.InnerText;
			}
			else
			{
				return "<Artist> - <Title>";
			}
		}

		private bool readCheckBoxValuesFromXml(XmlDocument xmlDoc, string pattern)
		{
			XmlNode node = xmlDoc.SelectSingleNode("//" + pattern);
			if (node != null)
			{
				return Convert.ToBoolean(node.InnerText);
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Loads the settings.
		/// </summary>
		private void loadSettings()
		{
			if (!File.Exists(settingFile))
			{
				nowPlayingPattern = "<Artist> - <Title>";
				displayNote = true;
				displayNowPlayingString = true;
			}
			else
			{
				XmlDocument xmD = new XmlDocument();
				xmD.Load(settingFile);
				nowPlayingPattern = readPatternFromXml(xmD, "pattern");
				displayNote = readCheckBoxValuesFromXml(xmD, "displaynote");
				displayNowPlayingString = readCheckBoxValuesFromXml(xmD, "displayNowPlaying");
			}
		}

		/// <summary>
		/// Creates the context menu.
		/// </summary>
		private void contextMenuCreator()
		{
			conmen = new ContextMenuStrip();

			//Creation of the ToolStripMenuItems
			ToolStripSeparator separator = new ToolStripSeparator();
			ToolStripMenuItem defaultFormat = new ToolStripMenuItem();
			ToolStripMenuItem artist = new ToolStripMenuItem();
			ToolStripMenuItem title = new ToolStripMenuItem();
			ToolStripMenuItem setNull = new ToolStripMenuItem();
			ToolStripMenuItem year = new ToolStripMenuItem();
			ToolStripMenuItem album = new ToolStripMenuItem();
			ToolStripMenuItem albumArtist = new ToolStripMenuItem();

			//Setting the text values of the new ToolStripMenuItems
			setNull.Text = "Empty Field";
			defaultFormat.Text = "Default";
			artist.Text = "Artist";
			albumArtist.Text = "Album Artist";
			title.Text = "Title";
			year.Text = "Year";
			album.Text = "Album";

			//Adding the MenuItems to the Context menu.
			conmen.Items.Add(setNull);
			conmen.Items.Add(separator);
			conmen.Items.Add(defaultFormat);
			conmen.Items.Add(artist);
			conmen.Items.Add(albumArtist);
			conmen.Items.Add(title);
			conmen.Items.Add(year);
			conmen.Items.Add(album);

			//Creating the EventHandlers for the Click Event oft
			defaultFormat.Click += defaultFormat_Clicked;
			setNull.Click += setNull_Clicked;
			artist.Click += artist_Clicked;
			albumArtist.Click += albumArtist_Clicked;
			title.Click += title_Clicked;
			year.Click += year_Clicked;
			album.Click += album_Clicked;

			conmen.BackColor = Color.FromArgb(mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputPanel, ElementState.ElementStateDefault, ElementComponent.ComponentBackground));
			conmen.ForeColor = Color.FromArgb(mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputPanel, ElementState.ElementStateDefault, ElementComponent.ComponentForeground));
		}

		#region context menu eventHandlers

		private void setNull_Clicked(object sender, EventArgs e)
		{
			textBox.Text = "";
		}

		private void artist_Clicked(object sender, EventArgs e)
		{
			if (textBox.SelectionLength > 0)
			{
				textBox.SelectedText = "<Artist>";
			}
			else
			{
				textBox.Text += "<Artist>";
			}
		}

		private void albumArtist_Clicked(object sender, EventArgs e)
		{
			if (textBox.SelectionLength > 0)
			{
				textBox.SelectedText = "<AlbumArtist>";
			}
			else
			{
				textBox.Text += "<AlbumArtist>";
			}
		}

		private void title_Clicked(object sender, EventArgs e)
		{
			if (textBox.SelectionLength > 0)
			{
				textBox.SelectedText = "<Title>";
			}
			else
			{
				textBox.Text += "<Title>";
			}
		}

		private void year_Clicked(object sender, EventArgs e)
		{
			if (textBox.SelectionLength > 0)
			{
				textBox.SelectedText = "<Year>";
			}
			else
			{
				textBox.Text += "<Year>";
			}
		}

		private void album_Clicked(object sender, EventArgs e)
		{
			if (textBox.SelectionLength > 0)
			{
				textBox.SelectedText = "<Album>";
			}
			else
			{
				textBox.Text += "<Album>";
			}
		}

		private void openContext_MouseClick(object sender, MouseEventArgs e)
		{
			contextMenuCreator();
			conmen.Show(openContext, new Point(openContext.Width, 0));
		}

		private void defaultFormat_Clicked(object sender, EventArgs e)
		{
			textBox.Text = "<Artist> - <Title>";
		}

		#endregion context menu eventHandlers

		private void textBox_TextChanged(object sender, EventArgs e)
		{
			// save the value
			nowPlayingPattern = textBox.Text;
			saveSettings();
		}

		// MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
		public void Close(PluginCloseReason reason)
		{
			skype.CurrentUserProfile.MoodText = previousMoodMessage;
		}

		/// <summary>
		/// Creates the nowPlayingString by replacing the Pattern witht he values of the respective fields.
		/// </summary>
		private void getNowPlayingTrackString()
		{
			nowPlayingString = nowPlayingPattern;

			//Regular Expressions for each supported TAG.
			Regex artistExpression = new Regex("<Artist>");
			Regex titleExpression = new Regex("<Title>");
			Regex albumArtistExpression = new Regex("<AlbumArtist>");
			Regex yearExpression = new Regex("<Year>");
			Regex albumExpression = new Regex("<Album>");

			//Replacing each tag with the current value of the specific tag
			nowPlayingString = artistExpression.Replace(nowPlayingString, artist);
			nowPlayingString = titleExpression.Replace(nowPlayingString, title);
			nowPlayingString = albumArtistExpression.Replace(nowPlayingString, albumArtist);
			nowPlayingString = yearExpression.Replace(nowPlayingString, year);
			nowPlayingString = albumExpression.Replace(nowPlayingString, album);
		}

		// uninstall this plugin - clean up any persisted files
		public void Uninstall()
		{
			if (File.Exists(settingFile))
			{
				File.Delete(settingFile);
			}
		}

		// receive event notifications from MusicBee
		// only required if about.ReceiveNotificationFlags = PlayerEvents
		public void ReceiveNotification(string sourceFileUrl, NotificationType type)
		{
			// perform some action depending on the notification type
			switch (type)
			{
				case NotificationType.PluginStartup:
					// perform startup initialisation
					break;
				case NotificationType.TrackChanged:

					//Get the values
					artist = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Artist);
					title = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.TrackTitle);
					albumArtist = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.AlbumArtist);
					year = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Year);
					album = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Album);

					getNowPlayingTrackString();

					if (skypeRunning)
					{
						skype.CurrentUserProfile.MoodText = getNowPlayingString();
					}
					else
					{
						initializeSkypeConnection();
						if (skypeRunning)
						{
							skype.CurrentUserProfile.MoodText = getNowPlayingString();
						}
					}

					break;
				case NotificationType.PlayStateChanged:
					switch (mbApiInterface.Player_GetPlayState())
					{
						case PlayState.Paused:
							if (skypeRunning)
							{
								skype.CurrentUserProfile.MoodText = previousMoodMessage;
							}
							break;
						case PlayState.Stopped:
							if (skypeRunning)
							{
								skype.CurrentUserProfile.MoodText = previousMoodMessage;
							}
							break;
						case PlayState.Playing:
							if (skypeRunning)
							{
								skype.CurrentUserProfile.MoodText = getNowPlayingString();
							}
							break;
					}
					break;
			}
		}

		// return an array of lyric or artwork provider names this plugin supports
		// the providers will be iterated through one by one and passed to the RetrieveLyrics/ RetrieveArtwork function in order set by the user in the MusicBee Tags(2) preferences screen until a match is found
		public string[] GetProviders()
		{
			return null;
		}

		// return lyrics for the requested artist/title from the requested provider
		// only required if PluginType = LyricsRetrieval
		// return null if no lyrics are found
		public string RetrieveLyrics(string sourceFileUrl, string artist, string trackTitle, string album, bool synchronisedPreferred, string provider)
		{
			return null;
		}

		// return Base64 string representation of the artwork binary data from the requested provider
		// only required if PluginType = ArtworkRetrieval
		// return null if no artwork is found
		public string RetrieveArtwork(string sourceFileUrl, string albumArtist, string album, string provider)
		{
			//Return Convert.ToBase64String(artworkBinaryData)
			return null;
		}
	}
}