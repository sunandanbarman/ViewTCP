using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

/**
* Major inspiration for DLL call conventions and correct structure usage is from pinvoke.net
* 
*/
namespace Connections
{
    class TCPConnections
    {
        public List<MIB_TCP6ROW_OWNER_PID> getTCP6Connections(ref string errorMessage)
        {
            return getConnections<MIB_TCP6TABLE_OWNER_PID,MIB_TCP6ROW_OWNER_PID>(
                                 GlobalVar.AF_INET6,ref errorMessage);
        }
        public List<MIB_TCPROW_OWNER_PID> getTCPv4Connections(ref string errorMessage)
        {
            return getConnections<MIB_TCPTABLE_OWNER_PID, MIB_TCPROW_OWNER_PID>(
                                  GlobalVar.AF_INET, ref errorMessage);
        }
        private List<TCP_ROW> getConnections<TCP_TABLE,TCP_ROW>(int ipVersion,ref string strErrorMessage)
        {
            TCP_ROW[] tableRows;
            int buffSize = 0;
            int dwResult = 0;
            var dwNumEntriesField = typeof(TCP_TABLE).GetField("dwNumEntries");

            // how much memory do we need?
            IpHelperApi.GetExtendedTcpTable(IntPtr.Zero, ref buffSize, true, ipVersion, 
                       TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);
            dwResult = Marshal.GetLastWin32Error();
            if (dwResult != IpHelperApi.ERROR_SUCCESS)
            {
                
                strErrorMessage = IpHelperApi.GetErrorMessage(dwResult);
                return null;
            }
            IntPtr tcpTablePtr = Marshal.AllocHGlobal(buffSize);

            try
            {
                IpHelperApi.GetExtendedTcpTable(tcpTablePtr, ref buffSize, true, ipVersion, 
                      TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);

                dwResult = Marshal.GetLastWin32Error();
                if (dwResult != IpHelperApi.ERROR_SUCCESS) // error condition , return !! 
                {
                    strErrorMessage = IpHelperApi.GetErrorMessage(dwResult);
                    return null;
                }

                // get the number of entries in the table
                TCP_TABLE table = (TCP_TABLE)Marshal.PtrToStructure(tcpTablePtr, typeof(TCP_TABLE));
                int rowStructSize = Marshal.SizeOf(typeof(TCP_ROW));
                uint numEntries = (uint)dwNumEntriesField.GetValue(table);

                // buffer we will be returning
                tableRows = new TCP_ROW[numEntries];

                IntPtr rowPtr = (IntPtr)((long)tcpTablePtr + 4);
                for (int i = 0; i < numEntries; i++)
                {
                    TCP_ROW tcpRow = (TCP_ROW)Marshal.PtrToStructure(rowPtr, typeof(TCP_ROW));
                    tableRows[i] = tcpRow;
                    rowPtr = (IntPtr)((long)rowPtr + rowStructSize);   // next entry
                }
            }
            finally
            {
                // Free the Memory
                Marshal.FreeHGlobal(tcpTablePtr);
            }
            return tableRows != null ? tableRows.ToList() : null;
        }
    }
    class UDPConnections
    {
        /*private List<UDP_ROW> getConnections<UDP_TABLE, UDP_ROW>(int ipVersion, 
                                            ref string strErrorMessage)
        {
            UDP_ROW[] tableRows;
            int dwResult = 0, buffSize = 0;
            var dwNumEntriesField = typeof(UDP_TABLE).GetField("dwNumEntries");
            IpHelperApi.GetExtendedUdpTable(IntPtr.Zero, ref buffSize, true, GlobalVar.AF_INET,
                                            UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
            dwResult = Marshal.GetLastWin32Error();
            if (dwResult != IpHelperApi.ERROR_SUCCESS)
            {
                dwResult = Marshal.GetLastWin32Error();
                strErrorMessage = IpHelperApi.GetErrorMessage(dwResult);
                return null;
            }
            IntPtr udpTablePtr = Marshal.AllocHGlobal(buffSize);
            try
            {
                IpHelperApi.GetExtendedUdpTable(udpTablePtr, ref buffSize, true, ipVersion,
                      UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID,0);

                dwResult = Marshal.GetLastWin32Error();
                if (dwResult != IpHelperApi.ERROR_SUCCESS) // error condition , return !! 
                {
                    strErrorMessage = IpHelperApi.GetErrorMessage(dwResult);
                    return null;
                }
                // get the number of entries in the table
                UDP_TABLE table = (UDP_TABLE)Marshal.PtrToStructure(udpTablePtr, typeof(UDP_TABLE));
                int rowStructSize = Marshal.SizeOf(typeof(UDP_ROW));
                uint numEntries = (uint)dwNumEntriesField.GetValue(table);

                // buffer we will be returning
                tableRows = new UDP_ROW[numEntries];

                IntPtr rowPtr = (IntPtr)((long)udpTablePtr + 4);
                for (int i = 0; i < numEntries; i++)
                {
                    UDP_ROW tcpRow = (UDP_ROW)Marshal.PtrToStructure(rowPtr, typeof(UDP_ROW));
                    tableRows[i] = tcpRow;
                    rowPtr = (IntPtr)((long)rowPtr + rowStructSize);   // next entry
                }
                
            }
            finally
            {
                Marshal.FreeHGlobal(udpTablePtr);
            }
            return tableRows != null ? tableRows.ToList() : null;
        } 
        public List<MIB_UDPROW_OWNER_PID> getUDPv4Connections(ref string errorMessage)
        {
            return getConnections<MIB_UDPTABLE_OWNER_PID, MIB_UDPROW_OWNER_PID>(
                                  GlobalVar.AF_INET, ref errorMessage);
        }*/
        public List<MIB_UDPROW_OWNER_PID> getUDPv4Connections(ref string strErrorMessage)
        {
            MIB_UDPROW_OWNER_PID[] tTable;
            
            int buffSize = 0;
            int dwResult = 0;
            IpHelperApi.GetExtendedUdpTable(IntPtr.Zero, ref buffSize, true, GlobalVar.AF_INET, 
                                            UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
            dwResult = Marshal.GetLastWin32Error();
            if ( dwResult != IpHelperApi.ERROR_SUCCESS)
            {
                strErrorMessage = IpHelperApi.GetErrorMessage(dwResult);
                return null;
            }
            IntPtr buffTable = Marshal.AllocHGlobal(buffSize);

            try
            {
                IpHelperApi.GetExtendedUdpTable(buffTable, ref buffSize, true, GlobalVar.AF_INET,
                                               UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
                dwResult = Marshal.GetLastWin32Error();
                if ( dwResult != IpHelperApi.ERROR_SUCCESS)
                {
                    strErrorMessage = IpHelperApi.GetErrorMessage(dwResult);
                    return null;
                }
               
                MIB_UDPTABLE_OWNER_PID tab = (MIB_UDPTABLE_OWNER_PID)Marshal.PtrToStructure(buffTable, 
                                             typeof(MIB_UDPTABLE_OWNER_PID));
                IntPtr rowPtr = (IntPtr)((long)buffTable + Marshal.SizeOf(tab.dwNumEntries));
                tTable = new MIB_UDPROW_OWNER_PID[tab.dwNumEntries];

                for (int i = 0; i < tab.dwNumEntries; i++)
                {
                    MIB_UDPROW_OWNER_PID udprow = (MIB_UDPROW_OWNER_PID)Marshal.PtrToStructure(rowPtr, 
                                                  typeof(MIB_UDPROW_OWNER_PID));
                    tTable[i] = udprow;
                    rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf(udprow));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffTable);
            }
            return tTable != null ? tTable.ToList() : null;
        }
        /**
        *
        */    
        public List<MIB_UDP6ROW_OWNER_PID> getUDPv6Connections(ref string strErrorMessage)
        {
            MIB_UDP6ROW_OWNER_PID[] tTable;

            int buffSize = 0;
            int dwResult = 0;
            IpHelperApi.GetExtendedUdpTable(IntPtr.Zero, ref buffSize, true, GlobalVar.AF_INET6,
                                            UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID);
            dwResult = Marshal.GetLastWin32Error();
            if (dwResult != IpHelperApi.ERROR_SUCCESS)
            {
                strErrorMessage = IpHelperApi.GetErrorMessage(dwResult);
                return null;
            }
            IntPtr buffTable = Marshal.AllocHGlobal(buffSize);

            try
            {
                IpHelperApi.GetExtendedUdpTable(buffTable, ref buffSize, true, GlobalVar.AF_INET6, 
                                                UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID);
                dwResult = Marshal.GetLastWin32Error();
                if (dwResult != IpHelperApi.ERROR_SUCCESS)
                {
                    strErrorMessage = IpHelperApi.GetErrorMessage(dwResult);
                    return null;
                }

                MIB_UDP6TABLE_OWNER_PID tab = (MIB_UDP6TABLE_OWNER_PID)Marshal.PtrToStructure(buffTable, 
                                                typeof(MIB_UDP6TABLE_OWNER_PID));
                IntPtr rowPtr = (IntPtr)((long)buffTable + Marshal.SizeOf(tab.dwNumEntries));
                tTable = new MIB_UDP6ROW_OWNER_PID[tab.dwNumEntries];

                for (int i = 0; i < tab.dwNumEntries; i++)
                {
                    MIB_UDP6ROW_OWNER_PID udpRow = (MIB_UDP6ROW_OWNER_PID)Marshal.PtrToStructure(rowPtr, 
                                                    typeof(MIB_UDP6ROW_OWNER_PID));
                    tTable[i] = udpRow;
                    rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf(udpRow));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffTable);
            }
            return tTable != null ? tTable.ToList() : null;
        }

    }
}
