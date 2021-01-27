using Intersect.GameObjects.Events;
using Intersect.GameObjects.Switches_and_Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Server.Entities.Events
{
    public static class VariableCheckHandlerRegistry
    {
        private static Dictionary<Type, HandleVariableComparison> CheckVariableComparisonFunctions = new Dictionary<Type, HandleVariableComparison>();
        private delegate bool HandleVariableComparison(VariableValue currentValue, VariableCompaison comparison, Player player, Event eventInstance);
        private delegate bool HandleVariableComparisonBool<TComparison>(VariableValue currentValue, TComparison comparison, Player player, Event eventInstance) where TComparison : VariableCompaison;
        private static MethodInfo CreateWeaklyTypedDelegateForVariableCheckMethodInfoInfo;
        private static bool Initialized = false;
        private static object mLock = new object();


        public static void Init()
        {
            if (CreateWeaklyTypedDelegateForVariableCheckMethodInfoInfo == null)
                CreateWeaklyTypedDelegateForVariableCheckMethodInfoInfo = typeof(VariableCheckHandlerRegistry).GetMethod(nameof(CreateWeaklyTypedDelegateForVariableCheckMethodInfo), BindingFlags.Static | BindingFlags.NonPublic);

            if (CheckVariableComparisonFunctions.Count == 0)
            {
                var methods = typeof(Conditions).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "CheckVariableComparison");
                foreach (var method in methods)
                {
                    var conditionType = method.GetParameters()[1].ParameterType;
                    var typedDelegateFactory = CreateWeaklyTypedDelegateForVariableCheckMethodInfoInfo.MakeGenericMethod(conditionType);

                    var weakDelegate = typedDelegateFactory.Invoke(null, new object[] { method, null }) as HandleVariableComparison;
                    CheckVariableComparisonFunctions.Add(conditionType, weakDelegate);
                }
            }

            Initialized = true;
        }

        public static bool CheckVariableComparison(VariableValue currentValue, VariableCompaison comparison, Player player, Event instance)
        {
            if (!Initialized)
            {
                lock (mLock)
                {
                    if (!Initialized)
                    {
                        Init();
                    }
                }
            }

            return CheckVariableComparisonFunctions[comparison.GetType()](currentValue, comparison, player, instance);
        }


        private static HandleVariableComparison CreateWeaklyTypedDelegateForVariableCheckMethodInfo<TComparison>(MethodInfo methodInfo, object target = null) where TComparison : VariableCompaison
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var stronglyTyped =
                    Delegate.CreateDelegate(typeof(HandleVariableComparisonBool<TComparison>), target, methodInfo) as
                        HandleVariableComparisonBool<TComparison>;

            return (VariableValue currentValue, VariableCompaison comparison, Player player, Event eventInstance) => stronglyTyped(
                currentValue, (TComparison)comparison, player, eventInstance
            );

            throw new ArgumentException($"Unsupported packet handler return type '{methodInfo.ReturnType.FullName}'.");
        }


    }
}
