using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace Image_to_Text_Converter
{
    public partial class Form1 : Form
    {
        private readonly string[] AsciiChars = { "#", "#", "@", "%", "=", "+", "*", ":", "-", ".", "&nbsp;" };
        private string Content;
        public Form1()
        {
            InitializeComponent();
        }
        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileBrowser = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.png; *.bmp",
                RestoreDirectory = true
            };
            if (fileBrowser.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = fileBrowser.FileName;
            }
        }
        private void BtnConvert_Click(object sender, EventArgs e)
        {
            btnConvert.Enabled = false;
            try
            {
                progressBar.Value = 0;
                Bitmap image = new Bitmap(txtPath.Text, true);
                image = GetResizedImage(image, Convert.ToInt32(NUDquality.Value));
                Content = ConvertToAscii(image);
                browserMain.DocumentText = "<pre>" + Content + "</pre>";
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error!" + Environment.NewLine + exception.Message);
            }
            btnConvert.Enabled = true;
        }
        private static Bitmap GetResizedImage(Bitmap inputBitmap, int asciiWidth)
        {
            int asciiHeight = (int)Math.Ceiling((double)inputBitmap.Height * asciiWidth / inputBitmap.Width);
            Bitmap result = new Bitmap(asciiWidth, asciiHeight);
            Graphics graphics = Graphics.FromImage(result);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(inputBitmap, 0, 0, asciiWidth, asciiHeight);
            graphics.Dispose();
            return result;
        }
        private string ConvertToAscii(Bitmap inputBitmap)
        {
            bool toggle = false;
            progressBar.Maximum = inputBitmap.Width * inputBitmap.Height;
            StringBuilder sb = new StringBuilder();
            for (int h = 0; h < inputBitmap.Height; h++)
            {
                for (int w = 0; w < inputBitmap.Width; w++)
                {
                    Color pixelColor = inputBitmap.GetPixel(w, h);
                    Color grayColor = Color.FromArgb((pixelColor.R + pixelColor.G + pixelColor.B) / 3, (pixelColor.R + pixelColor.G + pixelColor.B) / 3, (pixelColor.R + pixelColor.G + pixelColor.B) / 3);
                    if (!toggle)
                    {
                        sb.Append(AsciiChars[(grayColor.R * 10) / 255]);
                    }
                    progressBar.PerformStep();
                }
                if (!toggle)
                {
                    sb.Append("<BR>");
                    toggle = true;
                }
                else
                {
                    toggle = false;
                }
            }
            return sb.ToString();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(browserMain.DocumentText))
                {
                    SaveFileDialog saveFile = new SaveFileDialog
                    {
                        FileName = "untitled",
                        Filter = "Hyper Text Markup Language (.html)|*.html|Hyper Text Markup Language (.htm)|*.htm|Text File (.txt)|*.txt|Text Document (.docx)|*.docx|Text Document (.doc)|*.doc|Rich Text Format (.rtf)|*.rtf",
                        FilterIndex = 1,
                        RestoreDirectory = true
                    };
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        StreamWriter writer = new StreamWriter(saveFile.OpenFile());
                        if (saveFile.FilterIndex == 1 || saveFile.FilterIndex == 2)
                        {
                            writer.Write("<pre>" + Content + "</pre>");
                        }
                        else if (saveFile.FilterIndex == 3 || saveFile.FilterIndex == 4 || saveFile.FilterIndex == 5 || saveFile.FilterIndex == 6)
                        {
                            String[] splitter = new String[] { "<BR>" };
                            String[] browserLines = Content.Split(splitter, StringSplitOptions.None);
                            writer.WriteLine("<pre>");
                            for (int i = 0; i < browserLines.Length; i++)
                            {
                                writer.WriteLine(browserLines[i] + "<BR>");
                            }
                            writer.WriteLine("</pre>");
                        }
                        writer.Dispose();
                    }
                }
                else
                {
                    MessageBox.Show("There is no text to save." + Environment.NewLine + "Select a picture and convert it to text first.");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error!" + Environment.NewLine + exception.Message);
            }
        }
        private void NUDquality_ValueChanged(object sender, EventArgs e)
        {
            if (NUDquality.Value <= 415)
            {
                NUDquality.ForeColor = Color.DarkRed;
            }
            else if (NUDquality.Value > 415 && NUDquality.Value <= 830)
            {
                NUDquality.ForeColor = Color.DarkOrange;
            }
            else
            {
                NUDquality.ForeColor = Color.DarkGreen;
            }
        }
        private void txtPath_Enter(object sender, EventArgs e)
        {
            if (txtPath.Text == "Image path goes here")
            {
                txtPath.Clear();
            }
        }
        private void txtPath_TextChanged(object sender, EventArgs e)
        {
            txtPath.ForeColor = Color.Black;
        }
    }
}