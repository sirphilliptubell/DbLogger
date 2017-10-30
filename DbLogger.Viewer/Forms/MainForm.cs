using DbLogger.IO;
using DbLogger.Viewer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DbLogger.Viewer
{
    public partial class MainForm : Form
    {
        private bool _scheduleReset = false;
        private ILogReader _reader;
        private readonly Queue<LogEntry> _incomingLogEntries = new Queue<LogEntry>();
        private readonly Timer _unloadQueueTimer = new Timer();

        public MainForm()
        {
            InitializeComponent();

            lbLogEntries.EntrySelected += LbLogEntries_EntrySelected;
            lbLogEntries.NothingSelected += LbLogEntries_NothingSelected;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ChangeReader(ReaderSource.NamedPipe);

            _unloadQueueTimer.Interval = 100;
            _unloadQueueTimer.Tick += UnloadQueueTimer_Tick;
            _unloadQueueTimer.Start();
        }

        /// <summary>
        /// Changes the source of the data.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <exception cref="NotImplementedException"></exception>
        private void ChangeReader(ReaderSource source)
        {
            _reader?.Dispose();

            _scheduleReset = true;

            switch (source)
            {
                case ReaderSource.NamedPipe:
                    _reader = new NamedPipeReader();
                    break;
                default:
                    throw new NotImplementedException(source.ToString());
            }

            _reader.OnEntry += Reader_OnEntry;
            _reader.OnReset += Reader_OnReset;
        }

        private void LbLogEntries_NothingSelected(object sender, EventArgs e)
        {
            txtQuery.Text = string.Empty;
        }

        private void LbLogEntries_EntrySelected(object sender, LogEntry e)
        {
            txtQuery.Text 
                = e.Text 
                + Environment.NewLine 
                + Environment.NewLine 
                + e.StackTrace 
                + Environment.NewLine; //plus 1 extra to make selecting everything easier
        }

        private void UnloadQueueTimer_Tick(object sender, EventArgs e)
        {
            if (_scheduleReset)
            {
                _scheduleReset = false;
                lbLogEntries.Reset();
                txtQuery.Text = string.Empty;
            }

            while (_incomingLogEntries.Any())
            {
                var entry = _incomingLogEntries.Dequeue();
                lbLogEntries.Add(entry);
            }
        }

        private void Reader_OnReset(object sender, EventArgs e)
        {
            _scheduleReset = true;
        }

        private void Reader_OnEntry(object sender, LogEntry e)
        {
            _incomingLogEntries.Enqueue(e);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _reader?.Dispose();
        }

        private void TxtQuery_DoubleClick(object sender, EventArgs e)
        {
            txtQuery.WordWrap = !txtQuery.WordWrap;
        }
    }
}
