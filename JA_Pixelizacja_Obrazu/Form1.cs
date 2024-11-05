using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JA_Pixelizacja_Obrazu
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void processButton_Click(object sender, EventArgs e)
        {
            // Tutaj wywołujemy metodę odpowiedzialną za pixelizację obrazu
            // Wczytujemy obraz z pliku
            if (string.IsNullOrEmpty(filePathTextBox.Text))
            {
                MessageBox.Show("Please select an image file");
                return;
            }
            else
            {
                Bitmap originalImage = new Bitmap(filePathTextBox.Text);
                // Pobieramy rozmiar pixela z ComboBoxa
                int pixelSize = int.Parse(pixelNumSpecifier.Text);
                // Wywołujemy metodę pixelizacji
                Bitmap pixelizedImage = ImageProcessing.PixelizeImage(originalImage, pixelSize);
                // Wyświetlamy wynik w PictureBoxie
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; // Set the SizeMode to Zoom
                pictureBox1.Image = pixelizedImage;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void browseFilesButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select an image";
            openFileDialog.InitialDirectory = @"C:\"; // You can set initial directory
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp) | *.jpg; *.jpeg; *.png; *.bmp";
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePathTextBox.Text = openFileDialog.FileName;
            }

        }

        private void pixelNumSpecifier_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
