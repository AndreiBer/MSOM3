/////////////////////////////////////////////////////////////
// "class DisplayCommunicator"
//
// can be used by lengthy tasks to throw events about progress and errors.
// subscribe to the event in the GUI Task.
// REMEMBER!: You have to use the Dispatcher to invoke one task from a different task! 
////////////////////////////////////////////////////////////// 
using System;
//using Coherent;
//using Firgelli;

namespace General.Tools.Communication
{

    public delegate void MyMessageHandler(string sender, string receiver, string message);
    //public delegate void MyCoherentDataHandler(string sender, string receiver, EnergyMeterData data);
    public delegate void MyFirgelliDataHandler(string sender, string receiver, int ActualPointNumber, int positionReached);
    public delegate void MyLaserMessageHandler(string sender, string receiver, string message);
    public delegate void MyAttenuatorMessageHandler(string sender, string receiver, double wl, double reprate, double fluence, double attenuation, int status);

    public static class GUI_Communicator
    {
        //public static event MyMessageHandler MyStatusMessage;
        //public static event MyCoherentDataHandler MyCohDataMessage;
        public static event MyFirgelliDataHandler MyFirgelliDataMessage;
        public static event MyLaserMessageHandler MyLaserStatusMessage;
        public static event MyLaserMessageHandler MyLaserErrorMessage;
        public static event MyAttenuatorMessageHandler MyAttenuatorStatusMessage;

        // this method can fire an event
        /*
        public static void OnMyCohDataEvent(string sender, string receiver, EnergyMeterData data)
        {
            MyCoherentDataHandler myCohDataEvent = MyCohDataMessage;
            if (myCohDataEvent != null)
            {
                myCohDataEvent(sender, receiver, data);
            }
        }
         */

        // this method can fire an event
        public static void OnMyFirgelliDataEvent(string sender, string receiver, int actualPointNumber, int positionReached )
        {
            MyFirgelliDataHandler myFirgelliDataEvent = MyFirgelliDataMessage;
            if (myFirgelliDataEvent != null)
            {
                myFirgelliDataEvent(sender, receiver, actualPointNumber, positionReached);
            }
        }
        
       
         // this method can fire an event
        public static void OnMyLaserErrorEvent(string sender, string receiver, string message)
        {
            MyLaserMessageHandler myLaserErrorEvent = MyLaserErrorMessage;
            if (myLaserErrorEvent != null)
            {
                myLaserErrorEvent(sender, receiver, message);
            }
        }

        public static void OnMyLaserStatusEvent(string sender, string receiver, string message)
        {
            MyLaserMessageHandler myLaserStatusEvent = MyLaserStatusMessage;
            if (myLaserStatusEvent != null)
            {
                myLaserStatusEvent(sender, receiver, message);
            }
        }

        public static void OnMyAttenuatorStatusEvent(string sender, string receiver, double wl, double reprate,double fluence, double attenuation, int status)
        {
            MyAttenuatorMessageHandler myAttenuatorStatusEvent = MyAttenuatorStatusMessage;
            if (myAttenuatorStatusEvent != null)
            {
                myAttenuatorStatusEvent(sender, receiver, wl, reprate, fluence, attenuation, status);
            }
        }

        /*
        // use the following methods to send Data
        public static void sendCohData(string sender, string receiver, EnergyMeterData data)
        {
            OnMyCohDataEvent(sender, receiver, data);
        }
         */ 
                
        
        public static void sendFirgelliData(string sender, string receiver, int actualPointNumber, int positionReached)
        {
            OnMyFirgelliDataEvent(sender, receiver, actualPointNumber, positionReached);
        }
               
       
        public static void sendError(string sender, string receiver, string message)
        {
            OnMyLaserErrorEvent(sender, receiver, message);
        }

        public static void sendStatus(string sender, string receiver, string message)
        {
            OnMyLaserStatusEvent(sender, receiver, message);
        }

        public static void sendAttenuatorStatus(string sender, string receiver, double wl, double reprate, double fluence, double attenuation, int status)
        {
            OnMyAttenuatorStatusEvent(sender, receiver, wl, reprate,fluence, attenuation, status);
        }
        

    }
}


