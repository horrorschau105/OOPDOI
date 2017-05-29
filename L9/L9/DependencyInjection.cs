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
        Dictionary<Type, object> registeredInstances; // for registered instances
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
        HashSet<Type> typesToResolve; // avoid cycles in resolving tree by remembering types in set
        public T Resolve<T>()  
        {
            try
            {
                var currentType = typeof(T);  
                while (registeredDependencies.ContainsKey(currentType))  // first checks in Dependencies
                {
                    currentType = registeredDependencies[currentType];
                }
                if (registeredInstances.ContainsKey(currentType)) // check for registered instance
                    return (T)registeredInstances[currentType];

                typesToResolve = new HashSet<Type>();
                var ctors = currentType.GetConstructors();
                int maxCountOfParams = 0, countOfMaximals = 0; // remember max count of constructor params and how many of them are there
                ConstructorInfo constructor = null; // this will have most params
                foreach(var ctor in ctors)
                {
                    if(ctor.GetParameters().Count() > maxCountOfParams)
                    {
                        maxCountOfParams = ctor.GetParameters().Count();
                        constructor = ctor;
                        countOfMaximals = 1;
                    }
                    if(ctor.GetParameters().Count() == maxCountOfParams)
                    {
                        countOfMaximals++;
                    }
                }
                if(countOfMaximals > 1) // there is more than one ctor with maximal count of parameters
                {
                    throw new Exception("There is more than one constructor with maximal count of parameters\n");
                }
                constructor.Invoke(constructor.GetParameters().ToList().ForEach(param => { // horrible lambda, sorry.
                    Type paramType = param.GetType();
                    if (typesToResolve.Contains(paramType)
                    {
                        throw new Exception("There is a cycle in a tree");
                    }
                    else
                    {
                        typesToResolve.Add(paramType);
                        return Resolve<typeof(paramType)>(); // this actually doesn't work
                    }

                }));
                
                
                //if (registeredTypes.ContainsKey(currentType))  // otherwise T should be here registered
                //    return GetObject<T>(currentType);
                
                throw new Exception(string.Format("Not registered type: {0}\n", currentType.ToString()));
            }
            catch (Exception e)
            {
                throw new UnresolveableTypeException("Unable to resolve\n"+ e.ToString());
            }
        }
        T GetObject<T>(Type type)  // returns object of type T, handling the singletons
        {
            var key = type;  
            if (registeredTypes[type])  // return singleton
            {
                if (singletons.ContainsKey(key))
                    return (T)singletons[key];
                singletons[key] = (T)Activator.CreateInstance(type);
                return (T)singletons[key];
            }
            return (T)Activator.CreateInstance(type); 
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
