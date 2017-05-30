using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using L9;

namespace TestProject
{
    [TestClass]
    public class TestResolve
    {
        [TestMethod]
        public void ContainerNotNull()
        {
            SimplyContainer sc = new SimplyContainer();
            Assert.IsNotNull(sc);
        }
        [TestMethod]
        public void SimpleClassResolve()
        {
            SimplyContainer sc = new SimplyContainer();
            sc.RegisterType<Foo>(true);
            Assert.IsInstanceOfType(sc.Resolve<Foo>(), typeof(Foo));
        }
        [TestMethod]
        public void SimpleDependency()
        {
            SimplyContainer sc = new SimplyContainer();
            sc.RegisterType<IFoo, Foo>(false);
            Assert.IsInstanceOfType(sc.Resolve<IFoo>(), typeof(Foo));
        }
        [TestMethod]
        public void ConcreteSingleton()
        {
            SimplyContainer sc = new SimplyContainer();
            sc.RegisterType<Foo>(true);
            var f1 = sc.Resolve<Foo>();
            var f2 = sc.Resolve<Foo>();
            Assert.AreSame(f1, f2);
        }
        [TestMethod]
        public void NotSingleton()
        {
            SimplyContainer sc = new SimplyContainer();
            sc.RegisterType<Foo>(false);
            var f1 = sc.Resolve<Foo>();
            var f2 = sc.Resolve<Foo>();
            Assert.AreNotEqual(f1, f2);
        }
        [TestMethod]
        public void DependencySingleton()
        {
            SimplyContainer sc = new SimplyContainer();
            sc.RegisterType<IFoo, Foo>(true);
            Assert.AreEqual(sc.Resolve<IFoo>(), sc.Resolve<IFoo>());
        }
        [TestMethod]
        public void DoubleDeriving()
        {
            var sc = new SimplyContainer();
            sc.RegisterType<IFoo, IBur>(false);
            sc.RegisterType<IBur, Bur>(false);
            var bur = sc.Resolve<IFoo>();
            Assert.IsInstanceOfType(bur, typeof(Bur));
        }
        [TestMethod]
        public void ManipulatingSingletonPolicy()
        {
            var sc = new SimplyContainer();
            sc.RegisterType<Foo>(true);  
            var o2 = sc.Resolve<Foo>();
            sc.RegisterType<Foo>(false);
            var o3 = sc.Resolve<Foo>();
            Assert.AreNotEqual(o2, o3); 
        }
        [TestMethod]
        public void ManipulatingDependencies()
        {
            var sc = new SimplyContainer();
            sc.RegisterType<IFoo, Foo>(true);
            Foo o1 = (Foo)sc.Resolve<IFoo>();
            sc.RegisterType<IFoo, Fux>(true);
            Fux o2 = (Fux)sc.Resolve<IFoo>();
            Assert.AreNotEqual(o1, o2);  
        }
        [TestMethod]
        public void ManipulateSingletonPolicyAndDependencies()
        {
            var sc = new SimplyContainer();
            sc.RegisterType<IFoo, Foo>(false);
            var o1 = sc.Resolve<IFoo>();
            var o2 = sc.Resolve<IFoo>();
            sc.RegisterType<IFoo, Fux>(true);
            var o3 = sc.Resolve<IFoo>();
            var o4 = sc.Resolve<IFoo>();
            Assert.IsTrue(!o1.Equals(o2) && o3.Equals(o4) && o2.GetType() != o3.GetType());
        }
        [TestMethod]
        public void RegisterInstance()
        {
            SimplyContainer sc = new SimplyContainer();
            Qux q = new Qux(4);
            sc.RegisterInstance<Qux>(q);
            var resolvedQ = sc.Resolve<Qux>();
            Assert.IsTrue(resolvedQ == q);
        }
        [TestMethod]
        public void ManipulateInstance()
        {
            SimplyContainer sc = new SimplyContainer();
            Qux q1 = new Qux(4);
            sc.RegisterInstance<Qux>(q1);
            Qux q2 = new Qux(5);
            sc.RegisterInstance(q2);
            Qux resolved = sc.Resolve<Qux>();
            Assert.IsTrue(q2 == resolved);
        }
        [TestMethod]
        public void SimpleMultiparameterConstructor()
        {
            SimplyContainer sc = new SimplyContainer();
            sc.RegisterInstance<int>(5);
            Qux q1 = new Qux(6);
            var q2 = sc.Resolve<Qux>();
            Assert.IsTrue(q2.x == 5);
        }
        [TestMethod]
        [ExpectedException(typeof(UnresolveableTypeException))]
        public void SimpleCycleInTree()
        {
            SimplyContainer sc = new SimplyContainer();
            var x = sc.Resolve<Bux>();
            Assert.Fail();
        }
        [TestMethod]
        public void ConstructorWithFourParamsToResolve()
        {
            var sc = new SimplyContainer();
            sc.RegisterInstance<string>("test");
            sc.RegisterInstance<int>(17);
            var res = sc.Resolve<Qoo>();
            Assert.IsTrue(res.foo == "test" && res.q.x == 17);
        }
        [TestMethod]
        [ExpectedException(typeof(UnresolveableTypeException))]
        public void ManyConstructorsOfMaxParamCount()
        {
            SimplyContainer sc = new SimplyContainer();
            var x = sc.Resolve<Wux>();
            Assert.Fail();
        }
        [TestMethod]
        public void ClassWithSameTypeParam()
        {
            SimplyContainer sc = new SimplyContainer();
            sc.RegisterInstance(7);
            var p = sc.Resolve<Punkt>();
            Assert.IsTrue(p != null);
        }
        [TestMethod]
        //[ExpectedException(typeof(UnresolveableTypeException))]
        public void ResolveManyTimesSameTypeButNoCycleInTree()
        {
            SimplyContainer sc = new SimplyContainer();
            sc.RegisterInstance(5);
            sc.RegisterInstance("");
            var x = sc.Resolve<Var>();
            Assert.IsTrue(x != null);
        }


    }
    interface IFoo { }
    interface IBur : IFoo { }
    class Bux
    {
        public Bux(Bux x) { }
    }
    class Bur : IBur { }
    class Foo : IFoo { }  // with default constructor
    class Fux : IFoo { }
    class Bar
    {
        private Bar() { }
    }
    class Qoo
    {
        public string foo;
        public Qux q;
        public Qoo(Qux a, Foo f, Bur b, string x) {
            q = a;
            foo = x;
            }
    }
    class Qux
    {
        public int x;
        public Qux(int xd) {
            x = xd;
        }
    }
    public class Wux
    {
        public Wux(int d)
        { }
        public Wux(string s) { }
    }
    class Var
    {
        public Var(Qoo q, Qux u, Bur b) { }
    }
    class Punkt
    {
        public Punkt(int x,int y) { }
    }
}
