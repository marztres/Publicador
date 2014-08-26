using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Publicador
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("---Iniciando publicador--- Create by Team Fusionet 2014");
            Console.WriteLine("");            
            String rootDir = AppDomain.CurrentDomain.BaseDirectory;
            String baseDir = rootDir + "\\base";
            String src = rootDir + "base\\sygnus";
            String dst = rootDir + "base\\resultado";            
            List<String> listaDirectorios = new List<String>();
            String linea;

            Console.WriteLine("-Listado de directorios : ");
            Console.WriteLine("");
            Console.WriteLine("Directorio de origen : " + src);
            Console.WriteLine("Directorio de destino :" + dst);
            Console.WriteLine("Archivo de rutas : " + rootDir + "rutas.txt");
            Console.WriteLine("");
            Console.WriteLine("Pulse una tecla para continuar:");
            Console.WriteLine("");
            Console.ReadLine();

            try
            {
                if (!Directory.Exists(src))
                    throw new ApplicationException("No se encontro el directorio de origen : " + src);
                if (Directory.Exists(dst))
                {
                    Console.WriteLine("Limpiando directorio de destino. ");
                    DeleteDirectory(dst);
                    Directory.CreateDirectory(dst);
                }
                else
                {
                    Console.WriteLine("Creando directorio de destino.");
                    Directory.CreateDirectory(dst);
                }
                                    
                Console.WriteLine("");
                
                Console.WriteLine("Procesando archivo de rutas :");
                Console.WriteLine("");
                if (!File.Exists(rootDir + "rutas.txt"))
                    throw new ApplicationException("No se encontro el archivo de rutas.txt , debe ponerlo en : " + rootDir);

                Console.WriteLine("");

                System.IO.StreamReader file = new System.IO.StreamReader(rootDir + "rutas.txt");                                              
                //Armando lista de directorios a publicar :
                Int32 counter = 1;               
                while ((linea = file.ReadLine()) != null)
                {
                    if (!String.IsNullOrEmpty(linea))
                    {
                        String directorio = Path.GetFullPath(baseDir + "\\" + linea);
                        if (Directory.Exists(directorio))
                        {
                            listaDirectorios.Add(directorio);
                            Console.WriteLine("Ruta agregada : " + directorio + " Linea " + counter);
                        }
                        else
                        {
                            Console.WriteLine("Directorio no existe : " + directorio + " Linea " + counter);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Linea vacia." + " Linea " + counter);
                    }
                    counter++;           
                }
                Console.WriteLine("");
                Console.WriteLine("Listados de rutas validas :");
                Console.WriteLine("################################");
                listaDirectorios.ForEach(delegate(String directorio)
                {
                    Console.WriteLine(directorio);
                });
                Console.WriteLine("################################");



                Console.WriteLine("Creando estructura de archivos a directorio de destino.");
                Console.WriteLine("");
                //Copiando estructura de archivos en la carpeta de destino
                foreach (string dirPath in Directory.GetDirectories(src, "*", SearchOption.AllDirectories))
                {
                    if (listaDirectorios.Contains(dirPath+"\\"))
                    {
                        Directory.CreateDirectory(dirPath.Replace(src, dst));
                        Console.WriteLine("Directorio creado : " + dirPath);
                    }                                                          
                }
                Console.WriteLine("");

                //Copiando archivos en carpeta de destino                
                Console.WriteLine("Copiando archivos a directorio de destino : extenciones ignoradas '.dll','.config'");
                Console.WriteLine("");
                foreach (string newPath in Directory.GetFiles(src, "*.*", SearchOption.AllDirectories))
                {
                    
                    String rutaArchivo = Path.GetDirectoryName(newPath.Replace("\\sygnus", "\\resultado"));

                    if (listaDirectorios.Contains(Path.GetDirectoryName(newPath)+"\\"))
                    {
                        if (Directory.Exists(rutaArchivo))
                        {
                            if (!rutaArchivo.EndsWith("bin"))
                            {
                                if (!newPath.EndsWith(".dll") && !newPath.EndsWith(".config"))
                                {
                                    File.Copy(newPath, newPath.Replace(src, dst), true);
                                    Console.WriteLine("Copiado : " + newPath);
                                }
                                else
                                {
                                    Console.WriteLine("Archivo ignorado : " + newPath);
                                }
                            }
                        }                    
                    }                        
                }
                Console.WriteLine("");
                
                //Copiando archivos binarios
                Console.WriteLine("Copiando archivos binarios.");
                Console.WriteLine("");
                foreach (string newPath in Directory.GetFiles(dst, "*.aspx", SearchOption.AllDirectories))
                {
                    using (StreamReader reader = new StreamReader(newPath))
                    {
                        String lineaBin = reader.ReadLine().Replace("\"", ">");
                        String startTag = "App_Web_";
                        int startIndex = lineaBin.IndexOf(startTag) + startTag.Length;
                        int endIndex = lineaBin.IndexOf(">", startIndex);
                        String nameBin = lineaBin.Substring(startIndex - startTag.Length, ((endIndex - startIndex) + startTag.Length));

                        String rutaBinSrc = src+"\\bin\\";
                        String rutaBinDst = dst + "\\bin\\";
                        String binSrc = rutaBinSrc + nameBin + ".dll";
                        String binDst = rutaBinDst + nameBin + ".dll";

                        if (!Directory.Exists(rutaBinDst))
                        {
                            Directory.CreateDirectory(rutaBinDst);
                        }

                        if (File.Exists(binSrc))
                        {
                            if (!File.Exists(binDst))
                            {
                                File.Copy(binSrc, binDst, true);
                                Console.WriteLine("Archivo dll agregado : " + nameBin+".dll");
                            }                            
                        }
                    }                                                                                                                                              
                }
                Console.WriteLine("");
                Console.WriteLine("Proceso finalizado con exito.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Upss, sucedio un error : " + ex.Message);                
            }

            Console.WriteLine("");
            Console.WriteLine("----Finalizando publicador----");
            Console.ReadLine();
        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, true);
        }
    }
}
