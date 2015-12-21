using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net;
using System.Diagnostics;
using System.IO;
using Connections;

/**
* Things to improve :
* 1. Common function to update listview ( using interface )
* 2. Common function to get UDPv4 and UDPv6 data
* 3. Binary search to find IPAddress exists or not 
*/

namespace ViewTCP
{
    public partial class MainForm : Form
    {
        TCPConnections tc;
        UDPConnections uc;
        private StreamWriter fw;
        private string fileName = "";
        private const string TCPv4 = "TCPv4";
        private const string TCPv6 = "TCPv6";
        private const string UDPv4 = "UDPv4";
        private const string UDPv6 = "UDPv6";
        //     private List<MIB_TCP6ROW_OWNER_PID> listTCPv6;
        //   private List<MIB_TCPROW_OWNER_PID> listTCPv4;

        //  private List<MIB_UDP6ROW_OWNER_PID> listUDPv6;
        //    private List<MIB_UDPROW_OWNER_PID> listUDPv4;

        private Dictionary<string, List<int>> listViewData;
        //private Dictionary<string, int> newItemsList;

        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("  :");
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
        }
        /**
        * Sets 'DebugPrivilege' to current applications
        */
        private bool enableDebugPrivilege(ref string errorMessage)
        {
            Boolean bResult = false;
            IntPtr hToken;
            DebugPrivilege.LUID luidSEDebugNameValue;
            DebugPrivilege.TOKEN_PRIVILEGES tkpPrivileges;

            if (!DebugPrivilege.OpenProcessToken(DebugPrivilege.GetCurrentProcess(),
                            DebugPrivilege.TOKEN_ADJUST_PRIVILEGES | DebugPrivilege.TOKEN_QUERY, out hToken))
            {
                errorMessage = String.Format("OpenProcessToken() failed, error = {0} .SeDebugPrivilege is not available", Marshal.GetLastWin32Error());
                return bResult;
            }

            if (!DebugPrivilege.LookupPrivilegeValue(null, DebugPrivilege.SE_DEBUG_NAME, out luidSEDebugNameValue))
            {
                errorMessage = String.Format("LookupPrivilegeValue() failed, error = {0} .SeDebugPrivilege is not available", Marshal.GetLastWin32Error());
                IpHelperApi.CloseHandle(hToken);
                return bResult;
            }

            tkpPrivileges.PrivilegeCount = 1;
            tkpPrivileges.Luid = luidSEDebugNameValue;
            tkpPrivileges.Attributes = DebugPrivilege.SE_PRIVILEGE_ENABLED;

            if (!DebugPrivilege.AdjustTokenPrivileges(hToken, false, ref tkpPrivileges, 0, IntPtr.Zero, IntPtr.Zero))
            {
                errorMessage = String.Format("LookupPrivilegeValue() failed, error = {0} .SeDebugPrivilege is not available", Convert.ToString(Marshal.GetLastWin32Error()));
                return bResult;
            }
            bResult = true;
            IpHelperApi.CloseHandle(hToken);
            return bResult;

        }
        public string getStateInformation(MIB_TCP_STATE state)   {
            String sResult = "";
            switch(state)
            {
                case MIB_TCP_STATE.MIB_TCP_STATE_CLOSED: {
                        sResult = "Closed";
                        break;
                    }
                case MIB_TCP_STATE.MIB_TCP_STATE_LISTEN: {
                        sResult = "Listening";
                        break;
                    }
                case MIB_TCP_STATE.MIB_TCP_STATE_SYN_SENT:{
                        sResult = "SYN sent";
                        break;
                    }
                case MIB_TCP_STATE.MIB_TCP_STATE_SYN_RCVD:{
                        sResult = "SYN received";
                        break;
                    }
                case MIB_TCP_STATE.MIB_TCP_STATE_ESTAB:{
                        sResult = "Established";
                        break;
                    }
                case MIB_TCP_STATE.MIB_TCP_STATE_FIN_WAIT1:{
                        sResult = "Waiting for FIN-1";
                        break;
                    }
                case MIB_TCP_STATE.MIB_TCP_STATE_FIN_WAIT2:{
                        sResult = "Waiting for FIN-2";
                        break;
                    }
                case MIB_TCP_STATE.MIB_TCP_STATE_CLOSE_WAIT:{
                        sResult = "Close wait";
                        break;
                    }
                case MIB_TCP_STATE.MIB_TCP_STATE_CLOSING:{
                        sResult = "Closing";
                        break;
                    }
                case MIB_TCP_STATE.MIB_TCP_STATE_LAST_ACK:{
                        sResult = "Last ACK";
                        break;
                    }
                case MIB_TCP_STATE.MIB_TCP_STATE_TIME_WAIT:{
                        sResult = "Time wait";
                        break;
                    }
                case MIB_TCP_STATE.MIB_TCP_STATE_DELETE_TCB:{
                        sResult = "Delete TCB";
                        break;
                    }
            }
            return sResult;
        }
        /**
        * Expands the compressed ( '::1' etc.) ipv6 address to full format
        */
       private String expandCompressedIPv6Addr(String ipv6Addr)
        {
            StringBuilder sResult = new StringBuilder();
            if ( ipv6Addr == "::")
            {
                sResult.Append("0:0:0:0:0:0:0:0");
                return sResult.ToString();
            }
            try {
                IPAddress addr = IPAddress.Parse(ipv6Addr);
                if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    byte[] bytes = addr.GetAddressBytes();
                    for (int i = 0; i < 16; i += 2)
                    {
                        /*if ((bytes[i] == 0) && (bytes[i + 1] == 0))
                            sResult.Append("0:0");
                        else*/
                            sResult.AppendFormat("{0:x}{1:x}:", bytes[i], bytes[i + 1]);
                    }
                    sResult.Length = sResult.Length - 1;//last colon
                }
            } catch(Exception ex)
            {  }
            return sResult.ToString();
        }
       /**
       * gets the process name based on PID
       */ 
       private String getProcessName(int pid)
       {
            String sResult;
            if ( pid== GlobalVar.SYS_0)
            {
                sResult = "[System Process]";
            }
            else if (pid == GlobalVar.SYS_4)
            {
                sResult = "System";
            }
            else
            {
                try
                {
                    sResult = Process.GetProcessById(pid).ProcessName + ".exe";
                }
                catch (Exception ex)
                {
                    sResult = "<non-existent>";
                }
            }
            return sResult;
        }
        /**
        *
       */
        private void addItemsToListView<TABLE_ROW>(List<TABLE_ROW> list,string errorMessage,string protocol,bool isIPV6,bool isUDP)
        {
            if (list == null)
            {
                statusStrip1.Text = errorMessage;
                return;
            }
            String rowInformation = "", temp;
            ListViewItem[] lItem = new ListViewItem[list.Count];
            listView1.BeginUpdate();
            for(int i=0; i < list.Count; i++)
            {
                dynamic row = list[i];
                rowInformation = "";
                lItem[i] = new ListViewItem();
                //process name
                temp = getProcessName((int)row.ProcessId);
                rowInformation = temp;
                lItem[i].Text = temp;
                //pid
                temp = row.ProcessId.ToString();
                rowInformation = rowInformation + "::" + temp;
                lItem[i].SubItems.Add(temp);
                //protocol
                temp = protocol;
                rowInformation = rowInformation + "::" + temp;
                lItem[i].SubItems.Add(temp);
                //Local address
                if (isIPV6)
                {
                   temp = "[ " + expandCompressedIPv6Addr(row.LocalAddress.ToString()) + " ]";
                } else
                {
                    temp = row.LocalAddress.ToString();
                }
                rowInformation = rowInformation + "::" + temp;
                lItem[i].SubItems.Add(temp);
                //Local port
                temp = row.LocalPort.ToString();
                rowInformation = rowInformation + "::" + temp;
                lItem[i].SubItems.Add(temp);
                
                if ( !isUDP)
                {
                    //remote address
                    if (isIPV6)
                    {
                        temp = "[ " + expandCompressedIPv6Addr(row.RemoteAddress.ToString()) + " ]";
                    }
                    else
                    {
                        temp = row.RemoteAddress.ToString();
                    }
                    rowInformation = rowInformation + "::" + temp;
                    lItem[i].SubItems.Add(temp);
                    //remote port
                    temp = row.RemotePort.ToString();
                    rowInformation = rowInformation + "::" + temp;
                    lItem[i].SubItems.Add(temp);
                    //state information
                    temp = getStateInformation(row.State);
                    rowInformation = rowInformation + "::" + temp;
                    lItem[i].SubItems.Add(temp);
                } else
                {
                    lItem[i].SubItems.Add("***");
                    lItem[i].SubItems.Add("***");
                    lItem[i].SubItems.Add("-");
                    rowInformation = rowInformation + "::" + "***" + "::" + "***" + "::" + "-";
                }
                //lItem[i].SubItems.Add(DateTime.Now.ToString("en-US")); 
                // date-time info; will be used to check for item's uniqueness
                /*switch(protocol)
                {
                    case TCPv4:
                        lItem[i].ForeColor = Color.CadetBlue;
                        break;
                    case TCPv6:
                        lItem[i].ForeColor = Color.Ivory;
                        break;
                    case UDPv4:
                        lItem[i].ForeColor = Color.Honeydew;
                        break;
                    case UDPv6:
                        lItem[i].ForeColor = Color.Goldenrod;
                        break;

                }*/
                addOrUpdate(listViewData, rowInformation, i);
            }
            listView1.Items.AddRange(lItem);
            listView1.EndUpdate();


        }
        /**
        * Adds TCPv6 connections to list view
        */
        public void addTCPV6ConnectionsToListView()
        {
            int i;
            string strErrorMessage = "";
            List<MIB_TCP6ROW_OWNER_PID> listTCPv6 = new List<MIB_TCP6ROW_OWNER_PID>();
            listTCPv6 = tc.getTCP6Connections(ref strErrorMessage);
            addItemsToListView<MIB_TCP6ROW_OWNER_PID>(listTCPv6,strErrorMessage,TCPv6,true,false);
        }
        /**
        * Add TCPv4 connections to list view
        */
        public void addTcpv4ConnectionToListView()
        {
            string strErrorMessage ="";
            List<MIB_TCPROW_OWNER_PID> listTCPv4 = new List<MIB_TCPROW_OWNER_PID>();
            listTCPv4 =  tc.getTCPv4Connections(ref strErrorMessage);
            addItemsToListView(listTCPv4,strErrorMessage,TCPv4,false,false);
        }
        /**
        *
        */
        public void addUDPv4ConnectionsToListView()
        {
            string strErrorMessage = "";
            List<MIB_UDPROW_OWNER_PID> listUDPv4 = new List<MIB_UDPROW_OWNER_PID>();
            listUDPv4 = uc.getUDPv4Connections(ref strErrorMessage);
            addItemsToListView(listUDPv4, strErrorMessage, UDPv4, false, true);
        }
        /**
        *
        */
        public void addUDPv6ConnectionsToListView()
        {
            string strErrorMessage = "";
            List<MIB_UDP6ROW_OWNER_PID> listUDPv6 = new List<MIB_UDP6ROW_OWNER_PID>();
            listUDPv6 = uc.getUDPv6Connections(ref strErrorMessage);
            addItemsToListView(listUDPv6, strErrorMessage, UDPv6, true, true);
        }

        private void addData()
        {
            addTcpv4ConnectionToListView();
            addTCPV6ConnectionsToListView();
            addUDPv4ConnectionsToListView();
            addUDPv6ConnectionsToListView();
            for ( int i= 0; i< listView1.Columns.Count-1; i++) //avoid the last column
                listView1.AutoResizeColumn(i,ColumnHeaderAutoResizeStyle.ColumnContent);
            //addListViewDataToDict();
        }
        /**
        * Adds the dictionary fields in string output parameter
        */
        private String getDictionaryFields(Dictionary<string,int> Dict)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, int> kvp in Dict)
            {
                sb.AppendLine(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value));
            }
            return sb.ToString();
        }
        private String getDictionaryFields_IntList(Dictionary<string, List<int>> Dict)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbList = new StringBuilder();
            foreach (KeyValuePair<string, List<int>> kvp in Dict)
            {
                sbList.Clear();
                foreach (int i in kvp.Value)
                {
                    sbList.Append(i + " - ");
                }
                sb.AppendLine(string.Format("Key = {0}, Value = {1}", kvp.Key, sbList.ToString()));
            }
            return sb.ToString();
        }

        private void addOrUpdate(Dictionary<string, List<int>> dic, string key, int newValue)
        {
            List<int> val;
            if (dic.TryGetValue(key, out val))
            {
                //value exist
                val.Add(newValue);
                dic[key] = val;
            }
            else
            {
                //add the value
                val = new List<int>();
                val.Add(newValue);
                dic.Add(key,val);
            }
        }
        private void addOrUpdateNewItemsList(Dictionary<string, int> dic, string key)
        {
            int val;
            if (dic.TryGetValue(key, out val))
            {
                //value exist
                val = 1;
                dic[key] = val;
            }
            else
            {
                //add the value
                val = val + 1;
                dic.Add(key, val);
            }
        }

        /**
        *
        */
        private void  refreshListViewForNewData(Dictionary<string,int> newItemsList, 
               ref List<int> listIndexToRemove,ref List<string> toAddThisItems)
        {
            if (newItemsList == null)
            {
                return;
            }
            if ( listIndexToRemove == null) 
            {
                listIndexToRemove = new List<int>();
            }
            if ( toAddThisItems == null)
            {
                toAddThisItems = new List<string>();
            }
            String list = getDictionaryFields_IntList(listViewData);
            //Log("listViewData ::" +list, fw);
            //list = getDictionaryFields(newItemsList );
            //Log("newItemsList ::" + list,fw);
            //Check which endpoints have to be removed
            List<string> keys = listViewData.Keys.ToList();
            List<int> index;
            
            for (int i = keys.Count-1; i >=0; i--) //simple O(n) time check
            {
               
                if ( !newItemsList.ContainsKey(keys[i]))
                {
                    index = listViewData[keys[i]];
                    foreach (int k in index)
                    {
                        listIndexToRemove.Add(k);
                       // listView1.Items[k].BackColor = Color.Red;
                    }
                    listViewData.Remove(keys[i]);
                }
            }
            //Check which items to add
            keys.Clear();
            keys = newItemsList.Keys.ToList();
            int rowCount = listView1.Items.Count;
            for (int i=0; i < keys.Count; i++)
            {
                if (!listViewData.ContainsKey(keys[i]))
                {
                    rowCount++;
                    addOrUpdate(listViewData, keys[i], rowCount);
                    toAddThisItems.Add(keys[i]);

                } else //check if only a single instance of the connection is present
                // if not, it means that is a UDP endpoint, which can have multiple connections ON at the same time (Weird)
                {
                    if ( listViewData[keys[i]].Count > newItemsList[keys[i]]) //if oldCount > newCount, remove some old rows
                    {
                        listViewData[keys[i]].RemoveRange(newItemsList[keys[i]], listViewData[keys[i]].Count - newItemsList[keys[i]]);
                    } else if (listViewData[keys[i]].Count < newItemsList[keys[i]]) //if oldCount < newCount, add some new rows
                    {
                        List<int> listTemp = new List<int>();
                        for ( int cnt = listViewData[keys[i]].Count + 1; cnt < newItemsList.Count; cnt++)
                        {
                            rowCount++;
                            listTemp.Add(rowCount);
                        }
                        toAddThisItems.Add(keys[i]);
                        listViewData[keys[i]].AddRange(listTemp);
                    }
                    //if old count == new count, then no action required
                }
                
            }
        }
        /**
        *
        */
        private void refreshData_TCPv4(ref Dictionary<string,int> newItemsList)
        {
            string errorMessage = "";
            string oldEndpoint  = "";
            List<MIB_TCPROW_OWNER_PID> listTCPv4 = new List<MIB_TCPROW_OWNER_PID>();
            MIB_TCPROW_OWNER_PID row;
            //newItemsList.Clear();
            listTCPv4.Clear();
            listTCPv4 = tc.getTCPv4Connections(ref errorMessage);
            for (int i = 0; i < listTCPv4.Count; i++)
            {
                row = listTCPv4[i];
                oldEndpoint = getProcessName((int)row.ProcessId)
                            + "::" + row.ProcessId.ToString()
                            + "::" + TCPv4
                            + "::" + row.LocalAddress.ToString()
                            + "::" + row.LocalPort.ToString()
                            + "::" + row.RemoteAddress.ToString()
                            + "::" + row.RemotePort.ToString()
                            + "::" + getStateInformation(row.State)
                            ;

                addOrUpdateNewItemsList(newItemsList, oldEndpoint);
                oldEndpoint = oldEndpoint + "::" + DateTime.Now.ToString("en-US");
            }
            listTCPv4.Clear();
        }
        /**
        *
        */    
        private void refreshData_TCPv6(ref Dictionary<string, int> newItemsList)
        {
            string errorMessage = "";
            string oldEndpoint  = "";
            List<MIB_TCP6ROW_OWNER_PID> listTCPv6 = new List<MIB_TCP6ROW_OWNER_PID>();
            MIB_TCP6ROW_OWNER_PID row;
            listTCPv6.Clear();
            listTCPv6 = tc.getTCP6Connections(ref errorMessage);
            for (int i = 0; i < listTCPv6.Count; i++)
            {
                row = listTCPv6[i];
                oldEndpoint = getProcessName((int)row.ProcessId)
                            + "::" + row.ProcessId.ToString()
                            + "::" + TCPv6
                            + "::" + "[ " + expandCompressedIPv6Addr(row.LocalAddress.ToString()) + " ]"
                            + "::" + row.LocalPort.ToString()
                            + "::" + "[ " + expandCompressedIPv6Addr(row.RemoteAddress.ToString()) + " ]"
                            + "::" + row.RemotePort.ToString()
                            + "::" + getStateInformation(row.State);
                addOrUpdateNewItemsList(newItemsList, oldEndpoint);
                oldEndpoint = oldEndpoint + "::" + DateTime.Now.ToString("en-US");
            }
        }
        /**
        *
        */
        private void refreshData_UDPv4(ref Dictionary<string, int> newItemsList)
        {
            string errorMessage = "";
            string oldEndpoint  = "";
            List<MIB_UDPROW_OWNER_PID> listUDPv4 = new List<MIB_UDPROW_OWNER_PID>();
            MIB_UDPROW_OWNER_PID row;
            listUDPv4.Clear();
            listUDPv4 = uc.getUDPv4Connections(ref errorMessage);
            for (int i = 0; i < listUDPv4.Count; i++)
            {
                row = listUDPv4[i];
                oldEndpoint =   getProcessName((int)row.ProcessId) 
                                + "::" + row.ProcessId.ToString()
                                + "::" + UDPv4
                                + "::" + row.LocalAddress.ToString()
                                + "::" + row.LocalPort.ToString()
                                + "::" + "***" //Remote Address
                                + "::" + "***"// Remote port
                                + "::" + "-";
                addOrUpdateNewItemsList(newItemsList, oldEndpoint);
                oldEndpoint = oldEndpoint + "::" + DateTime.Now.ToString("en-US");
            }
        }
        /**
        * 
        */
        private void refreshData_UDPv6(ref Dictionary<string, int> newItemsList )
        {
            string errorMessage = "";
            string oldEndpoint  = "";
            MIB_UDP6ROW_OWNER_PID row;
            List<MIB_UDP6ROW_OWNER_PID> listUDPv6 = new List<MIB_UDP6ROW_OWNER_PID>();
            listUDPv6.Clear();
            listUDPv6 = uc.getUDPv6Connections(ref errorMessage);
            for (int i = 0; i < listUDPv6.Count; i++)
            {
                row = listUDPv6[i];
                oldEndpoint =   getProcessName((int)row.ProcessId) 
                                + "::" + row.ProcessId.ToString()
                                + "::" + UDPv6
                                + "::" + "[ " + expandCompressedIPv6Addr(row.LocalAddress.ToString()) + " ]"
                                + "::" + row.LocalPort.ToString() 
                                + "::" + "***" //Remote Address
                                + "::" + "***" //Remote port
                                + "::" + "-";  //state
                addOrUpdateNewItemsList(newItemsList, oldEndpoint);
                oldEndpoint = oldEndpoint + "::" + DateTime.Now.ToString("en-US");
              //  if (!newItemsListWithDateTime.Contains(oldEndpoint))
                //    newItemsListWithDateTime.Add(oldEndpoint);
            }
        }
        /**
        *
        */
        private void addNewItemsToListView(List<string> toAddThisItems)
        {
            listView1.BeginUpdate();
            for (int i=0;i< listView1.Items.Count; i++)
            {
                if (listView1.Items[i].BackColor == Color.Green)
                {
                    listView1.Items[i].BackColor = Color.White;
                }
            }
            listView1.EndUpdate();
            if (toAddThisItems.Count == 0)
                return;
            listView1.BeginUpdate();
            ListViewItem[] lItem = new ListViewItem[toAddThisItems.Count];
            for ( int i= 0; i < toAddThisItems.Count; i++)
            {
                
                string[] tokens = toAddThisItems.ElementAt(i).Split(new[] { "::" }, 
                                  StringSplitOptions.None);
                lItem[i] = new ListViewItem();
                for (int k=0; k <tokens.Length; k++)
                {
                    if (k == 0)
                        lItem[i].Text = tokens[k];
                    else
                    {
                        lItem[i].SubItems.Add(tokens[k]);
                    }
                }
                lItem[i].BackColor = Color.Green;
            }
           
            listView1.Items.AddRange(lItem);
            
            listView1.EndUpdate();
        }
        /**
        *
        */
        private void clearItemsFromListView(List<int> listIndexToRemove)
        {
            
            if ( listIndexToRemove.Count == 0 )
            {
                return;
            }
            listView1.BeginUpdate();
            for (int i = listView1.Items.Count-1; i>=0; i--)
            {
                if ( listView1.Items[i].BackColor == Color.Red)
                {
                    listView1.Items[i].Remove();
                }
                if ( listIndexToRemove.Contains(i))
                {
                    listView1.Items[i].BackColor = Color.Red;
                }
            }
            listView1.EndUpdate();
        }
        /**
        * Refresh the listview data, check which item is old / new
        * Old is to be removed, new is to be added
        * To check for old items : Check for equality between listView data and Structure data 
                                   ( Outer for loop contains listView data )
        * To check for new items : Equality check between Structure data and listView data
        *                          ( Outer for loop contains Structure data)
        */
        private void refreshData()
        {
            Dictionary<string, int> newItemsList   = new Dictionary<string, int>();
            List<string> toAddThisItems = new List<string>();
            List<string> newItemsListWithDateTime = new List<string>();
            List<int> listIndexToRemove = new List<int>();
            refreshData_TCPv4(ref newItemsList);
            refreshData_TCPv6(ref newItemsList);
            refreshData_UDPv4(ref newItemsList);
            refreshData_UDPv6(ref newItemsList);
            refreshListViewForNewData(newItemsList,ref listIndexToRemove,ref toAddThisItems); // to mark items "red" which are to be removed
            
            clearItemsFromListView(listIndexToRemove);
            addNewItemsToListView(toAddThisItems);

        }

        public MainForm()
        {
            InitializeComponent();
            listView1.Columns[8].Width = 0;

            fw = File.AppendText("log.txt");
            string errorMessage = "";
            Boolean bResult = enableDebugPrivilege(ref errorMessage);
            if (!bResult)
            {
                MessageBox.Show("failed :" + errorMessage);
                                
            } 
            tc = new TCPConnections();
            uc = new UDPConnections();
            listViewData = new Dictionary<string, List<int>>();
            addData();
            timerRefreshData.Interval = 5000;
            timerRefreshData.Enabled = true;
            
        }

        private void timerRefreshData_Tick(object sender, EventArgs e)
        {
            timerRefreshData.Enabled = false;
            refreshData();
            timerRefreshData.Enabled = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //listView1.Width = ViewTCP.MainForm.ActiveForm.Width - 30;

        }

        
        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timerAddRemoveData_Tick(object sender, EventArgs e)
        {
            //clearItemsFromListView();

        }

        private void showProcessPropertiesForm()
        {
            if (listView1.SelectedItems.Count == 1)
            {
                string processName = listView1.SelectedItems[0].Text;
                string pid = listView1.SelectedItems[0].SubItems[1].Text;
                if ((String.Equals("System", processName, StringComparison.OrdinalIgnoreCase))
                || ((String.Equals("[System Process]", processName, StringComparison.OrdinalIgnoreCase))
                || (String.Equals("<non-existent>", processName, StringComparison.OrdinalIgnoreCase))))
                {
                    MessageBox.Show("Unable to query properties for " + processName, "ViewTCP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                PropertiesForm prop = new PropertiesForm();
                try
                {
                    prop.p = Process.GetProcessById(Convert.ToInt32(pid));
                    //prop.PID = pid;
                    prop.processPath = prop.p.MainModule.FileName;
                    prop.Text = "Properties for " + processName + ":" + pid;

                    prop.ShowDialog(this);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to query properties for " + processName, "ViewTCP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;

                }
            }

        }
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            showProcessPropertiesForm();
        }

        private void processPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showProcessPropertiesForm();
        }
        /**
        *
        */
        private void endProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                try
                {
                    Process p = Process.GetProcessById(Convert.ToInt32(listView1.SelectedItems[0].SubItems[1].Text));
                    p.Kill();
                }
                catch(System.Exception)
                {
                    MessageBox.Show("Unable to kill process " + listView1.SelectedItems[0].Text);
                }
            }
        }

        private void secondToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timerRefreshData.Enabled = false;
            timerRefreshData.Interval= 1000;
            timerRefreshData.Enabled = true;
        }

        private void secondToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            timerRefreshData.Enabled = false;
            timerRefreshData.Interval= 2000;
            timerRefreshData.Enabled = true;
        }

        private void secondToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            timerRefreshData.Enabled = false;
            timerRefreshData.Interval= 5000;
            timerRefreshData.Enabled = true;
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timerRefreshData.Enabled = !(timerRefreshData.Enabled);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timerRefreshData.Enabled = false;
            timerRefreshData.Enabled = true;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutTcpView about = new AboutTcpView();
            about.ShowDialog(this);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListView.SelectedListViewItemCollection selectedItems =
                            listView1.SelectedItems;
                String text = "";
                foreach (ListViewItem item in selectedItems)
                {
                    for(int i=0; i < item.SubItems.Count; i++) 
                        text = text + "\t" + item.SubItems[i].Text;
                    text = text + "\r\n";
                }
                Clipboard.SetText(text);
            }
        }
        private void writeToFile(string fileName)
        {
            string text = "";
            listView1.BeginUpdate();
            foreach (ListViewItem lItem in listView1.Items)
            {
                for (int i = 0; i < lItem.SubItems.Count; i++)
                    text = text + "\t" + lItem.SubItems[i].Text;
                text = text + "\r\n";
            }
            using (StreamWriter sw = new StreamWriter(fileName))
                sw.WriteLine(text);
            listView1.EndUpdate();

        }
        private void saveFile(bool bIsOnlySave = false)
        {
            if (!bIsOnlySave)
            {
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                save.RestoreDirectory = true;
                save.OverwritePrompt = true;
                if (save.ShowDialog() == DialogResult.OK)
                {
                    fileName = save.FileName;
                    writeToFile(fileName);
                }
            } else
            {
                writeToFile(fileName);
            }            
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFile(true);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFile();
        }
    }
}
