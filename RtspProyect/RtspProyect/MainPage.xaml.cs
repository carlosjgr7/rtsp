using RtspClientSharp;
using RtspClientSharp.RawFrames.Video;
using RtspClientSharp.Rtsp;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RtspProyect
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var serverUri = new Uri("rtsp://wowzaec2demo.streamlock.net/vod/mp4:BigBuckBunny_115k.mov");
            
            
            // Credenciales de conexion
            var credentials = new NetworkCredential("", "");

            var connectionParameters = new ConnectionParameters(serverUri, credentials);
            var cancellationTokenSource = new CancellationTokenSource();

            Task connectTask = ConnectAsync(connectionParameters, cancellationTokenSource.Token);

          
        }

        private async Task ConnectAsync(ConnectionParameters connectionParameters, CancellationToken token)
        {
            try
            {
                TimeSpan delay = TimeSpan.FromSeconds(5);

                using (var rtspClient = new RtspClient(connectionParameters))
                {

                    rtspClient.FrameReceived += RtspClient_FrameReceived;
                    while (true)
                    {
                        Console.WriteLine("Connecting...");

                        try
                        {
                            await rtspClient.ConnectAsync(token);
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                        catch (RtspClientException e)
                        {
                            Console.WriteLine(e.ToString());
                            await Task.Delay(delay, token);
                            continue;
                        }

                        Console.WriteLine("Connected.");

                        try
                        {
                            await rtspClient.ReceiveAsync(token);
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                        catch (RtspClientException e)
                        {
                            Console.WriteLine(e.ToString());
                            await Task.Delay(delay, token);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void RtspClient_FrameReceived(object sender, RtspClientSharp.RawFrames.RawFrame e)
        {
            

           
            //separamos fragment de videos y de sonido
            if (e.GetType().Name != "RawAACFrame")
            {

                RawH264IFrame frm = e as RawH264IFrame;
                //convirtiendo los segmentos a array de byte 
                var spsPpsSegment = frm.SpsPpsSegment.Array;

                
            }


        }

}
}