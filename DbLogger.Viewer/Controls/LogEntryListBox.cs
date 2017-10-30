using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DbLogger.Viewer.Extensions;

namespace DbLogger.Viewer.Controls
{
    /// <summary>
    /// A ListBox of LogEntries.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Control" />
    class LogEntryListBox : Control
    {
        private readonly ListBox _listbox = new ListBox();
        private readonly LogEntriesStatistics _stastics = new LogEntriesStatistics();

        public int SLOW_QUERY_MILLISECONDS = 30_000;
        public event EventHandler NothingSelected;
        public event EventHandler<LogEntry> EntrySelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntryListBox"/> class.
        /// </summary>
        public LogEntryListBox()
        {
            _listbox.Dock = DockStyle.Fill;
            _listbox.SelectedIndexChanged += _listbox_SelectedIndexChanged;
            _listbox.DrawMode = DrawMode.OwnerDrawFixed; //so we can override DrawItem
            _listbox.DrawItem += _listbox_DrawItem;
            _listbox.SizeChanged += _listbox_SizeChanged;
            this.Controls.Add(_listbox);
        }

        private void _listbox_SizeChanged(object sender, EventArgs e)
        {
            //items in the listbox may not get rendered correctly when the listbox is resized. Invalidate to ensure painting happens correctly.
            _listbox.Invalidate();
        }

        private void _listbox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;

            LogEntry entry = (LogEntry)_listbox.Items[e.Index];

            e.DrawBackground();

            var title = entry.SourceName;

            //update to indicate which query # this is, eg: "DidSomethingQuery 3x"
            var timesQueryUsed = _stastics.GetQueryCount(entry);
            if (timesQueryUsed > 1)
                title = title + " " + timesQueryUsed + "x";

            // Draw the current item text
            e.Graphics.DrawString(title, e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);
            
            var paddingX = 4;

            //draw a rectangle on the right side showing how slow the command was
            if (entry.DurationInMilliseconds.HasValue)
            {
                var ms = entry.DurationInMilliseconds.Value;
                var msText = ms.ToString() + " ms";
                var textBounds = e.Graphics.MeasureString(msText, e.Font);

                //draw the rectangle on the right side
                var warningColor = GetColorForDuration(ms);
                var warningBrush = new SolidBrush(warningColor);
                var warningBounds = e.Bounds.RightSide((int)textBounds.Width + 2 * paddingX);
                e.Graphics.FillRectangle(warningBrush, warningBounds);

                //draw the number of milliseconds
                var warningTextBounds = warningBounds.RightSide(warningBounds.Width - paddingX);
                e.Graphics.DrawString(msText, e.Font, Brushes.Black, warningTextBounds, StringFormat.GenericDefault);                
            }

            //if the same query AND PARAMETERS were used, give another warning showing how many times.
            var timesSameParamsUsed = _stastics.GetQueryAndParamCount(entry);
            if (timesSameParamsUsed > 1)
            {
                //we want this to appear before the above warning
                //assuming milliseconds won't be a string longer than 1,000,000ms
                var maxBoundsForPreviousWarning = e.Graphics.MeasureString("100,000 ms", e.Font);

                var text = timesSameParamsUsed + "x";
                var textBounds = e.Graphics.MeasureString(text, e.Font);
                var warningBounds = new Rectangle(e.Bounds.Width - (int)maxBoundsForPreviousWarning.Width - (int)textBounds.Width - paddingX, e.Bounds.Y, (int)textBounds.Width + 2*paddingX, e.Bounds.Height);
                e.Graphics.DrawString(text, e.Font, Brushes.Red, warningBounds, StringFormat.GenericDefault);
            }
            
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();            
        }

        private Color GetColorForDuration(int ms)
        {
            var good = Color.White;
            var bad = Color.Red;

            if (ms > SLOW_QUERY_MILLISECONDS)
                ms = SLOW_QUERY_MILLISECONDS;
            var percent = (double)ms / SLOW_QUERY_MILLISECONDS;
            
            var rAverage = good.R + (int)((bad.R - good.R) * percent);
            var bAverage = good.B + (int)((bad.B - good.B) * percent);
            var gAverage = good.G + (int)((bad.G - good.G) * percent);

            return Color.FromArgb(rAverage, gAverage, bAverage);
        }

        private void _listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listbox.SelectedIndex == -1)
            {
                NothingSelected?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                var selected = (LogEntry)_listbox.Items[_listbox.SelectedIndex];
                EntrySelected?.Invoke(this, selected);
            }
        }

        /// <summary>
        /// Adds the specified log entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public void Add(LogEntry entry)
        {
            _listbox.Items.Add(entry);
            _stastics.Add(entry);
        }

        /// <summary>
        /// Resets the state and removes all entries.
        /// </summary>
        public void Reset()
        {
            _stastics.Reset();
            _listbox.Items.Clear();
            NothingSelected?.Invoke(this, EventArgs.Empty);
        }
    }
}
