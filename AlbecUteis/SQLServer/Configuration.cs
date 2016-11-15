using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

//      ,--._______,-. 
//         ,','  ,    .  ,_`-. 
//        / /  ,' , _` ``. |  )       `-.. 
//       (,';'""`/ '"`-._ ` \/ ______    \\ 
//         : ,o.-`- ,o.  )\` -'      `---.)) 
//         : , __  ^-.   '|   `.      `    `. 
//         |/ __:_     `. |  ,  `       `    \ 
//         | ( ,-.`-.    ;'  ;   `       :    ; 
//         | |  ,   `.      /     ;      :    \ 
//         ;-'`:::._,`.__),'             :     ; 
//        / ,  `-   `--                  ;     | 
//       /  \                   `       ,      | 
//      (    `     :              :    ,\      | 
//       \   `.    :     :        :  ,'  \    : 
//        \    `|-- `     \ ,'    ,-'     :-.-'; 
//        :     |`--.______;     |        :    : 
//         :    /           |    |         |   \ 
//         |    ;           ;    ;        /     ; 
//       _/--' | Black Dog :`-- /         \_:_:_| 
//     ,',','  |           |___ \ 
//     `^._,--'           / , , .) 
//                        `-._,-' 

namespace SQL
{
    class Configuration
    {
        public static Dictionary<string, Stream> GetEmbeddedScripts()
        {
            Dictionary<string, Stream> stream = new Dictionary<string, Stream>();
            foreach (string nameRes in 
                Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .OrderBy(x => Path.GetFileNameWithoutExtension(x).Substring(Path.GetFileNameWithoutExtension(x).LastIndexOf('.') + 1)))
            {
                stream.Add(nameRes, Assembly.GetExecutingAssembly().GetManifestResourceStream(nameRes));
            }
            return stream;
        }
    }

}
