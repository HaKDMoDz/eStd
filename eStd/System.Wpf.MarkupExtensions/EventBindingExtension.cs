using System;
using System.Reflection;
using System.Windows.Markup;
using System.Linq;
using System.Windows;

namespace System.Wpf.MarkupExtensions
{
    public class EventBindingExtension : MarkupExtension
    {
        public EventBindingExtension()
        {
        }

        public EventBindingExtension(string eventHandlerName)
        {
            this.EventHandlerName = eventHandlerName;
        }

        public object Context { get; set; }

        [ConstructorArgument("eventHandlerName")]
        public string EventHandlerName { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(EventHandlerName))
                throw new ArgumentException("The EventHandlerName property is not set", "EventHandlerName");

            var target = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));

            EventInfo eventInfo = target.TargetProperty as EventInfo;
            if (eventInfo == null)
                throw new InvalidOperationException("The target property must be an event");

            object dataContext = GetDataContext(target.TargetObject);
            if (Context != null)
                dataContext = Context;

            if (dataContext == null)
                throw new InvalidOperationException("No DataContext found");

            var handler = GetHandler(dataContext, eventInfo, EventHandlerName);
            if (handler == null)
                throw new ArgumentException("No valid event handler was found", "EventHandlerName");

            return handler;
        }

        #region Helper methods

        static object GetHandler(object dataContext, EventInfo eventInfo, string eventHandlerName)
        {
            Type dcType = dataContext.GetType();

            var method = dcType.GetMethod(
                eventHandlerName,
                GetParameterTypes(eventInfo));
            if (method != null)
            {
                if (method.IsStatic)
                    return Delegate.CreateDelegate(eventInfo.EventHandlerType, method);
                else
                    return Delegate.CreateDelegate(eventInfo.EventHandlerType, dataContext, method);
            }

            return null;
        }

        static Type[] GetParameterTypes(EventInfo eventInfo)
        {
            var invokeMethod = eventInfo.EventHandlerType.GetMethod("Invoke");
            return invokeMethod.GetParameters().Select(p => p.ParameterType).ToArray();
        }

        static object GetDataContext(object target)
        {
            var depObj = target as DependencyObject;
            if (depObj == null)
                return null;

            return depObj.GetValue(FrameworkElement.DataContextProperty) ??
                   depObj.GetValue(FrameworkContentElement.DataContextProperty);
        }
        
        #endregion
    }
}