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
        private string _nowPlayingPattern;

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
        public string NowPlayingPattern
        {
            set { _nowPlayingPattern = value; }
        }

        public bool DisplayNowPlayingString
        {
            set { _displayNowPlayingString = value; }
        }

        private bool _displayNowPlayingString;

        /// <summary>
        /// Creates the nowPlayingString by replacing the Pattern witht he values of the respective fields.
        /// </summary>
        /// <remarks></remarks>
        public string GetNowPlayingTrackString()
        {
            string nowPlayingString = _displayNowPlayingString == false ? _nowPlayingPattern : String.Format("Now Playing: {0}", _nowPlayingPattern);
        

            //Regular Expressions for each supported TAG.
            Regex artistExpression = new Regex("<Artist>");
            Regex titleExpression = new Regex("<Title>");
            Regex albumArtistExpression = new Regex("<AlbumArtist>");
            Regex yearExpression = new Regex("<Year>");
            Regex albumExpression = new Regex("<Album>");

            //Replacing each tag with the current value of the specific tag
            nowPlayingString = artistExpression.Replace(nowPlayingString, Artist);
            nowPlayingString = titleExpression.Replace(nowPlayingString, Title);
            nowPlayingString = albumArtistExpression.Replace(nowPlayingString, AlbumArtist);
            nowPlayingString = yearExpression.Replace(nowPlayingString, Year);
            nowPlayingString = albumExpression.Replace(nowPlayingString, Album);

            return nowPlayingString;
        }

    }
}
