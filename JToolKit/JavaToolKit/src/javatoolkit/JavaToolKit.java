
package javatoolkit;
import com.sun.org.apache.bcel.internal.generic.ClassObserver;
import java.io.File;
import java.io.FileInputStream;
import java.io.ObjectInputStream;
import java.lang.reflect.Method;
import java.lang.reflect.Field;
import java.lang.reflect.Constructor;
import java.util.AbstractList;
import java.util.ArrayList;
import java.util.List;

/**
 *
 * @author osina
 */
public class JavaToolKit 
{
    private String filePath;
    private Class classObject;
   
    private Class getClassFromFile(File classFile) throws Exception 
    {
        Object primativeClz;
        primativeClz = new Object();
        ObjectInputStream ois = null;
        ois = new ObjectInputStream(new FileInputStream(classFile));
        primativeClz = ois.readObject();
        ois.close();
        return primativeClz.getClass();
    }
    
    public Class getClass(String filePath) throws Exception
    {
        File file = new File(filePath);
        return getClassFromFile(file);
    }
    
    public void initializeJTK(String filePath) throws Exception
    {
        this.filePath = filePath;
        this.classObject = getClass(filePath);
    }
   
    public Class GetClass(String className) throws ClassNotFoundException
    {
        return Class.forName(className);
    }
}
