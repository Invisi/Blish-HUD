using System;
using System.Collections.Generic;
using System.Text;

namespace Blish_HUD.Library.Adhesive {
    public abstract class Binding {

        public Binding() {
            BindManager._bindings.Add(this);
        }

        public abstract bool Enabled { get; }

        public abstract void Enable();
        public abstract void Disable();

    }
}
