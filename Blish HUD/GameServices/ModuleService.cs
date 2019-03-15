using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Blish_HUD {
    public class ModuleService : GameService {

        public SettingEntry<Dictionary<string, bool>> ModuleStates { get; private set; }

        private readonly List<Modules.Module> _availableModules = new List<Modules.Module>();
        public ReadOnlyCollection<Modules.Module> AvailableModules => _availableModules.AsReadOnly();

        private Queue<Modules.Module> _pendingModules = new Queue<Modules.Module>();

        protected override void Initialize() {
            this.ModuleStates = GameService
                               .Settings
                               .CoreSettings
                               .DefineSetting(
                                              "ModuleStates",
                                              new Dictionary<string, bool>(),
                                              new Dictionary<string, bool>()
                                             );
        }

        public void RegisterModule(Modules.Module module, bool defaultEnabled = true) {
            this._availableModules.Add(module);

            // All modules are now enabled by default as starting them disabled confused some people
            // The defaultEnabled param will allow some uncommon modules to default to disabled
            if (this.ModuleStates.Value.ContainsKey(module.GetModuleInfo().Namespace)) {
                module.ModuleEnabled = this.ModuleStates.Value[module.GetModuleInfo().Namespace];
            } else {
                this.ModuleStates.Value.Add(module.GetModuleInfo().Namespace, defaultEnabled);
            }
        }

        protected override void Load() {
            // TODO: Load modules dynamically
            RegisterModule(new Modules.DebugText());
            RegisterModule(new Modules.DiscordRichPresence());
            RegisterModule(new Modules.BeetleRacing.BeetleRacing());
            RegisterModule(new Modules.EventTimers.EventTimers());
            RegisterModule(new Modules.Compass(), false);
            // RegisterModule(new Modules.RangeCircles(), false);
            RegisterModule(new Modules.PoiLookup.PoiLookup());
            RegisterModule(new Modules.MouseUsability.MouseUsability(), false);
            // RegisterModule(new Modules.MarkersAndPaths.MarkersAndPaths());
        }

        public void LoadModule(Modules.Module module) {
            if (module.ModuleEnabled)
                module.Load().ContinueWith(
                    (Task<Modules.Module> moduleTask) => {
                        switch (moduleTask.Status) {
                            case TaskStatus.RanToCompletion:
                                Console.WriteLine($"Module '{module.GetModuleInfo().Name}' has finished loading.");
                                if (GameService.GameServicesReady)
                                    //moduleTask.Result.OnStart();
                                    _pendingModules.Enqueue(moduleTask.Result);
                                else
                                    GameService.Ready += delegate { _pendingModules.Enqueue(moduleTask.Result); };
                                break;
                            case TaskStatus.Faulted:
                                Console.WriteLine($"Module '{module.GetModuleInfo().Name}' faulted while loading!");
                                break;
                            default:
                                Console.WriteLine("Module '{module.GetModuleInfo().Name}' has not finished or faulted yet: " + moduleTask.Status.ToString());
                                break;
                        }
                    });
        }

        private void OnModuleLoaded(Task<Modules.Module> moduleTask) {
            
        }

        protected override void Unload() {

        }

        protected override void Update(GameTime gameTime) {
            while (_pendingModules.Any()) {
                var pendingModule = _pendingModules.Dequeue();
                pendingModule.OnStart();

                Console.WriteLine($"Module '{pendingModule.GetModuleInfo().Name}' has finished starting.");
            }

            this._availableModules.ForEach(s => {
                //try {
                    if (s.ModuleEnabled) s.Update(gameTime);
                //} catch (Exception ex) {
                //    Console.WriteLine($"{s.GetModuleInfo().Name} module had an error:");
                //    Console.WriteLine(ex.Message);
                //}
            });
        }
    }
}
