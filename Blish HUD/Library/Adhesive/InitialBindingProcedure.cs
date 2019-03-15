using System;
using System.Collections.Generic;
using System.Text;

namespace Blish_HUD.Library.Adhesive {
    public enum InitialBindingProcedure {

        /// <summary>
        /// Assigns the value of the right member to the left member.
        /// </summary>
        ApplyLeft = 0,

        /// <summary>
        /// Assigns the value of the left member to the right member.
        /// </summary>
        ApplyRight = 1,

        /// <summary>
        /// Does not change the current value of the members.
        /// </summary>
        DoNotApply = 2,

    }
}