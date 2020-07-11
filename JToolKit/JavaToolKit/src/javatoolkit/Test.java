/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package javatoolkit;

import java.lang.reflect.Method;
import java.util.Scanner;

/**
 *
 * @author osina
 */
public class Test 
{
    public static void main(String[] args)
    {
        JavaToolKit jtk = new JavaToolKit();
        Scanner s = new Scanner(System.in);
        while(true)
        {
            System.out.print("Input> ");
            String cur = s.nextLine();
            try
            {
                Class c = jtk.GetClass(cur);
                System.out.println(c.getName());
                
                for(Method n : c.getMethods())
                {
                    System.out.println(n.getName());
                }
            }
            catch(Exception e)
            {
                
            }
        }
    }
}
