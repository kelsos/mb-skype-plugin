using System;
using System.Drawing;
using System.Windows.Forms;

namespace MusicBeePlugin
{
    internal class UserSettingsPanel
    {
        private int _backColor;
        private int _foreColor;

        private TextBox _textBox;
        private Button _openContext;

        public bool CreatePanel(IntPtr panelHandle, int backColor, int foreColor)
        {
            // panelHandle will only be set if you set about.ConfigurationPanelHeight to a non-zero value
            // keep in mind the panel width is scaled according to the font the user has selected
            if (panelHandle != IntPtr.Zero)
            {
                _backColor = backColor;
                _foreColor = foreColor;
                Panel configPanel = (Panel) Control.FromHandle(panelHandle);
                ToolTip tpOpenContext = new ToolTip();
                _textBox = new TextBox();

                tpOpenContext.SetToolTip(_textBox,
                                         "Tag indentifiers that can be used are: <Artist>, <AlbumArtist>, <Title>, <Year> and <Album>");

                Label patternBoxLabel = new Label
                                            {
                                                Bounds = new Rectangle(0, 0, configPanel.Width, 22),
                                                Text = "The pattern to be displayed:"
                                            };
                CheckBox nowPlayingCheck = new CheckBox
                                               {
                                                   Bounds = new Rectangle(0,
                                                                          _textBox.Bottom + 30,
                                                                          configPanel.Right,
                                                                          _textBox.Height),
                                                   Checked = SettingsManager.DisplayNowPlayingString,
                                                   Text = "Display \"Now Playing:\" text in front of the pattern",
                                                   FlatStyle = FlatStyle.Flat,
                                                   AutoSize = true
                                               };


                //Text Box
                _textBox.Text = SettingsManager.NowPlayingPattern;
                _textBox.Bounds = new Rectangle(0, patternBoxLabel.Height + 2, configPanel.Width - 50, _textBox.Height);
                _textBox.BackColor = Color.FromArgb(backColor);
                _textBox.ForeColor = Color.FromArgb(foreColor);
                _textBox.BorderStyle = BorderStyle.FixedSingle;
                _textBox.HideSelection = false;
                //Button Creation
                _openContext = new Button
                                   {
                                       Bounds =
                                           new Rectangle(_textBox.Width + 2, patternBoxLabel.Height + 2,
                                                         _textBox.Height + 10,
                                                         _textBox.Height)
                                   };
                _openContext.BringToFront();
                _openContext.TextAlign = ContentAlignment.MiddleCenter;
                _openContext.Text = "...";


                configPanel.Controls.AddRange(new Control[]
                                                  {
                                                      patternBoxLabel, _textBox, _openContext, nowPlayingCheck
                                                  });
                //EventHandlers Created.
                _openContext.MouseClick += OpenContextMouseClick;
                _textBox.TextChanged += TextBoxTextChanged;
                nowPlayingCheck.CheckedChanged += NowPlayingCheckChanged;
            }
            return false;
        }

        /// <summary>
        /// Creates the context menu.
        /// </summary>
        /// <remarks></remarks>
        private void ContextMenuCreator(ref ContextMenuStrip conmen)
        {
            if (conmen == null) throw new ArgumentNullException("conmen");

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
            defaultFormat.Click += DefaultFormatClicked;
            setNull.Click += SetNullClicked;
            artist.Click += ArtistClicked;
            albumArtist.Click += AlbumArtistClicked;
            title.Click += TitleClicked;
            year.Click += YearClicked;
            album.Click += AlbumClicked;

            conmen.BackColor = Color.FromArgb(_backColor);
            conmen.ForeColor = Color.FromArgb(_foreColor);
        }

        /// <summary>
        /// Handles the textbox text changed event and saves the settings.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            //// save the value
            SettingsManager.NowPlayingPattern = _textBox.Text;
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
            AddTagToTextbox("<Artist>");
        }

        /// <summary>
        /// Albums the artist clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void AlbumArtistClicked(object sender, EventArgs e)
        {
            AddTagToTextbox("<AlbumArtist>");
        }

        /// <summary>
        /// Titles the clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void TitleClicked(object sender, EventArgs e)
        {
            AddTagToTextbox("<Title>");
        }

        /// <summary>
        /// Years the clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void YearClicked(object sender, EventArgs e)
        {
            AddTagToTextbox("<Year>");
        }

        private void AddTagToTextbox(string tag)
        {
            if (_textBox.SelectionLength > 0)
            {
                _textBox.SelectedText = tag;
            }
            else
            {
                int selectionIndex = _textBox.SelectionStart;
                string textBoxContents = _textBox.Text;
                _textBox.Text = textBoxContents.Insert(selectionIndex, tag);
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
            AddTagToTextbox("<Album>");
        }

        /// <summary>
        /// Opens the context mouse click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void OpenContextMouseClick(object sender, MouseEventArgs e)
        {
            ContextMenuStrip conmen = new ContextMenuStrip();
            ContextMenuCreator(ref conmen);
            conmen.Show(_openContext, new Point(_openContext.Width, 0));
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

        /// <summary>
        /// Event Handler that changes the value of the _displayNowPlayingString when the corresponding checkbox changes value.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void NowPlayingCheckChanged(object sender, EventArgs e)
        {
            SettingsManager.DisplayNowPlayingString = !SettingsManager.DisplayNowPlayingString;
        }
    }
}