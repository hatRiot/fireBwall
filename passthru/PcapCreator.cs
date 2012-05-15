using System;
using System.Collections.Generic;
using System.Text;

namespace PassThru
{
    /// <summary>
    /// Generates a unique timestamp for pcap files
    /// </summary>
	class PcapCreator
    {
        /// <summary>
        /// Sets the last time used as now
        /// </summary>
		PcapCreator() 
        {
			last = DateTime.Now;
		}

		DateTime last;
		static readonly object padlock = new object();

        /// <summary>
        /// Returns new timestamp for file name
        /// </summary>
        /// <returns></returns>
		public string GetNewDate() 
        {
			while (DateTime.Now == last)
			{
					System.Threading.Thread.Sleep(1);
			}
			last = DateTime.Now;
            return last.Month.ToString() + "-" + last.Day.ToString() + "-" + last.Year.ToString() + "_" + last.Hour.ToString() + "_" + last.Minute.ToString() + "_" + last.Second.ToString();
		}

        /// <summary>
        /// Singleton implementation
        /// </summary>
		public static PcapCreator Instance 
        {
			get 
            {
				lock (padlock)
				{
						return instance ?? (instance = new PcapCreator());
				}
			}
		}
		static PcapCreator instance = null;
	}
}
