using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using java.io;
using java.lang;
using java.lang.reflect;

namespace JSharp
{
    class JavaToolKit
    {
        public JavaToolKit()
        {
           
        }

        public static java.lang.Class GetClassFromFile(java.io.File classFile)
        {
            java.lang.Object primClass;
            primClass = new java.lang.Object();

            java.io.ObjectInputStream ois = new ObjectInputStream(new FileInputStream(classFile));
            primClass = ois.readObject();
            ois.close();
            return primClass.getClass();
        }

        public static Class GetClass(java.lang.String className) 
        {
            return Class.forName(className);
        }

    }
}
