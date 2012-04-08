using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using MusicBeePlugin.Data;
using MusicBeePlugin.SkypeControl;
using Timer = System.Timers.Timer;

namespace MusicBeePlugin
{
    public partial class Plugin
    {
        private TrackInfo _trackInfo;
        /// <summary>
        /// Represents the plugin info.
        /// </summary>
        private readonly PluginInfo _about = new PluginInfo();
        /// <summary>
        /// Represents an instance of the MusicBee API interface.
        /// </summary>
        private MusicBeeApiInterface _mbApiInterface;
        /// <summary>
        /// Represents the timer that prevents the stop message from appearing in case of track change.
        /// </summary>
        private static Timer _timer;
        /// <summary>
        /// Initialises the specified API interface PTR.
        /// </summary>
        /// <param name="apiInterfacePtr">The API interface PTR.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            _mbApiInterface = (MusicBeeApiInterface) Marshal.PtrToStructure(apiInterfacePtr, typeof (MusicBeeApiInterface));
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            //_displayNowPlayingString = true;
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

            SettingsManager.PersistentStorage = _mbApiInterface.Setting_GetPersistentStoragePath();
            SettingsManager.LoadSettings();

            //Stop timer initialization at 5 seconds. The timer is disabled at start.
            _timer = new Timer {Interval = 5000, Enabled = false};
            _timer.Elapsed += StopTimerElapse;
            _trackInfo = new TrackInfo();
            StartSkypeProxy();
            return _about;
        }

        /// <summary>
        /// Handles the tick event of the Stop Timer. On the event the method restores the default message, 
        /// and disables the timer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void StopTimerElapse(object sender, EventArgs e)
        {
            //RestoreDefaultMessage();
            _timer.Enabled = false;
        }

        // called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
        // its up to you to figure out whether anything has changed and needs updating
        public void SaveSettings()
        {
            SettingsManager.SaveSettings();
        }

        /// <summary>
        /// Configures the specified panel handle.
        /// </summary>
        /// <param name="panelHandle">The panel handle.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Configure(IntPtr panelHandle)
        {
            int backColor = _mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputControl, ElementState.ElementStateDefault,
                                                                               ElementComponent. ComponentBackground);
            int foreColor = _mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputControl, ElementState.ElementStateDefault,
                                                                               ElementComponent. ComponentForeground);
            UserSettingsPanel panel = new UserSettingsPanel();
            return panel.CreatePanel(panelHandle,backColor,foreColor);
        }

        // MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
        /// <summary>
        /// Closes the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <remarks></remarks>
        public void Close(PluginCloseReason reason)
        {
            //SettingsManager.RestoreDefaultMessage();
        }

        // uninstall this plugin - clean up any persisted files
        /// <summary>
        /// Uninstalls this instance.
        /// </summary>
        /// <remarks></remarks>
        public void Uninstall()
        {
            SettingsManager.RemoveSettings();
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
                case NotificationType.TrackChanged:
                    HandleTrackChanged();
                    break;
                case NotificationType.PlayStateChanged:
                    HandlePlayStateChanged();
                    break;
            }
        }

        private void HandleTrackChanged()
        {
            if (_timer.Enabled)
                _timer.Enabled = false;
            UpdateTrackInfo();
            SkypeCommunicationAdapter.GetInstance().Connect();
            SkypeCommunicationAdapter.GetInstance().SendMessage(Messages.SetMood + _trackInfo.GetNowPlayingTrackString());
        }

        private void HandlePlayStateChanged()
        {
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
                    _timer.Enabled = true;
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

        private void StartSkypeProxy()
        {
            SkypeCommunicationAdapter.GetInstance().SkypeAttach+=Plugin_SkypeAttach;
            SkypeCommunicationAdapter.GetInstance().SkypeResponse+=Plugin_SkypeResponse;
            SkypeCommunicationAdapter.GetInstance().Connect();

        }

        private void Plugin_SkypeResponse(object sender, SkypeResponseEventArgs eventargs)
        {
            Debug.WriteLine(eventargs.Response);
        }

        private void Plugin_SkypeAttach(object sender, SkypeAttachEventArgs eventargs)
        {
            Debug.WriteLine(eventargs.AttachStatus);
        }

        private void UpdateTrackInfo()
        {

            _trackInfo.DisplayNowPlayingString = SettingsManager.DisplayNowPlayingString;
            _trackInfo.NowPlayingPattern = SettingsManager.NowPlayingPattern;
            //Get the values
            _trackInfo.Artist = _mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Artist);
            _trackInfo.Title = _mbApiInterface.NowPlaying_GetFileTag(MetaDataType.TrackTitle);
            _trackInfo.AlbumArtist = _mbApiInterface.NowPlaying_GetFileTag(MetaDataType.AlbumArtist);
            _trackInfo.Year = _mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Year);
            _trackInfo.Album = _mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Album);
        }

                    //        GetNowPlayingTrackString();
                    //if (!_skypeRunning)
                    //{
                    //    InitializeSkypeConnection();
                    //}
                    //if (_skypeRunning)
                    //{
                    //    if (GetRestoredStatusFromXml())
                    //    {
                    //        SetRestoredStatusToXml(false);
                    //    }
                    //    proxy.Connect();
                    //    proxy.SendMessage("SET PROFILE MOOD_TEXT " + GetNowPlayingString());
                    //}
}
}