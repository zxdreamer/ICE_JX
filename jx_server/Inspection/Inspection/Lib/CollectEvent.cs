using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace DataCollect.Lib
{

    public class CollectEvent
    {
        /// <summary>
        /// 数据接收事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public delegate void DataReceivedHandler(object sender,byte[] data);

        /// <summary>
        /// 发送数据事件
        /// </summary>
        /// <param name="data"></param>
        public delegate bool DataSendHandler(byte[] data);
        public delegate void Period_InquiryHandler();
        //查询录入事件
        //public delegate void Inquiry_InsertInfoHandler();

        //删除录入事件
       // public delegate void Delete_InsertInfoHandler();


    }
}
