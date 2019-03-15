using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;

namespace Blish_HUD.Library.Adhesive {
    public class TwoWayBinding<TLeftMember, TRightMember>:Binding {

        public OneWayBinding<TLeftMember, TRightMember> LeftBinding { get; }
        public OneWayBinding<TRightMember, TLeftMember> RightBinding { get; }

        public override bool Enabled => this.LeftBinding.Enabled && this.RightBinding.Enabled;

        public TwoWayBinding(Expression<Func<TLeftMember>> leftTarget, Expression<Func<TRightMember>> rightTarget, Func<object, TLeftMember> leftValueConverter = null, Func<object, TRightMember> rightValueConverter = null, InitialBindingProcedure setupProcedure = InitialBindingProcedure.ApplyLeft) {
            this.LeftBinding = new OneWayBinding<TLeftMember, TRightMember>(
                                                                            leftTarget,
                                                                            rightTarget,
                                                                            leftValueConverter,
                                                                            setupProcedure == InitialBindingProcedure.ApplyLeft
                                                                            );

            this.RightBinding = new OneWayBinding<TRightMember, TLeftMember>(
                                                                             rightTarget,
                                                                             leftTarget,
                                                                             rightValueConverter,
                                                                             setupProcedure == InitialBindingProcedure.ApplyRight
                                                                             );
        }

        public override void Enable() {
            this.LeftBinding.Enable();
            this.RightBinding.Enable();
        }

        public override void Disable() {
            this.LeftBinding.Disable();
            this.RightBinding.Disable();
        }

    }
}
