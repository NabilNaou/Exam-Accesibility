﻿using System.Collections.Generic;
using System.Linq;
using StreetTalk.Models;

namespace StreetTalk.Seeders
{
    public class PostSeeder : Seeder
    {
        public override bool shouldSeed => !Context.Post.Any();
        
        public override void DoSeed()
        {
            var rows = new List<Post>
            {
                new PublicPost
                {
                    Id = 1,
                    Title = "Overlast touristen",
                    Content = SeederUtils.LoremIpsum,
                    Photo = new PostPhoto
                    {
                        Sensitive = false,
                        Photo = new Photo
                        {
                            Filename = "https://assets.nos.nl/data/image/2017/11/23/433105/xxl.jpg",
                        }
                    },
                    User = Context.User.Single(u => u.Id == 1)
                },
                createGarbagePost(2, 1),
                createGarbagePost(3, 2),
                createGarbagePost(4, 4),
                createGarbagePost(5, 3),
                createGarbagePost(6, 1),
                createGarbagePost(7, 4),
                createGarbagePost(8, 2),
                createGarbagePost(9, 3),
                createGarbagePost(10, 1),
                createGarbagePost(11, 4),
                createGarbagePost(12, 2),
                new AnonymousPost
                {
                    Id = 13,
                    Title = "Wiet kwekerij bij de buren",
                    Content = SeederUtils.LoremIpsum,
                    Photo = new PostPhoto
                    {
                        Sensitive = false,
                        Photo = new Photo
                        {
                            Filename = "https://www.nydailynews.com/resizer/Syhve42srvQoXzEPE6ToPsIXBec=/800x1066/top/arc-anglerfish-arc2-prod-tronc.s3.amazonaws.com/public/3XUJPSVHKIUBKZJCFU2WQFA7WY.jpg",
                        }
                    },
                    Pseudonym = "AyZgjE"
                }
            };

            Context.Post.AddRange(rows);
        }

        private PublicPost createGarbagePost(int id, int userId)
        {
            return new PublicPost
            {
                Id = id,
                Title = "Afval op straat",
                Content = SeederUtils.LoremIpsum,
                Closed = true,
                Photo = new PostPhoto
                {
                    Sensitive = true,
                    Photo = new Photo
                    {
                        Filename = "https://upload.wikimedia.org/wikipedia/commons/1/14/Klein_gevaarlijk_afval_A.jpg",
                    }
                },
                User = Context.User.Single(u => u.Id == userId)
            };
        }
    }
}