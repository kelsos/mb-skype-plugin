using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MusicBeePlugin
{
    class UserSettingsPanel
    {
        private bool CreatePanel(IntPtr panelHandle, int backColor, int foreColor)
        {
            ////// panelHandle will only be set if you set about.ConfigurationPanelHeight to a non-zero value
            ////// keep in mind the panel width is scaled according to the font the user has selected
            ////if (panelHandle != IntPtr.Zero)
            ////{
                
            ////    Panel configPanel = (Panel)Control.FromHandle(panelHandle);
            ////    ToolTip tpOpenContext = new ToolTip();
            ////    ToolTip tpSkypeButton = new ToolTip();
            ////    TextBox textBox = new TextBox();

            ////    tpOpenContext.SetToolTip(textBox,
            ////                             "Tag indentifiers that can be used are: <Artist>, <AlbumArtist>, <Title>, <Year> and <Album>");

            ////    Label patternBoxLabel = new Label
            ////    {
            ////        Bounds = new Rectangle(0, 0, configPanel.Width, 22),
            ////        Text = "The pattern to be displayed:"
            ////    };
            ////    CheckBox nowPlayingCheck = new CheckBox
            ////    {
            ////        Bounds = new Rectangle(0,
            ////                               textBox.Bottom + 30,
            ////                               configPanel.Right,
            ////                               textBox.Height),
            ////        Checked = _displayNowPlayingString,
            ////        Text = "Display \"Now Playing:\" text in front of the pattern",
            ////        FlatStyle = FlatStyle.Flat,
            ////        AutoSize = true
            ////    };


            ////    //Text Box
            ////    textBox.Text = _nowPlayingPattern;
            ////    textBox.Bounds = new Rectangle(0, patternBoxLabel.Height + 2, configPanel.Width - 50, textBox.Height);
            ////    textBox.BackColor =
            ////        Color.FromArgb(backColor);
            ////    textBox.ForeColor =
            ////        Color.FromArgb(foreColor);
            ////    textBox.BorderStyle = BorderStyle.FixedSingle;
            ////    textBox.HideSelection = false;
            ////    //Button Creation
            ////    Button openContext = new Button
            ////    {
            ////        Bounds =
            ////            new Rectangle(textBox.Width + 2, patternBoxLabel.Height + 2,
            ////                          textBox.Height + 10,
            ////                          textBox.Height)
            ////    };
            ////    openContext.BringToFront();
            ////    openContext.TextAlign = ContentAlignment.MiddleCenter;
            ////    openContext.Text = "...";

            ////    //About Button
            ////    Font tempFont = new Font(openContext.Font.FontFamily, 7);
            ////   Button skypeStatusCheckButton = new Button
            ////    {
            ////        Bounds =
            ////            new Rectangle(openContext.Left, nowPlayingCheck.Top,
            ////                          textBox.Height + 10,
            ////                          textBox.Height - 1),
            ////        Parent = textBox,
            ////        Font = tempFont,
            ////        ForeColor = Color.White,
            ////        FlatStyle = FlatStyle.Flat
            ////    };
            ////    ButtonColorChanger();
            ////    skypeStatusCheckButton.BringToFront();
            ////    skypeStatusCheckButton.TextAlign = ContentAlignment.MiddleCenter;

            ////    tpSkypeButton.SetToolTip(skypeStatusCheckButton,
            ////                             "If the Button is green the plugin communicates with skype,\nIf the button is red either Skype is closed or there was an issue communicating with it.\nOn press the button checks if Skype is Running, in order to make the connection.");

            ////    configPanel.Controls.AddRange(new Control[]
            ////                                       {
            ////                                           patternBoxLabel, textBox, openContext, nowPlayingCheck,
            ////                                           skypeStatusCheckButton
            ////                                       });
            ////    //EventHandlers Created.
            ////    openContext.MouseClick += OpenContextMouseClick;
            ////    textBox.TextChanged += TextBoxTextChanged;
            ////    nowPlayingCheck.CheckedChanged += NowPlayingCheckChanged;
            ////    skypeStatusCheckButton.MouseClick += SkypeStatusCheckButtonMouseClick;
            ////}
            return false;
        }
        /// <summary>
        /// Creates the context menu.
        /// </summary>
        /// <remarks></remarks>
        private void ContextMenuCreator()
        {
            //_conmen = new ContextMenuStrip();

            ////Creation of the ToolStripMenuItems
            //ToolStripSeparator separator = new ToolStripSeparator();
            //ToolStripMenuItem defaultFormat = new ToolStripMenuItem();
            //ToolStripMenuItem artist = new ToolStripMenuItem();
            //ToolStripMenuItem title = new ToolStripMenuItem();
            //ToolStripMenuItem setNull = new ToolStripMenuItem();
            //ToolStripMenuItem year = new ToolStripMenuItem();
            //ToolStripMenuItem album = new ToolStripMenuItem();
            //ToolStripMenuItem albumArtist = new ToolStripMenuItem();

            ////Setting the text values of the new ToolStripMenuItems
            //setNull.Text = "Empty Field";
            //defaultFormat.Text = "Default";
            //artist.Text = "Artist";
            //albumArtist.Text = "Album Artist";
            //title.Text = "Title";
            //year.Text = "Year";
            //album.Text = "Album";

            ////Adding the MenuItems to the Context menu.
            //_conmen.Items.Add(setNull);
            //_conmen.Items.Add(separator);
            //_conmen.Items.Add(defaultFormat);
            //_conmen.Items.Add(artist);
            //_conmen.Items.Add(albumArtist);
            //_conmen.Items.Add(title);
            //_conmen.Items.Add(year);
            //_conmen.Items.Add(album);

            ////Creating the EventHandlers for the Click Event oft
            //defaultFormat.Click += DefaultFormatClicked;
            //setNull.Click += SetNullClicked;
            //artist.Click += ArtistClicked;
            //albumArtist.Click += AlbumArtistClicked;
            //title.Click += TitleClicked;
            //year.Click += YearClicked;
            //album.Click += AlbumClicked;

            //_conmen.BackColor =
            //    Color.FromArgb(_mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputPanel,
            //                                                                ElementState.ElementStateDefault,
            //                                                                ElementComponent.ComponentBackground));
            //_conmen.ForeColor =
            //    Color.FromArgb(_mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputPanel,
            //                                                                ElementState.ElementStateDefault,
            //                                                                ElementComponent.ComponentForeground));
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
            //_nowPlayingPattern = _textBox.Text;
            //SaveSettings();
        }
        /// <summary>
        /// Sets the null clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void SetNullClicked(object sender, EventArgs e)
        {
            //_textBox.Text = "";
        }

        /// <summary>
        /// Artists the clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void ArtistClicked(object sender, EventArgs e)
        {
            //if (_textBox.SelectionLength > 0)
            //{
            //    _textBox.SelectedText = "<Artist>";
            //}
            //else
            //{
            //    _textBox.Text += "<Artist>";
            //}
        }

        /// <summary>
        /// Albums the artist clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void AlbumArtistClicked(object sender, EventArgs e)
        {
            //if (_textBox.SelectionLength > 0)
            //{
            //    _textBox.SelectedText = "<AlbumArtist>";
            //}
            //else
            //{
            //    _textBox.Text += "<AlbumArtist>";
            //}
        }

        /// <summary>
        /// Titles the clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void TitleClicked(object sender, EventArgs e)
        {
            //if (_textBox.SelectionLength > 0)
            //{
            //    _textBox.SelectedText = "<Title>";
            //}
            //else
            //{
            //    _textBox.Text += "<Title>";
            //}
        }

        /// <summary>
        /// Years the clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void YearClicked(object sender, EventArgs e)
        {
            //if (_textBox.SelectionLength > 0)
            //{
            //    _textBox.SelectedText = "<Year>";
            //}
            //else
            //{
            //    _textBox.Text += "<Year>";
            //}
        }

        /// <summary>
        /// Albums the clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void AlbumClicked(object sender, EventArgs e)
        {
            //if (_textBox.SelectionLength > 0)
            //{
            //    _textBox.SelectedText = "<Album>";
            //}
            //else
            //{
            //    _textBox.Text += "<Album>";
            //}
        }

        /// <summary>
        /// Opens the context mouse click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void OpenContextMouseClick(object sender, MouseEventArgs e)
        {
            //ContextMenuCreator();
            //_conmen.Show(_openContext, new Point(_openContext.Width, 0));
        }

        /// <summary>
        /// Defaults the format clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void DefaultFormatClicked(object sender, EventArgs e)
        {
            //_textBox.Text = "<Artist> - <Title>";
        }
        /// <summary>
        /// Event Handler that changes the value of the _displayNowPlayingString when the corresponding checkbox changes value.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        private void NowPlayingCheckChanged(object sender, EventArgs e)
        {
            //_displayNowPlayingString = !_displayNowPlayingString;
        }

        private void SkypeStatusCheckButtonMouseClick(object sender, MouseEventArgs e)
        {
            //InitializeSkypeConnection();
            ButtonColorChanger();
        }

        private void ButtonColorChanger()
        {
            //_skypeStatusCheckButton.BackColor = _isInitialized ? Color.Green : Color.Red;
            //_skypeStatusCheckButton.Text = _isInitialized ? "OK" : "OFF";
        }
    }
}
