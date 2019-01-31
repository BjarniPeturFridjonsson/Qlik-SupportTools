using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FreyrViewer.Common;

namespace FreyrViewer.Extensions
{
    public static class ControlExceptionExtensions
    {
        private static readonly object _syncObj = new object();
        private static readonly Dictionary<Control, ControlExceptions> _controlExceptions = new Dictionary<Control, ControlExceptions>();

        private class ControlExceptions
        {
            public ControlExceptions(Control control)
            {
                Control = control;
                Exceptions = new Queue<Exception>();
            }

            public Control Control { get; }

            public Button Button { get; set; }

            public Queue<Exception> Exceptions { get; }
        }

        public static void SetException(this Control control, Exception exception)
        {
            try
            {
                ControlExceptions controlExceptions;
                bool added;

                lock (_syncObj)
                {
                    if (_controlExceptions.TryGetValue(control, out controlExceptions))
                    {
                        added = false;
                    }
                    else
                    {
                        added = true;
                        controlExceptions = new ControlExceptions(control);
                        _controlExceptions.Add(control, controlExceptions);
                    }
                }

                controlExceptions.Exceptions.Enqueue(exception);

                if (added)
                {
                    control.OnUiThread(() =>
                    {
                        Button showExceptionButton = CreateShowExceptionButton(control);

                        lock (_syncObj)
                        {
                            controlExceptions.Button = showExceptionButton;
                            UpdateButton(controlExceptions);
                        }

                        showExceptionButton.Click += (s, e) => ClickExceptionButton(controlExceptions);
                    });
                }
                else
                {
                    control.OnUiThread(() => UpdateButton(controlExceptions));
                }
            }
            catch
            {
            }
        }

        private static void UpdateButton(ControlExceptions controlExceptions)
        {
            try
            {
                lock (_syncObj)
                {
                    if (controlExceptions.Button != null)
                    {
                        controlExceptions.Button.Text = $"Show exception... ({controlExceptions.Exceptions.Count})";
                        controlExceptions.Button.BringToFront();
                    }
                }
            }
            catch
            {
            }
        }

        private static void ClickExceptionButton(ControlExceptions controlExceptions)
        {
            try
            {
                Exception exception;
                Button removeButton;

                lock (_syncObj)
                {
                    exception = controlExceptions.Exceptions.Count > 0
                        ? controlExceptions.Exceptions.Dequeue()
                        : null;

                    if (controlExceptions.Exceptions.Count == 0)
                    {
                        _controlExceptions.Remove(controlExceptions.Control);
                        removeButton = controlExceptions.Button;
                        controlExceptions.Button = null;
                    }
                    else
                    {
                        removeButton = null;
                    }
                }

                if (removeButton != null)
                {
                    try
                    {
                        controlExceptions.Control.OnUiThread(removeButton.Dispose);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    controlExceptions.Control.OnUiThread(() => UpdateButton(controlExceptions));
                }

                if (exception != null)
                {
                    Mbox.Error(exception, "Sorry! We could not load the data you wanted.");
                }
            }
            catch
            {
            }
        }

        private static Button CreateShowExceptionButton(Control control)
        {
            AnchorStyles anchor = AnchorStyles.None;
            if (control.Anchor.HasFlag(AnchorStyles.Top))
            {
                anchor |= AnchorStyles.Top;
            }
            else if (control.Anchor.HasFlag(AnchorStyles.Bottom))
            {
                anchor |= AnchorStyles.Bottom;
            }

            if (control.Anchor.HasFlag(AnchorStyles.Left))
            {
                anchor |= AnchorStyles.Left;
            }
            else if (control.Anchor.HasFlag(AnchorStyles.Right))
            {
                anchor |= AnchorStyles.Right;
            }

            return new Button
            {
                Location = new Point(control.Location.X + 4, control.Location.Y + 4),
                Size = new Size(144, 24),
                Anchor = anchor,
                TabStop = false,
                ForeColor = Color.DarkRed,
                TextAlign = ContentAlignment.MiddleRight,
                Image = Properties.Resources.cross,
                ImageAlign = ContentAlignment.MiddleLeft,
                UseVisualStyleBackColor = true,
                Parent = control.Parent
            };
        }
    }
}