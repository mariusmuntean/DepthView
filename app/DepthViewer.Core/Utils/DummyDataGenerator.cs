using System;
using System.Collections.Generic;
using DepthViewer.Shared.Models;

namespace DepthViewer.Core.Utils
{
    public static class DummyDataGenerator
    {

        public static List<Mapping> GetDummyMappings()
        {
            List<Mapping> dummyMappings = new List<Mapping>();
            dummyMappings.Add(new Mapping("ss",
                new List<Measurement>
                {
                    new Measurement(22.0d, 23.0d, 222,
                        @"https://upload.wikimedia.org/wikipedia/commons/c/c4/PM5544_with_non-PAL_signals.png"),
                },
                DateTime.Now.AddDays(-12),
                DateTime.Today)
            { IsSavedLocally = true });

            dummyMappings.Add(new Mapping("ss", new List<Measurement>
                            {
                                            new Measurement(22.0d, 23.0d, 222,
                                                @"http://www.planwallpaper.com/static/images/o-COOL-CAT-facebook.jpg"),
                                            new Measurement(22.0d, 23.0d, 222,
                                                @"http://www.planwallpaper.com/static/images/o-COOL-CAT-facebook.jpg"),
                                            new Measurement(22.0d, 23.0d, 222,
                                                @"http://www.planwallpaper.com/static/images/o-COOL-CAT-facebook.jpg"),
                                            new Measurement(22.0d, 23.0d, 222,
                                                @"http://www.planwallpaper.com/static/images/o-COOL-CAT-facebook.jpg")
                            },
                            DateTime.Now.AddDays(-12),
                            DateTime.Today));


            dummyMappings.Add(new Mapping("ss",
    new List<Measurement>
    {
                    new Measurement(22.0d, 23.0d, 222,
                        @"https://mir-s3-cdn-cf.behance.net/project_modules/disp/d6437817472683.562ba5812f243.jpg"),
                     new Measurement(22.0d, 23.0d, 222,
                        @"https://mir-s3-cdn-cf.behance.net/project_modules/disp/d6437817472683.562ba5812f243.jpg"),
                      new Measurement(22.0d, 23.0d, 222,
                        @"https://mir-s3-cdn-cf.behance.net/project_modules/disp/d6437817472683.562ba5812f243.jpg"),
                       new Measurement(22.0d, 23.0d, 222,
                        @"https://mir-s3-cdn-cf.behance.net/project_modules/disp/d6437817472683.562ba5812f243.jpg"),
                        new Measurement(22.0d, 23.0d, 222,
                        @"https://mir-s3-cdn-cf.behance.net/project_modules/disp/d6437817472683.562ba5812f243.jpg"),
                         new Measurement(22.0d, 23.0d, 222,
                        @"https://mir-s3-cdn-cf.behance.net/project_modules/disp/d6437817472683.562ba5812f243.jpg"),
                          new Measurement(22.0d, 23.0d, 222,
                        @"https://mir-s3-cdn-cf.behance.net/project_modules/disp/d6437817472683.562ba5812f243.jpg"),
                           new Measurement(22.0d, 23.0d, 222,
                        @"https://mir-s3-cdn-cf.behance.net/project_modules/disp/d6437817472683.562ba5812f243.jpg"),

    },
    DateTime.Now.AddDays(-12),
    DateTime.Today)
            { IsSavedLocally = DateTime.Now.Millisecond % 2 == 0 });


            dummyMappings.Add(new Mapping("ss",
    new List<Measurement>
    {
                    new Measurement(22.0d, 23.0d, 222,
                        @"http://img.xcitefun.net/users/2011/05/248717,xcitefun-wide-wallpaper016.jpg"),
                    new Measurement(22.0d, 23.0d, 222,
                        @"http://img.xcitefun.net/users/2011/05/248717,xcitefun-wide-wallpaper016.jpg"),
                    new Measurement(22.0d, 23.0d, 222,
                        @"http://img.xcitefun.net/users/2011/05/248717,xcitefun-wide-wallpaper016.jpg"),
                    new Measurement(22.0d, 23.0d, 222,
                        @"http://img.xcitefun.net/users/2011/05/248717,xcitefun-wide-wallpaper016.jpg"),
                    new Measurement(22.0d, 23.0d, 222,
                        @"http://img.xcitefun.net/users/2011/05/248717,xcitefun-wide-wallpaper016.jpg"),
                    new Measurement(22.0d, 23.0d, 222,
                        @"http://img.xcitefun.net/users/2011/05/248717,xcitefun-wide-wallpaper016.jpg"),
                    new Measurement(22.0d, 23.0d, 222,
                        @"http://img.xcitefun.net/users/2011/05/248717,xcitefun-wide-wallpaper016.jpg")
    },
    DateTime.Now.AddDays(-12),
    DateTime.Today));


            dummyMappings.Add(new Mapping("ss",
    new List<Measurement>
    {
                    new Measurement(22.0d, 23.0d, 222,
                        @"https://upload.wikimedia.org/wikipedia/commons/c/c4/PM5544_with_non-PAL_signals.png")
    },
    DateTime.Now.AddDays(-12),
    DateTime.Today));

            return dummyMappings;
        }

    }
}
