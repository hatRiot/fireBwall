using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net.NetworkInformation;
using System.Net;
using FM;

namespace IPMonitor
{
    public partial class IPMonitorDisplay : UserControl
    {
        // local ipmonitoring mod
        private IPMonitorModule ipmon;
        
        // local tcp cache
        private List<TcpConnectionInformation> tcpcache;
        public List<TcpConnectionInformation> Tcpcache
             { get { return tcpcache; } set { tcpcache = new List<TcpConnectionInformation>(value); } }

        // local udp cache
        private List<IPEndPoint> udpcache;
        public List<IPEndPoint> Udpcache
            { get { return udpcache; } set { udpcache = new List<IPEndPoint>(value); } }

        public IPMonitorDisplay(IPMonitorModule mon)
        {
            this.ipmon = mon;
            tcpcache = new List<TcpConnectionInformation>();
            InitializeComponent();
        }

        /// <summary>
        /// Update the TCP connections
        /// </summary>
        private void TCPUpdate()
        {
            // preserve the idx of the viewport
            int idx = tcpDisplay.FirstDisplayedScrollingRowIndex;

            // if the datasource hasn't been set, set it
            if (tcpDisplay.DataSource != this.tcpcache)
                tcpDisplay.DataSource = this.tcpcache;

            // update local tcpcache
            tcpcache = new List<TcpConnectionInformation>(ipmon.Tcpcache);
            
            // -1 = no viewport
            if ( idx >= 0 )
                tcpDisplay.FirstDisplayedScrollingRowIndex = idx;

            // set the total connections label
            tcpTotal.Text = String.Format("{0}: {1}","Connections", tcpcache.Count.ToString());
        }

        /// <summary>
        /// Update the UDP connections
        /// </summary>
        private void UDPUpdate()
        {
            int idx = udpDisplay.FirstDisplayedScrollingRowIndex;

            if (udpDisplay.DataSource != this.udpcache)
                udpDisplay.DataSource = this.udpcache;

            udpcache = new List<IPEndPoint>(ipmon.Udpcache);

            if (idx >= 0)
                udpDisplay.FirstDisplayedScrollingRowIndex = idx;

            udpTotal.Text = String.Format("{0}: {1}", "Listeners", udpcache.Count.ToString());
        }

        /// <summary>
        /// Set the TCP statistics fields
        /// </summary>
        private void DumpStats()
        {
            IPGlobalProperties stats = IPGlobalProperties.GetIPGlobalProperties();
            TcpStatistics tcpstat = stats.GetTcpIPv4Statistics();
            UdpStatistics udpstat = stats.GetUdpIPv4Statistics();

            // set labels 
            // this doesn't need to be done each time
            setLabels();

            // set the TCP field labels
            connAcceptField.Text = tcpstat.ConnectionsAccepted.ToString();
            connInitiatedField.Text = tcpstat.ConnectionsInitiated.ToString();
            cumulativeConnectionsField.Text = tcpstat.CumulativeConnections.ToString();

            errorsReceivedField.Text = tcpstat.ErrorsReceived.ToString();
            failedConAttField.Text = tcpstat.FailedConnectionAttempts.ToString();
            maxConnField.Text = tcpstat.MaximumConnections.ToString();

            resetConnField.Text = tcpstat.ResetConnections.ToString();
            resetsSentField.Text = tcpstat.ResetsSent.ToString();
            segsReceivedField.Text = tcpstat.SegmentsReceived.ToString();

            segsResentField.Text = tcpstat.SegmentsResent.ToString();
            segsSentField.Text = tcpstat.SegmentsSent.ToString();

            // set the UDP field labels
            dataReceivedField.Text = udpstat.DatagramsReceived.ToString();
            dataSentField.Text = udpstat.DatagramsSent.ToString();

            incDataDiscField.Text = udpstat.IncomingDatagramsDiscarded.ToString();
            incDataErrField.Text = udpstat.IncomingDatagramsWithErrors.ToString();
        }
        /// <summary>
        /// invoke the TCP datagridview updater
        /// </summary>
        public void UpdateTCP()
        {
            if (tcpDisplay.InvokeRequired)
            {
                System.Threading.ThreadStart up = new System.Threading.ThreadStart(TCPUpdate);
                tcpDisplay.Invoke(up);
            }
            else
                TCPUpdate();
        }

        /// <summary>
        /// invoke the UDP datagridview updater
        /// </summary>
        public void UpdateUDP()
        {
            if (udpDisplay.InvokeRequired)
            {
                System.Threading.ThreadStart up = new System.Threading.ThreadStart(UDPUpdate);
                udpDisplay.Invoke(up);
            }
            else
                UDPUpdate();
        }

        /// <summary>
        /// invoke the statistics updater
        /// </summary>
        public void UpdateStats()
        {
            if (statistics.InvokeRequired)
            {
                System.Threading.ThreadStart up = new System.Threading.ThreadStart(DumpStats);
                statistics.Invoke(up);
            }
            else
                DumpStats();
        }

