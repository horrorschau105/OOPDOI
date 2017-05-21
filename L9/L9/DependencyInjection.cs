using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L9
{
    public class SimplyContainer // should it be singleton? -> I'd say: doesn't have to
    {
        Dictionary<Type, object> singletons; // for constructed singletons
        Dictionary<Type, bool> registeredTypes; //  foreach class remember if it wants singleton
        Dictionary<Type, Type> registeredDependencies; // foreach class/interface remember deriving class
        public SimplyContainer()
        {
            registeredDependencies = new Dictionary<Type, Type>();
            registeredTypes = new Dictionary<Type, bool>();
            singletons = new Dictionary<Type, object>();
        }
        public void RegisterType<T>(bool Singleton) where T : class
        {
            if (registeredTypes.ContainsKey(typeof(T))) registeredTypes[typeof(T)] = Singleton;
            else registeredTypes[typeof(T)] = Singleton;  // add/update settings of class
            // else registeredTypes.Add(typeof(T), Singleton);  // it adds or throws exeption if key exists, why this over indexer?
        }
        public void RegisterType<From, To>(bool Singleton) where To : class, From
        {
            if (registeredDependencies.ContainsKey(typeof(From))) registeredDependencies[typeof(From)] = typeof(To);
            else registeredDependencies[typeof(From)] = typeof(To); // same as upper
            // else registeredDependencies.Add(typeof(From), typeof(To)); // same as upper

            // register 'To' type too
            RegisterType<To>(Singleton);
        }
        public T Resolve<T>()  // IMO: we have to first check if T is in Dependencies even if T has default constructor
        {
            Func<Type, T> getSingleton = (Type type) =>  // helper function
            {
                if (singletons.ContainsKey(typeof(T)))
                    return (T)singletons[typeof(T)];
                singletons[typeof(T)] = (T)Activator.CreateInstance(type);
                return (T)singletons[typeof(T)];
            };
            try
            {
                if (registeredDependencies.ContainsKey(typeof(T)))  // first check in Dependencies
                {
                    if (registeredTypes.ContainsKey(typeof(T)))  // singleton
                        return getSingleton(registeredDependencies[typeof(T)]);
                    return (T)Activator.CreateInstance(registeredDependencies[typeof(T)]);
                }
                if (registeredTypes.ContainsKey(typeof(T)))  // singleton
                    return getSingleton(typeof(T));
                return (T)Activator.CreateInstance(typeof(T));
            }
            catch (Exception e)
            {
                throw new UnregisteredTypeException("TODO: fancy info with Exception e");
            }
        }
    }

    [Serializable]
    public class UnregisteredTypeException : Exception
    {
        public UnregisteredTypeException() { }
        public UnregisteredTypeException(string message) : base(message) { }
        public UnregisteredTypeException(string message, Exception inner) : base(message, inner) { }
        protected UnregisteredTypeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
