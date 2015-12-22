using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Net;

namespace Connections
{
    public static class GlobalVar
    {
        public const int AF_INET = 2;
        public const int AF_INET6= 23;
        public const int SYS_0 = 0; //name to be [System Process]
        public const int SYS_4 = 4; //name to be System 
    }
    enum FormatMessageFlags
    {
        FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100,
        FORMAT_MESSAGE_ARGUMENT_ARRAY  = 0x00002000,
        FORMAT_MESSAGE_FROM_HMODULE    = 0x00000800,
        FORMAT_MESSAGE_FROM_STRING     = 0x00000400,
        FORMAT_MESSAGE_FROM_SYSTEM     = 0x00001000,
        FORMAT_MESSAGE_IGNORE_INSERTS  = 0x00000200
    };

    #region TCP_Structure
    enum TCP_TABLE_CLASS
    {
        TCP_TABLE_BASIC_LISTENER,
        TCP_TABLE_BASIC_CONNECTIONS,
        TCP_TABLE_BASIC_ALL,
        TCP_TABLE_OWNER_PID_LISTENER,
        TCP_TABLE_OWNER_PID_CONNECTIONS,
        TCP_TABLE_OWNER_PID_ALL,
        TCP_TABLE_OWNER_MODULE_LISTENER,
        TCP_TABLE_OWNER_MODULE_CONNECTIONS,
        TCP_TABLE_OWNER_MODULE_ALL
    }
    public enum MIB_TCP_STATE
    {
        MIB_TCP_STATE_CLOSED = 1,
        MIB_TCP_STATE_LISTEN = 2,
        MIB_TCP_STATE_SYN_SENT = 3,
        MIB_TCP_STATE_SYN_RCVD = 4,
        MIB_TCP_STATE_ESTAB = 5,
        MIB_TCP_STATE_FIN_WAIT1 = 6,
        MIB_TCP_STATE_FIN_WAIT2 = 7,
        MIB_TCP_STATE_CLOSE_WAIT = 8,
        MIB_TCP_STATE_CLOSING = 9,
        MIB_TCP_STATE_LAST_ACK = 10,
        MIB_TCP_STATE_TIME_WAIT = 11,
        MIB_TCP_STATE_DELETE_TCB = 12
    }
    #endregion
    #region TCP6

    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_TCP6ROW_OWNER_PID
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] localAddr;
        public uint localScopeId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] localPort;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] remoteAddr;
        public uint remoteScopeId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] remotePort;
        public uint state;
        public uint owningPid;

        public uint ProcessId
        {
            get { return owningPid; }
        }

        public long LocalScopeId
        {
            get { return localScopeId; }
        }

        public IPAddress LocalAddress
        {
            get { return new IPAddress(localAddr, LocalScopeId); }
        }

        public ushort LocalPort
        {
            get { return BitConverter.ToUInt16(localPort.Take(2).Reverse().ToArray(), 0); }
        }

        public long RemoteScopeId
        {
            get { return remoteScopeId; }
        }

        public IPAddress RemoteAddress
        {
            get { return new IPAddress(remoteAddr, RemoteScopeId); }
        }

        public ushort RemotePort
        {
            get { return BitConverter.ToUInt16(remotePort.Take(2).Reverse().ToArray(), 0); }
        }

        public MIB_TCP_STATE State
        {
            get { return (MIB_TCP_STATE)state; }
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_TCP6TABLE_OWNER_PID
    {
        public uint dwNumEntries;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
        public MIB_TCP6ROW_OWNER_PID[] table;
    }
    #endregion
    #region TCPv4
    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_TCPROW_OWNER_PID
    {
        public uint state;
        public uint localAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] localPort;
        public uint remoteAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] remotePort;
        public uint owningPid;

        public uint ProcessId
        {
            get { return owningPid; }
        }

        public IPAddress LocalAddress
        {
            get { return new IPAddress(localAddr); }
        }

        public ushort LocalPort
        {
            get
            {
                return BitConverter.ToUInt16(new byte[2] { localPort[1], localPort[0] }, 0);
            }
        }

        public IPAddress RemoteAddress
        {
            get { return new IPAddress(remoteAddr); }
        }

        public ushort RemotePort
        {
            get
            {
                return BitConverter.ToUInt16(new byte[2] { remotePort[1], remotePort[0] }, 0);
            }
        }

        public MIB_TCP_STATE State
        {
            get { return (MIB_TCP_STATE)state; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_TCPTABLE_OWNER_PID
    {
        public uint dwNumEntries;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
        public MIB_TCPROW_OWNER_PID[] table;
    }
    #endregion

    #region UDP_Structure
    enum UDP_TABLE_CLASS
    {
        UDP_TABLE_BASIC =0,
        UDP_TABLE_OWNER_PID =1,
        UDP_OWNER_MODULE =2
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_UDPTABLE_OWNER_PID
    {
        public int dwNumEntries;
        MIB_UDPROW_OWNER_PID[] table;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_UDPROW_OWNER_PID
    {
        public uint dwLocalAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dwLocalPort;
        public uint dwOwningPid;

        public uint ProcessId
        {
            get { return dwOwningPid; }
        }

        public IPAddress LocalAddress
        {
            get { return new IPAddress(dwLocalAddr); }
        }

        public ushort LocalPort
        {
            get
            {
                return BitConverter.ToUInt16(new byte[2] { dwLocalPort[1], dwLocalPort[0] }, 0);
            }
        }

    }

    #endregion UDP_Structure
    #region UDP6_Structure
    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_UDP6TABLE_OWNER_PID
    {
        public int dwNumEntries;
        MIB_UDP6ROW_OWNER_PID[] table;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_UDP6ROW_OWNER_PID
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] localAddr;
        public uint dwLocalScopeId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] dwLocalPort;
        public uint dwOwningPid;
        

        public uint ProcessId
        {
            get { return dwOwningPid; }
        }
        public uint ScopeId
        {
            get { return dwLocalScopeId;  }
        }
        public IPAddress LocalAddress
        {
            get { return new IPAddress(localAddr); }
        }

        public ushort LocalPort
        {
            get
            {
                return BitConverter.ToUInt16(new byte[2] { dwLocalPort[1], dwLocalPort[0] }, 0);
            }
        }

    }
    #endregion
    #region DebugPrivilege
    class DebugPrivilege
    {
        public const string SE_DEBUG_NAME = "SeDebugPrivilege";
        public const UInt32 SE_PRIVILEGE_ENABLED    = 0x00000002;
        public static uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        public static uint STANDARD_RIGHTS_READ     = 0x00020000;
        public static uint TOKEN_ASSIGN_PRIMARY     = 0x0001;
        public static uint TOKEN_DUPLICATE          = 0x0002;
        public static uint TOKEN_IMPERSONATE        = 0x0004;
        public static uint TOKEN_QUERY              = 0x0008;
        public static uint TOKEN_QUERY_SOURCE       = 0x0010;
        public static uint TOKEN_ADJUST_PRIVILEGES  = 0x0020;
        public static uint TOKEN_ADJUST_GROUPS      = 0x0040;
        public static uint TOKEN_ADJUST_DEFAULT     = 0x0080;
        public static uint TOKEN_ADJUST_SESSIONID   = 0x0100;
        public static uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);
        public static uint TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY |
            TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE |
            TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT |
            TOKEN_ADJUST_SESSIONID);

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            public UInt32 LowPart;
            public Int32 HighPart;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_PRIVILEGES
        {
            public UInt32 PrivilegeCount;
            public LUID Luid;
            public UInt32 Attributes;
        }
        #region DLLCalls
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle,
            UInt32 DesiredAccess, out IntPtr TokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeValue(string lpSystemName, string lpName,
            out LUID lpLuid);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle,
                                   [MarshalAs(UnmanagedType.Bool)]bool DisableAllPrivileges,
                                   ref TOKEN_PRIVILEGES NewState, UInt32 Zero,
                                   IntPtr Null1, IntPtr Null2);
        #endregion

    }
    #endregion
    #region IPHelperApi
    class IpHelperApi
    {
        public const int ERROR_SUCCESS = 0;
        public const int ERROR_INVALID_PARAMETER = 87;
        public const int ERROR_INSUFFICIENT_BUFFER = 122;

        #region DLLCalls
        [DllImport("iphlpapi.dll", SetLastError = true , CallingConvention = CallingConvention.StdCall)]
        public static extern uint GetExtendedTcpTable(IntPtr pTcpTable, ref int dwOutBufLen, bool sort, 
            int ipVersion, TCP_TABLE_CLASS tblClass, uint reserved = 0);

        [DllImport("iphlpapi.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern uint GetExtendedUdpTable(IntPtr pUdpTable, ref int dwOutBufLen, 
                bool sort, int ipVersion, UDP_TABLE_CLASS tblClass, int reserved = 0);

        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        private static extern int FormatMessage(int flags, IntPtr source, int messageId,
            int languageId, StringBuilder buffer, int size, IntPtr arguments);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hHandle);


        #endregion

        public static string GetErrorMessage(int ErrorNumber)
        {
            StringBuilder sErr = new StringBuilder(1024);
            if (FormatMessage((int)FormatMessageFlags.FORMAT_MESSAGE_FROM_SYSTEM, (IntPtr)0, ErrorNumber, 0, sErr, sErr.Length, (IntPtr)0) > 0)
            {
                return sErr.ToString();
            }
            return sErr.ToString();
        }

    }
    #endregion

}
