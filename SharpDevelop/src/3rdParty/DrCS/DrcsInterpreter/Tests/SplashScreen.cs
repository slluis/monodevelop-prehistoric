using System;
using System.Windows.Forms;
using System.Drawing;

namespace Rice.Drcsharp.Tests
{
	/// <summary>
	/// Summary description for SplashScreen.
	/// </summary>
	class SplashScreen : Form {
		public SplashScreen() {
			FormBorderStyle = FormBorderStyle.None;
			StartPosition   = FormStartPosition.CenterScreen;
			ShowInTaskbar   = false;
			//ResourceManager resources = new ResourceManager("IconResources", Assembly.GetCallingAssembly());
			//Bitmap bitmap = new Bitmap(this.GetType(), "drcsSplashscreen1.bmp"); //(Bitmap)resources.GetObject("SplashScreen");
			Bitmap bitmap = DrcsResourceManager.GetBitmap("drcsSplashscreen1.bmp");
			Size = bitmap.Size;
			BackgroundImage = bitmap;
		}
	}
}