        /// <summary>
        /// Big fat language-setting-label-setter
        /// </summary>
        private void setLabels()
        {
            switch (LanguageConfig.GetCurrentLanguage())
            {
                case LanguageConfig.Language.NONE:
                case LanguageConfig.Language.ENGLISH:
                    connAcceptLabel.Text = "Connections Accepted:";
                    connInitLabel.Text = "Connections Initiated:";
                    cumulativeConnLabel.Text = "Cumulative Connections:";
                    errRecLabel.Text = "Errors Received:";
                    failedConnAttLabel.Text = "Failed Connection Attempts:";
                    maxConLabel.Text = "Maximum Connections:";
                    resetConLabel.Text = "Reset Connections:";
                    resetsSentLabel.Text = "Resets Sent:";
                    segRecLabel.Text = "Segments Received:";
                    segResentLabel.Text = "Segments Resent:";
                    segSentLabel.Text = "Segments Sent:";
                    dataRecLabel.Text = "Datagrams Received:";
                    dataSentLabel.Text = "Datagrams Sent:";
                    incDataDiscLabel.Text = "Incoming Datagrams Discarded:";
                    incDataWErrLabel.Text = "Incoming Datagrams With Errors:";
                    break;
                case LanguageConfig.Language.CHINESE:
                    connAcceptLabel.Text = "連接接受:";
                    connInitLabel.Text = "連接啟動:";
                    cumulativeConnLabel.Text = "累計連接:";
                    errRecLabel.Text = "錯誤時接收:";
                    failedConnAttLabel.Text = "連接嘗試失敗:";
                    maxConLabel.Text = "最大連接數:";
                    resetConLabel.Text = "重置連接:";
                    resetsSentLabel.Text = "復位發送:";
                    segRecLabel.Text = "分部收稿:";
                    segResentLabel.Text = "段反感:";
                    segSentLabel.Text = "段發送:";
                    dataRecLabel.Text = "數據報收稿:";
                    dataSentLabel.Text = "數據報發送:";
                    incDataDiscLabel.Text = "捨去傳入的數據報:";
                    incDataWErrLabel.Text = "有錯誤的傳入的數據報:";
                    break;
                case LanguageConfig.Language.GERMAN:
                    connAcceptLabel.Text = "Verbindungen angenommen:";
                    connInitLabel.Text = "Verbindungen initiiert:";
                    cumulativeConnLabel.Text = "Kumulierte Connections:";
                    errRecLabel.Text = "Errors Received:";
                    failedConnAttLabel.Text = "Fehlgeschlagene Verbindungsversuche:";
                    maxConLabel.Text = "maximale Verbindungen:";
                    resetConLabel.Text = "Rücksetzanschlüsse:";
                    resetsSentLabel.Text = "Setzt Sent:";
                    segRecLabel.Text = "Segmente empfangen:";
                    segResentLabel.Text = "Segmente Resent:";
                    segSentLabel.Text = "Segmente Sent:";
                    dataRecLabel.Text = "Empfangene Datagramme:";
                    dataSentLabel.Text = "Datagramme:";
                    incDataDiscLabel.Text = "Eingehende Datagramme verworfen:";
                    incDataWErrLabel.Text = "Eingehende Datagramme mit Fehler:";
                    break;
                case LanguageConfig.Language.RUSSIAN:
                    connAcceptLabel.Text = "Принято подключений:";
                    connInitLabel.Text = "соединения, инициированные:";
                    cumulativeConnLabel.Text = "Накопительное подключений:";
                    errRecLabel.Text = "Ошибки Поступила в редакцию:";
                    failedConnAttLabel.Text = "Неудачных попыток подключения:";
                    maxConLabel.Text = "Максимальная подключений:";
                    resetConLabel.Text = "Сброс подключений:";
                    resetsSentLabel.Text = "Сброс Отправленные:";
                    segRecLabel.Text = "Сегменты Поступила в редакцию:";
                    segResentLabel.Text = "Сегменты Resent:";
                    segSentLabel.Text = "Сегменты Отправленные:";
                    dataRecLabel.Text = "Датаграммы Поступило:";
                    dataSentLabel.Text = "датаграммы, посланные:";
                    incDataDiscLabel.Text = "Входящих датаграмм Выкинуть:";
                    incDataWErrLabel.Text = "Входящие пакеты с ошибками:";
                    break;
                case LanguageConfig.Language.PORTUGUESE:
                case LanguageConfig.Language.SPANISH:
                    connAcceptLabel.Text = "conexiones aceptadas:";
                    connInitLabel.Text = "conexiones que se inician:";
                    cumulativeConnLabel.Text = "Conexiones acumulada:";
                    errRecLabel.Text = "errores recibidos:";
                    failedConnAttLabel.Text = "Intentos fallidos de conexión:";
                    maxConLabel.Text = "máximo de conexiones:";
                    resetConLabel.Text = "Conexiones de reajuste:";
                    resetsSentLabel.Text = "restablece Enviado:";
                    segRecLabel.Text = "segmentos recibidos:";
                    segResentLabel.Text = "segmentos Reenviado:";
                    segSentLabel.Text = "segmentos enviados:";
                    dataRecLabel.Text = "datagramas recibidos:";
                    dataSentLabel.Text = "datagramas enviados:";
                    incDataDiscLabel.Text = "Datagramas de entrada desechados:";
                    incDataWErrLabel.Text = "Los datagramas entrantes con errores:";
                    break;
            }
        }
    }
}
