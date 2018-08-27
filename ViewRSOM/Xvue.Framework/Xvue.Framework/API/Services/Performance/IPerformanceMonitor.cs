using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections;


namespace Xvue.Framework.API.Services.Performance
{
    public interface IPerformanceMonitor
	{
        /// <summary>
        /// Register a counter in the PerformanceMonitor list and returns a unique id
        /// </summary>
        /// <param name="deviceName">The module that requests the counter</param>
        /// <param name="functionName">The functionality that is monitored by the performance counter in the given module</param>
        /// <returns>The newly regustere counter id</returns>
        int RegisterCounter(string deviceName, string functionName);

        /// <summary>
        /// Remove counter from the list
        /// </summary>
        /// <param name="counterId">the id of the counter</param>        
        void UnregisterCounter(int counterId);

        /// <summary>Clears all interval records, min, max and average values</summary> 
        /// <param name="counterId">the id of the counter</param>
		void Init(int counterId);

		/// <summary>Creates a new interval record</summary>
        /// <param name="counterId">the id of the counter</param>
		/// <param name="curFrameTime">Provide the frame time of the new interval record</param>        
		void Start(int counterId,double curFrameTime);

		/// <summary>Terminates the current interval record</summary>		
        /// <param name="counterId">the id of the counter</param>
        void StopCounter(int counterId);

        ///// <summary>Returns the interval of the requested record in usecs</summary>		
        ///// <param name="Iteration">Provide the requested iteration number</param>
        ///// <returns>The interval of the requested record in usecs</returns>		
        //double GetInterval(int counterId,int Iteration);

        ///// <summary>Returns the interval of the requested record in usecs, and the associated time frame</summary>		
        ///// <param name="Iteration">Provide the requested iteration number</param>
        ///// <param name="curFrameTime">Get the timeframe for the requested record</param>
        ///// <returns>The interval of the requested record in usecs</returns>
        //double GetInterval(int counterId, int Iteration, out double curFrameTime);
	}
	
}















