using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NAudio.Wave;

namespace NAudioPlayerTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button3.Enabled = false;
            button2.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            listBoxDeviceList.Items.Clear();
            Application.DoEvents();
            int waveInDevices = WaveIn.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                String item = String.Format("Device {0}: {1}, {2} channels", waveInDevice, deviceInfo.ProductName, deviceInfo.Channels);
                Console.WriteLine(item);
                listBoxDeviceList.Items.Add(item);
            }
            button1.Enabled = true;
            
        }


        private void button2_Click(object sender, EventArgs e)
        {
            int device = listBoxDeviceList.SelectedIndex;
            if (device < 0)
            {
                MessageBox.Show("Please select a device from the list first");
            }
            else
            {
                button3.Enabled = true;
                button2.Enabled = false;

                StartRecording(device);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button2.Enabled = true;
            StopRecording();
        }


        private IWavePlayer waveOut;
        private BufferedWaveProvider waveProvider;
        private WaveIn m_WaveIn;
        WaveFormat m_waveFormat = new WaveFormat(16000, 1);

        private void StartRecording(int deviceNumber) {
            // Setup Incoming
            m_WaveIn = new WaveIn();
            m_WaveIn.BufferMilliseconds = 50; // This is very very important.
            m_WaveIn.DeviceNumber = deviceNumber;
            m_WaveIn.DataAvailable += WaveIn_DataAvailable;
            m_WaveIn.WaveFormat = m_waveFormat;
            m_WaveIn.StartRecording();

            // Setup Output


            waveOut = new WaveOut();
            waveProvider = new BufferedWaveProvider(m_waveFormat);
            waveOut.Init(waveProvider);
            waveOut.Play();


        }

        private void StopRecording()
        {
            m_WaveIn.StopRecording();
            waveOut.Stop();

            m_WaveIn.DataAvailable -= WaveIn_DataAvailable;
            m_WaveIn.Dispose();
            m_WaveIn = null;
            waveOut.Dispose();
            waveOut = null;
            waveProvider = null;
        }

        public void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] buffer = e.Buffer;
            waveProvider.AddSamples(buffer, 0, buffer.Length);
        }        
    }
}
