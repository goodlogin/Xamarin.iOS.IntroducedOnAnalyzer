using ObjCRuntime;
using System;
using System.IO;
using System.Linq;
using UIKit;

namespace Xamarin.iOS.IntroducedOnAnalyzer.Console
{
    class Program
    {
        private static StreamWriter _file = File.CreateText("log.txt");

        static void Main(string[] args)
        {
            WritePropertiesIntroducedOn(Platform.iOS_8_0);
            WritePropertiesIntroducedOn(Platform.iOS_7_0);
            WritePropertiesIntroducedOn(Platform.iOS_6_0);

            System.Console.ReadLine();
        }

        private static void WritePropertiesIntroducedOn(Platform platform)
        {
            Log("Introduced on: {0} START\n\n", platform);

            var uiTypes = typeof(UILabel).Assembly.GetExportedTypes().Where(x => x.IsSubclassOf(typeof(UIView)) || x.GetType() == typeof(UIView)).OrderBy(x => x.Name).ToList();

            foreach (var uiType in uiTypes)
            {
                var haveTypeIntroducedAttr = false;

                var props = typeof(UILabel).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(AvailabilityAttribute))).OrderBy(x => x.Name).ToList();

                foreach (var prop in props)
                {
                    foreach (var attr in prop.CustomAttributes)
                    {
                        if (attr.AttributeType == typeof(AvailabilityAttribute))
                        {
                            var introducedAttr = attr.NamedArguments.FirstOrDefault(x => x.MemberName == "Introduced");
                            if (introducedAttr != null && (Platform)Enum.Parse(typeof(Platform), introducedAttr.TypedValue.Value.ToString()) == platform)
                            {
                                if (!haveTypeIntroducedAttr)
                                {
                                    Log("type: {0}", uiType);
                                    haveTypeIntroducedAttr = true;
                                }

                                Log("  property: {0}", prop.Name);
                            }
                        }
                    }
                }
            }

            Log("Introduced on: {0} END", platform);
        }

        private static void Log(string str, object arg)
        {
            System.Console.WriteLine(str, arg);
            _file.WriteLine(str, arg);
        }
    }
}
