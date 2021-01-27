using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Server.Entities.Events
{
    public static class ConditionHandlerRegistry
    {
        private delegate bool HandleCondition(Condition condition, Player player, Event eventInstance, QuestBase questBase);
        private delegate bool HandleConditionBool<TCondition>(TCondition condition, Player player, Event eventInstance, QuestBase questBase) where TCondition : Condition;
        private static Dictionary<Type, HandleCondition> MeetsConditionFunctions = new Dictionary<Type, HandleCondition>();
        private static MethodInfo CreateWeaklyTypedDelegateForMethodInfoInfo;
        private static bool Initialized = false;
        private static object mLock = new object();


        public static void Init()
        {
            if (CreateWeaklyTypedDelegateForMethodInfoInfo == null)
                CreateWeaklyTypedDelegateForMethodInfoInfo = typeof(ConditionHandlerRegistry).GetMethod(nameof(CreateWeaklyTypedDelegateForConditionMethodInfo), BindingFlags.Static | BindingFlags.NonPublic);

            if (MeetsConditionFunctions.Count == 0)
            {
                var methods = typeof(Conditions).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "MeetsCondition");
                foreach (var method in methods)
                {
                    var conditionType = method.GetParameters()[0].ParameterType;
                    var typedDelegateFactory = CreateWeaklyTypedDelegateForMethodInfoInfo.MakeGenericMethod(conditionType);

                    var weakDelegate = typedDelegateFactory.Invoke(null, new object[] { method, null }) as HandleCondition;
                    MeetsConditionFunctions.Add(conditionType, weakDelegate);
                }
            }

            Initialized = true;
        }

        public static bool CheckCondition(Condition condition, Player player, Event eventInstance, QuestBase questBase)
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

            return MeetsConditionFunctions[condition.GetType()](condition, player, eventInstance, questBase);
        }


        private static HandleCondition CreateWeaklyTypedDelegateForConditionMethodInfo<TCondition>(MethodInfo methodInfo, object target = null) where TCondition : Condition
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var stronglyTyped =
                    Delegate.CreateDelegate(typeof(HandleConditionBool<TCondition>), target, methodInfo) as
                        HandleConditionBool<TCondition>;

            return (Condition condition, Player player, Event eventInstance, QuestBase questBase) => stronglyTyped(
                (TCondition)condition, player, eventInstance, questBase
            );

            throw new ArgumentException($"Unsupported condition handler return type '{methodInfo.ReturnType.FullName}'.");
        }


    }
}
