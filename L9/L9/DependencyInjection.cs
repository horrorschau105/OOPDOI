using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L9
{
    public class SimplyContainer  
    {
        Dictionary<Type, object> registeredInstances;  // for registered instances
        Dictionary<Type, object> singletons;  // for constructed singletons
        Dictionary<Type, bool> registeredTypes;  //  foreach class remember if it wants singleton
        Dictionary<Type, Type> registeredDependencies;  // foreach class/interface remember deriving class
        public SimplyContainer()
        { 
            registeredDependencies = new Dictionary<Type, Type>();
            registeredTypes = new Dictionary<Type, bool>();
            singletons = new Dictionary<Type, object>();
            registeredInstances = new Dictionary<Type, object>();
        }
        public void RegisterType<T>(bool Singleton) where T : class
        {
            registeredTypes[typeof(T)] = Singleton;
        }
        public void RegisterType<From, To>(bool Singleton) where To : class, From
        {
            registeredDependencies[typeof(From)] = typeof(To);  // same as upper
            registeredTypes[typeof(From)] = Singleton;
            RegisterType<To>(Singleton); // register 'To' type too
        }
        public void RegisterInstance<T>(T Instance)
        {
            registeredInstances[typeof(T)] = Instance;
        }
        public T Resolve<T>(HashSet<Type> resolvedTypes = null)
        // avoid cycles in resolving tree by remembering types in set
        // at the beginning, our set is empty
        {
            try
            {
                var currentType = typeof(T);
                while (registeredDependencies.ContainsKey(currentType))  // first checks in Dependencies
                    currentType = registeredDependencies[currentType];
                if (registeredInstances.ContainsKey(currentType))  // check for registered instance
                    return (T)registeredInstances[currentType];

                
                if (resolvedTypes == null) resolvedTypes = new HashSet<Type>(); // if empty, initialize

                var ctors = currentType.GetConstructors();
                // take constructors with max parameters count
                var maxCtors = ctors.Where(ci => ci.GetParameters().Count() == 
                    ctors.Max(x => x.GetParameters().Count()));
                if (maxCtors.Count() > 1)
                    throw new Exception("There is more than one constructor with maximal count of parameters\n");
                var constructor = maxCtors.First();
                // handle singleton, pass as a parameter 'a way' to create new instance
                var resolvedInstance = GetObject<T>(currentType, 
                    () => InvokeConstructor<T>(constructor, resolvedTypes));

                return resolvedInstance;
                
                }
            catch (Exception e)
            {
                throw new UnresolveableTypeException("Unable to resolve\n"+ e.ToString());
            }
        }
        T GetObject<T>(Type key, Func<T> getNewInstance)
        {
            if (registeredTypes.ContainsKey(key) && registeredTypes[key])  // return singleton
            {
                if (singletons.ContainsKey(key))
                    return (T)singletons[key];
                singletons[key] = getNewInstance(); //(T)Activator.CreateInstance(key);
                return (T)singletons[key];
            }
            return getNewInstance();
        }
        T InvokeConstructor<T>(ConstructorInfo constructor, HashSet<Type> resolvedTypes)  // returns object of type T, handling the singletons
        {
            return (T)constructor.Invoke(
                constructor
                .GetParameters()
                .Aggregate(  // iterate over parameters, resolve each one, add resolved to List
                    new List<object>(),
                    (acc, param) => {
                        Type paramType = param.ParameterType;
                        if (resolvedTypes.Contains(paramType))
                            throw new Exception("There is a cycle in a tree");
                        else
                        {
                            HashSet<Type> resolvedTypesForNextCall = new HashSet<Type>(resolvedTypes); // we want a COPY of this set!
                            resolvedTypesForNextCall.Add(typeof(T)); // add current type to copied set
                            acc.Add(
                                this
                                .GetType()
                                .GetMethod("Resolve")
                                .MakeGenericMethod(paramType)
                                .Invoke(this, new object[] { resolvedTypesForNextCall }) // and call recursive
                                // #Msg:
                                // we remember only 'Type' path between root and leaf in this way
                                // instead of whole tree
                            );
                            return acc;
                        }
                    }
                )
                .ToArray()
            );  
        }
    }

    [Serializable]
    public class UnresolveableTypeException : Exception
    {
        public UnresolveableTypeException() { }
        public UnresolveableTypeException(string message) : base(message) { }
        public UnresolveableTypeException(string message, Exception inner) : base(message, inner) { }
        protected UnresolveableTypeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
