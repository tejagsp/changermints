using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ChangerMints.Business;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace ChangerMintsWorkerRole {
    public class WorkerRole : RoleEntryPoint {
        private AutoResetEvent connectionWaitHandle = new AutoResetEvent(false);

        public override void Run() {
            Trace.WriteLine("Starting Socket server...", "Information");

            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            // running the listener is "host.contoso.com".
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["ChangerMintsEndpoint"].IPEndpoint;

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            Trace.WriteLine("Starting server...", "Information");
            try {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true) {
                    // Set the event to nonsignaled state.
                    //allDone.Reset();
                    connectionWaitHandle.Reset();
                    // Start an asynchronous socket to listen for connections.
                    listener.BeginAccept(
                    new AsyncCallback(HandleAsyncConnection),
                    listener);

                    // Wait until a connection is made before continuing.
                    //allDone.WaitOne();
                    connectionWaitHandle.WaitOne();
                }
            }
            catch (Exception e) {
                Trace.Write("Socket server could not start.", "Error");
                return;
            }
        }

        private void HandleAsyncConnection(IAsyncResult result) {
            // Signal the main thread to continue.
            //allDone.Set();
            connectionWaitHandle.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)result.AsyncState;
            Socket handler = listener.EndAccept(result);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
            new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult ar) {
            String content = String.Empty;
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0) {
                // There might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read
                // more data.
                content = state.sb.ToString();
                //var nc = new CultureInfo(state.sb.ToString(), true);
                if (content.IndexOf("#") > -1) {
                    // All the data has been read from the
                    // client. Display it on the console.
                    Trace.Write("Read " + content.Length + " bytes from socket. \n Data : " + content, "Information");
                    // Echo the data back to the client.

                    string output = null;

                    if (content.Contains('?') == false)
                    {
                    }
                    else {
                        RunTerminalTransactions terminalTransaction = new RunTerminalTransactions();
                        RunCustomerRegistration customerRegistration = new RunCustomerRegistration();

                        var input = content.Split('?');
                        var inputRequest = Convert.ToInt64(input[0]);

                        switch (inputRequest) {
                            case 5001:
                            case 5002:
                            case 5003:
                            case 5004:
                                output = terminalTransaction.RunTransaction(content);
                                break;

                            case 5100:
                                output = customerRegistration.NFCCustomerRegistration(content);
                                break;

                            case 5101:
                                output = customerRegistration.NFCCustomerRegistration(content);
                                break;

                            default:
                                break;
                        }
                    }
                    CustomerRegistrationService cr = new CustomerRegistrationService();
                    //writer.WriteLine("Role ID: " + RoleEnvironment.CurrentRoleInstance.Id + " Connection ID: " + clientId.ToString() + "Status " + status.ToString());
                    Send(handler, output.ToString(CultureInfo.InvariantCulture));
                } else {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private void Send(Socket handler, String data) {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar) {
            try {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Trace.Write("Sent " + bytesSent + " bytes to client.", "Information");
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        public override bool OnStart() {
            // Set the maximum number of concurrent connections
            //ServicePointManager.DefaultConnectionLimit = 55;
            //DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString");
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            //RoleEnvironment.Changing += RoleEnvironmentChanging;

            return base.OnStart();
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e) {
            // If a configuration setting is changing
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange)) {
                // Set e.Cancel to true to restart this role instance
                e.Cancel = true;
            }
        }
    }

    // State object for reading client data asynchronously
    public class StateObject {
        // Client socket.
        public Socket workSocket = null;

        // Size of receive buffer.
        public const int BufferSize = 1024;

        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];

        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
}