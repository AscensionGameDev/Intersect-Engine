using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Intersect.Editor.Forms.Editors
{
    public partial class FrmIconSelector : Form
    {
        private const int IconsPerPage = 160; // Perfect fit for 16 columns x 10 rows
        private const int IconSize = 32;

        private readonly string _iconDirectory;

        private List<string> _allIcons = new List<string>();
        private List<string> _filteredIcons = new List<string>();
        private int _currentPage = 0;
        private PictureBox _selectedPictureBox;
        private List<PictureBox> _pictureBoxPool = new List<PictureBox>();

        public string SelectedIcon { get; private set; }

        public FrmIconSelector(string iconDirectory, string currentIcon = null)
        {
            InitializeComponent();
            
            _iconDirectory = iconDirectory;
            SelectedIcon = currentIcon;

            LoadIcons();
            UpdatePagination();
            LoadCurrentPage();
        }

        private void LoadIcons()
        {
            if (!Directory.Exists(_iconDirectory)) return;

            _allIcons = Directory.GetFiles(_iconDirectory, "*.png")
                .Concat(Directory.GetFiles(_iconDirectory, "*.jpg"))
                .Concat(Directory.GetFiles(_iconDirectory, "*.jpeg"))
                .ToList();

            _filteredIcons = new List<string>(_allIcons);
        }

        private void LoadCurrentPage()
        {
            flowIconPanel.SuspendLayout();

            var startIndex = _currentPage * IconsPerPage;
            var endIndex = Math.Min(startIndex + IconsPerPage, _filteredIcons.Count);
            var itemsToDisplay = endIndex - startIndex;

            // Dispose old images to prevent memory leaks and lag
            foreach (var picBox in _pictureBoxPool)
            {
                if (picBox.Image != null)
                {
                    var img = picBox.Image;
                    picBox.Image = null;
                    img.Dispose();
                }
            }

            // Ensure we have enough controls in the pool
            while (_pictureBoxPool.Count < itemsToDisplay)
            {
                var picBox = new PictureBox
                {
                    Width = IconSize,
                    Height = IconSize,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Margin = new Padding(2),
                    Cursor = Cursors.Hand,
                    BackColor = System.Drawing.Color.FromArgb(60, 63, 65)
                };
                picBox.Click += IconButton_Click;
                _pictureBoxPool.Add(picBox);
                flowIconPanel.Controls.Add(picBox);
            }

            int poolIndex = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                var iconPath = _filteredIcons[i];
                
                Image img = null;
                try
                {
                    // Use MemoryStream to avoid file locking on disk
                    if (File.Exists(iconPath))
                    {
                        var bytes = File.ReadAllBytes(iconPath);
                        using (var ms = new MemoryStream(bytes))
                        {
                            img = Image.FromStream(ms);
                        }
                    }
                }
                catch
                {
                    continue;
                }

                if (img == null) continue;

                var picBox = _pictureBoxPool[poolIndex];
                picBox.Image = img;
                picBox.Tag = iconPath;
                picBox.Visible = true;
                
                // Clear selection visuals if not selected
                if (SelectedIcon == Path.GetFileName(iconPath))
                {
                    picBox.BorderStyle = BorderStyle.FixedSingle;
                    picBox.BackColor = System.Drawing.Color.FromArgb(100, 100, 100);
                    _selectedPictureBox = picBox;
                }
                else
                {
                    picBox.BorderStyle = BorderStyle.None;
                    picBox.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
                }

                poolIndex++;
            }

            // Hide unused controls in the pool
            for (int i = poolIndex; i < _pictureBoxPool.Count; i++)
            {
                _pictureBoxPool[i].Visible = false;
            }

            flowIconPanel.ResumeLayout();
        }

        private void UpdatePagination()
        {
            int totalPages = Math.Max(1, (int)Math.Ceiling(_filteredIcons.Count / (double)IconsPerPage));
            
            lblPageInfo.Text = $"Page {_currentPage + 1} / {totalPages}";
            btnPrevPage.Enabled = _currentPage > 0;
            btnNextPage.Enabled = _currentPage < totalPages - 1;
        }

        private void IconButton_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox picBox && picBox.Tag is string iconPath)
            {
                // Clear previous selection
                if (_selectedPictureBox != null)
                {
                    _selectedPictureBox.BorderStyle = BorderStyle.None;
                    _selectedPictureBox.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
                }

                // Set new selection with visual feedback
                _selectedPictureBox = picBox;
                picBox.BorderStyle = BorderStyle.FixedSingle;
                picBox.BackColor = System.Drawing.Color.FromArgb(100, 100, 100);
                SelectedIcon = Path.GetFileName(iconPath);
            }
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            _currentPage--;
            UpdatePagination();
            LoadCurrentPage();
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            _currentPage++;
            UpdatePagination();
            LoadCurrentPage();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectedIcon))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
