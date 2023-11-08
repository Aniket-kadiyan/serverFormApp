﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serverInteract
{
    public class EchoServiceProvider : TcpServiceProvider
    {
        private string _receivedStr;
        public ConnectionState connstate;

        public override object Clone()
        {
            return new EchoServiceProvider();
        }

        public override void
               OnAcceptConnection(ConnectionState state)
        {
            _receivedStr = "";
            connstate= state;
            if (!state.Write(Encoding.UTF8.GetBytes(
                            "Hello World!\r\n"), 0, 14))
                state.EndConnection();
            //if write fails... then close connection
        }


        public override void OnReceiveData(ConnectionState state)
        {
            connstate = state;
            byte[] buffer = new byte[1024];
            while (state.AvailableData > 0)
            {
                int readBytes = state.Read(buffer, 0, 1024);
                if (readBytes > 0)
                {
                    _receivedStr +=
                      Encoding.UTF8.GetString(buffer, 0, readBytes);
                    if (_receivedStr.IndexOf("<EOF>") >= 0)
                    {
                        state.Write(Encoding.UTF8.GetBytes(_receivedStr), 0,
                        _receivedStr.Length);
                        _receivedStr = "";
                    }
                }
                else state.EndConnection();
                //If read fails then close connection
            }
        }


        public override void OnDropConnection(ConnectionState state)
        {
            connstate = state;
            //Nothing to clean here
        }
    }
}
