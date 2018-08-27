using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basler.Pylon;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Media;

namespace ViewRSOM.Hardware.BaslerCamera
{
    public class BaslerCamera
    {
        
        #region localvariables
        private Camera camera;
        private bool _cameraRecord;
        private BitmapSource bmpSource;
        private PixelDataConverter converter = new PixelDataConverter();
        #endregion localvariables

        // contructor
        public BaslerCamera()
        {
            StartCamera();
        }
        
        // public versions
        public bool cameraRecord
        {
            get { return _cameraRecord; }
            set
            {
                _cameraRecord = value;
            }
        }

        // initialize camera
        public void StartCamera()
        {
            try
            {
                camera = new Camera();
                // Create a camera object that selects the first camera device found.
                // More constructors are available for selecting a specific camera device.
                //using (Camera camera = new Camera())
                {
                    // Print the model name of the camera.
                    Console.WriteLine("Using camera {0}.", camera.CameraInfo[CameraInfoKey.ModelName]);

                    // Set the acquisition mode to free running continuous acquisition when the camera is opened.
                    camera.CameraOpened += Configuration.AcquireContinuous;

                    // Open the connection to the camera device.
                    camera.Open();

                    // The parameter MaxNumBuffer can be used to control the amount of buffers
                    // allocated for grabbing. The default value of this parameter is 10.
                    camera.Parameters[PLCameraInstance.MaxNumBuffer].SetValue(5);

                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exception: {0}", e.Message);

                // show laser warning sign --> no camera means clinical version
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.UriSource = new Uri("pack://application:,,,/ViewMSOTc/ViewsOAM/LaserSafetyImageWearGoggles.png");
                src.EndInit();
                src.Freeze();

                // show
                systemState.currentCameraImage = src;
            }              
        }

        // start image grabbing
        public void GrabImages()
        {

            if (camera != null)
            {

                _cameraRecord = true;

                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object o)
                {
                    try
                    {
                        // Start grabbing
                        camera.StreamGrabber.Start();

                        // Grab a number of images.
                        while (cameraRecord && systemState.reconThreadFree)
                        {
                            // Wait for an image and then retrieve it. A timeout of 5000 ms is used.
                            IGrabResult grabResult = camera.StreamGrabber.RetrieveResult(5000, TimeoutHandling.ThrowException);
                            using (grabResult)
                            {
                                // Image grabbed successfully?
                                if (grabResult.GrabSucceeded)
                                {
                                    // Access the image data.
                                    int stride = (int)grabResult.ComputeStride();
                                    byte[] buffer = grabResult.PixelData as byte[];

                                    // new buffer for format conversion
                                    byte[] new_buffer = new byte[grabResult.Width * grabResult.Height * 3];
                                    int new_stride = 3 * stride;

                                    // pixel conversion from Bayer to rgb 
                                    converter.OutputPixelFormat = PixelType.RGB8packed;
                                    converter.Convert<byte>(new_buffer, grabResult);//converter.Convert(buffer, grabResult);

                                    // create Bitmap
                                    bmpSource = BitmapSource.Create(grabResult.Width, grabResult.Height, 0, 0,
                                    PixelFormats.Rgb24, null, new_buffer, new_stride);
                                    bmpSource.Freeze();

                                    systemState.currentCameraImage = bmpSource;

                                }
                                else
                                {
                                    Console.WriteLine("ERROR: {0} {1}", grabResult.ErrorCode, grabResult.ErrorDescription);
                                }
                            }

                        }

                        // show monitor mouse message, when recon thread is active
                        if (systemState.reconThreadFree == false)
                        {
                            // show laser warning sign --> no camera means clinical version
                            BitmapImage src = new BitmapImage();
                            src.BeginInit();
                            src.CacheOption = BitmapCacheOption.OnLoad;
                            src.UriSource = new Uri("pack://application:,,,/ViewMSOTc/ViewsOAM/CameraInactive.png");
                            src.EndInit();
                            src.Freeze();

                            systemState.currentCameraImage = src;
                        }

                        // Stop grabbing.
                        camera.StreamGrabber.Stop();

                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine("INFO: {0}", e.Message);

                        // show laser warning sign --> no camera means clinical version
                        BitmapImage src = new BitmapImage();
                        src.BeginInit();
                        src.CacheOption = BitmapCacheOption.OnLoad;
                        src.UriSource = new Uri("pack://application:,,,/ViewMSOTc/ViewsOAM/LaserSafetyImageWearGoggles.png");
                        src.EndInit();
                        src.Freeze();

                        systemState.currentCameraImage = src;
                    }

                }));

            }
            
        }

        // stop and close camera
        public void StopCamera()
        {

            if (camera != null)
            {

                try
                {
                    // Close the connection to the camera device.
                    camera.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: {0}", e.Message);
                }

            }

        }

    }
}
