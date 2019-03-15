using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Blish_HUD.Library.Adhesive {
    public class OneWayBinding<TTargetMember, TSourceMember>:Binding {

        private MemberExpression _targetReference;
        private MemberExpression _sourceReference;

        private object _targetInstance;
        private INotifyPropertyChanged _sourceInstance;

        private PropertyInfo _targetProperty;
        private PropertyInfo _sourceProperty;

        private Action _applyToTarget;

        private bool _enabled;
        public override bool Enabled => _enabled;

        public OneWayBinding(Expression<Func<TTargetMember>> bindTarget, Expression<Func<TSourceMember>> bindSource, Func<object, TTargetMember> valueConverter = null, bool applyLeft = false) {

            _targetReference = bindTarget.Body as MemberExpression;
            _sourceReference = bindSource.Body as MemberExpression;

            if (_targetReference == null)
                throw new ArgumentException($"{nameof(bindTarget)} is expected to contain a MemberExpression, instead it got {bindTarget.Body.NodeType}.");
            if (_sourceReference == null)
                throw new ArgumentException($"{nameof(bindSource)} is expected to contain a MemberExpression, instead it got {bindSource.Body.NodeType}.");

            _targetInstance = ExpressionHelper.EvaluateExpression(_targetReference.Expression);
            _sourceInstance = ExpressionHelper.EvaluateExpression(_sourceReference.Expression) as INotifyPropertyChanged;

            if (bindTarget == null)
                throw new ArgumentException($"{nameof(bindTarget)} must be the member of an instance.");
            if (_sourceInstance == null)
                throw new ArgumentException($"{nameof(bindSource)} is expected to be a property in a class that implements {nameof(INotifyPropertyChanged)}.");

            _targetProperty = _targetReference.Member as PropertyInfo;
            _sourceProperty = _sourceReference.Member as PropertyInfo;

            if (valueConverter == null) {
                // Default just directly applies the value of bindSource to bindTarget
                _applyToTarget = Expression.Lambda<Action>(Expression.Assign(_targetReference, _sourceReference)).Compile();
            } else {
                // Converter was provided so we package it into an Action to call later
                var setTargetAction = ExpressionHelper.MakeAssignmentAction<TTargetMember>(_targetProperty.SetMethod, _targetInstance);
                _applyToTarget = () => setTargetAction.Invoke(valueConverter(_sourceInstance));
            }

            _sourceInstance.PropertyChanged += (sender, e) => {
                if (_sourceProperty.Name == e.PropertyName && this.Enabled) {
                    Run();
                }
            };

            _enabled = true;

            if (applyLeft) {
                Run();
            }
        }

        public void Run() {
            _applyToTarget?.Invoke();
        }

        public override void Enable() {
            _enabled = true;
        }

        public override void Disable() {
            _enabled = false;
        }

    }
}
