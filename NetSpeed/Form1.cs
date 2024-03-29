using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;

namespace NetSpeed
{

    public partial class Form1 : Form
    {
        long CurrentBytesReceived = 1;
        long LastSecondBytesReceived = 0;
        long SpeedSum = 1;

        System.Timers.Timer timer = new System.Timers.Timer();
        NotifyIcon notifyIcon = new NotifyIcon();  
        public Form1()
        {
            InitializeComponent();
            label1.Text = "0";
            timer.Interval = 1000;
            timer.AutoReset = true;
            timer.Start();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(ShowInterfaceSpeed);
            notifyIcon.Visible = true;
            notifyIcon.Click += new EventHandler(ExitApp);
        }
    
        public void ExitApp (object? source, EventArgs? e)
        {
            Application.Exit();
        }

    public void ShowInterfaceSpeed(object? source, ElapsedEventArgs? e)
    {      
        NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

            try
            {
                UpdateLabel();
            }
            catch (Exception)
            {
                timer.Start();

            }

            CurrentBytesReceived = 0;
            foreach (NetworkInterface adapter in adapters)
            {
                if (!adapter.Description.Contains("VPN"))
                    CurrentBytesReceived += adapter.GetIPv4Statistics().BytesReceived;
            }

            SpeedSum = CurrentBytesReceived - LastSecondBytesReceived;

     }

        public void UpdateLabel()
        {
            LastSecondBytesReceived = CurrentBytesReceived;
            if (label1.InvokeRequired)
            {
                label1.Invoke(UpdateLabel);
            }
            else
            {
                label1.Text = ((SpeedSum/1024)).ToString();

                Font fontToUse = new Font("Trebuchet MS", 10, FontStyle.Regular, GraphicsUnit.Pixel);
                Brush brushToUse = new SolidBrush(Color.White);
                Bitmap bitmapText = new Bitmap(16, 16);
                Graphics g = System.Drawing.Graphics.FromImage(bitmapText);

                IntPtr hIcon;

                g.Clear(Color.Transparent);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                g.DrawString(label1.Text, fontToUse, brushToUse, -2, 0);
                hIcon = (bitmapText.GetHicon());
                notifyIcon.Icon = System.Drawing.Icon.FromHandle(hIcon);
                //DestroyIcon(hIcon.ToInt32);
                Icon icon = Icon.FromHandle(hIcon);
                
                    // do what you need to do with the icon
                    notifyIcon.Icon = icon;
                DestroyIcon(icon.Handle);
                // notifyIcon.Icon = Icon.FromHandle(iconBitmap.GetHicon());
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);
    }
}