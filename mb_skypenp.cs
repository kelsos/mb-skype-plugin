using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using MusicBeePlugin.Data;
using MusicBeePlugin.SkypeControl;
using Timer = System.Timers.Timer;
using System.ComponentModel;

namespace MusicBeePlugin
{
    public partial class Plugin
    {
        private TrackInfo info;

        /// <summary>
        /// Represents the plugin info.
        /// </summary>
        private readonly PluginInfo about = new PluginInfo();

        /// <summary>
        /// Represents an instance of the MusicBee API interface.
        /// </summary>
        private MusicBeeApiInterface mbApiInterface;

        /// <summary>
        /// Represents the timer that prevents the stop message from appearing in case of track change.
        /// </summary>
        private static Timer timer;

        /// <summary>
        /// Initialises the specified API interface PTR.
        /// </summary>
        /// <param name="apiInterfacePtr">The API interface PTR.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            mbApiInterface =
                (MusicBeeApiInterface)Marshal.PtrToStructure(apiInterfacePtr, typeof(MusicBeeApiInterface));
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            about.PluginInfoVersion = PluginInfoVersion;
            about.Name = "Skype: Now Playing";
            about.Description = "Changes the skype mood to the currently playing track";
            about.Author = "Konstantinos Paparas (Kelsos)";
            about.TargetApplication = "Skype";
            // current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
            about.Type = PluginType.General;
            about.VersionMajor = Convert.ToInt16(v.Major); // your plugin version
            about.VersionMinor = Convert.ToInt16(v.Minor);
            about.Revision = Convert.ToInt16(v.Revision);
            about.MinInterfaceVersion = MinInterfaceVersion;
            about.MinApiRevision = MinApiRevision;
            about.ReceiveNotifications = ReceiveNotificationFlags.PlayerEvents;
            about.ConfigurationPanelHeight = 100; //Height of the panel.

            SettingsManager.PersistentStorage = mbApiInterface.Setting_GetPersistentStoragePath();
            SettingsManager.LoadSettings();

            //Stop timer initialization at 5 seconds. The timer is disabled at start.
            timer = new Timer { Interval = 2000, Enabled = false };
            timer.Elapsed += StopTimerElapse;
            info = new TrackInfo();
            StartSkypeProxy();
            return about;
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
            timer.Enabled = false;
            SkypeCommunicationAdapter.GetInstance().SendMessage(Messages.SetMood + "");
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
            int backColor = mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputControl,
                ElementState.ElementStateDefault,
                ElementComponent.ComponentBackground);
            int foreColor = mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputControl,
                ElementState.ElementStateDefault,
                ElementComponent.ComponentForeground);
            UserSettingsPanel panel = new UserSettingsPanel();
            return panel.CreatePanel(panelHandle, backColor, foreColor);
        }

        // MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
        /// <summary>
        /// Closes the specified reason.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <remarks></remarks>
        public void Close(PluginCloseReason reason)
        {
            SkypeCommunicationAdapter.GetInstance().SendMessage(Messages.SetMood + "");
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
            if (timer.Enabled)
                timer.Enabled = false;
            UpdateTrackInfo();

            BackgroundWorker b = new BackgroundWorker();
            b.DoWork += new DoWorkEventHandler(delegate (object s, DoWorkEventArgs ar)
            {
                SkypeCommunicationAdapter.GetInstance().Connect();
                SkypeCommunicationAdapter.GetInstance().SendMessage(Messages.SetMood + info.GetNowPlayingTrackString());
            });
            b.RunWorkerAsync();
        }

        private void HandlePlayStateChanged()
        {
            switch (mbApiInterface.Player_GetPlayState())
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
                    timer.Enabled = true;
                    break;
            }
        }

        private void StartSkypeProxy()
        {
            //I made this on a backgroundworker, but it seems to cause some issues.
            SkypeCommunicationAdapter.GetInstance().SkypeAttach += Plugin_SkypeAttach;
            SkypeCommunicationAdapter.GetInstance().SkypeResponse += Plugin_SkypeResponse;
            SkypeCommunicationAdapter.GetInstance().Connect();
        }

        private void Plugin_SkypeResponse(object sender, SkypeResponseEventArgs eventargs)
        {
#if DEBUG
            Debug.WriteLine(eventargs.Response);
#endif
        }

        private void Plugin_SkypeAttach(object sender, SkypeAttachEventArgs eventargs)
        {
#if DEBUG
            Debug.WriteLine(eventargs.AttachStatus);
#endif
        }

        private void UpdateTrackInfo()
        {
            info.DisplayNowPlayingString = SettingsManager.DisplayNowPlayingString;
            info.NowPlayingPattern = SettingsManager.NowPlayingPattern;
            info.Artist = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Artist);
            info.Title = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.TrackTitle);
            info.AlbumArtist = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.AlbumArtist);
            info.Year = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Year);
            info.Album = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Album);
            info.Duration = mbApiInterface.NowPlaying_GetDuration();
        }
    }
}