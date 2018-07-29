using System;
using System.Net;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Widget;

namespace BookLib.TestApp.Droid.Controls
{
    [Register("BookLib.TestApp.Droid.Controls.WebImageView")]
    public class WebImageView : ImageView
    {
        private string _url;
        public string Url
        {
            get { return _url; }
            set { _url = value; SetImageFromUrl(value); }
        }

        private WebClient _webClient;

        public WebImageView(Context context) : base(context)
        {
        }

        public WebImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        private void SetImageFromUrl(string url)
        {
            Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(url))
                {
                    SetImageBitmap(null);
                }
                else
                {
                    var imageBitmap = await GetImageBitmapFromUrl(url);
                    SetImageBitmap(imageBitmap);
                }
            });
        }

        private async Task<Bitmap> GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                byte[] imageBytes = await webClient.DownloadDataTaskAsync(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
    }
}
