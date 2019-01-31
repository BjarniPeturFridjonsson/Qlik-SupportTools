using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FreyrViewer.Extensions;
using WeifenLuo.WinFormsUI.Docking;

namespace FreyrViewer.Ui.Splashes
{
    internal class SplashManager
    {
        private static Form _mainForm;
        private readonly List<Splash> _floatingSplashes = new List<Splash>();
        private readonly object _syncObj = new object();
        private Form _splashForm;

        public static readonly SplashManager Loader = new SplashManager();

        private static readonly TimeSpan _splashDelay = TimeSpan.FromSeconds(1);

        private SplashManager()
        {
        }

        public const string DEFAULT_TEXT = "Please wait...";

        public static void SetMainForm(Form mainForm)
        {
            _mainForm = mainForm;
        }

        public Control GetDefaultOwner()
        {
            return _mainForm;
        }

        public static Splash ShowEmbeddedSplash(Control parentControl, SplashKind splashKind = SplashKind.Normal, string text = DEFAULT_TEXT)
        {
            Splash splash = new Splash(text);

            Control newSplashControl = parentControl.OnUiThread(() =>
            {
                Control splashControl;
                switch (splashKind)
                {
                    case SplashKind.Normal:
                        splashControl = new SplashControl
                        {
                            Parent = parentControl,
                            Anchor = AnchorStyles.None,
                            Visible = false,
                            Text = splash.Text
                        };
                        break;

                    case SplashKind.Mini:
                        splashControl = new MiniSplashControl
                        {
                            Parent = parentControl,
                            Anchor = AnchorStyles.None,
                            Visible = false
                        };
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(splashKind), splashKind, null);
                }

                splashControl.Left = (parentControl.ClientSize.Width - splashControl.Width) / 2;
                splashControl.Top = (parentControl.ClientSize.Height - splashControl.Height) / 2;

                return splashControl;
            });

            Action splashOnDisposing = null;
            splash.Disposing += splashOnDisposing = () =>
            {
                splash.Disposing -= splashOnDisposing;
                try
                {
                    Control copy = newSplashControl;
                    newSplashControl = null;

                    copy?.OnUiThread(() => copy.Dispose());
                }
                catch
                {
                }
            };

            Task.Run(async () =>
            {
                await Task.Delay(_splashDelay);

                parentControl.OnUiThread(() =>
                {
                    Control copy = newSplashControl;
                    if (copy != null)
                    {
                        try
                        {
                            copy.BringToFront();
                            copy.Show();
                        }
                        catch
                        {
                        }
                    }
                });
            });

            return splash;
        }

        public Splash ShowFloatingSplash(Control ownerControl, string text = DEFAULT_TEXT)
        {
            
            DisableControl(ownerControl, out Action reenableControl);

            Splash splash = new Splash(text);

            Action splashOnDisposing = null;
            splash.Disposing += splashOnDisposing = () =>
            {
                splash.Disposing -= splashOnDisposing;
                reenableControl();
                HideFloatingSplash(splash);
            };

            Form splashForm;

            lock (_syncObj)
            {
                _floatingSplashes.Add(splash);

                if (_splashForm == null)
                {
                    if (_mainForm == null)
                    {
                        throw new ArgumentException("Either do the call from the main thread or set the owner form first!");
                    }

                    _splashForm = _mainForm.OnUiThread(() => new SplashLoading());
                }

                splashForm = _splashForm;
            }

            if (splashForm != null)
            {
                UpdateSplashForm(splashForm, splash);
            }

            return splash;
        }

        private void HideFloatingSplash(Splash splash)
        {
            Form splashForm;
            Splash prevSplash;

            lock (_syncObj)
            {
                if (!_floatingSplashes.Remove(splash))
                {
                    return;
                }

                prevSplash = _floatingSplashes.LastOrDefault();

                splashForm = _splashForm;
            }

            UpdateSplashForm(splashForm, prevSplash);
        }

        private void UpdateSplashForm(Form splashForm, Splash splash)
        {
            try
            {
                splashForm.OnUiThread(() =>
                {
                    if (splash == null)
                    {
                        splashForm.Hide();
                    }
                    else
                    {
                        Rectangle ownerBounds;
                        lock (_syncObj)
                        {
                            if (_mainForm?.WindowState == FormWindowState.Minimized)
                            {
                                ownerBounds = Screen.PrimaryScreen.Bounds;
                            }
                            else
                            {
                                ownerBounds = _mainForm?.Bounds ?? Screen.PrimaryScreen.Bounds;
                            }
                        }

                        splashForm.Location = new Point(ownerBounds.Location.X + (ownerBounds.Width / 2) - (splashForm.Width / 2), ownerBounds.Location.Y + (ownerBounds.Height / 2) - (splashForm.Height / 2));

                        splashForm.Text = splash.Text;
                        splashForm.Visible = true;
                    }

                    splashForm.Refresh();
                    Application.DoEvents();
                });
            }
            catch
            {
            }
        }

        private static void DisableControl(Control ownerControl, out Action reenableControl)
        {
            bool previousEnabled = false;
            ownerControl?.OnUiThread(() =>
            {
                previousEnabled = ownerControl.Enabled;
                ownerControl.Enabled = false;

                // Doh! Disabling a DockContent makes another be selected.
                // Hack: Force a reactivation on the current one.
                var dockContent = ownerControl as DockContent;
                dockContent?.Activate();
            });

            reenableControl = () =>
            {
                try
                {
                    ownerControl?.OnUiThread(() => ownerControl.Enabled = previousEnabled);
                }
                catch
                {
                }
            };
        }

    }
}