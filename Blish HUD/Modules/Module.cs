using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blish_HUD.Controls;

namespace Blish_HUD.Modules {

    public struct ModuleInfo {

        public readonly string Name;
        public readonly string Author;
        public readonly string Version;
        public readonly string Description;
        public readonly string Namespace;

        public ModuleInfo(string name, string @namespace, string description, string author, string version) {
            this.Name = name;
            this.Namespace = @namespace;
            this.Description = description;
            this.Author = author;
            this.Version = version;
        }

    }

    public abstract class Module {

        public event EventHandler<EventArgs> Loaded;
        public event EventHandler<EventArgs> Enabled;
        public event EventHandler<EventArgs> Disabled;

        public abstract ModuleInfo GetModuleInfo();
        public abstract void DefineSettings(Settings settings);

        public readonly Settings Settings;
        
        private List<WindowTab2> _tabsAdded = new List<WindowTab2>();

        private bool _moduleEnabled = false;
        public bool ModuleEnabled {
            get => _moduleEnabled;
            set {
                if (_moduleEnabled == value) return;

                _moduleEnabled = value;

                if (_moduleEnabled) GameService.Module.LoadModule(this);
                else OnStop();
            }
        }

        private bool _hasLoaded = false;
        public bool HasLoaded {
            get => _hasLoaded;
            set {
                _hasLoaded = value;

                if (_hasLoaded) OnLoaded(EventArgs.Empty);
            }
        }


        public Module() {
            this.Settings = GameService.Settings
                                       .RegisterSettings(
                                                         this.GetModuleInfo().Namespace,
                                                         true
                                                        );

            this.DefineSettings(this.Settings);
        }

        public async Task<Module> Load() {
            OnLoad();

            return await Task.FromResult(this);
        }

        /// <summary>
        /// Override this method to allow your module to load in content while other services and
        /// modules are also loading.  Do not create controls as doing so may cause an exception -
        /// create instances of controls in <see cref="OnStart"/> instead.  If the module fails to
        /// load, it should throw an exception.
        /// </summary>
        protected virtual void OnLoad() { }

        /* TODO: Consider making this protected virtual to match the event patterns used elsewhere
                 I just don't currently see a reason why a module would need it since they can just
                 stick it at the end of OnLoad */
        private void OnLoaded(EventArgs e) {
            this.Loaded?.Invoke(this, e);
        }

        /// <summary>
        /// This method is called once <see cref="OnLoad"/> has successfully completed and all
        /// <see cref="GameService"/>s have been loaded.
        /// </summary>
        public virtual void OnStart() { }

        protected virtual void OnStop() {
            // Clear out any tabs that were made
            foreach (var windowTab2 in _tabsAdded) {
                GameService.Director.BlishHudWindow.RemoveTab(windowTab2);
            }
        }
        public virtual void Update(GameTime gameTime) { /* NOOP */ }

        // Module Options

        protected void AddSectionTab(string tabName, string icon, Panel panel) {
            _tabsAdded.Add(GameService.Director.BlishHudWindow.AddTab(tabName, icon, panel));
        }

        protected void AddSettingsTab() {

        }

    }
}
