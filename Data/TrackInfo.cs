using System;
using System.Text.RegularExpressions;

namespace MusicBeePlugin.Data
{
    class TrackInfo
    {
        /// <summary>
        /// Represents the album artist tag information.
        /// </summary>
        public string AlbumArtist { get; set; }

        /// <summary>
        /// Represents the artist tag information.
        /// </summary>
        public string Artist { get; set; }

        ///     /// <summary>
        /// Represents the Now Playing String tha will be finally passed to Skype's User Mood.
        /// </summary>
        private string _nowPlayingString;

        /// <summary>
        /// Represents the song title tag information.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Represents the album release Year tag information.
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// Represents the album tag information.
        /// </summary>
        public string Album { get; set; }

        ///     /// <summary>
        /// Represents the Now Playing String tha will be finally passed to Skype's User Mood.
        /// </summary>
        public string NowPlayingString
        {
            get { return _nowPlayingString; }
            set { _nowPlayingString = value; }
        }

        private bool _displayNowPlayingString;
        private string GetNowPlayingString()
        {
            return _displayNowPlayingString == false ? _nowPlayingString : String.Format("Now Playing: {0}", _nowPlayingString);
        }
        /// <summary>
        /// Creates the nowPlayingString by replacing the Pattern witht he values of the respective fields.
        /// </summary>
        /// <remarks></remarks>
        public string GetNowPlayingTrackString()
        {
            _nowPlayingString = "<Artist> - <Title>";

            //Regular Expressions for each supported TAG.
            Regex artistExpression = new Regex("<Artist>");
            Regex titleExpression = new Regex("<Title>");
            Regex albumArtistExpression = new Regex("<AlbumArtist>");
            Regex yearExpression = new Regex("<Year>");
            Regex albumExpression = new Regex("<Album>");

            //Replacing each tag with the current value of the specific tag
            _nowPlayingString = artistExpression.Replace(_nowPlayingString, Artist);
            _nowPlayingString = titleExpression.Replace(_nowPlayingString, Title);
            _nowPlayingString = albumArtistExpression.Replace(_nowPlayingString, AlbumArtist);
            _nowPlayingString = yearExpression.Replace(_nowPlayingString, Year);
            _nowPlayingString = albumExpression.Replace(_nowPlayingString, Album);

            return _nowPlayingString;
        }

    }
}
