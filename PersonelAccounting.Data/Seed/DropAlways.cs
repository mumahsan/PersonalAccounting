﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace PersonelAccounting.Data.Seed
{
    class DropAlways : DropCreateDatabaseAlways<DatabaseContext>
    {

        internal DropAlways(IList<ISeed> seeds)
        {
            Seeds = seeds;
        }

        protected override void Seed(DatabaseContext context)
        {
            foreach (var seed in Seeds)
            {
                seed.Seed(context);
            }
        }

        public IList<ISeed> Seeds { get; set; }
    }
}
