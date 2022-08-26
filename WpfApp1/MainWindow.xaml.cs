using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WasapiLoopbackCapture capture = new WasapiLoopbackCapture(); // 32 bit IEEFloat: 48000Hz 2 channels
        public MainWindow()
        {
            InitializeComponent();

            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;

            progressBar2.Minimum = 0;
            progressBar2.Maximum = 100;
            
            capture.DataAvailable += (s, a) =>
            {
                float max1 = 0;
                float max2 = 0;
                var buffer = new WaveBuffer(a.Buffer);
                // interpret as 32 bit floating point audio
                for (int index = 0; index < a.BytesRecorded / 4; index += 2)
                {

                    var sample1 = buffer.FloatBuffer[index];
                    var sample2 = buffer.FloatBuffer[index + 1];

                    // absolute value 
                    if (sample1 < 0) sample1 = -sample1;
                    if (sample2 < 0) sample2 = -sample2;

                    // is this the max value?
                    if (sample1 > max1) max1 = sample1;
                    if (sample2 > max2) max2 = sample2;
                }
                try
                {
                    progressBar1.Dispatcher.Invoke(
                    new Action(() => progressBar1.Value = 100 * max1));
                    progressBar2.Dispatcher.Invoke(
                    new Action(() => progressBar2.Value = 100 * max2));
                }
                catch (TaskCanceledException e)
                {
                }
            };
            capture.StartRecording();

        }

        private void Window_Activated(object sender, EventArgs e)
        {
            this.Topmost = true;
        }
        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Activate();
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            capture.StopRecording();
        }
    }
}
